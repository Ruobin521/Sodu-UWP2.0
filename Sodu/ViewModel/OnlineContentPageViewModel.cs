using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.View;
using Windows.Phone.Devices.Power;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;
using Sodu.Core.Util;
using Sodu.Service;

namespace Sodu.ViewModel
{
    public class OnlineContentPageViewModel : CommonPageViewModel
    {
        #region 属性

        /// <summary>
        /// 缓存数据（url,pages ,content）
        /// </summary>
        public Dictionary<string, Tuple<List<string>, string>> DicContentCache = new Dictionary<string, Tuple<List<string>, string>>();

        /// <summary>
        /// 是否正在加载目录页数据（目录，简介，封面，作者）
        /// </summary>
        private bool IsLoadingCatalogData { get; set; }


        private BookCatalog _currentCatalog;

        /// <summary>
        /// 当前目录项
        /// </summary>
        public BookCatalog CurrentCatalog
        {
            get
            {
                return _currentCatalog;

            }
            set
            {
                Set(ref _currentCatalog, value);
            }
        }

        private Book _currentBook;
        /// <summary>
        /// 当前小说
        /// </summary>
        public Book CurrentBook
        {
            get { return _currentBook; }
            set { Set(ref _currentBook, value); }
        }



        private ObservableCollection<BookCatalog> _bookCatalogs;
        /// <summary>
        ///目录列表
        /// </summary>
        public ObservableCollection<BookCatalog> BookCatalogs
        {
            get
            {
                return _bookCatalogs ?? (_bookCatalogs = new ObservableCollection<BookCatalog>());
            }
            set { Set(ref _bookCatalogs, value); }
        }



        private List<string> _contentPages;
        /// <summary>
        ///当前章节分页内容
        /// </summary>
        public List<string> ContentPages
        {
            get
            {
                return _contentPages ?? (_contentPages = new List<string>());
            }
            set { Set(ref _contentPages, value); }
        }


        private string _contentText;
        /// <summary>
        ///当前章节内容（未分页）
        /// </summary>
        public string ContentText
        {
            get { return _contentText; }
            set
            {
                Set(ref _contentText, value);
            }
        }


        #endregion

        #region 命令
        private ICommand _catalogCloseCommand;
        public ICommand CatalogCloseCommand => _catalogCloseCommand ?? (_catalogCloseCommand = new RelayCommand<object>(OnCatalogCloseCommand));

        public virtual void OnCatalogCloseCommand(object obj)
        {
            NavigationService.GoBack();
            HideStatusBar(true);
        }



        #endregion

        #region 方法

        ~OnlineContentPageViewModel()
        {
            _battery.RemainingChargePercentChanged -= _battery_RemainingChargePercentChanged;
            _timer.Stop();
        }


        public OnlineContentPageViewModel()
        {
            InitBattery();
            InitTimer();
            InitSettingValue();
            HideStatusBar(true);
        }

        public void LoadData(Book book)
        {
            CurrentBook = book;

            var catalog = new BookCatalog()
            {
                CatalogName = book.NewestChapterName,
                CatalogUrl = book.NewestChapterUrl,
            };

            SetCurrentContent(catalog);
            SetCatalogData(catalog);
        }


        public async void SetCurrentContent(BookCatalog catalog)
        {
            if (catalog == null)
            {
                return;
            }

            CurrentCatalog = catalog;
            var value = await GetCatalogContent(catalog);
            ContentPages = value.Item1;
            ContentText = value.Item2;
        }

