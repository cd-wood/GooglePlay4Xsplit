using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using GooglePlay4XSplit.Model;
using GooglePlay4XSplit.View;
using GooglePlay4XSplit.MVVM;

namespace GooglePlay4XSplit.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private static readonly GooglePlayHandler gpHandler = new GooglePlayHandler();
        public static GooglePlayHandler GooglePlayHandler
        {
            get { return gpHandler; }
        }

        private readonly LoginControlViewModel loginControlViewModel;
        public LoginControlViewModel LoginControlViewModel
        {
            get { return loginControlViewModel; }
        }

        

        public MainWindowViewModel()
        {
            loginControlViewModel = new LoginControlViewModel(GooglePlayHandler, LogIn);
            this.IsLoggedIn = false;
        }

        public LoginControlViewModel LoginControlViewModel
        {
            get { return loginControlViewModel; }
        }

        private bool loggedIn;
        public bool IsLoggedIn
        {
            get { return loggedIn; }
            protected set
            {
                loggedIn = value;
                RaisePropertyChanged("IsLoggedIn");
                RaisePropertyChanged("IsNotLoggedIn");
            }
        }
        public bool IsNotLoggedIn
        {
            get { return !loggedIn; }
        }



        private void LogIn(object sender)
        {
            this.IsLoggedIn = true;
        }
    }
}
