using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Sodu.Controls
{
    public sealed class TabbarButton : Button
    {
        public static readonly DependencyProperty PathDataProperty = DependencyProperty.Register(
            "PathData", typeof(Geometry), typeof(TabbarButton), new PropertyMetadata(default(Geometry)));

        public Geometry PathData
        {
            get { return (Geometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        //public static readonly DependencyProperty CommonPathFillProperty = DependencyProperty.Register(
        //    "CommonPathFill", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 122, 122, 122))));

        public static readonly DependencyProperty CommonPathFillProperty = DependencyProperty.Register(
          "CommonPathFill", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public Brush CommonPathFill
        {
            get { return (Brush)GetValue(CommonPathFillProperty); }
            set { SetValue(CommonPathFillProperty, value); }
        }


        public static readonly DependencyProperty SelectedPathFillProperty = DependencyProperty.Register(
            "SelectedPathFill", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 51, 136, 255))));

        public Brush SelectedPathFill
        {
            get { return (Brush)GetValue(SelectedPathFillProperty); }
            set { SetValue(SelectedPathFillProperty, value); }
        }

        public static readonly DependencyProperty PathFillProperty = DependencyProperty.Register(
            "PathFill", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 122, 122, 122))));

        public Brush PathFill
        {
            get { return (Brush)GetValue(PathFillProperty); }
            set { SetValue(PathFillProperty, value); }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
                PathFill = IsSelected ? SelectedPathFill : CommonPathFill;
            }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ImageSource), new PropertyMetadata(false));



        public TabbarButton()
        {
            this.DefaultStyleKey = typeof(TabbarButton);
            Background = new SolidColorBrush() { Color = Colors.Transparent };
            BorderBrush = new SolidColorBrush() { Color = Colors.Transparent };
            BorderThickness = new Thickness(0);
        }
    }
}
