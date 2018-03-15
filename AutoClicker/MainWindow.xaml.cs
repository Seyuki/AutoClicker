using AutoClicker.Classes;
using AutoClicker.Dialogs;
using AutoClicker.Windows;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// ViewModel
        /// </summary>
        public static ViewModel viewModel;

        /// <summary>
        /// KeyListener
        /// </summary>
        KeyboardListener KListener = new KeyboardListener();

        /// <summary>
        /// Keys list (keyboard)
        /// </summary>
        List<Key> keys = new List<Key>();

        /// <summary>
        /// Main window constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Load localization strings
            LocalizationLib.SetLanguageResourceDictionary(this);

            // Adds the data context
            viewModel = new ViewModel();
            this.DataContext = viewModel;

            // Loads user settings
            Options.LoadOnStartup(viewModel);

            // Keys events
            KListener.KeyDown += new RawKeyEventHandler(KListener_KeyDown);
            KListener.KeyUp += new RawKeyEventHandler(KListener_KeyUp);
        }

        /// <summary>
        /// Window closed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(object sender, EventArgs e)
        {
            // Saves user settings
            Options.SaveBots(viewModel);
        }

        #region ContextMenu

        /// <summary>
        /// Context menu : Delete item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBot(object sender, RoutedEventArgs e)
        {
            // Get context menu
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            DataGridRow Row = (DataGridRow)contextMenu.PlacementTarget;

            // Get bot
            ClickBot clickBot = null;
            if (Row.Item.GetType() == typeof(ClickBot))
                clickBot = (ClickBot)Row.Item;

            if (clickBot == null) return;

            // If bot is running, we don't do anything
            if (clickBot.IsRunning)
                return;

            // Deletes bot
            viewModel.ClickBots.Remove(clickBot);
            clickBot = null;

            // Datagrid commit and refresh source
            dataGrid_Processes.CommitEdit(DataGridEditingUnit.Row, true);
            dataGrid_Processes.ItemsSource = null;
            dataGrid_Processes.ItemsSource = viewModel.ClickBots;
        }

        #endregion ContextMenu

        #region Buttons

        /// <summary>
        /// Button "about"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            About AboutWindow = new About();
            AboutWindow.Owner = this;
            AboutWindow.DataContext = new AboutViewModel();
            AboutWindow.ShowDialog();
        }

        /// <summary>
        /// Button "Show shortcuts"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ShowShortcuts(object sender, RoutedEventArgs e)
        {
            await DialogHost.Show(new KeyboardShortcuts { DataContext = new DialogViewModel() }, "RootDialog");
        }

        /// <summary>
        /// Button "Start all", to start all bots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartAll(object sender, RoutedEventArgs e) => viewModel.StartAll();

        /// <summary>
        /// Button "Stop all", to stop all bots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopAll(object sender, RoutedEventArgs e) => viewModel.StopAll();

        #endregion Buttons

        #region Key Events

        /// <summary>
        /// KeyDown Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void KListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            try
            {
                SetKeyDown(args.Key);

                // F1 (Start foreground application bot)
                if (IsKeyDown(Key.F1))
                {
                    viewModel.StartForeground();
                }
                // F2 (Stop foreground application bot)
                else if (IsKeyDown(Key.F2))
                {
                    viewModel.StopForeground();
                }
                // Ctrl + F3 (Add process in list)
                else if (IsKeyDown(Key.LeftCtrl) && IsKeyDown(Key.F3))
                {
                    viewModel.AddForeground();
                }
                // Ctrl + F5 (Start all bots)
                else if (IsKeyDown(Key.LeftCtrl) && IsKeyDown(Key.F5))
                {
                    viewModel.StartAll();
                }
                // Ctrl + F6 (Stop all bots)
                else if (IsKeyDown(Key.LeftCtrl) && IsKeyDown(Key.F6))
                {
                    viewModel.StopAll();
                }
            }
            catch { }
        }

        /// <summary>
        /// KeyUp Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void KListener_KeyUp(object sender, RawKeyEventArgs args)
        {
            SetKeyUp(args.Key);
        }

        /// <summary>
        /// Check if a key is pressed
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns></returns>
        private bool IsKeyDown(Key key)
        {
            return keys.Contains(key);
        }

        /// <summary>
        /// Add key to keys pressed list
        /// </summary>
        /// <param name="key">Key to add</param>
        private void SetKeyDown(Key key)
        {
            if (!keys.Contains(key))
                keys.Add(key);
        }

        /// <summary>
        /// Withdraw key to keys pressed list
        /// </summary>
        /// <param name="key">Key to withdraw</param>
        private void SetKeyUp(Key key)
        {
            if (keys.Contains(key))
                keys.Remove(key);
        }

        #endregion Key Events
    }
}
