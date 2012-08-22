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

namespace GooglePlay4XSplit.View
{
    /// <summary>
    /// Interaction logic for PopUp.xaml
    /// </summary>
    public partial class PopUp : Window
    {
        public enum MessageType { ERROR, WARNING, INFO };

        public PopUp()
        {
            InitializeComponent();
        }

        public static void DisplayMessage(String message, String caption, MessageType type)
        {
            MessageBoxImage image;
            switch (type)
            {
                case MessageType.ERROR:
                    image = MessageBoxImage.Error;
                    break;
                case MessageType.WARNING:
                    image = MessageBoxImage.Warning;
                    break;
                case MessageType.INFO:
                    image = MessageBoxImage.Information;
                    break;
                default:
                    image = MessageBoxImage.None;
                    break;
            }

            MessageBox.Show(message, caption, MessageBoxButton.OK, image);
        }
    }
}
