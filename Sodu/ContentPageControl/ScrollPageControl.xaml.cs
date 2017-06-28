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


            ContentText.DataContextChanged += ContentText_DataContextChanged;
        }

        private async void ContentText_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var windowHeight = Window.Current.Bounds.Height;

            if (ContentText.DataContext == null)
            {
                ContentText.Text = "";
                ContentText.Height = windowHeight + 200;
                await Task.Delay(2);
                Viewer.ChangeView(0, 40, null, false);
            }
            else
            {
                ContentText.Height = double.NaN;
                ContentText.Text = ContentText.DataContext.ToString();
                await Task.Delay(2);
                var textHeight = ContentText.ActualHeight;
                if (textHeight < windowHeight + 100)
                {
                    ContentText.Height = windowHeight + 200;

                    await Task.Delay(2);

                    //  ContentText.Height = windowHeight + 500;
                }
                else
                {
                    ContentText.Height = textHeight;
                }

                Viewer.ChangeView(0, 40, null, true);
            }
        }


        private async void Viewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            //所有内容垂直高度 - 当前滚动的高度
            var v1 = Viewer.ExtentHeight - Viewer.VerticalOffset;

            //可视区域的高度
            var v2 = Viewer.ViewportHeight;

            //向上滚动
            if (Math.Abs((Viewer.VerticalOffset - 0.1)) <= 0.1)
            {
                if (!e.IsIntermediate)
                {
                    if (IsLoading)
                    {
                        return;
                    }

                    Debug.WriteLine("------------------------------上一章");
                    IsLoading = true;
                    (DataContext as OnlineContentPageViewModel)?.SwitchCatalog(CatalogDirection.Pre);
                    await Task.Delay(5);
                    IsLoading = false;
                }
            }

            else if (Viewer.ExtentHeight > Viewer.ViewportHeight && v1 <= v2 + 1)
            {
                if (!e.IsIntermediate)
                {
                    if (IsLoading)
                    {
                        return;
                    }

                    IsLoading = true;
                    Debug.WriteLine("----------------------------------------下一章");
                    (DataContext as OnlineContentPageViewModel)?.SwitchCatalog(CatalogDirection.Next);
                    await Task.Delay(5);
                    IsLoading = false;

                    Debug.WriteLine("-------------------------------------" + Viewer.VerticalOffset);
                }
            }
        }
    }
}
