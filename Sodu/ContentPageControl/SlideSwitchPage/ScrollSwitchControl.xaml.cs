using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

        private double Threshold { get; set; } = 50;

        private OnlineContentPageViewModel ViewModel { get; set; }

        private bool IsAnimating { get; set; }


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


            ViewModel = ViewModelInstance.Instance.OnlineBookContent;

            SizeChanged += ScrollSwitchControl_SizeChanged;
        }

        private void ScrollSwitchControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            ViewModel?.ResizePageContent();
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
            if (IsAnimating)
            {
                return;
            }
            SwipeX = 0;
            SetLeftPageData(_leftControl);
            SetRightPageData(_rightControl);
        }

        private void The_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var x = e.Delta.Translation.X;

            if (IsAnimating)
            {
                return;
            }

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
            if (SwipeX == 0.0)
            {
                return;
            }
            //上一页（章节）
            if (SwipeX > 0)
            {
                RightAnimation();
            }
            // 下一页（章节）
            else if (SwipeX < 0)
            {
                LeftAnimation();
            }

            if (_centerControl.Catalog != null)
            {
              //  ViewModel.CurrentCatalog = _centerControl.Catalog;
            }
        }

        private async void LeftAnimation()
        {
            IsAnimating = true;
            _centerControl.CenterToLeftAction();
            _rightControl.RightToCenterAction();
            _leftControl.LeftToRightAction();

            var control = _leftControl;
            _leftControl = _centerControl;
            _centerControl = _rightControl;
            _rightControl = control;

            await Task.Delay(200);
            IsAnimating = false;
        }

        private async void RightAnimation()
        {
            IsAnimating = true;

            _leftControl.LeftToCenterAction();
            _centerControl.CenterToRightAction();
            _rightControl.RightToLeftAction();


            var control = _rightControl;
            _rightControl = _centerControl;
            _centerControl = _leftControl;
            _leftControl = control;


            await Task.Delay(200);
            IsAnimating = false;

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
            if (ViewModel.CurrentBook == null || ViewModel.CurrentCatalog == null)
            {
                return;
            }

            var pages = ViewModel.GetCatalogPagesFormDicrionary(ViewModel.CurrentCatalog.CatalogUrl);

            if (pages == null || pages.Count <= 0)
            {
                return;
            }
            _centerControl.Text = pages[0];
            _centerControl.CatalogIndex = ViewModel.CurrentCatalog.Index;
            _centerControl.CatalogCount = ViewModel.CurrentBook.CatalogList?.Count ?? 0;
            _centerControl.PageIndex = 0;
            _centerControl.PageCount = pages.Count;
            _centerControl.Catalog = ViewModel.CurrentCatalog;


        }


        public async void SetLeftPageData(ScrollSwitchItem item)
        {
            var catalog = ViewModel.CurrentCatalog;

            if (catalog == null)
            {
                return;
            }
            var value = await ViewModel.GetCatalogContent(catalog);

            if (value == null)
            {
                return;
            }
            var index = _centerControl.PageIndex;

            if (index > 0 && index <= value.Item1.Count - 1)
            {
                item.SetData(value.Item1[index - 1], 
                    catalog.Index, 
                    ViewModel.CurrentBook.CatalogList?.Count ?? 0, 
                    index - 1, 
                    value.Item1.Count,
                    catalog);
            }
            else if (index == 0)
            {
                if (ViewModel.PreCatalog == null)
                {
                    return;
                }

                var preCatalogData = ViewModel.GetCatalogPagesFormDicrionary(ViewModel.PreCatalog.CatalogUrl);

                if (preCatalogData == null)
                {
                    _leftControl.ClearData();
                    return;
                }
                var precatalog = ViewModel.PreCatalog;

                item.SetData(preCatalogData.LastOrDefault(),
                    precatalog.Index, 
                    ViewModel.CurrentBook.CatalogList?.Count ?? 0, 
                    preCatalogData.Count - 1,
                    preCatalogData.Count, 
                    precatalog);

            }
            else
            {

            }
        }

        public async void SetRightPageData(ScrollSwitchItem item)
        {
            var catalog = _centerControl.Catalog;

            if (catalog == null)
            {
                return;
            }

            var value = await ViewModel.GetCatalogContent(catalog);

            if (value == null)
            {
                return;
            }
            var index = _centerControl.PageIndex;
            if (index >= 0 && index < value.Item1.Count - 1)
            {
                item.SetData(value.Item1[index + 1],
                   catalog.Index,
                    ViewModel.CurrentBook.CatalogList?.Count ?? 0,
                    index + 1,
                    value.Item1.Count,
                    catalog);
            }
            else
            {
                var catalogData = await ViewModel.GetCatalogDataByDirection(CatalogDirection.Next);

                if (catalogData == null)
                {
                    _rightControl.ClearData();
                    return;
                }
                var tempcatalog = catalogData.Item3;
                item.SetData(catalogData.Item1.FirstOrDefault(),
                  tempcatalog.Index,
                  ViewModel.CurrentBook.CatalogList?.Count ?? 0,
                  0,
                  catalogData.Item1.Count,
                  tempcatalog);
            }
        }


        /// <summary>
        /// 检测是否可以加载下一页数据
        /// </summary>
        /// <returns></returns>
        private bool CanSwitchToNext()
        {
            var catalog = _centerControl.Catalog;

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
            var catalog = _centerControl.Catalog;

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

            var temp = ViewModel.GetCatalogByDirction(CatalogDirection.Pre);

            return temp != null;
        }

    }
}
