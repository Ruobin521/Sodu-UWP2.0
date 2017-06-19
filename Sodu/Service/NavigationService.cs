using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Sodu
{
    public class NavigationService
    {
        private static Frame ContentFrame { get; } = App.RootFrame;

        public static void NavigateTo(Type type, object para = null)
        {
            ContentFrame.Navigate(type);
        }

        public static void GoBack()
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }

        }
    }
}
