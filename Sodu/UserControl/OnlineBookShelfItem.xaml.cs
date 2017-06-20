﻿using System;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UserControl
{
    public sealed partial class OnlineBookShelfItem : Button
    {
        public OnlineBookShelfItem()
        {
            this.InitializeComponent();

            if (PlatformHelper.CurrentPlatform == PlatformHelper.Platform.IsPc)
            {
                this.RightTapped += OnlineBookShelfItem_OnRightTapped;
            }
        }

        private void OnlineBookShelfItem_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
