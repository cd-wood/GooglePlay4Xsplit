using GoogleMusicAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePlay4XSplit.Model
{
    public class GoogleMusicSongComparer : IComparer<GoogleMusicSong>
    {
        public int Compare(GoogleMusicSong x, GoogleMusicSong y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
