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
using Sodu.Core.Entity;
using Sodu.Core.Util;
using Sodu.ViewModel;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sodu.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            this.InitializeComponent();

            Loaded += CatalogPage_Loaded;
        }

        private void CatalogPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (CatalogList.Items != null && CatalogList.SelectedIndex > 4)
            {
                if (CatalogList.Items.Count > CatalogList.SelectedIndex + 1 + 4)
                {
                    CatalogList.ScrollIntoView(CatalogList.Items[CatalogList.SelectedIndex + 4]);
                }
                else
                {
                    CatalogList.ScrollIntoView(CatalogList.SelectedItem);
                }

                this.ScroolButton.Tag = "1";
                this.ScroolButton.Content = "到顶部";

            }
        }

        private void ScroolButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.ScroolButton.Tag == null || ScroolButton.Tag.ToString() == "0")
            {
                List<ScrollViewer> list = new List<ScrollViewer>();
                SoduVisualTreeHelper.GetVisualChildCollection(CatalogList, list);
                var scroolViewer = list.FirstOrDefault();
                scroolViewer.ChangeView(0, scroolViewer.ExtentHeight, null, false);
                this.ScroolButton.Tag = "1";
                this.ScroolButton.Content = "到顶部";
            }

            else if (this.ScroolButton.Tag?.ToString() == "1")
            {

                List<ScrollViewer> list = new List<ScrollViewer>();
                SoduVisualTreeHelper.GetVisualChildCollection(CatalogList, list);
                var scroolViewer = list.FirstOrDefault();
                scroolViewer.ChangeView(0, 0, null, false);
                this.ScroolButton.Tag = "0";
                this.ScroolButton.Content = "到底部";

            }
        }



    }
}
