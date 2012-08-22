using GooglePlay4XSplit.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ScrollbarWithValue.xaml
    /// </summary>
    public partial class SliderWithValue : UserControl
    {
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Double), typeof(SliderWithValue), new PropertyMetadata(on_ValuePropertyChanged));
        private static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register("Precision", typeof(Int32), typeof(SliderWithValue), new PropertyMetadata(on_PrecisionPropertyChanged));
        private static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register("MinimumValue", typeof(Double), typeof(SliderWithValue), new PropertyMetadata(on_MinimumValuePropertyChanged));
        private static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register("MaximumValue", typeof(Double), typeof(SliderWithValue), new PropertyMetadata(on_MaximumValuePropertyChanged));

        public Double Value
        {
            get { return (Double) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Int32 Precision
        {
            get { return (Int32)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        public Double MinimumValue
        {
            get { return (Double)GetValue(MinimumValueProperty); }
            set { SetValue(MinimumValueProperty, value); }
        }

        public Double MaximumValue
        {
            get { return (Double)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        private readonly SliderWithValueViewModel viewModel = new SliderWithValueViewModel();
        public SliderWithValueViewModel ViewModel { get { return viewModel; } }

        public SliderWithValue()
        {
            InitializeComponent();
        }

        private static void on_ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as SliderWithValue;
            Double newValue = (Double)e.NewValue;

            if (owner != null)
            {
                owner.ViewModel.TextValue = newValue.ToString();
            }
        }

        private static void on_PrecisionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as SliderWithValue;
            int newValue = (int)e.NewValue;

            if (owner != null)
            {
                owner.ViewModel.Precision = newValue;
            }
        }

        private static void on_MinimumValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as SliderWithValue;
            Double newValue = (Double)e.NewValue;

            if (owner != null)
            {
                owner.ViewModel.MinimumValue = newValue;
                owner.slider.SmallChange = 0.01 * (owner.MaximumValue - newValue);
                owner.slider.LargeChange = 0.1 * (owner.MaximumValue - newValue);
            }
        }

        private static void on_MaximumValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as SliderWithValue;
            Double newValue = (Double)e.NewValue;

            if (owner != null)
            {
                owner.ViewModel.MaximumValue = newValue;
                owner.slider.SmallChange = 0.01 * (newValue - owner.MinimumValue);
                owner.slider.LargeChange = 0.1 * (newValue - owner.MinimumValue);
            }
        }

        private void on_SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Value = e.NewValue;
        }
    }
}
