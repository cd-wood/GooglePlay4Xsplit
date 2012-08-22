using GoogleMusicAPI;
using GooglePlay4XSplit.Model;
using GooglePlay4XSplit.MVVM;
using GooglePlay4XSplit.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;

namespace GooglePlay4XSplit.ViewModel
{
    public class SongSelectionViewModel : ObservableObject
    {
        private static readonly int MAX_PLDOWNLOADING_LENGTH = "Downloading Playlists".Length + 7;
        private static readonly int MAX_LOADINGSONGS_LENGTH = "Loading Songs".Length + 3;
        private static readonly int TIMER_UPDATE_INTERVAL = 500;

        private System.Timers.Timer downloadingPLTimer;
        private System.Timers.Timer loadingSongsTimer;

        public GooglePlayHandler GooglePlayHandler
        {
            get { return MainWindowViewModel.GooglePlayHandler; }
        }

        private readonly MusicControlViewModel musicControlViewModel;
        public MusicControlViewModel MusicControlViewModel
        {
            get { return musicControlViewModel; }
        }

        private bool isAttemptingDownload = false;
        public bool IsAttemptingDownload
        {
            get { return isAttemptingDownload; }
            set
            {
                if (isAttemptingDownload != value)
                {
                    isAttemptingDownload = value;
                    RaisePropertyChanged("IsAttemptingDownload");
                    if (isAttemptingDownload)
                        downloadingPLTimer.Start();
                    else
                        downloadingPLTimer.Stop();
                }
            }
        }

        private SortedList<String, GoogleMusicPlaylist> playlistMap;
        private SortedList<String, GoogleMusicPlaylist> PlaylistMap
        {
            set
            {
                playlistMap = value;
                RaisePropertyChanged("PlaylistList");
            }
        }
        public List<String> PlaylistList
        {
            get { return playlistMap.Keys.ToList(); }
        }

        private List<GoogleMusicSong> songList;
        public List<GoogleMusicSong> SongList
        {
            get { return songList; }
            private set
            {
                songList = value;
                RaisePropertyChanged("SongList");
            }
        }

        private int selectedPlaylist;
        public int SelectedPlaylist
        {
            get { return selectedPlaylist; }
            set
            {
                selectedPlaylist = value;
                RaisePropertyChanged("SelectedPlaylist");
                if (selectedPlaylist >= 0)
                {
                    this.SongList = new List<GoogleMusicSong>();
                    Thread loadSongThread = new Thread(new ThreadStart(delegate { doLoadSongList(playlistMap.ElementAt(selectedPlaylist).Value); }));
                    loadSongThread.Start();
                }
            }
        }

        private bool isLoadingSongs = false;
        public bool IsLoadingSongs
        {
            get { return isLoadingSongs; }
            set
            {
                if (isLoadingSongs != value)
                {
                    isLoadingSongs = value;
                    RaisePropertyChanged("IsLoadingSongs");
                    if (isLoadingSongs)
                        loadingSongsTimer.Start();
                    else
                        loadingSongsTimer.Stop();
                }
            }
        }

        private String downloadingPlaylistsText;
        public String DownloadingPlaylistsText
        {
            get { return downloadingPlaylistsText; }
            private set
            {
                downloadingPlaylistsText = value;
                RaisePropertyChanged("DownloadingPlaylistsText");
            }
        }

        private String loadingSongsText;
        public String LoadingSongsText
        {
            get { return loadingSongsText; }
            private set
            {
                loadingSongsText = value;
                RaisePropertyChanged("LoadingSongsText");
            }
        }

        public ICommand LogoutCommand
        {
            get { return new RelayCommand(this.Logout); }
        }

        public SongSelectionViewModel(MusicControlViewModel musicControl)
        {
            this.PlaylistMap = new SortedList<String, GoogleMusicPlaylist>();

            downloadingPLTimer = new System.Timers.Timer(TIMER_UPDATE_INTERVAL);
            downloadingPLTimer.Elapsed += downloadingPLTimer_Elapsed;

            loadingSongsTimer = new System.Timers.Timer(TIMER_UPDATE_INTERVAL);
            loadingSongsTimer.Elapsed += loadingSongsTimer_Elapsed;

            musicControlViewModel = musicControl;

            this.DownloadingPlaylistsText = "Downloading Playlists";
            this.LoadingSongsText = "Loading Songs";
        }

