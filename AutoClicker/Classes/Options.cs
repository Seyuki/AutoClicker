using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoClicker.Classes
{
    /// <summary>
    /// Static Options class
    /// </summary>
    public static class Options
    {
        /// <summary>
        /// Load applications settings
        /// </summary>
        public static void LoadOnStartup(ViewModel viewModel)
        {
            // Settings migration
            Options.UpgradeSettings();

            // Add bots in the list
            Options.LoadBots(viewModel);
        }

        /// <summary>
        /// User settings migration
        /// </summary>
        private static void UpgradeSettings()
        {
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Add bots in the settings, then save settings
        /// </summary>
        /// <param name="viewModel">Main window View Model</param>
        public static void SaveBots(ViewModel viewModel)
        {
            string tmpBotsList = string.Empty;

            foreach (ClickBot Bot in viewModel.ClickBots)
                tmpBotsList += (string.IsNullOrEmpty(tmpBotsList) ? string.Empty : "\n") + Bot.ToString();

            Properties.Settings.Default.SavedBotsList = tmpBotsList;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Add saved bots in the list
        /// </summary>
        private static void LoadBots(ViewModel viewModel)
        {
            foreach (string BotInfo in Properties.Settings.Default.SavedBotsList.Split('\n'))
            {
                if (!string.IsNullOrEmpty(BotInfo))
                    viewModel.ClickBots.Add(new ClickBot(BotInfo));
            }
        }
    }
}
