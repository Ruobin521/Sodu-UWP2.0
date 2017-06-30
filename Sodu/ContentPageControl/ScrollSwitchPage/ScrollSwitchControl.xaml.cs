using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Sodu.Core.Entity;
using Sodu.ViewModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.ContentPageControl.ScrollSwitchPage
{

    public sealed partial class ScrollSwitchControl
    {
        private ScrollSwitchItem _leftControl;
        private ScrollSwitchItem _centerControl;
        private ScrollSwitchItem _rightControl;

        private double SwipeX { get; set; }//用来接收手势水平滑动的长度

        private double Threshold { get; set; } = 80;

        private OnlineContentPageViewModel ViewModel { get; set; }

        public ScrollSwitchControl()
        {
            this.InitializeComponent();

            _leftControl = Control1;
            _centerControl = Control2;
            _rightControl = Control3;

            SizeChanged += MainPage_SizeChanged;


            ManipulationCompleted += The_ManipulationCompleted;//订阅手势滑动结束后的事件
            ManipulationStarted += The_ManipulationStarted;   //订阅手势滑动结束后的事件
            ManipulationDelta += The_ManipulationDelta;//订阅手势滑动事件

            Messenger.Default.Register<string>(this, "ContentTextChanged", OnContentTextChanged);

            this.Loaded += ScrollSwitchControl_Loaded;
        }

        private void ScrollSwitchControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as OnlineContentPageViewModel;
        }


        public Tuple<double, double> GetPageSize()
        {
            return new Tuple<double, double>(_centerControl.ActualWidth, _centerControl.ActualHeight);
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            ((CompositeTransform)_leftControl.RenderTransform).TranslateX = -ActualWidth;
            // ReSharper disable once PossibleNullReferenceException
            ((CompositeTransform)_centerControl.RenderTransform).TranslateX = 0;
            // ReSharper disable once PossibleNullReferenceException
            (_rightControl.RenderTransform as CompositeTransform).TranslateX = ActualWidth;
        }

        private void The_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            SwipeX = 0;
            SetLeftPageData();
            SetRightPageData();
        }

        private void The_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {

            var x = e.Delta.Translation.X;

            if (x > 0 && !CanSwitchToPre())
            {
                return;
            }

            if (x < 0 && !CanSwitchToNext())
            {
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            SwipeX = (_centerControl.RenderTransform as CompositeTransform).TranslateX + e.Delta.Translation.X;

            // ReSharper disable once PossibleNullReferenceException
            (_centerControl.RenderTransform as CompositeTransform).TranslateX = SwipeX;

            // ReSharper disable once PossibleNullReferenceException
            (_rightControl.RenderTransform as CompositeTransform).TranslateX = _rightControl.ActualWidth + SwipeX;

            // ReSharper disable once PossibleNullReferenceException
            (_leftControl.RenderTransform as CompositeTransform).TranslateX = -_leftControl.ActualWidth + SwipeX;
        }




        private void The_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs(SwipeX) < Threshold)
            {
                RevertAnimation();
                return;
            }

            //上一页（章节）
            if (SwipeX > Threshold)
            {
                RightAnimation();

                ViewModel.CurrentCatalog = _leftControl.Tag as BookCatalog;

            }
            // 下一页（章节）
            else if (SwipeX < -Threshold)
            {
                LeftAnimation();

                ViewModel.CurrentCatalog = _rightControl.Tag as BookCatalog;
            }
        }

        private void LeftAnimation()
        {
            _centerControl.CenterToLeftAction();
            _rightControl.RightToCenterAction();
            _leftControl.LeftToRightAction();

            var control = _leftControl;
            _leftControl = _centerControl;
            _centerControl = _rightControl;
            _rightControl = control;

        }

        private void RightAnimation()
        {
            _leftControl.LeftToCenterAction();
            _centerControl.CenterToRightAction();
            _rightControl.RightToLeftAction();


            var control = _rightControl;
            _rightControl = _centerControl;
            _centerControl = _leftControl;
            _leftControl = control;
        }


        private void RevertAnimation()
        {
            _leftControl.LeftToLeftAction();
            _centerControl.CenterToCenterAction();
            _rightControl.RightToRightAction();
        }

        public Tuple<double, double> GetControlSize()
        {
            return _centerControl.GetContainerSize();
        }
    }


    /// <summary>
    /// 数据相关业务
    /// </summary>
    public sealed partial class ScrollSwitchControl
    {

        private string CurrentPageText { get; set; }
        private string NextPageText { get; set; }
        private string PrePageText { get; set; }

        private void OnContentTextChanged(string obj)
        {
            var pages = ViewModel.GetCatalogPagesFormDicrionary(ViewModel.CurrentCatalog.CatalogUrl);

            if (pages == null || pages.Count <= 0)
            {
                return;
            }
            _centerControl.Title = ViewModel.CurrentCatalog.CatalogName;
            _centerControl.Text = pages[0];
            _centerControl.CatalogIndex = ViewModel.CurrentCatalog.Index;
            _centerControl.CatalogCount = ViewModel.CurrentBook.CatalogList?.Count ?? 0;
            _centerControl.PageIndex = 0;
            _centerControl.PageCount = pages.Count;

            _centerControl.Tag = ViewModel.CurrentCatalog;
        }


        public async void SetLeftPageData()
        {
            var centerCatalog = _centerControl.Tag as BookCatalog;

            if (centerCatalog == null)
            {
                return;
            }

            var value = await ViewModel.GetCatalogContent(centerCatalog);

            if (value == null)
            {
                return;
            }
            var index = _centerControl.PageIndex;

            if (index > 0 && index <= value.Item1.Count - 1)
            {
                _leftControl.Title = centerCatalog.CatalogName;
                _leftControl.Text = value.Item1[index - 1];
                _leftControl.CatalogIndex = centerCatalog.Index;
                _leftControl.CatalogCount = ViewModel.CurrentBook.CatalogList?.Count ?? 0;
                _leftControl.PageIndex = index - 1;
                _leftControl.PageCount = value.Item1.Count;

                _leftControl.Tag = centerCatalog;
            }
            else
            {
                var preCatalogData = await ViewModel.GetCatalogDataByDirection(CatalogDirection.Pre);

                if (preCatalogData == null)
                {
                    return;
                }
                var precatalog = preCatalogData.Item3;

                _leftControl.Title = precatalog.CatalogName;
                _leftControl.Text = preCatalogData.Item1.LastOrDefault();
                _leftControl.CatalogIndex = precatalog.Index;
                _leftControl.CatalogCount = ViewModel.CurrentBook.CatalogList?.Count ?? 0;
                _leftControl.PageIndex = preCatalogData.Item1.Count - 1;
                _leftControl.PageCount = preCatalogData.Item1.Count;
                _leftControl.Tag = precatalog;
            }
        }

        public async void SetRightPageData()
        {
            var centerCatalog = _centerControl.Tag as BookCatalog;

            if (centerCatalog == null)
            {
                return;
            }

            var value = await ViewModel.GetCatalogContent(centerCatalog);

            if (value == null)
            {
                return;
            }
            var index = _centerControl.PageIndex;
            if (index >= 0 && index < value.Item1.Count - 1)
            {
                _leftControl.Title = centerCatalog.CatalogName;
                _rightControl.Text = value.Item1[index + 1];

                _rightControl.CatalogIndex = centerCatalog.Index;
                _rightControl.CatalogCount = ViewModel.CurrentBook.CatalogList?.Count ?? 0;
                _rightControl.PageIndex = index + 1;
                _rightControl.PageCount = value.Item1.Count;

                _rightControl.Tag = centerCatalog;
            }
            else
            {
                var catalogData = await ViewModel.GetCatalogDataByDirection(CatalogDirection.Next);

                if (catalogData == null)
                {
                    return;
                }
                var catalog = catalogData.Item3;

                _rightControl.Title = catalog.CatalogName;
                _rightControl.Text = catalogData.Item1.FirstOrDefault();
                _rightControl.CatalogIndex = catalog.Index;
                _rightControl.CatalogCount = ViewModel.CurrentBook.CatalogList?.Count ?? 0;
                _rightControl.PageIndex = 0;
                _rightControl.PageCount = catalogData.Item1.Count;
                _rightControl.Tag = catalog;
            }
        }


        /// <summary>
        /// 检测是否可以加载下一页数据
        /// </summary>
        /// <returns></returns>
        private bool CanSwitchToNext()
        {
            var catalog = _centerControl.Tag as BookCatalog;

            if (catalog == null)
            {
                return false;
            }

            var index = _centerControl.PageIndex;

            var pages = ViewModel.GetCatalogPagesFormDicrionary(catalog.CatalogUrl);

            if (pages == null)
            {
                return false;
            }

            if (index < pages.Count - 1)
            {
                return true;
            }

            var temp = ViewModel.GetCatalogByDirction(CatalogDirection.Next);
            return temp != null;
        }


        /// <summary>
        /// 检测是否可以加载上一页数据
        /// </summary>
        /// <returns></returns>
        private bool CanSwitchToPre()
        {
            var catalog = _centerControl.Tag as BookCatalog;

            if (catalog == null)
            {
                return false;
            }

            var index = _centerControl.PageIndex;

            var pages = ViewModel.GetCatalogPagesFormDicrionary(catalog.CatalogUrl);

            if (pages == null)
            {
                return false;
            }

            if (index > 0)
            {
                return true;
            }

            var temp = ViewModel.GetCatalogByDirction(CatalogDirection.Next);

            return temp != null;
        }

    }
}
