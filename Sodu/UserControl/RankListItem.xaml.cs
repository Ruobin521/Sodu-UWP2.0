using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Sodu.Core.Util;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UserControl
{
    public sealed partial class RankListItem
    {

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
"Command", typeof(ICommand), typeof(RankListItem), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(RankListItem), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }


        public RankListItem()
        {
            this.InitializeComponent();
            if (PlatformHelper.CurrentPlatform == PlatformHelper.Platform.IsPc)
            {
                this.RightTapped += RankListItem_RightTapped;
            }

            Tapped += RankListItem_Tapped;
        }

        private void RankListItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Command?.Execute(CommandParameter);
        }

        private void RankListItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
