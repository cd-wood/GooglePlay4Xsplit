﻿using GooglePlay4XSplit.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GooglePlay4XSplit.View
{
    /// <summary>
    /// Interaction logic for MusicControlView.xaml
    /// </summary>
    public partial class MusicControlView : UserControl
    {
        public MusicControlView()
        {
            InitializeComponent();
        }

        public void SetViewModel(MusicControlViewModel viewModel)
        {
            this.DataContext = viewModel;
        }
    }
}
