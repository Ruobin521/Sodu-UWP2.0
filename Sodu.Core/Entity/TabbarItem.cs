using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;

namespace Sodu.Core.Entity
{
    public class TabbarItem : ViewModelBase
    {

        public int Index { get; set; }
        public string PathData { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                Set(ref _isSelected, value);

            }
        }
        public string Title { get; set; }
        public UIElement ContentElement { get; set; }

    }
}
