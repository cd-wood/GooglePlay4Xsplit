using GoogleMusicAPI;
using GooglePlay4XSplit.MVVM;
using GooglePlay4XSplit.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace GooglePlay4XSplit.ViewModel
{
    public class MusicControlViewModel : ObservableObject
    {
        private SongBox songBox;

        private GoogleMusicSong currentSong;
        public GoogleMusicSong CurrentSong
        {
            get { return currentSong; }
        }

        public String CurrentSongInfo
        {
            get
            {
                return "";
            }
        }

        private ImageSource playButtonImage;
        public ImageSource PlayButtonImage
        {
            get { return playButtonImage; }
            set
            {
                playButtonImage = value;
                RaisePropertyChanged("PlayButtonImage");
            }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                RaisePropertyChanged("IsPlaying");
                if (isPlaying)
                {
                    this.PlayButtonImage = Settings.image_PauseButton;
                }
                else
                {
                    this.PlayButtonImage = Settings.image_PlayButton;
                }
            }
        }

        public ICommand TogglePlayCommand
        {
            get { return new RelayCommand(this.TogglePlay); }
        }

        public MusicControlViewModel()
        {
            this.IsPlaying = false;
        }

        public void RegisterSongBox(SongBox box)
        {
            this.songBox = box;
        }

        private void TogglePlay()
        {
            this.IsPlaying = !this.IsPlaying;
        }
    }
}
