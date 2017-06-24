using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Sodu.Control
{
    public sealed class TabbarButton : AppBarButton
    {

        public Brush CommonPathFill
        {
            get { return (Brush)GetValue(CommonPathFillProperty); }
            set { SetValue(CommonPathFillProperty, value); }
        }

        public static readonly DependencyProperty CommonPathFillProperty = DependencyProperty.Register(
          "CommonPathFill", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public Brush SelectedPathFill
        {
            get { return (Brush)GetValue(SelectedPathFillProperty); }
            set { SetValue(SelectedPathFillProperty, value); }
        }

        public static readonly DependencyProperty SelectedPathFillProperty = DependencyProperty.Register(
            "SelectedPathFill", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 0, 138, 255))));



        public static readonly DependencyProperty IsSelectedItemProperty = DependencyProperty.Register(
            "IsSelectedItem", typeof(bool), typeof(TabbarButton), new PropertyMetadata(default(bool), IsSelectedChanged));

        private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn = (d as TabbarButton);
            if (btn != null)
            {
                btn.Foreground = btn.IsSelectedItem ? btn.SelectedPathFill : btn.CommonPathFill;
            }
        }
        public bool IsSelectedItem
        {
            get { return (bool)GetValue(IsSelectedItemProperty); }
            set { SetValue(IsSelectedItemProperty, value); }
        }



        public static readonly DependencyProperty PathDataProperty = DependencyProperty.Register(
            "PathData", typeof(string), typeof(TabbarButton), new PropertyMetadata(default(string)));

        public string PathData
        {
            get { return (string) GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }



        public static readonly DependencyProperty HighLightBackColorProperty = DependencyProperty.Register(
            "HighLightBackColor", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray)));

        public Brush HighLightBackColor
        {
            get { return (Brush) GetValue(HighLightBackColorProperty); }
            set { SetValue(HighLightBackColorProperty, value); }
        }

        public TabbarButton()
        {
            DefaultStyleKey = typeof(TabbarButton);
            
        }
    }
}
