﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using Sodu.Control;

namespace Sodu.Service
{
    public class ToastHelper
    {
        public static void ShowMessage(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var popup = new PopupWindow(message);
                popup.ShowWindow();
            });
        }
    }
}
