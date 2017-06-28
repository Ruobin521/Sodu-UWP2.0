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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sodu.ContentPageControl
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SwitchPageItem
    {
        public SwitchPageItem()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(SwitchPageItem), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public void AnimationToLeft()
        {
            var compositeTransform = this.RenderTransform as CompositeTransform;
            if (compositeTransform != null)
            {
                LeftStartValue.Value = compositeTransform.TranslateX;
                LeftEndValue.Value = -ActualWidth;
                StoryboardToLeft.Begin();
            }
        }

        public void AnimationToRight()
        {
            var compositeTransform = RenderTransform as CompositeTransform;
            if (compositeTransform != null)
            {
                RightStartValue.Value = compositeTransform.TranslateX;
                RightEndValue.Value = ActualWidth;
                StoryboardToRight.Begin();
            }
        }
    }

}
