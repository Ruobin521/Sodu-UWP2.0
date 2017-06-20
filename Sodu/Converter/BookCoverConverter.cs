using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Sodu.Core.Config;

namespace Sodu.Converter
{
    public class BookCoverConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return null;
            }
            var filePath = AppDataPath.GetBookCoverPath(value.ToString());
            return filePath == null ? new BitmapImage(new Uri("ms-appx:Assets/Icon/cover.png", UriKind.Absolute)) : new BitmapImage(new Uri(filePath, UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
