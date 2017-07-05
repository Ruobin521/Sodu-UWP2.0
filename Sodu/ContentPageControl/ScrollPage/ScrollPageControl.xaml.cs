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
        private BookContentPageViewModel vm;

        private bool IsLoadingContent { get; set; }

        public ScrollPageControl()
        {
            this.InitializeComponent();
            Viewer.ViewChanged += Viewer_ViewChanged;

            Messenger.Default.Register<string>(this, "CurrentCatalogContentChanged", OnCurrentCatalogContentChanged);

            vm = ViewModelInstance.Instance.BookContent;
        }

        private async void OnCurrentCatalogContentChanged(string str)
        {
            IsLoadingContent = true;

            var windowHeight = Window.Current.Bounds.Height;
            ContentText.Height = double.NaN;
            Viewer.ChangeView(0, 0, null, true);
            await Task.Delay(2);

            var textHeight = ContentText.ActualHeight;
            if (textHeight < windowHeight + 200)
            {
                ContentText.Height = windowHeight + 200;
                await Task.Delay(2);
            }
            else
            {
                ContentText.Height = textHeight;
            }

            Viewer.ChangeView(0, 50, null, Viewer.VerticalOffset > 50);

            IsLoadingContent = false;
        }
        private void Viewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (IsLoadingContent || vm == null || vm.IsLoading)
            {
                return;
            }

            //所有内容垂直高度 - 当前滚动的高度
            var v1 = Viewer.ExtentHeight - Viewer.VerticalOffset;

            //可视区域的高度
            var v2 = Viewer.ViewportHeight;

            //向上滚动
            if (Math.Abs(Viewer.VerticalOffset) <= 1.0)
            {
                if (!e.IsIntermediate)
                {
                    SwitchCatalog(CatalogDirection.Pre);
                }
            }

            else if (Viewer.ExtentHeight > Viewer.ViewportHeight && v1 <= v2 + 1)
            {
                if (!e.IsIntermediate)
                {
                    SwitchCatalog(CatalogDirection.Next);
                }
            }
        }


        private async void SwitchCatalog(CatalogDirection dir)
        {
            try
            {
                if (IsLoadingContent)
                {
                    return;
                }
                IsLoadingContent = true;
                await Task.Delay(0);
                var vm = (BookContentPageViewModel)DataContext;
                vm.ScrollToSwitchCurrentCatalog(dir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsLoadingContent = false;
            }

        }

    }
}