        void loadingSongsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.LoadingSongsText.Length >= MAX_LOADINGSONGS_LENGTH)
                this.LoadingSongsText = "Loading Songs";
            else
                this.LoadingSongsText += ".";
        }

        void downloadingPLTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.DownloadingPlaylistsText.Length >= MAX_PLDOWNLOADING_LENGTH)
                this.DownloadingPlaylistsText = "Downloading Playlists";
            else
                this.DownloadingPlaylistsText += ".";
        }

        // This will try to get all of the playlists and songs
        public void Begin()
        {
            Thread doBeginThread = new Thread(new ThreadStart(delegate { doBegin(); }));
            doBeginThread.Start();
        }

        private void doBegin()
        {
            this.IsAttemptingDownload = true;

            ThreadResult getPlaylistResult = this.GooglePlayHandler.GetAllPlaylists();
            bool success = false;
            switch (getPlaylistResult)
            {
                case ThreadResult.GOTALLPL:
                    success = true;
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
                    PopUp.DisplayMessage("Error: Unknown Response [" + getPlaylistResult.ToString() + "]", "Error", PopUp.MessageType.ERROR);
                    break;
            }

            if (success)
            {
                PopulatePlaylistList(GooglePlayHandler.AllPlaylists);
            }

            this.IsAttemptingDownload = false;
            if (PlaylistList.Count > 0)
                this.SelectedPlaylist = 0;
            else
                this.SelectedPlaylist = -1;
        }

        private void PopulatePlaylistList(GoogleMusicPlaylists playlists)
        {
            int numPlaylists = playlists.InstantMixes.Count + playlists.UserPlaylists.Count + 1;
            SortedList<String, GoogleMusicPlaylist> result = new SortedList<String, GoogleMusicPlaylist>(numPlaylists);

            result.Add("All Songs", null);

            foreach (GoogleMusicPlaylist userPl in playlists.UserPlaylists)
            {
                String name = "Playlist: " + userPl.Title;
                if (result.ContainsKey(name))
                {
                    int counter = 1;
                    while (result.ContainsKey(name + " (" + counter.ToString() + ")"))
                        counter++;
                    name += " (" + counter.ToString() + ")";
                }
                result.Add(name, userPl);
            }

            foreach (GoogleMusicPlaylist mix in playlists.InstantMixes)
            {
                String name = "Instant Mix: " + mix.Title;
                if (result.ContainsKey(name))
                {
                    int counter = 1;
                    while (result.ContainsKey(name + " (" + counter.ToString() + ")"))
                        counter++;
                    name += " (" + counter.ToString() + ")";
                }
                result.Add(name, mix);
            }

            this.PlaylistMap = result;
        }

        private void doLoadSongList(GoogleMusicPlaylist playlist)
        {
            this.IsLoadingSongs = true;
            if (playlist == null)
            {
                if (GooglePlayHandler.AllSongs == null)
                {
                    if (GetAllSongs())
                    {
                        SortedSet<GoogleMusicSong> sortAll = new SortedSet<GoogleMusicSong>(GooglePlayHandler.AllSongs, new GoogleMusicSongComparer());
                        this.SongList = sortAll.ToList();
                    }
                }
            }
            else
                this.SongList = playlist.Songs;
            this.IsLoadingSongs = false;
        }

        private bool GetAllSongs()
        {
            ThreadResult getSongLoadResult = this.GooglePlayHandler.GetAllSongs();
            bool success = false;
            switch (getSongLoadResult)
            {
                case ThreadResult.GOTSONGS:
                    success = true;
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
                    PopUp.DisplayMessage("Error: Unknown Response [" + getSongLoadResult.ToString() + "]", "Error", PopUp.MessageType.ERROR);
                    break;
            }

            return success;
        }

        private void Logout()
        {
            this.playlistMap.Clear();
            this.GooglePlayHandler.Logout();
        }
    }
}
