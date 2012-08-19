using GooglePlay4XSplit.Model;
using GooglePlay4XSplit.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GooglePlay4XSplit.ViewModel
{
    public delegate void SuccessfulLogin(object sender);

    public class LoginControlViewModel : ObservableObject
    {
        private event SuccessfulLogin afterLogin;

        private GooglePlayHandler gpHandler;
        public GooglePlayHandler GooglePlayHandler
        {
            get { return gpHandler; }
        }

        // This should not be created with default constructor
        private LoginControlViewModel()
        {
            gpHandler = null;
        }

        public LoginControlViewModel(GooglePlayHandler gph, SuccessfulLogin func)
        {
            this.gpHandler = gph;
            this.afterLogin += func;
        }

        public void LoginSuccessful(object loginControl)
        {
            afterLogin(loginControl);
        }
    }
}
