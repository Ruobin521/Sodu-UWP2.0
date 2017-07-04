using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Config;
using Sodu.Core.Entity;
using Sodu.Core.Util;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UserControl
{
    public sealed partial class CoverImage : Windows.UI.Xaml.Controls.UserControl
    {

        public static readonly DependencyProperty CurrentBookProperty = DependencyProperty.Register(
            "CurrentBook", typeof(Book), typeof(CoverImage), new PropertyMetadata(default(Book), OnBookValueChanged));

        private static void OnBookValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CoverImage;
            var book = control?.CurrentBook;

            //  var book = DataContext as Book;
            if (book == null)
            {
                if (control != null)
                {
                    control.ImageCover.Source = null;
                }
                return;
            }
            var filePath = AppDataPath.GetBookCoverPath(book.BookId);
            if (filePath == null)
            {
                if (!string.IsNullOrEmpty(book.Cover))
                {
                    DownloadCoverHelper.SaveHttpImage(AppDataPath.GetBookCoverFolderPath(), book.BookId + ".jpg",
                        book.Cover,
                        control.SetCoverImage);
                }
                else
                {
                    control.ImageCover.Source = null;
                }
            }
            else
            {
                control.ImageCover.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            }
        }

        public Book CurrentBook
        {
            get { return (Book)GetValue(CurrentBookProperty); }
            set { SetValue(CurrentBookProperty, value); }
        }
        public CoverImage()
        {
            this.InitializeComponent();

            //  this.Loaded += CoverImage_Loaded;
        }

        private void CoverImage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var book = DataContext as Book;
            if (book == null)
            {
                return;
            }
            var filePath = AppDataPath.GetBookCoverPath(book.BookId);
            if (filePath == null)
            {
                if (!string.IsNullOrEmpty(book.Cover))
                {
                    DownloadCoverHelper.SaveHttpImage(AppDataPath.GetBookCoverFolderPath(), book.BookId + ".jpg",
                        book.Cover,
                        SetCoverImage);
                }
                else
                {
                    this.ImageCover.Source = null;
                }
            }
            else
            {
                this.ImageCover.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            }
        }

        private void CoverImage_Loaded(object sender, RoutedEventArgs e)
        {


        }
        public void SetCoverImage()
        {
            var book = CurrentBook as Book;
            var filePath = AppDataPath.GetBookCoverPath(book?.BookId);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.ImageCover.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            });

        }
    }
}
