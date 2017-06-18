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


        public Brush PathFill
        {
            get { return (Brush)GetValue(PathFillProperty); }
            set { SetValue(PathFillProperty, value); }
        }

        public static readonly DependencyProperty PathFillProperty = DependencyProperty.Register(
            "PathFill", typeof(Brush), typeof(TabbarButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));



        public static readonly DependencyProperty IsSelectedItemProperty = DependencyProperty.Register(
            "IsSelectedItem", typeof(bool), typeof(TabbarButton), new PropertyMetadata(default(bool), IsSelectedChanged));

        private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn = (d as TabbarButton);
            if (btn != null)
            {
                btn.PathFill = btn.IsSelectedItem ? btn.SelectedPathFill : btn.CommonPathFill;
            }
        }
        public bool IsSelectedItem
        {
            get { return (bool)GetValue(IsSelectedItemProperty); }
            set { SetValue(IsSelectedItemProperty, value); }
        }

        public TabbarButton()
        {
            DefaultStyleKey = typeof(TabbarButton);
        }
    }
}
