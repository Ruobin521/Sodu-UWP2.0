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
using Sodu.ViewModel;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sodu.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DownloadCenterPage : Page
    {
        public DownloadCenterPage()
        {
            this.InitializeComponent();
            Loaded += DownLoadCenterPage_Loaded;
            Unloaded += DownLoadCenterPage_Unloaded;
        }

        private void DownLoadCenterPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModelInstance.Instance.DownloadCenter.IsFrameContent = false;
        }

        private void DownLoadCenterPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModelInstance.Instance.DownloadCenter.IsFrameContent = true;
        }
    }
}
