using GooglePlay4XSplit.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GooglePlay4XSplit.ViewModel
{
    public class MusicControlPanelViewModel : ObservableObject
    {
        private readonly MusicControlViewModel musicControl;
        public MusicControlViewModel MusicControl
        {
            get { return musicControl; }
        }

        private bool autoUpdateSize;
        public bool AutoUpdateSize
        {
            get { return autoUpdateSize; }
            set
            {
                autoUpdateSize = value;
                RaisePropertyChanged("AutoUpdateSize");
            }
        }

        private Double controlWidth;
        public Double ControlWidth
        {
            get { return controlWidth; }
            set
            {
                controlWidth = value;
                if (this.AutoUpdateSize)
                    RaisePropertyChanged("ControlWidth");
            }
        }

        private Double controlHeight;
        public Double ControlHeight
        {
            get { return controlHeight; }
            set
            {
                controlHeight = value;
                if (this.AutoUpdateSize)
                    RaisePropertyChanged("ControlHeight");
            }
        }

        private ICommand updateControlSizeCommand;
        public ICommand UpdateControlSizeCommand
        {
            get { return updateControlSizeCommand ?? (updateControlSizeCommand = new RelayCommand(UpdateControlSize)); }
        }

        public MusicControlPanelViewModel(MusicControlViewModel musicControl)
        {
            this.musicControl = musicControl;
        }

        private void UpdateControlSize()
        {
            RaisePropertyChanged("ControlWidth");
            RaisePropertyChanged("ControlHeight");
        }
    }
}
