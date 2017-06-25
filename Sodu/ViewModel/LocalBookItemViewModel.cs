using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using Sodu.Core.Entity;

namespace Sodu.ViewModel
{
    public class LocalBookItemViewModel : ViewModelBase
    {
        private Book _currentBook;
        public Book CurrentBook
        {
            get
            {
                return _currentBook;
            }
            set
            {
                Set(ref _currentBook, value);
            }
        }


        public LocalBookItemViewModel()
        {

        }

        public LocalBookItemViewModel(Book book)
        {
            CurrentBook = book;
        }

    }
}
