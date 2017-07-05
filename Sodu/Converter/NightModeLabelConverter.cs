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
    public class NightModeLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isNightMode = (bool)value;

            return isNightMode ? "日间模式" : "夜间模式";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
