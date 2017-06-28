using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Sodu.Service;
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
        private async void The_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var vm = (OnlineContentPageViewModel)DataContext;

            //上一章
            if (x > 85)
            {
                Item2.Text = Item1.Text;

                var value = await vm.GetCatalogDataByDirection(CatalogDirection.Pre, false);
                if (value != null)
                {
                    Item1.Text = value.Item2;

                    var catalog = vm.GetCatalogByDirction(CatalogDirection.Pre);

                    if (catalog != null)
                    {
                        vm.CurrentCatalog = catalog.Item2;
                    }
                }

                AnimationToRight();
            }
            // 下一章
            else if (x < -85)
            {
                Item2.Text = Item1.Text;
                var value = await vm.GetCatalogDataByDirection(CatalogDirection.Next, true);
                if (value != null)
                {
                    Item1.Text = value.Item2;
                    var catalog = vm.GetCatalogByDirction(CatalogDirection.Next);
                    if (catalog != null)
                    {
                        vm.CurrentCatalog = catalog.Item2;
                    }
                }
                else
                {
                    ToastHelper.ShowMessage("没有下一章了");
                    return;
                }
                AnimationToLeft();
            }
        }

        public string GetNextPageText()
        {
            var vm = (OnlineContentPageViewModel)DataContext;
            var value = vm.GetCatalogByDirction(CatalogDirection.Current);

            if (value != null)
            {
                var pages = vm.DicContentCache[value.Item2.CatalogUrl];

                if (pages?.Item1 != null && pages.Item1.Count > 0 && Index <= pages.Item1.Count - 1)
                {
                    Item1.Text = pages.Item1?[Index + 1];
                    Index += 1;
                }
                else
                {
                    var next = vm.GetCatalogByDirction(CatalogDirection.Next);
                    var nextPages = vm.DicContentCache[value.Item2.CatalogUrl];
                    if (nextPages?.Item1 != null && nextPages.Item1.Count > 0)
                    {
                        Item1.Text = nextPages.Item1?[0];
                        Index += 1;
                    }
                }

            }
            return null;
        }


        /// <summary>
        /// 向后翻页
        /// </summary>
        public void AnimationToLeft()
        {
            Item2.AnimationToLeft();
        }

        //向前翻页
        public void AnimationToRight()
        {
            //Item1.Text = Item2.Text;

            //var vm = (OnlineContentPageViewModel)DataContext;

            //if (Index == 0)
            //{
            //    vm.SwitchCatalog(CatalogDirection.Pre);
            //}
            //var value = vm.GetCatalogByDirction(CatalogDirection.Pre);

            //if (value != null && vm.DicContentCache.ContainsKey(value.Item2.CatalogUrl))
            //{
            //    var pages = vm.DicContentCache[value.Item2.CatalogUrl];

            //    if (pages != null)
            //    {
            //        Item1.Text = pages.Item1?.LastOrDefault();
            //    }
            //}

            Item2.AnimationToRight();
        }



        public void SetPreAndNextText(string text)
        {
            Item1.Text = Item2.Text;

            var vm = (OnlineContentPageViewModel)DataContext;

            if (Index == 0)
            {
                vm.SwitchCatalog(CatalogDirection.Pre);
            }
            var value = vm.GetCatalogByDirction(CatalogDirection.Pre);

            if (value != null)
            {
                var pages = vm.DicContentCache[value.Item2.CatalogUrl];

                if (pages != null)
                {
                    Item1.Text = pages.Item1?.FirstOrDefault();
                }
            }

            Item2.AnimationToRight();
        }

        public void SetCurrentText()
        {
            var vm = (OnlineContentPageViewModel)DataContext;

            var value = vm.DicContentCache[vm.CurrentCatalog.CatalogUrl];

            if (value?.Item1 != null && value.Item1.Count > 0)
            {
                Item1.Text = value.Item1[Index];
                Item2.Text = value.Item1[Index];
            }
        }
    }
}
