using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Sodu.Control
{
    public sealed class SettingButton : Button
    {



        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(SettingButton), new PropertyMetadata(default(bool), CallBack));

        private static void CallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn = (SettingButton)d;

            btn.BorderBrush = btn.IsSelected
                ? new SolidColorBrush(Color.FromArgb(255, 0, 122, 255))
                : new SolidColorBrush(Colors.White);
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public SettingButton()
        {
            this.DefaultStyleKey = typeof(SettingButton);
        }

      
    }
}
