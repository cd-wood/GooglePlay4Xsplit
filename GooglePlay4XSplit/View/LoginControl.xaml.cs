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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using GooglePlay4XSplit.Model;
using System.Threading;

namespace GooglePlay4XSplit.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private static readonly int MAX_CONNECTING_STATE = 7;
        private static readonly int CONNECTING_STATE_INTERVAL = 500;
        private static readonly int MAX_LOGIN_WAIT = 7000;

        private System.Timers.Timer connecting_timer;
        private int connecting_text_state;

        private delegate void DummyDelegate();
        private static DummyDelegate DelegateCast(DummyDelegate d)
        {
            return d;
        }

        public LoginControl()
        {
            InitializeComponent();
            connecting_timer = new System.Timers.Timer(CONNECTING_STATE_INTERVAL);
            connecting_timer.Elapsed += connectingTimer_Elapsed;
            HideConnecting();
        }

        private void connectingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            connecting_text.Dispatcher.Invoke(DelegateCast(delegate
            {
                connecting_text_state++;
                if (connecting_text_state > MAX_CONNECTING_STATE)
                    connecting_text_state = 0;
                String text = "Connecting";
                for (int i = 0; i < connecting_text_state; i++)
                    text += ".";
                connecting_text.Content = text;
                connecting_text.UpdateLayout();
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void HideConnecting()
        {
            connecting_grid.Visibility = System.Windows.Visibility.Hidden;
            login_grid.IsEnabled = true;
            connecting_timer.Stop();
            super_grid.UpdateLayout();
        }

        private void ShowConnecting()
        {
            login_grid.IsEnabled = false;
            connecting_grid.Visibility = System.Windows.Visibility.Visible;
            connecting_text.Content = "Connecting";
            connecting_text_state = 0;
            super_grid.UpdateLayout();
            connecting_timer.Start();
        }

        private void on_LoginClick(object sender, RoutedEventArgs e)
        {
            var login_name = this.login_input.Text;
            var login_pwd = this.password_input.Password;
            password_input.Password = "";

            if ((login_name.Length == 0) || (login_pwd.Length == 0))
                return;

            var vm = (LoginControlViewModel)this.DataContext;

            if ((vm == null) || (vm.GooglePlayHandler == null))
            {
                // Error: DataContext not set or not initialized
                return;
            }

            Thread loginThread = new Thread(new ThreadStart(delegate
            {
                super_grid.Dispatcher.BeginInvoke(DelegateCast(delegate { ShowConnecting(); }), System.Windows.Threading.DispatcherPriority.Normal);
                // Set up Interrupt action
                var interruptTimer = new System.Timers.Timer(MAX_LOGIN_WAIT);
                interruptTimer.Elapsed += delegate
                {
                    interruptTimer.Stop();
                    vm.GooglePlayHandler.InterruptAction();
                };
                ThreadResult loginResult = vm.GooglePlayHandler.AttemptLogin(login_name, login_pwd);
                interruptTimer.Stop();

                switch (loginResult)
                {
                    case ThreadResult.LOGGEDIN:
                        vm.LoginSuccessful(this);
                        break;
                    case ThreadResult.UNINIT:
                        DisplayError("Error: Google Play API Not Initialized");
                        break;
                    case ThreadResult.BUSY:
                        DisplayError("Google Play API is currently busy.", "API Busy", MessageBoxImage.Exclamation);
                        break;
                    case ThreadResult.TIMEDOUT:
                        DisplayError("Unable to connect.", "Connection Timedout", MessageBoxImage.Warning);
                        break;
                    case ThreadResult.ERROR:
                        DisplayError("Error: " + vm.GooglePlayHandler.GetErrorException());
                        break;
                    default:
                        // Shouldn't be reached?
                        break;
                }
                super_grid.Dispatcher.BeginInvoke(DelegateCast(delegate { HideConnecting(); }), System.Windows.Threading.DispatcherPriority.Normal);
            }));
            loginThread.Start();            
        }

        private void DisplayError(String message, String caption = "Error", MessageBoxImage image = MessageBoxImage.Error)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, image);
        }
    }
}
