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
using Sodu.UserControl;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sodu.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class OnlineBookShelfPage : BaseListUserControl
    {
        public OnlineBookShelfPage()
        {
            this.InitializeComponent();
            base.CurrentListView = BookListView;
        }

        private void BookListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as OnlineBookShelfItem;
            throw new NotImplementedException();
        }
    }
}
