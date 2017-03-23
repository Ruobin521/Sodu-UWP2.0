using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Sodu.ViewModel
{
    public class ViewModelInstance : ViewModelBase
    {

        private static readonly object SynObject = new object();

        private static ViewModelInstance _instance;
        public static ViewModelInstance Instance
        {
            get
            {
                lock (SynObject)
                {
                    return _instance ?? (_instance = new ViewModelInstance());
                }
            }
        }

        private MainViewModel _main;
        public MainViewModel Main
        {
            get { return _main ?? (_main = new MainViewModel()); }
            set
            {
                Set(ref _main, value);
            }
        }
    }
}
