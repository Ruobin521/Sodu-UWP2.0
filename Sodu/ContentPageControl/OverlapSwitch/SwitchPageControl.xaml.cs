using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Sodu.Service;
using Sodu.View;
using Sodu.ViewModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.ContentPageControl
{
    public sealed partial class SwitchPageControl
    {
        private int Index { get; set; }
        private double x = 0;//用来接收手势水平滑动的长度

        public SwitchPageControl()
        {
            this.InitializeComponent();

            ManipulationCompleted += The_ManipulationCompleted;//订阅手势滑动结束后的事件
            ManipulationStarted += The_ManipulationStarted;   //订阅手势滑动结束后的事件
            ManipulationDelta += The_ManipulationDelta;//订阅手势滑动事件


            Messenger.Default.Register<string>(this, "ContentTextChanged", OnCurrentPageContentChanged);
            this.Loaded += SwitchPageControl_Loaded;

            Grid.Tapped += Grid_Tapped;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var point = e.GetPosition(sender as UIElement);

            //点击中间区域
            if (point.X <= Window.Current.Bounds.Width / 3)
            {
                SwithToPre();
            }

            if (point.X >= Window.Current.Bounds.Width / 3 * 2)
            {
                SwitchToNext();
            }
        }

        private void OnCurrentPageContentChanged(string obj)
        {
           // Item1.Text = obj;
        }

        private void SwitchPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            SizeChanged -= SwitchPageControl_SizeChanged;
            SizeChanged += SwitchPageControl_SizeChanged;
        }


        private void SwitchPageControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var vm = (OnlineContentPageViewModel)DataContext;

            //var str = vm.SpiltContent(new Tuple<double, double>(
            //      Item1.ActualWidth,
            //      Item1.ActualHeight
            //      ));

            //Item1.Text = str;
        }

        private void The_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            x = 0;
        }

        private void The_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var vm = (OnlineContentPageViewModel)DataContext;
            if (vm.IsLoading)
            {
                return;
            }
            x += e.Delta.Translation.X;
        }


        private void The_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var vm = (OnlineContentPageViewModel)DataContext;

            //上一页（章节）
            if (x > 50)
            {
                SwithToPre();
            }
            // 下一页（章节）
            else if (x < -50)
            {
                SwitchToNext();
            }
        }



        public async Task<bool> OnSwitchToNextPage()
        {
            Item2.Text = Item1.Text;

            var vm = (OnlineContentPageViewModel)DataContext;

            if (vm.PageIndex < vm.PageCount - 1)
            {
                if (!vm.DicContentCache.ContainsKey(vm.CurrentCatalog.CatalogUrl))
                {
                    return false;
                }

                var value = vm.DicContentCache[vm.CurrentCatalog.CatalogUrl];
                if (value?.Item1 != null && value.Item1.Count > 0)
                {
                    Item1.Text = value.Item1[vm.PageIndex + 1];

                    vm.PageIndex += 1;

                    return true;
                }
            }
            else
            {
                if (vm.IsPreLoadingCatalog)
                {
                    ToastHelper.ShowMessage("目录正在加载中，请稍后");
                    return false;
                }

                var value = await vm.GetCatalogDataByDirection(CatalogDirection.Next, true);

                if (value == null)
                {
                    NavigationService.NavigateTo(typeof(CatalogPage));
                    return false;
                }

                if (value?.Item1 != null && value.Item1.Count > 0)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        //vm.ContentText = value.Item2;
                        //Item1.Text = value.Item1[0];
                        //vm.PageIndex = 0;
                        //vm.PageCount = value.Item1.Count;
                        //vm.CurrentCatalog = value.Item3;
                    });

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> OnSwitchToPrePage()
        {
            Item2.Text = Item1.Text;

            var vm = (OnlineContentPageViewModel)DataContext;


            if (vm.PageIndex > 0)
            {
                var value = vm.DicContentCache[vm.CurrentCatalog.CatalogUrl];
                if (value?.Item1 != null && value.Item1.Count > 0)
                {
                    Item1.Text = value.Item1[vm.PageIndex - 1];
                    vm.PageIndex -= 1;
                    return true;
                }
            }
            else
            {

                var value = await vm.GetCatalogDataByDirection(CatalogDirection.Pre, true);

                if (value == null)
                {
                    NavigationService.NavigateTo(typeof(CatalogPage));
                    return false;
                }

                if (value.Item1 != null && value.Item1.Count > 0)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        //vm.ContentText = value.Item2;
                        //Item1.Text = value.Item1.LastOrDefault();
                        //vm.PageIndex = value.Item1.Count - 1;
                        //vm.PageCount = value.Item1.Count;
                        //vm.CurrentCatalog = value.Item3;
                    });

                    return true;
                }
            }

            return false;
        }

        private async void SwithToPre()
        {
            Item2.Text = Item1.Text;

            var result = await OnSwitchToPrePage();

            if (result)
            {
                Item2.AnimationToRight();
            }
        }

        private async void SwitchToNext()
        {
            Item2.Text = Item1.Text;

            var result = await OnSwitchToNextPage();

            if (result)
            {
                Item2.AnimationToLeft();
            }
            else
            {
                NavigationService.NavigateTo(typeof(CatalogPage));
            }
        }

    }
}
