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
using Sodu.Core.Config;
using Sodu.Core.Entity;
using Sodu.Core.Util;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UserControl
{
    public sealed partial class CoverImage : Windows.UI.Xaml.Controls.UserControl
    {
        public CoverImage()
        {
            this.InitializeComponent();

            this.Loaded += CoverImage_Loaded;
        }

        private void CoverImage_Loaded(object sender, RoutedEventArgs e)
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
                    DownloadCoverHelper.SaveHttpImage(AppDataPath.GetBookCoverFolderPath(), book.BookId + ".jpg", book.Cover,
                SetCoverImage);
                }
            }
            else
            {
                this.ImageCover.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
            }
        }
        public void SetCoverImage()
        {
            var book = DataContext as Book;
            var filePath = AppDataPath.GetBookCoverPath(book.BookId);

            this.ImageCover.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
        }
    }
}
