using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GoogleMusicAPI;
using System.Threading;

namespace GooglePlay4XSplit.Model
{
    public enum ThreadResult { NONE, BUSY, UNINIT, TIMEDOUT, ERROR, LOGGEDIN, GOTSONGS, GOTALLPL };
    public class GooglePlayHandler
    {
        private API gpApi;
        private AutoResetEvent threadHandler;

        private List<GoogleMusicSong> allSongs;
        private GoogleMusicPlaylists allPlaylists;

        private ThreadResult threadResult;
        private Exception errorException;

        public GooglePlayHandler()
        {
            gpApi = new API();
            threadHandler = new AutoResetEvent(false);

            // Add Events
            // gpApi.OnCreatePlaylistComplete += null; // Not Used
            // gpApi.OnDeletePlaylist += null; // Not Used
            gpApi.OnError += CatchError;
            gpApi.OnGetAllSongsComplete += GotAllSongs;
            //gpApi.OnGetPlaylistComplete += null; // Not Used
            gpApi.OnGetPlaylistsComplete += GotAllPlaylists;
            //gpApi.OnGetSongURL += null; // Not Used
            gpApi.OnLoginComplete += LoginSuccessful;
        }

        // Return the latest Error
        public Exception GetErrorException()
        {
            return errorException;
        }

        public void InterruptAction()
        {
            if (threadResult == ThreadResult.NONE)
                return;

            threadResult = ThreadResult.TIMEDOUT;
            threadHandler.Set();
        }

        public ThreadResult AttemptLogin(string name, string password)
        {
            errorException = null;
            if (gpApi == null)
            {
                return ThreadResult.UNINIT;
            }
            if (threadResult != ThreadResult.NONE)
            {
                return ThreadResult.BUSY;
            }
            threadResult = ThreadResult.BUSY;

            gpApi.Login(name, password);

            threadHandler.WaitOne();
            ThreadResult result = threadResult;
            threadResult = ThreadResult.NONE;

            return result;
        }

        private void LoginSuccessful(object sender, EventArgs e)
        {
            threadResult = ThreadResult.LOGGEDIN;
            threadHandler.Set();
        }

        public ThreadResult GetAllSongs()
        {
            errorException = null;
            if (gpApi == null)
            {
                return ThreadResult.UNINIT;
            }
            if (threadResult != ThreadResult.NONE)
            {
                return ThreadResult.BUSY;
            }
            threadResult = ThreadResult.BUSY;

            gpApi.GetAllSongs();

            threadHandler.WaitOne();
            ThreadResult result = threadResult;
            threadResult = ThreadResult.NONE;

            return result;
        }

        private void GotAllSongs(List<GoogleMusicSong> songs)
        {
            this.allSongs = songs;
            threadResult = ThreadResult.GOTSONGS;
            threadHandler.Set();
        }

        private ThreadResult GetAllPlaylists()
        {
            errorException = null;
            if (gpApi == null)
            {
                return ThreadResult.UNINIT;
            }
            if (threadResult != ThreadResult.NONE)
            {
                return ThreadResult.BUSY;
            }
            threadResult = ThreadResult.BUSY;

            gpApi.GetPlaylist();

            threadHandler.WaitOne();
            ThreadResult result = threadResult;
            threadResult = ThreadResult.NONE;

            return result;
        }

        private void GotAllPlaylists(GoogleMusicPlaylists playlists)
        {
            this.allPlaylists = playlists;
            threadResult = ThreadResult.GOTALLPL;
            threadHandler.Set();
        }

        private void CatchError(Exception e)
        {
            threadResult = ThreadResult.ERROR;
            errorException = e;
            threadHandler.Set();
        }
    }
}
