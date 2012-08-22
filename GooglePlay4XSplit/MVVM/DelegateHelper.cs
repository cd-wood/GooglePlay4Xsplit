using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePlay4XSplit.MVVM
{
    public class DelegateHelper
    {
        public delegate void DummyDelegate();
        public static Delegate CreateDelegate(DummyDelegate d)
        {
            return d;
        }
    }
}
