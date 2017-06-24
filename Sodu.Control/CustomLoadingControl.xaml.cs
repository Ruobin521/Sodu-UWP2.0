using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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

namespace Sodu.Control
{
    public sealed partial class CustomLoadingControl : UserControl
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(CustomLoadingControl), new PropertyMetadata(default(bool), propertyChangedCallback: callBack));

        private static void callBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (d as CustomLoadingControl);
            control.Visibility = control.IsActive ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty CancleCommandProperty = DependencyProperty.Register(
            "CancleCommand", typeof(ICommand), typeof(CustomLoadingControl), new PropertyMetadata(default(ICommand)));

        public ICommand CancleCommand
        {
            get { return (ICommand) GetValue(CancleCommandProperty); }
            set { SetValue(CancleCommandProperty, value); }
        }
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public CustomLoadingControl()
        {
            this.InitializeComponent();
            Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
