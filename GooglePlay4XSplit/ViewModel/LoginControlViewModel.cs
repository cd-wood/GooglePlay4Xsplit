using GooglePlay4XSplit.Model;
using GooglePlay4XSplit.MVVM;
using GooglePlay4XSplit.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;

namespace GooglePlay4XSplit.ViewModel
{
    public delegate void SuccessfulLogin(object sender);

    public class LoginControlViewModel : ObservableObject
    {
        private static readonly int MAX_CONNECTING_LENGTH =  "Connecting".Length + 7;
        private static readonly int CONNECTING_STATE_INTERVAL = 500;
        private static readonly int MAX_LOGIN_WAIT = 7000;

        private System.Timers.Timer connecting_timer;

        public GooglePlayHandler GooglePlayHandler
        {
            get { return MainWindowViewModel.GooglePlayHandler; }
        }

        private String username = "";
        public String Username
        {
            get { return username; }
            set { username = value; }
        }

        private String connectingText;
        public String ConnectingText
        {
            get { return connectingText; }
            private set
            {
                connectingText = value;
                RaisePropertyChanged("ConnectingText");
            }
        }

        private bool isConnecting;
        public bool IsConnecting
        {
            get { return isConnecting; }
            private set
            {
                if (isConnecting != value)
                {
                    isConnecting = value;
                    RaisePropertyChanged("IsConnecting");
                    if (isConnecting == true)
                    {
                        connecting_timer.Start();
                        this.ConnectingText = "Connecting";
                    }
                    else
                        connecting_timer.Stop();
                }
            }
        }

        private ICommand loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return this.loginCommand ?? (this.loginCommand = new RelayCommand<PasswordBox>(AttemptLogin));
            }
        }

        public LoginControlViewModel()
        {
            connecting_timer = new System.Timers.Timer(CONNECTING_STATE_INTERVAL);
            connecting_timer.Elapsed += connecting_timer_Elapsed;
            this.IsConnecting = false;
            this.ConnectingText = "Connecting";
        }

        private void connecting_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.ConnectingText.Length >= MAX_CONNECTING_LENGTH)
                this.ConnectingText = "Connecting";
            else
                this.ConnectingText += ".";
        }

        private void AttemptLogin(PasswordBox password_box)
        {
            if ((Username.Length == 0) || (password_box.Password.Length == 0))
                return;
            String password = password_box.Password;
            password_box.Password = "";
            Thread loginThread = new Thread(new ThreadStart(delegate { doAttemptLogin(Username, password); }));
            loginThread.Start();
        }

        private void doAttemptLogin(String login_name, String login_pwd)
        {
            this.IsConnecting = true;
            // Set up Interrupt action
            var interruptTimer = new System.Timers.Timer(MAX_LOGIN_WAIT);
            interruptTimer.Elapsed += delegate
            {
                interruptTimer.Stop();
                GooglePlayHandler.InterruptAction();
            };
            ThreadResult loginResult = GooglePlayHandler.AttemptLogin(login_name, login_pwd);
            interruptTimer.Stop();

            switch (loginResult)
            {
                case ThreadResult.LOGGEDIN:
                    break;
                case ThreadResult.UNINIT:
                    PopUp.DisplayMessage("Error: Google Play API Not Initialized", "Error", PopUp.MessageType.ERROR);
                    break;
                case ThreadResult.BUSY:
                    PopUp.DisplayMessage("Google Play API is currently busy.", "API Busy", PopUp.MessageType.INFO);
                    break;
                case ThreadResult.TIMEDOUT:
                    PopUp.DisplayMessage("Unable to connect.", "Connection Timedout", PopUp.MessageType.WARNING);
                    break;
                case ThreadResult.ERROR:
                    PopUp.DisplayMessage("Error: " + GooglePlayHandler.ErrorException.Message, "Error", PopUp.MessageType.ERROR);
                    break;
                default:
                    // Shouldn't be reached?
                    PopUp.DisplayMessage("Error: Unknown Response [" + loginResult.ToString() + "]", "Error", PopUp.MessageType.ERROR);
                    break;
            }
            this.IsConnecting = false;
        }

    }
}
