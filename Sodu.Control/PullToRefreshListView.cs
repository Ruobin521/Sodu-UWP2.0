using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sodu.Control
{
    public class RefreshProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Value from 0.0 to 1.0 where 1.0 is active
        /// </summary>
        public double PullProgress { get; set; }
    }

    public class PullToRefreshListView : ListView
    {
        #region Private Variables
        const string PART_ROOT = "Root";
        const string PART_SCROLLER = "ScrollViewer";
        const string PART_CONTENT_TRANSFORM = "ContentTransform";
        const string PART_SCROLLER_CONTENT = "ScrollerContent";
        const string PART_REFRESH_INDICATOR_BORDER = "RefreshIndicator";
        const string PART_INDICATOR_TRANSFORM = "RefreshIndicatorTransform";
        const string PART_DEFAULT_INDICATOR_CONTENT = "DefaultIndicatorContent";

        // Root element in template
        private Border Root;
        // Container of Refresh Indicator
        private Border RefreshIndicatorBorder;
        // Transform used for moving the Refresh Indicator veticaly while overscrolling
        private CompositeTransform RefreshIndicatorTransform;
        // Main scrollviewer used by the listview
        public ScrollViewer Scroller;
        // Transfrom used for moving the content when overscrolling
        private CompositeTransform ContentTransform;
        // Container for main content
        private Grid ScrollerContent;
        // Container for default content for the Refresh Indicator
        private TextBlock DefaultIndicatorContent;
        // used for calculating distance pulled between ticks
        private double lastOffset = 0.0;
        // used for storing pulled distance
        private double pullDistance = 0.0;
        // used for flagging render function if the user is overscrolling
     //   private bool manipulating = false;
        // used for determining if Refresh should be requested
        DateTime lastRefreshActivation = default(DateTime);
        // used for flagging if refresh has been activated
        private bool refreshActivated = false;
        // property used to calculate the overscroll rate
        private double OverscrollMultiplier;

        bool _isHookedScrollEvent { get; set; }

        #endregion

        #region Properties
        /// <summary>
        /// Occurs when the user has requested content to be refreshed
        /// </summary>
        public event EventHandler RefreshRequested;

        /// <summary>
        /// Occurs when listview overscroll distance is changed
        /// </summary>
        public event EventHandler<RefreshProgressEventArgs> PullProgressChanged;
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="PullToRefreshListView"/>
        /// </summary>
        public PullToRefreshListView()
        {
            DefaultStyleKey = typeof(PullToRefreshListView);
            this.SelectionMode = SelectionMode;
            SizeChanged += RefreshableListView_SizeChanged;
            this.Loaded += PullToRefreshListView_Loaded;
        }

        private void PullToRefreshListView_Loaded(object sender, RoutedEventArgs e)
        {
            //  //注册滚动到底部自动加载事件
            // RegisterRequestEvent();
        }

        #region Methods and Events
        /// <summary>
        /// Handler for SizeChanged event, handles cliping
        /// </summary>
        private void RefreshableListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
            };
        }

        /// <summary>
		/// Invoked whenever application code or internal processes (such as a rebuilding
		/// layout pass) call <see cref="OnApplyTemplate"/>. In simplest terms, this means the method
		/// is called just before a UI element displays in an application. Override this
		/// method to influence the default post-template logic of a class.
		/// </summary>
        protected override void OnApplyTemplate()
        {
            Root = GetTemplateChild<Border>(PART_ROOT);
            Scroller = this.GetTemplateChild<ScrollViewer>(PART_SCROLLER);
            ContentTransform = GetTemplateChild<CompositeTransform>(PART_CONTENT_TRANSFORM);
            ScrollerContent = GetTemplateChild<Grid>(PART_SCROLLER_CONTENT);
            RefreshIndicatorBorder = GetTemplateChild<Border>(PART_REFRESH_INDICATOR_BORDER);
            RefreshIndicatorTransform = GetTemplateChild<CompositeTransform>(PART_INDICATOR_TRANSFORM);
            DefaultIndicatorContent = GetTemplateChild<TextBlock>(PART_DEFAULT_INDICATOR_CONTENT);

            Scroller.DirectManipulationCompleted += Scroller_DirectManipulationCompleted;
            Scroller.DirectManipulationStarted += Scroller_DirectManipulationStarted;

            Scroller.ViewChanged += Scroller_ViewChanged;

            DefaultIndicatorContent.Visibility = RefreshIndicatorContent == null ? Visibility.Visible : Visibility.Collapsed;
            DefaultIndicatorContent.Text = "↓ 下拉刷新";
            RefreshIndicatorBorder.SizeChanged += (s, e) =>
            {
                RefreshIndicatorTransform.TranslateY = -RefreshIndicatorBorder.ActualHeight;
            };

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                OverscrollMultiplier = OverscrollLimit * 10;
            }
            else
            {
                OverscrollMultiplier = (OverscrollLimit * 10) / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            }


            base.OnApplyTemplate();
        }

        private void Scroller_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ///所有内容垂直高度 - 当前滚动的高度
            var v1 = Scroller.ExtentHeight - Scroller.VerticalOffset;
            //可视区域的高度
            var v2 = Scroller.ViewportHeight;
            if (v1 <= v2)
            {
                if (RequestCommand != null && RequestCommand.CanExecute(null))
                {
                    RequestCommand.Execute(null);
                }
            }
        }

        T GetTemplateChild<T>(string name, string message = null) where T : DependencyObject
        {
            var child = GetTemplateChild(name) as T;

            if (child == null)
            {
                if (message == null)
                {
                    message = $"{name} should not be null! Check the default Generic.xaml.";
                }

                throw new NullReferenceException(message);
            }

            return child;
        }

        
        /// <summary>
        /// 获取指定元素的指定视图状态组。
        /// </summary>
        /// <param name="element">目标元素</param>
        /// <param name="name">视图状态名</param>
        /// <returns>视图状态组</returns>
        private VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
            {
                return null;
            }
            var groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            return null;
        }
        #endregion

        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            var visualState = e.NewState.Name;
            if (visualState == "NoIndicator")
            {
                ///所有内容垂直高度 - 当前滚动的高度
                var v1 = Scroller.ExtentHeight - Scroller.VerticalOffset;
                //可视区域的高度
                var v2 = Scroller.ViewportHeight;
                if (v1 <= v2 + 1)
                {
                    // TODO: 在此处加载新数据
                    if (RequestCommand != null && RequestCommand.CanExecute(null))
                    {
                        RequestCommand.Execute(null);
                    }
                }
            }
        }
        /// <summary>
        /// Event handler for when the user has started scrolling
        /// </summary>
        private void Scroller_DirectManipulationStarted(object sender, object e)
        {
            // sometimes the value gets stuck at 0.something, so checking if less than 1
            if (Scroller.VerticalOffset < 1)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        /// <summary>
        /// Event handler for when the user has stoped scrolling
        /// </summary>
        private void Scroller_DirectManipulationCompleted(object sender, object e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            RefreshIndicatorTransform.TranslateY = -RefreshIndicatorBorder.ActualHeight;
            ContentTransform.TranslateY = 0;

            if (refreshActivated)
            {
                if (RefreshRequested != null)
                    RefreshRequested(this, new EventArgs());
                if (RefreshCommand != null && RefreshCommand.CanExecute(null))
                    RefreshCommand.Execute(null);
            }

            refreshActivated = false;
            lastRefreshActivation = default(DateTime);

            if (RefreshIndicatorContent == null)
                DefaultIndicatorContent.Text = "↓ 下拉刷新";

            if (PullProgressChanged != null)
            {
                PullProgressChanged(this, new RefreshProgressEventArgs() { PullProgress = 0 });
            }
        }

        /// <summary>
        /// Event handler called before core rendering process renders a frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompositionTarget_Rendering(object sender, object e)
        {
            Rect elementBounds = ScrollerContent.TransformToVisual(Root).TransformBounds(new Rect());

            var offset = elementBounds.Y;
            var delta = offset - lastOffset;
            lastOffset = offset;

            pullDistance += delta * OverscrollMultiplier;

            if (pullDistance > 0)
            {
                ContentTransform.TranslateY = pullDistance - offset;
                RefreshIndicatorTransform.TranslateY = pullDistance - offset - RefreshIndicatorBorder.ActualHeight;
            }
            else
            {
                ContentTransform.TranslateY = 0;
                RefreshIndicatorTransform.TranslateY = -RefreshIndicatorBorder.ActualHeight;

            }

            var pullProgress = 0.0;

            if (pullDistance >= PullThreshold)
            {
                lastRefreshActivation = DateTime.Now;
                refreshActivated = true;
                pullProgress = 1.0;
                if (RefreshIndicatorContent == null)
                    DefaultIndicatorContent.Text = "↑ 释放刷新";
            }
            else if (lastRefreshActivation != DateTime.MinValue)
            {
                TimeSpan timeSinceActivated = DateTime.Now - lastRefreshActivation;
                // if more then a second since activation, deactivate
                if (timeSinceActivated.TotalMilliseconds > 1000)
                {
                    refreshActivated = false;
                    lastRefreshActivation = default(DateTime);
                    pullProgress = pullDistance / PullThreshold;
                    if (RefreshIndicatorContent == null)
                        DefaultIndicatorContent.Text = "↓ 下拉刷新";
                }
                else
                {
                    pullProgress = 1.0;
                }
            }
            else
            {
                pullProgress = pullDistance / PullThreshold;
            }

            if (PullProgressChanged != null)
            {
                PullProgressChanged(this, new RefreshProgressEventArgs() { PullProgress = pullProgress });
            }
        }

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the Overscroll Limit. Value between 0 and 1 where 1 is the height of the control. Default is 0.3
        /// </summary>
        public double OverscrollLimit
        {
            get { return (double)GetValue(OverscrollLimitProperty); }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                    {
                        OverscrollMultiplier = value * 10;
                    }
                    else
                    {
                        OverscrollMultiplier = (value * 10) / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    }
                    SetValue(OverscrollLimitProperty, value);
                }
                else
                    throw new IndexOutOfRangeException("OverscrollCoefficient has to be a double value between 0 and 1 inclusive.");
            }
        }

        /// <summary>
        /// Identifies the <see cref="OverscrollLimit"/> property.
        /// </summary>
        public static readonly DependencyProperty OverscrollLimitProperty =
            DependencyProperty.Register("OverscrollLimit", typeof(double), typeof(PullToRefreshListView), new PropertyMetadata(0.4));


        /// <summary>
        /// Gets or sets the PullThreshold in pixels for when Refresh should be Requested. Default is 100
        /// </summary>
        public double PullThreshold
        {
            get { return (double)GetValue(PullThresholdProperty); }
            set { SetValue(PullThresholdProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="PullThreshold"/> property.
        /// </summary>
        public static readonly DependencyProperty PullThresholdProperty =
            DependencyProperty.Register("PullThreshold", typeof(double), typeof(PullToRefreshListView), new PropertyMetadata(85.00));

        /// <summary>
        /// Gets or sets the Command that will be invoked when Refresh is requested
        /// </summary>
        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="RefreshCommand"/> property.
        /// </summary>
        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(PullToRefreshListView), new PropertyMetadata(null));

        public ICommand RequestCommand
        {
            get { return (ICommand)GetValue(RequestCommandProperty); }
            set { SetValue(RequestCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoRequestAddCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestCommandProperty =
            DependencyProperty.Register("RequestCommand", typeof(ICommand), typeof(PullToRefreshListView), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the Content of the Refresh Indicator
        /// </summary>
        public object RefreshIndicatorContent
        {
            get { return (object)GetValue(RefreshIndicatorContentProperty); }
            set
            {
                if (DefaultIndicatorContent != null)
                    DefaultIndicatorContent.Visibility = value == null ? Visibility.Visible : Visibility.Collapsed;
                SetValue(RefreshIndicatorContentProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="RefreshIndicatorContent"/> property.
        /// </summary>
        public static readonly DependencyProperty RefreshIndicatorContentProperty =
            DependencyProperty.Register("RefreshIndicatorContent", typeof(object), typeof(PullToRefreshListView), new PropertyMetadata(null));

        #endregion

    }

}