        public async Task<Tuple<List<string>, string>> GetCatalogContent(BookCatalog catalog)
        {
            Tuple<List<string>, string> value = null;
            try
            {
                IsLoading = true;
                await Task.Run(async () =>
                {
                    string html = null;
                    List<string> list = null;

                    if (DicContentCache.ContainsKey(catalog.CatalogUrl))
                    {
                        value = DicContentCache[catalog.CatalogUrl];
                        return;
                    }
                    if (CurrentBook.IsLocal)
                    {
                        html = await GetCatalogContentFormDb(catalog);
                        list = SplitContentToPages(html);
                        if (html != null && list != null)
                        {
                            value = new Tuple<List<string>, string>(list, html);
                            if (!DicContentCache.ContainsKey(catalog.CatalogUrl))
                            {
                                DicContentCache.Add(catalog.CatalogUrl, value);
                            }
                            return;
                        }
                    }

                    html = await GetCatalogContentFromWeb(catalog);
                    list = SplitContentToPages(html);
                    if (html != null && list != null)
                    {
                        value = new Tuple<List<string>, string>(list, html);
                        if (!DicContentCache.ContainsKey(catalog.CatalogUrl))
                        {
                            DicContentCache.Add(catalog.CatalogUrl, value);
                        }
                    }
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            finally
            {
                IsLoading = false;
            }
            return value;
        }


        public void SetCatalogData(BookCatalog catalog)
        {
            IsLoadingCatalogData = true;
            var catalogUrl = AnalisysSourceHelper.GetCatalogPageUrl(catalog.CatalogUrl);
            SetCatalogPageData(catalogUrl);
            IsLoadingCatalogData = false;
        }


        private async void SetCatalogPageData(string url, int retryCount = 3)
        {
            try
            {
                var i = 0;
                Tuple<List<BookCatalog>, string, string, string> value = null;
                while (i < retryCount)
                {
                    value = await AnalisysSourceHelper.GetCatalogPageData(url);
                    if (value != null)
                    {
                        break;
                    }
                    Debug.WriteLine($"加载目录失败，第{i}次尝试");
                    i++;
                }

                if (value == null)
                {
                    Debug.WriteLine("加载目录失败，不再次尝试");
                    return;
                }

                if (value.Item1 != null)
                {
                    CurrentBook.CatalogList = new ObservableCollection<BookCatalog>();
                    foreach (var bookCatalog in value.Item1)
                    {
                        CurrentBook.CatalogList.Add(bookCatalog);
                    }
                }
                CurrentBook.Description = value.Item2;
                CurrentBook.Cover = value.Item3;
                CurrentBook.AuthorName = value.Item4;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task<string> GetCatalogContentFromWeb(BookCatalog catalog, int retryCount = 3)
        {
            var i = 0;
            string html = null;
            while (i < retryCount)
            {
                html = await AnalisysSourceHelper.GetCatalogContent(catalog.CatalogUrl);
                if (html != null)
                {
                    break;
                }
                Debug.WriteLine($"加载目录失败，第{i}次尝试");
                i++;
            }

            if (html == null)
            {
                Debug.WriteLine("加载目录失败，不再次尝试");
            }
            return html;
        }


        private async Task<string> GetCatalogContentFormDb(BookCatalog catalog)
        {
            await Task.Delay(100);
            return null;
        }


        private List<string> SplitContentToPages(string html)
        {
            return new List<string>();
        }


        public override void OnBackCommand(object obj)
        {
            base.OnBackCommand(obj);
            ShowStatusBar();
        }

        #endregion


        #region 设置相关

        private readonly Tuple<Brush, Brush> _nightModeColor = new Tuple<Brush, Brush>(new SolidColorBrush(Color.FromArgb(255, 12, 12, 12)), new SolidColorBrush(Color.FromArgb(255, 90, 90, 90)));

        private Tuple<Brush, Brush> _selectedColor;
        /// <summary>
        ///  当前选中的颜色
        /// </summary>
        public Tuple<Brush, Brush> SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                Set(ref _selectedColor, value);

                var index = ContentColors.IndexOf(value);
                if (index != -1)
                {
                    AppSettingService.SetKeyValue(SettingKey.ContentColorIndex, index);
                }
            }
        }

        private List<Tuple<Brush, Brush>> _contentColors;
        /// <summary>
        /// 背景 字体颜色（固定提供的）
        /// </summary>
        public List<Tuple<Brush, Brush>> ContentColors
        {
            get
            {
                return _contentColors ?? (_contentColors = GetColors());
            }
            set
            {
                Set(ref _contentColors, value);
            }
        }

        private double _fontSize;

        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                Set(ref _fontSize, value);
                AppSettingService.SetKeyValue(SettingKey.FontSize, value);
            }
        }

        private double _lineHeight;

        public double LineHeight
        {
            get { return _lineHeight; }
            set
            {
                Set(ref _lineHeight, value);
                AppSettingService.SetKeyValue(SettingKey.LineHeight, value);
            }
        }


        private double _lightValue = 100;

        public double LightValue
        {
            get { return _lightValue; }
            set
            {
                Set(ref _lightValue, value);
                AppSettingService.SetKeyValue(SettingKey.LightValue, value);
            }
        }


        private bool _isNightMode;

        public bool IsNightMode
        {
            get { return _isNightMode; }
            set
            {
                Set(ref _isNightMode, value);
                AppSettingService.SetKeyValue(SettingKey.IsNightMode, value);
                if (value)
                {
                    SelectedColor = _nightModeColor;
                }
                else
                {
                    var index = AppSettingService.GetKeyValue(SettingKey.ContentColorIndex);
                    SelectedColor = index == null ? ContentColors[0] : ContentColors[int.Parse(index.ToString())];
                }
            }
        }

        private bool _isLandscape = false;
        /// <summary>
        /// 是否开启横向模式
        /// </summary>
        public bool IsLandscape
        {

            get
            {
                return _isLandscape;
            }
            set
            {

                if (value == _isLandscape)
                {
                    return;
                }
                Set(ref _isLandscape, value);
                SetLandscape(value);
            }
        }

        private bool _isScroll = false;
        /// <summary>
        /// 是否滚动阅读
        /// </summary>
        public bool IsScroll
        {

            get
            {
                return _isScroll;
            }
            set
            {

                if (value == _isScroll)
                {
                    return;
                }
                Set(ref _isScroll, value);
                AppSettingService.SetKeyValue(SettingKey.LightValue, value);

            }
        }

        private ICommand _lineHeightCommand;
        public ICommand LineHeightCommand => _lineHeightCommand ?? (
                 _lineHeightCommand = new RelayCommand<object>(
                       (obj) =>
                       {
                           if (obj.ToString().Equals("-") && LineHeight < 50)
                           {
                               LineHeight = LineHeight + 2;
                           }

                           if (obj.ToString().Equals("0") && LineHeight > FontSize)
                           {
                               LineHeight = LineHeight - 2;
                           }

                       }));


        private ICommand _fontSizeChangedCommand;
        public ICommand FontSizeChangedCommand => _fontSizeChangedCommand ?? (
                 _fontSizeChangedCommand = new RelayCommand<object>(
                       (obj) =>
                     {
                         if (obj.ToString().Equals("+") && FontSize < 30)
                         {
                             FontSize = FontSize + 1;
                         }

                         if (obj.ToString().Equals("-") && FontSize > 14)
                         {
                             FontSize = FontSize - 1;
                         }
                     }));


        private ICommand _landsacpeCommand;
        public ICommand LandsacpeCommand => _landsacpeCommand ?? (
                 _landsacpeCommand = new RelayCommand<object>(
                       (obj) =>
                       {
                           IsLandscape = !IsLandscape;
                       }));

        private ICommand _nightModeCommand;
        public ICommand NightModeCommand => _nightModeCommand ?? (
                 _nightModeCommand = new RelayCommand<object>(
                       (obj) =>
                       {
                           IsNightMode = !IsNightMode;
                       }));

        private ICommand _catalogCommand;
        public ICommand Catalogcommand => _catalogCommand ?? (
                 _catalogCommand = new RelayCommand<object>(
                       (obj) =>
                       {
                           if (IsLoadingCatalogData)
                           {
                               ToastHelper.ShowMessage("正在加载目录，请稍后");
                               return;
                           }
                           NavigationService.NavigateTo(typeof(CatalogPage));
                           ShowStatusBar(true);
                       }));

        private void InitSettingValue()
        {
            //初始化字体颜色 背景颜色
            var index = AppSettingService.GetKeyValue(SettingKey.ContentColorIndex);
            if (index == null)
            {
                SelectedColor = ContentColors[0];
            }
            else
            {
                _selectedColor = ContentColors[int.Parse(index.ToString())];
            }

            //字体大小
            var fontSize = AppSettingService.GetKeyValue(SettingKey.FontSize);
            if (fontSize == null)
            {
                FontSize = 20;
            }
            else
            {
                _fontSize = double.Parse(fontSize.ToString());
            }

            //行高
            var lineHeight = AppSettingService.GetKeyValue(SettingKey.LineHeight);
            if (lineHeight == null)
            {
                LineHeight = 32;
            }
            else
            {
                _lineHeight = double.Parse(lineHeight.ToString());

            }

            //亮度
            var lightValue = AppSettingService.GetKeyValue(SettingKey.LightValue);
            if (lightValue == null)
            {
                LightValue = 100;
            }
            else
            {
                _lightValue = double.Parse(lightValue.ToString());

            }

            //夜间模式
            var isNightMode = AppSettingService.GetKeyValue(SettingKey.IsNightMode);
            if (isNightMode == null)
            {
                IsNightMode = false;
            }
            else
            {
                _isNightMode = bool.Parse(isNightMode.ToString());
            }

        }

        private List<Tuple<Brush, Brush>> GetColors()
        {
            var list = new List<Tuple<Brush, Brush>>();

            for (var i = 1; i <= 8; i++)
            {
                var backColor = "BackColor" + i;
                var textColor = "TextColor" + i;
                list.Add(new Tuple<Brush, Brush>(Application.Current.Resources[backColor] as SolidColorBrush, Application.Current.Resources[textColor] as SolidColorBrush));
            }
            return list;
        }

        public void SetLandscape(bool value)
        {
            DisplayInformation.AutoRotationPreferences = IsLandscape ? DisplayOrientations.Landscape : DisplayOrientations.None;
        }
        #endregion

        #region 系统数据（电量，时间）

        private Battery _battery;

        private DispatcherTimer _timer;

        private string _batteryValue = "N/A";
        /// <summary>
        ///电池电量
        /// </summary>
        public string BatteryValue
        {
            get { return _batteryValue; }
            set
            {
                Set(ref _batteryValue, value);
            }
        }

        private string _currentTime;
        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                Set(ref _currentTime, value);
            }
        }


        private void InitBattery()
        {
            if (PlatformHelper.IsMobileDevice)
            {
                _battery = Battery.GetDefault();
                BatteryValue = string.Format("{0}", _battery.RemainingChargePercent);
                _battery.RemainingChargePercentChanged += _battery_RemainingChargePercentChanged;
            }

        }
        private void _battery_RemainingChargePercentChanged(object sender, object e)
        {
            BatteryValue = string.Format("{0}", _battery.RemainingChargePercent);
        }


        private void InitTimer()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(20) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        #endregion

    }
}
