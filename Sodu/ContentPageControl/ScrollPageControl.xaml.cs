using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Sodu.View;
using Sodu.ViewModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.ContentPageControl
{
    public sealed partial class ScrollPageControl : Windows.UI.Xaml.Controls.UserControl
    {

        private bool IsLoading { get; set; }

        public ScrollPageControl()
        {
            this.InitializeComponent();
            Viewer.ViewChanged += Viewer_ViewChanged;

            Messenger.Default.Register<string>(this, "ContentTextChanged", UpdateScrollContent);
        }

        private async void UpdateScrollContent(string str)
        {
            IsLoading = true;

            var windowHeight = Window.Current.Bounds.Height;

            ContentText.Text = "";
            ContentText.Height = double.NaN;
            await Task.Delay(1);

            Viewer.ChangeView(0, 0, null, true);

            if (string.IsNullOrEmpty(str))
            {
                ContentText.Height = windowHeight + 200;
                await Task.Delay(1);
            }
            else
            {
                ContentText.Text = str;
                await Task.Delay(1);
                var textHeight = ContentText.ActualHeight;
                if (textHeight < windowHeight + 100)
                {
                    ContentText.Height = windowHeight + 200;
                }
                else
                {
                    ContentText.Height = textHeight;
                }
                await Task.Delay(1);
            }

            Viewer.ChangeView(0, 50, null, Viewer.VerticalOffset > 50);
            IsLoading = false;
        }



        private void Viewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var vm = (OnlineContentPageViewModel)DataContext;

            if (IsLoading || vm.IsLoading)
            {
                return;
            }

            //所有内容垂直高度 - 当前滚动的高度
            var v1 = Viewer.ExtentHeight - Viewer.VerticalOffset;

            //可视区域的高度
            var v2 = Viewer.ViewportHeight;

            //向上滚动
            if (Math.Abs((Viewer.VerticalOffset - 0.1)) <= 0.1)
            {
                if (!e.IsIntermediate)
                {
                    SwitchToPre();
                }
            }

            else if (Viewer.ExtentHeight > Viewer.ViewportHeight && v1 <= v2 + 1)
            {
                if (!e.IsIntermediate)
                {
                    SwitchToNext();
                }
            }
        }


        private async void SwitchToPre()
        {
            var vm = (OnlineContentPageViewModel)DataContext;

            if (IsLoading)
            {
                return;
            }

            vm.SwitchCatalog(CatalogDirection.Pre);
            await Task.Delay(5);

        }

        private async void SwitchToNext()
        {
            var vm = (OnlineContentPageViewModel)DataContext;
            try
            {
                if (IsLoading)
                {
                    return;
                }
                var value = vm.GetCatalogByDirction(CatalogDirection.Next);

                if (value == null)
                {
                    await Task.Delay(10);
                    NavigationService.NavigateTo(typeof(CatalogPage));
                }
                else
                {
                    vm.SwitchCatalog(CatalogDirection.Next);
                    await Task.Delay(5);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
