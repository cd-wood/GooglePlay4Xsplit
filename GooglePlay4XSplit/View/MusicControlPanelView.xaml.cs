using GooglePlay4XSplit.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using XSplit.Wpf;

namespace GooglePlay4XSplit.View
{
    /// <summary>
    /// Interaction logic for MusicControlView.xaml
    /// </summary>
    public partial class MusicControlPanelView : UserControl
    {
        private static readonly int REFRESH_RATE = 20;

        public static readonly DependencyProperty OutputWidthProperty = DependencyProperty.Register("OutputWidth", typeof(int), typeof(MusicControlPanelView), new PropertyMetadata(on_OutputWidthChanged));
        public static readonly DependencyProperty OutputHeightProperty = DependencyProperty.Register("OutputHeight", typeof(int), typeof(MusicControlPanelView), new PropertyMetadata(on_OutputHeightChanged));
        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register("CanClose", typeof(bool), typeof(MusicControlPanelView), new PropertyMetadata(false));

        public int OutputWidth
        {
            get { return (int)GetValue(OutputWidthProperty); }
            set { SetValue(OutputWidthProperty, value); }
        }

        public int OutputHeight
        {
            get { return (int)GetValue(OutputHeightProperty); }
            set { SetValue(OutputHeightProperty, value); }
        }

        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        private readonly TimedBroadcasterPlugin broadcaster;

        public MusicControlPanelView()
        {
            InitializeComponent();
            this.Measure(new Size(this.Width, this.Height));
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));

            broadcaster = TimedBroadcasterPlugin.CreateInstance("3A56008B-E2EE-4180-95EB-8B1D9BC23ED7", contentView, this.OutputWidth, this.OutputHeight, REFRESH_RATE);
        }

        private void ResizeWidth()
        {
            this.contentView.Width = this.OutputWidth;

            broadcaster.Width = this.OutputWidth;
        }

        private void ResizeHeight()
        {
            this.contentView.Height = this.OutputHeight;

            broadcaster.Height = this.OutputHeight;
        }
        
        private static void on_OutputWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as MusicControlPanelView;
            owner.ResizeWidth();
        }

        private static void on_OutputHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as MusicControlPanelView;
            owner.ResizeHeight();
        }
    }
}
