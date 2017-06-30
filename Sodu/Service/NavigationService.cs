using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sodu.Service;

namespace Sodu
{
    public class NavigationService
    {
        private static DateTime FirstTime { get; set; }
        private static DateTime SecondTime { get; set; }
        public static Frame ContentFrame { get; } = App.RootFrame;

        public static void NavigateTo(Type type, object para = null)
        {
            try
            {
                ContentFrame.Navigate(type, para);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void GoBack()
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
            else
            {
                if (CheckIfShutDown())
                {
                    Application.Current.Exit();
                }
                else
                {
                    ToastHelper.ShowMessage("再按一次返回键退出");
                }
            }

        }

        public static void ClearHistory()
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.BackStack.Clear();
            }
            if (ContentFrame.CanGoForward)
            {
                ContentFrame.ForwardStack.Clear();
            }
        }


        private static bool CheckIfShutDown()
        {
            if (FirstTime == DateTime.MinValue)
            {
                FirstTime = DateTime.Now;
                return false;
            }

            else if (FirstTime != DateTime.MinValue)
            {
                SecondTime = DateTime.Now;
                if ((SecondTime - FirstTime).TotalSeconds > 2)
                {
                    FirstTime = DateTime.Now;
                    return false;
                }
                else if ((SecondTime - FirstTime).TotalSeconds < 2)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
