using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Threading;
using Sodu.ContentPageControl.ScrollSwitchPage;
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

            MenuBarShow.Completed += MenuBarShow_Completed;
            MenuBarHide.Completed += MenuBarHide_Completed;

            NavigationCacheMode = NavigationCacheMode.Enabled;

            this.Loaded += OnlineContentPage_Loaded;
        }

        private void MenuBarHide_Completed(object sender, object e)
        {
            IsShow = false;
            IsAnimating = false;

            ScrollControl.IsEnabled = true;
            SwitchControl.IsEnabled = true;

            BtnAdd.Visibility = Visibility.Collapsed;
        }

        private void MenuBarShow_Completed(object sender, object e)
        {
            IsShow = true;
            IsAnimating = false;

            ScrollControl.IsEnabled = false;
            SwitchControl.IsEnabled = false;

            Task.Run(() =>
            {
                var bookId = ViewModelInstance.Instance.OnlineBookContent.CurrentBook.BookId;

                var ifExist = ViewModelInstance.Instance.LocalBookPage.CheckBookExist(bookId);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    BtnAdd.Visibility = ifExist ? Visibility.Collapsed : Visibility.Visible;
                });
            });

        }

        private void OnlineContentPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetMenuVisibility(false);
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(sender as UIElement);

            //点击中间区域
            if (point.X > Window.Current.Bounds.Width / 3 && point.X < Window.Current.Bounds.Width / 3 * 2)
            {
                SetMenuVisibility(!IsShow);
            }
        }


        private void SetMenuVisibility(bool isShow)
        {
            if (IsAnimating)
            {
                return;
            }
            IsAnimating = true;

            FontSetingPanel.Visibility = Visibility.Collapsed;

            if (!isShow)
            {
                MenuBarHide.Begin();

            }
            else
            {
                MenuBarShow.Begin();
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
            SetMenuVisibility(false);
        }

        public Tuple<double,double> GetControlSize()
        {
            return SwitchControl.GetControlSize();
        }
    }
}
