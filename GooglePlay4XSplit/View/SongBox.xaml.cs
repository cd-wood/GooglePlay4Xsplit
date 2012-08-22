using GoogleMusicAPI;
using GooglePlay4XSplit.MVVM;
using GooglePlay4XSplit.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace GooglePlay4XSplit.View
{
    /// <summary>
    /// Interaction logic for SongBox.xaml
    /// </summary>
    public partial class SongBox : ListBox
    {
        public static readonly DependencyProperty SongSourceProperty = DependencyProperty.Register("SongSource", typeof(List<GoogleMusicSong>), typeof(SongBox), new PropertyMetadata(new List<GoogleMusicSong>(), on_BasicItemSourceChanged));
        public static readonly DependencyProperty IsShuffledProperty = DependencyProperty.Register("IsShuffled", typeof(bool), typeof(SongBox), new PropertyMetadata(false));
        public static readonly DependencyProperty SetBackreferenceProperty = DependencyProperty.Register("SetBackreference", typeof(MusicControlViewModel), typeof(SongBox), new PropertyMetadata(on_BackreferenceChange));

        public List<GoogleMusicSong> SongSource
        {
            get { return (List<GoogleMusicSong>) GetValue(SongSourceProperty); }
            set { SetValue(SongSourceProperty, value); }
        }

        public bool IsShuffled
        {
            get { return (bool) GetValue(IsShuffledProperty); }
            set { SetValue(IsShuffledProperty, value); }
        }

        public MusicControlViewModel SetBackreference
        {
            get { return (MusicControlViewModel)GetValue(SetBackreferenceProperty); }
            set { SetValue(SetBackreferenceProperty, value); }
        }

        private List<GoogleMusicSong> currentSongs;
        public List<GoogleMusicSong> CurrentSongs
        {
            get { return currentSongs; }
            private set
            {
                currentSongs = value;
                int songCount = 0;

                if (currentSongs != null)
                {
                    songCount = currentSongs.Count;
                }

                List<String> items = new List<string>(songCount);
                for (int i = 0; i < songCount; i++)
                    items.Add(currentSongs.ElementAt(i).Title);

                this.ItemsSource = items;
            }
        }

        private ICommand shuffleCommand;
        public ICommand ShuffleCommand
        {
            get
            {
                return shuffleCommand ?? (shuffleCommand = new RelayCommand(Shuffle));
            }
        }

        private ICommand unshuffleCommand;
        public ICommand UnshuffleCommand
        {
            get
            {
                return unshuffleCommand ?? (unshuffleCommand = new RelayCommand(Unshuffle));
            }
        }

        public SongBox()
        {
            InitializeComponent();

            this.IsShuffled = false;
        }

        public void Shuffle()
        {
            (new Thread(new ThreadStart(delegate { doShuffle(); }))).Start();
        }

        private void doShuffle()
        {
            Dispatcher.Invoke(DelegateHelper.CreateDelegate(delegate { this.IsShuffled = true; }));


            List<GoogleMusicSong> mySongSource = new List<GoogleMusicSong>();
            Dispatcher.Invoke(DelegateHelper.CreateDelegate(delegate {mySongSource = this.SongSource.ToList(); }));

            List<GoogleMusicSong> resultingList = new List<GoogleMusicSong>(mySongSource.Count);
            TimeSpan seedTimeSpan = DateTime.Now.Subtract(new DateTime(1993, 1, 8));
            int seed = (int)seedTimeSpan.Ticks;
            Random random = new Random(seed);
            while (mySongSource.Count > 0)
            {
                int randLoc = random.Next(mySongSource.Count);
                resultingList.Add(mySongSource.ElementAt(randLoc));
                mySongSource.RemoveAt(randLoc);
            }

            Dispatcher.Invoke(DelegateHelper.CreateDelegate(delegate { this.CurrentSongs = resultingList; }));
        }

        public void Unshuffle()
        {
            doUnshuffle();
        }

        private void doUnshuffle()
        {
            this.IsShuffled = false;
            this.CurrentSongs = this.SongSource;
        }

        private static void on_BasicItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SongBox owner = (SongBox)d;
            owner.CurrentSongs = e.NewValue as List<GoogleMusicSong>;
        }

        private static void on_BackreferenceChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SongBox owner = (SongBox)d;
            MusicControlViewModel backref = (MusicControlViewModel)e.NewValue;
            backref.RegisterSongBox(owner);
        }
    }
}
