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

namespace Sodu.UserControl
{
    public sealed partial class TopNavigationBar
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(TopNavigationBar), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register(
            "SearchCommand", typeof(ICommand), typeof(TopNavigationBar), new PropertyMetadata(default(ICommand)));

        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }


        public static readonly DependencyProperty RefreshCommandProperty = DependencyProperty.Register(
            "RefreshCommand", typeof(ICommand), typeof(TopNavigationBar), new PropertyMetadata(default(ICommand)));

        public ICommand RefreshCommand
        {
            get { return (ICommand) GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        public TopNavigationBar()
        {
            this.InitializeComponent();
        }
    }
}
