using GooglePlay4XSplit.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePlay4XSplit.ViewModel
{
    public class SliderWithValueViewModel : ObservableObject
    {
        private int precision;
        public int Precision
        {
            get { return precision; }
            set
            {
                precision = value;
                RaisePropertyChanged("Precision");
            }
        }

        private Double minimumValue;
        public Double MinimumValue
        {
            get { return minimumValue; }
            set { minimumValue = value; RaisePropertyChanged("MinimumValue"); }
        }

        private Double maximumValue;
        public Double MaximumValue
        {
            get { return maximumValue; }
            set { maximumValue = value; RaisePropertyChanged("MaximumValue"); }
        }

        private String textValue = "";
        public String TextValue
        {
            get { return textValue ?? ""; }
            set
            {
                Double newValue = 0;
                if (Double.TryParse(value, out newValue))
                {
                    Double roundedValue = Math.Round(newValue, this.Precision);
                    if (roundedValue < this.MinimumValue)
                        newValue = this.MinimumValue;
                    else if (roundedValue > this.MaximumValue)
                        newValue = this.MaximumValue;
                    else
                        newValue = roundedValue;
                }
                else
                {
                    newValue = this.SliderValue;
                }
                ValueChanged(newValue);
            }
        }

        private Double sliderValue;
        public Double SliderValue
        {
            get { return sliderValue; }
            set
            {
                Double roundedValue = Math.Round(value, this.Precision);
                ValueChanged(roundedValue);
            }
        }

        public SliderWithValueViewModel()
        {
            this.TextValue = "";
        }

        private void ValueChanged(Double newValue)
        {
            if (this.SliderValue != newValue)
            {
                this.sliderValue = newValue;
                RaisePropertyChanged("SliderValue");
            }
            if (!this.TextValue.Equals(newValue.ToString()))
            {
                this.textValue = newValue.ToString();
                RaisePropertyChanged("TextValue");
            }
        }
    }
}
