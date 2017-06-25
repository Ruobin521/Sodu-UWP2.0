using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Sodu.Core.Config;
using Sodu.Core.Entity;
using Sodu.Core.Util;

namespace Sodu.Converter
{
    public class BookCoverConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var defaultValue = new BitmapImage(new Uri("ms-appx:Assets/Icon/cover.png", UriKind.Absolute));
            var book = value as Book;
            if (string.IsNullOrEmpty(book?.Cover))
            {
                return defaultValue;
            }
            var filePath = AppDataPath.GetBookCoverPath(book.BookId);
            if (filePath == null)
            {
                DownloadCoverHelper.SaveHttpImage(AppDataPath.GetBookCoverFolderPath(), book.BookId + ".jpg", book.Cover);
            }
            return filePath == null ? new BitmapImage(new Uri(book.Cover, UriKind.RelativeOrAbsolute)) : new BitmapImage(new Uri(filePath, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
