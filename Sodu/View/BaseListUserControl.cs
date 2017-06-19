using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Control;

namespace Sodu.View
{
    public class BaseListUserControl : Windows.UI.Xaml.Controls.UserControl
    {
        public PullToRefreshListView CurrentListView { get; set; }

        public void GoTopOrBottom()
        {
            if (CurrentListView?.Items == null || CurrentListView.Items.Count == 0)
            {
                return;
            }

            if (CurrentListView.Scroller.ViewportHeight > CurrentListView.Scroller.ExtentHeight)
            {
                return;
            }

            if (CurrentListView.Scroller.VerticalOffset > 10)
            {
                CurrentListView.Scroller.ChangeView(0, 0, 1, false);
            }
            else
            {
                CurrentListView.Scroller.ChangeView(0, CurrentListView.Scroller.ExtentHeight, 1, false);
            }
        }

    }
}
