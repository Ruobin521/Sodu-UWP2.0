using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Threading;
using Sodu.Control;
using Sodu.Core.Util;
using Sodu.ViewModel;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sodu.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OnlineContentPage
    {
        private bool IsAnimating { get; set; }
        private bool IsShow { get; set; }


        public OnlineContentPage()
        {
            InitializeComponent();

            TopBar.Visibility = BottomBar.Visibility = FontSetingPanel.Visibility = Visibility.Collapsed;

            BlackControl.Visibility = Visibility.Visible;

            BtnAdd.Visibility = Visibility.Collapsed;

            MenuBarShow.Completed += MenuSb_Completed;
            MenuBarHide.Completed += MenuSb_Completed;


            SizeChanged += OnlineContentPage_SizeChanged;
        }

        private void OnlineContentPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var vm = (OnlineContentPageViewModel)DataContext;
            vm.SetSwihchControl(SwitchControl);
        }
 

      


        private void MenuSb_Completed(object sender, object e)
        {
            IsAnimating = false;
            IsShow = !IsShow;

            // Viewer.IsEnabled = !IsShow;

            if (IsShow)
            {
                var bookId = ViewModelInstance.Instance.OnlineBookContent.CurrentBook.BookId;

                var ifExist = ViewModelInstance.Instance.LocalBookPage.CheckBookExist(bookId);

                BtnAdd.Visibility = ifExist ? Visibility.Collapsed : Visibility.Visible;

            }
            else
            {
                BtnAdd.Visibility = Visibility.Collapsed;
            }
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(sender as UIElement);

            //点击中间区域
            if (point.X > Window.Current.Bounds.Width / 3 && point.X < Window.Current.Bounds.Width / 3 * 2)
            {
                SetMenuVisibility();
            }
        }


        private void SetMenuVisibility()
        {
            if (IsAnimating)
            {
                return;
            }
            IsAnimating = true;

            FontSetingPanel.Visibility = Visibility.Collapsed;

            if (IsShow)
            {
                MenuBarHide.Begin();
                if (PlatformHelper.IsMobileDevice)
                {
                    // await StatusBar.GetForCurrentView().HideAsync();
                }
            }
            else
            {
                MenuBarShow.Begin();
                if (PlatformHelper.IsMobileDevice)
                {
                    //  await StatusBar.GetForCurrentView().ShowAsync();
                }
            }
        }

        private void Prevent_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void BtnFontSetingPanel_OnClick(object sender, RoutedEventArgs e)
        {
            FontSetingPanel.Visibility = (FontSetingPanel.Visibility == Visibility.Collapsed)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        private void BtnNightMode_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (TabbarButton)sender;

            btn.Label = btn.Label == "夜间模式" ? "日间模式" : "夜间模式";
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            SetMenuVisibility();
        }


    }
}
