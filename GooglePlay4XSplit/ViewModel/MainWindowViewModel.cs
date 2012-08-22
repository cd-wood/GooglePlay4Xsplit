using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using GooglePlay4XSplit.Model;
using GooglePlay4XSplit.View;
using GooglePlay4XSplit.MVVM;
using System.Windows;

namespace GooglePlay4XSplit.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private static GooglePlayHandler gpHandler;
        public static GooglePlayHandler GooglePlayHandler
        {
            get { return gpHandler; }
            private set { gpHandler = value; }
        }

        private readonly LoginControlViewModel loginControlViewModel;
        public LoginControlViewModel LoginControlViewModel
        {
            get { return loginControlViewModel; }
        }

        private readonly SongSelectionViewModel songSelectionViewModel;
        public SongSelectionViewModel SongSelectionViewModel
        {
            get { return songSelectionViewModel; }
        }

        private readonly MusicControlPanelViewModel musicControlPanelViewModel;
        public MusicControlPanelViewModel MusicControlPanelViewModel
        {
            get { return musicControlPanelViewModel; }
        }

        private readonly MusicControlViewModel musicControl = new MusicControlViewModel();
        public MusicControlViewModel MusicControl
        {
            get { return musicControl; }
        }

        private bool loggedIn;
        public bool IsLoggedIn
        {
            get { return loggedIn; }
            protected set
            {
                loggedIn = value;
                RaisePropertyChanged("IsLoggedIn");
            }
        }


        public MainWindowViewModel()
        {
            CreateGooglePlayHandler();
            loginControlViewModel = new LoginControlViewModel();
            songSelectionViewModel = new SongSelectionViewModel(this.MusicControl);
            musicControlPanelViewModel = new MusicControlPanelViewModel(this.MusicControl);
            this.IsLoggedIn = false;
        }

        private void CreateGooglePlayHandler()
        {
            GooglePlayHandler = new GooglePlayHandler();
            GooglePlayHandler.OnLogIn += onLogIn;
            GooglePlayHandler.OnLogout += onLogout;
        }

        private void onLogIn(object sender, EventArgs e)
        {
            this.IsLoggedIn = true;
            this.SongSelectionViewModel.Begin();
        }

        public bool onLogout(object sender, EventArgs e)
        {
            // Tell music player to stop
            // this.MusicControl.Finish();

            this.IsLoggedIn = false;
            CreateGooglePlayHandler();
            return true;
        }
    }
}
