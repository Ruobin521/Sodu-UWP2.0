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
using System.Windows.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UserControl
{
    public sealed partial class UpdateListItem
    {

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
"Command", typeof(ICommand), typeof(UpdateListItem), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(UpdateListItem), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }


        public UpdateListItem()
        {
            this.InitializeComponent();
            Tapped += UpdateListItem_Tapped;

        }

        private void UpdateListItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Command?.Execute(CommandParameter);
        }
    }
}
