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
using Sodu.Core.Util;
using Sodu.Service;
using System.Windows.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UserControl
{
    public sealed partial class OnlineBookShelfItem
    {

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
 "Command", typeof(ICommand), typeof(OnlineBookShelfItem), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(OnlineBookShelfItem), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public OnlineBookShelfItem()
        {
            this.InitializeComponent();

            if (PlatformHelper.CurrentPlatform == PlatformHelper.Platform.IsPc)
            {
                this.RightTapped += OnlineBookShelfItem_OnRightTapped;
            }

            RootGrid.Tapped += RootGrid_Tapped;
        }

        private void RootGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
           Command?.Execute(CommandParameter);
        }

        private void OnlineBookShelfItem_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
