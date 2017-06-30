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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.ContentPageControl.ScrollSwitchPage
{
    public sealed partial class ScrollSwitchItem : Windows.UI.Xaml.Controls.UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ScrollSwitchItem), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }



        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ScrollSwitchItem), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        public static readonly DependencyProperty PageCountProperty = DependencyProperty.Register(
            "PageCount", typeof(int), typeof(ScrollSwitchItem), new PropertyMetadata(default(int)));

        public int PageCount
        {
            get { return (int) GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }


        public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register(
            "PageIndex", typeof(int), typeof(ScrollSwitchItem), new PropertyMetadata(default(int)));

        public int PageIndex
        {
            get { return (int) GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }


        public static readonly DependencyProperty CatalogCountProperty = DependencyProperty.Register(
            "CatalogCount", typeof(int), typeof(ScrollSwitchItem), new PropertyMetadata(default(int)));

        public int CatalogCount
        {
            get { return (int) GetValue(CatalogCountProperty); }
            set { SetValue(CatalogCountProperty, value); }
        }

        public static readonly DependencyProperty CatalogIndexProperty = DependencyProperty.Register(
            "CatalogIndex", typeof(int), typeof(ScrollSwitchItem), new PropertyMetadata(default(int)));

        public int CatalogIndex
        {
            get { return (int) GetValue(CatalogIndexProperty); }
            set { SetValue(CatalogIndexProperty, value); }
        }

        public ScrollSwitchItem()
        {
            this.InitializeComponent();
        }

        public void LeftToRightAction()
        {
            LeftToRightStart.Value = -ActualWidth;
            LeftToRightEnd.Value = ActualWidth;
            LeftToRight.Begin();
        }
        public void CenterToLeftAction()
        {
            CenterToLeftStart.Value = (RenderTransform as CompositeTransform)?.TranslateX ?? 0;

            CenterToLeftEnd.Value = -ActualWidth;

            CenterToLeft.Begin();
        }

        public void RightToCenterAction()
        {
            RightToCenterStart.Value = (RenderTransform as CompositeTransform)?.TranslateX ?? ActualWidth;

            RightToCenter.Begin();
        }


        public void CenterToRightAction()
        {
            CenterToRightStart.Value = (RenderTransform as CompositeTransform)?.TranslateX ?? 0;
            CenterToRightEnd.Value = ActualWidth;
            CenterToRight.Begin();
        }


        public void LeftToCenterAction()
        {
            LeftToCenterStart.Value = (RenderTransform as CompositeTransform)?.TranslateX ?? -ActualWidth;
            LeftToCenter.Begin();
        }


        public void RightToLeftAction()
        {
            RightToLeftStart.Value = ActualWidth;
            RightToLeftEnd.Value = -ActualWidth;
            RightToLeft.Begin();
        }

        public void LeftToLeftAction()
        {
            LeftToLeftStart.Value = (RenderTransform as CompositeTransform)?.TranslateX ?? -ActualWidth;
            LeftToLeftEnd.Value = -ActualWidth;
            LeftToLeft.Begin();
        }

        public void RightToRightAction()
        {
            RightToRighttStart.Value = (RenderTransform as CompositeTransform)?.TranslateX ?? ActualWidth;
            RightToRightEnd.Value = ActualWidth;
            RightToRight.Begin();
        }
        public void CenterToCenterAction()
        {
            CenterToCenterStart.Value = (RenderTransform as CompositeTransform)?.TranslateX ?? 0;
            CenterToCenter.Begin();
        }


        public Tuple<double, double> GetContainerSize()
        {
            return  new Tuple<double, double>(this.Grid.ActualWidth,this.Grid.ActualHeight);
        }	
    }
}
