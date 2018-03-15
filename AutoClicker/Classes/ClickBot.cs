using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoClicker.Classes
{
    /// <summary>
    /// Class used to represent an application ClickBot
    /// </summary>
    public class ClickBot : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Name of the process
        /// </summary>
        private string _ProcessName;

        /// <summary>
        /// X coordinate (to receive click)
        /// </summary>
        private uint? _X;

        /// <summary>
        /// Y coordinate (to receive click)
        /// </summary>
        private uint? _Y;

        /// <summary>
        /// Delay between clicks
        /// </summary>
        private int? _Delay;

        /// <summary>
        /// Delay between button pressed and button released
        /// </summary>
        private int? _ReleaseDelay;

        /// <summary>
        /// Window's hWnd
        /// </summary>
        private IntPtr _hWnd;

        /// <summary>
        /// lParam (coordinates)
        /// </summary>
        private IntPtr _lParam;

        /// <summary>
        /// Active flag
        /// </summary>
        private bool _IsRunning;

        /// <summary>
        /// Error flag
        /// </summary>
        private bool _IsError;

        /// <summary>
        /// Bot state (0 : Running, 1 : Waiting, 2 : Error)
        /// </summary>
        private uint _State;

        /// <summary>
        /// Botting task (for multi threading)
        /// </summary>
        private Task _BotTask;

        #endregion Properties

        #region Accessors

        /// <summary>
        /// Name of the process
        /// </summary>
        public string ProcessName
        {
            get { return _ProcessName; }
            set
            {
                if (_IsRunning)
                    return;

                if (value.ToLower().EndsWith(".exe"))
                    value = value.Substring(0, value.Length - 4);

                _ProcessName = value;
                OnPropertyChanged("ProcessName");
            }
        }

        /// <summary>
        /// X coordinate (to receive click)
        /// </summary>
        public uint? X
        {
            get { return _X; }
            set
            {
                if (_IsRunning)
                    return;

                _X = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// Y coordinate (to receive click)
        /// </summary>
        public uint? Y
        {
            get { return _Y; }
            set
            {
                if (_IsRunning)
                    return;

                _Y = value;
                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// Delay between clicks
        /// </summary>
        public int? Delay
        {
            get { return _Delay; }
            set
            {
                if (value == null || value < 0 || value > 1000000)
                    value = 0;

                _Delay = value;
                OnPropertyChanged("Delay");
            }
        }

        /// <summary>
        /// Delay between button pressed and button released
        /// </summary>
        public int? ReleaseDelay
        {
            get { return _ReleaseDelay; }
            set
            {
                if (value == null || value < 0 || value > 1000000)
                    value = 0;

                _ReleaseDelay = value;
                OnPropertyChanged("ReleaseDelay");
            }
        }

        /// <summary>
        /// Window's hWnd
        /// </summary>
        public IntPtr hWnd
        {
            get { return _hWnd; }
            set { _hWnd = value; }
        }

        /// <summary>
        /// lParam (coordinates)
        /// </summary>
        public IntPtr lParam
        {
            get { return _lParam; }
            set { _lParam = value; }
        }

        /// <summary>
        /// Active flag
        /// </summary>
        public bool IsRunning
        {
            get { return _IsRunning; }
            private set
            {
                _IsRunning = value;
                OnPropertyChanged("IsRunning");

                DefineState();
            }
        }

        /// <summary>
        /// Error flag
        /// </summary>
        public bool IsError
        {
            get { return _IsError; }
            private set
            {
                _IsError = value;
                OnPropertyChanged("IsError");

                DefineState();
            }
        }

        /// <summary>
        /// Bot state (0 : Running, 1 : Waiting, 2 : Error)
        /// </summary>
        public uint State
        {
            get { return _State; }
        }

        /// <summary>
        /// Botting task (for multi threading)
        /// </summary>
        public Task BotTask
        {
            get { return _BotTask; }
            set { _BotTask = value; }
        }

        #endregion Accessors

        #region Commands

        /// <summary>
        /// Starting command
        /// </summary>
        private ICommand _StartCommand;

        /// <summary>
        /// Stopping command
        /// </summary>
        private ICommand _StopCommand;

        /// <summary>
        /// Starting command
        /// </summary>
        public ICommand StartCommand
        {
            get
            {
                return _StartCommand ?? (_StartCommand = new CommandHandler(param => Start(), true));
            }
        }

        /// <summary>
        /// Stopping command
        /// </summary>
        public ICommand StopCommand
        {
            get
            {
                return _StopCommand ?? (_StopCommand = new CommandHandler(param => Stop(), true));
            }
        }

        #endregion Commands

        #region Events

        /// <summary>
        /// Property value changed event
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected void OnPropertyChanged(string propertyName)
        {
            var evt = this.PropertyChanged;
            if (evt != null)
                evt(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Property value changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ClickBot()
        {
            this._ProcessName = "";
            this._X = 0;
            this._Y = 0;
            this._Delay = 20;
            this._ReleaseDelay = 0;

            this._hWnd = IntPtr.Zero;
            this._lParam = IntPtr.Zero;

            this._IsRunning = false;
            this._IsError = false;
            this._State = 1;
        }

        /// <summary>
        /// Constructor using string
        /// </summary>
        /// <param name="String">String containing properties of the object to create</param>
        public ClickBot(string String)
        {
            string[] Properties = String.Split(';');

            uint tmpX = 0;
            uint tmpY = 0;
            int tmpDelay = 0;
            int tmpReleaseDelay = 0;

            if (Properties.Length >= 2) UInt32.TryParse(Properties[1], out tmpX);
            if (Properties.Length >= 3) UInt32.TryParse(Properties[2], out tmpY);
            if (Properties.Length >= 4) Int32.TryParse(Properties[3], out tmpDelay);
            if (Properties.Length >= 5) Int32.TryParse(Properties[4], out tmpReleaseDelay);

            this._ProcessName = Properties[0];
            this._X = (uint)tmpX;
            this._Y = (uint)tmpY;
            this._Delay = (int)tmpDelay;
            this._ReleaseDelay = (int)tmpReleaseDelay;

            this._IsRunning = false;
            this._IsError = false;
            this._State = 1;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Change the bot state
        /// </summary>
        private void DefineState()
        {
            if (this._IsError == true)
                this._State = 2;
            else if (this._IsRunning == true)
                this._State = 0;
            else
                this._State = 1;

            OnPropertyChanged("State");
        }

        /// <summary>
        /// Start the bot
        /// </summary>
        public void Start()
        {
            // Si déjà démarrer, on sort
            if (this.IsRunning) return;

            // Récupération du hWnd et de coordonnées
            this.hWnd = ClickLib.GethWnd(this.ProcessName);
            this.lParam = ClickLib.lParam(this.X, this.Y);

            if (this.hWnd == IntPtr.Zero || this.lParam == IntPtr.Zero)
                return;

            // Démarrage de la tâche
            this.IsError = false;
            this.IsRunning = true;
            this.BotTask = Task.Run(() => Run());
        }

        /// <summary>
        /// Stop the bot
        /// </summary>
        public void Stop() => this.IsRunning = false;

        /// <summary>
        /// Bot running
        /// </summary>
        private void Run()
        {
            try
            {
                while (this.IsRunning)
                {
                    bool downSuccess = ClickLib.PostMessage(this.hWnd, ClickLib.WM_LBUTTONDOWN, 1, this.lParam);
                    Thread.Sleep((int)this.ReleaseDelay);
                    bool upSuccess = ClickLib.PostMessage(this.hWnd, ClickLib.WM_LBUTTONUP, 0, this.lParam);

                    if (!(downSuccess & upSuccess))
                        throw new Exception("Can't find hWnd!");

                    Thread.Sleep((int)this.Delay);
                }
            }
            catch
            {
                this.IsRunning = false;
                this.IsError = true;
            }
        }

        /// <summary>
        /// Transform object to a string (for saving purpose)
        /// </summary>
        /// <returns>String containing properties of the object to create</returns>
        public override string ToString()
        {
            string tmp = string.Empty;

            tmp = this.ProcessName;
            tmp += ";" + this.X.ToString();
            tmp += ";" + this.Y.ToString();
            tmp += ";" + this.Delay.ToString();
            tmp += ";" + this.ReleaseDelay.ToString();

            return tmp;
        }

        #endregion Methods
    }
}
