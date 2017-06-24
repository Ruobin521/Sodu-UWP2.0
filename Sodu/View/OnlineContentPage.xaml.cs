using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Sodu.Contants;
using Sodu.Core.Util;
using Sodu.ViewModel;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sodu.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OnlineContentPage : Page
    {

        private bool isAnimating { get; set; }
        private bool isShow { get; set; }
        public OnlineContentPage()
        {
            this.InitializeComponent();

            TopBar.Visibility = BottomBar.Visibility = FontSetingPanel.Visibility = Visibility.Collapsed;

            BlackControl.Visibility = Visibility.Visible;

            MenuBarShow.Completed += MenuSb_Completed;
            MenuBarHide.Completed += MenuSb_Completed;

        }


        private void MenuSb_Completed(object sender, object e)
        {
            isAnimating = false;
            isShow = !isShow;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(sender as UIElement);

            //上一章
            if (point.X > Window.Current.Bounds.Width / 3 && point.X < Window.Current.Bounds.Width / 3 * 2)
            {
                SetMenuVisibility();
            }
            else
            {
                // OnTapped(point);
            }
        }


        private async void SetMenuVisibility()
        {
            if (isAnimating)
            {
                return;
            }
            isAnimating = true;

            FontSetingPanel.Visibility = Visibility.Collapsed;

            if (isShow)
            {
                MenuBarHide.Begin();
                if (PlatformHelper.IsMobileDevice)
                {
                    await StatusBar.GetForCurrentView().HideAsync();
                }
            }
            else
            {
                MenuBarShow.Begin();

                if (PlatformHelper.IsMobileDevice)
                {
                    await StatusBar.GetForCurrentView().ShowAsync();
                }
            }
        }

        private void Prevent_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void BtnFontSetingPanel_OnClick(object sender, RoutedEventArgs e)
        {
            FontSetingPanel.Visibility = (FontSetingPanel.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            FontSetingPanel.Visibility = Visibility.Collapsed;
        }
    }
}
