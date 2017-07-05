using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Sodu.Core.Util;

namespace Sodu.UserControl
{
    public class BaseListViewItem : Windows.UI.Xaml.Controls.UserControl
    {

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(BaseListViewItem), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(BaseListViewItem), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public BaseListViewItem()
        {

            if (PlatformHelper.IsMobile)
            {
                this.Holding += (sender, e) =>
                {
                    if (FlyoutBase.GetAttachedFlyout((FrameworkElement) sender) == null)
                    {
                        return;
                    }
                    FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                };
            }
            else
            {
                this.RightTapped += (sender, e) =>
                {
                    if (FlyoutBase.GetAttachedFlyout((FrameworkElement)sender) == null)
                    {
                        return;
                    }
                    FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
                };
            }

            this.Tapped += (sender, e) =>
            {
                Command?.Execute(CommandParameter);
            };
        }
    }
}
