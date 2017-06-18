using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Sodu.Converter
{
    public class RankChangValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || value.ToString().Equals("-"))
            {
                return null;
            }
            var change = value.ToString();
            return change.StartsWith("-") ? new BitmapImage(new Uri("ms-appx:Assets/down.png", UriKind.Absolute)) : new BitmapImage(new Uri("ms-appx:Assets/up.png", UriKind.Absolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
