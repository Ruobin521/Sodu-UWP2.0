using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.Control
{
    public sealed partial class PopupWindow : UserControl
    {

        private Storyboard storyBoard;

        private Popup m_Popup;

        private string txtMessage;
        public PopupWindow()
        {
            this.InitializeComponent();
            m_Popup = new Popup();

            m_Popup.Child = this;
            this.Loaded += PopupWindow_Loaded;
            //this.Unloaded += PopupWindow_Unloaded;
        }
        public PopupWindow(string message, VerticalAlignment vertical = VerticalAlignment.Top, HorizontalAlignment horizontal = HorizontalAlignment.Right) : this()
        {
            this.txtMessage = message;

            double left = 0;
            double top = 0;

            if (horizontal == HorizontalAlignment.Left)
            {
                left = 0;
            }
            else if (horizontal == HorizontalAlignment.Right)
            {
                left = Window.Current.Bounds.Width;
            }

            if (vertical == VerticalAlignment.Top)
            {
                top = 66;
            }

            else if (vertical == VerticalAlignment.Bottom)
            {
                top = Window.Current.Bounds.Height - 10;
            }

            m_Popup.Margin = new Thickness(left, top, 0, 0);
        }


        private void PopupWindow_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.tb_Notify.Text = this.txtMessage;
            this.storyBoard = this.tb_Notify_in;
            this.easeKeyframe.Value = -(this.tb_Notify.Text.Length * 16 > 150 ? this.tb_Notify.Text.Length * this.tb_Notify.FontSize : 150);
            this.storyBoard.Completed += StoryBoard_Completed;
            this.storyBoard.Begin();
        }


        private void StoryBoard_Completed(object sender, object e)
        {
            m_Popup.IsOpen = false;
        }
        public void ShowWindow()
        {
            m_Popup.IsOpen = true;
        }


    }
}
