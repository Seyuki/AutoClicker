using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker.Classes
{
    /// <summary>
    /// ViewModel for the main window
    /// </summary>
    public class ViewModel
    {
        #region Properties

        /// <summary>
        /// ClickBots collection
        /// </summary>
        private ObservableCollection<ClickBot> _ClickBots;

        #endregion Properties

        #region Accessors

        /// <summary>
        /// ClickBots collection
        /// </summary>
        public ObservableCollection<ClickBot> ClickBots
        {
            get { return _ClickBots; }
            set { _ClickBots = value; }
        }

        #endregion Accessors

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewModel()
        {
            this._ClickBots = new ObservableCollection<ClickBot>();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Start all bots
        /// </summary>
        public void StartAll()
        {
            foreach (ClickBot clickBot in ClickBots)
                clickBot.Start();
        }
        
        /// <summary>
        /// Stop all bots
        /// </summary>
        public void StopAll()
        {
            foreach (ClickBot clickBot in ClickBots)
                clickBot.Stop();
        }

        /// <summary>
        /// Start foreground application bot
        /// </summary>
        public void StartForeground()
        {
            // Get the name of the foreground application
            string ProcessName = ClickLib.GetForegroundProcessName();

            // Get the bot attached to
            foreach (ClickBot clickBot in ClickBots)
            {
                if (clickBot.ProcessName == ProcessName)
                    clickBot.Start();
            }
        }

        /// <summary>
        /// Stop foreground application bot
        /// </summary>
        public void StopForeground()
        {
            // Get the name of the foreground application
            string ProcessName = ClickLib.GetForegroundProcessName();

            // Get the bot attached to
            foreach (ClickBot clickBot in ClickBots)
            {
                if (clickBot.ProcessName == ProcessName)
                    clickBot.Stop();
            }
        }

        /// <summary>
        /// Add foreground application in list
        /// </summary>
        public void AddForeground()
        {
            // Get hWnd of the foreground application
            IntPtr hWnd = ClickLib.GetForegroundWindow();

            // Get process name
            string ProcessName = ClickLib.GetProcessName(hWnd);

            // Get cursor coordinates
            ClickLib.RECT rct = new ClickLib.RECT();
            ClickLib.GetWindowRect(hWnd, ref rct);

            int mouserelativepositionX = Control.MousePosition.X - rct.Left;
            int mouserelativepositionY = Control.MousePosition.Y - rct.Top;

            // Checking if a bot for this application already exists
            foreach (ClickBot Bot in ClickBots)
            {
                if (Bot.ProcessName == ProcessName)
                {
                    Bot.X = (uint?)mouserelativepositionX;
                    Bot.Y = (uint?)mouserelativepositionY;

                    return;
                }
            }
            
            // Creates the bot
            ClickBot clickBot = new ClickBot();
            clickBot.ProcessName = ProcessName;
            clickBot.X = (uint?)mouserelativepositionX;
            clickBot.Y = (uint?)mouserelativepositionY;

            ClickBots.Add(clickBot);
        }

        #endregion Methods
    }
}
