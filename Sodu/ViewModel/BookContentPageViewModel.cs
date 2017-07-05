using GalaSoft.MvvmLight.Command;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.Core.Util;
using Sodu.Service;
using Sodu.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Display;
using Windows.Phone.Devices.Power;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Sodu.ContentPageControl;
using Sodu.Core.Config;
using Sodu.Core.DataBase;

namespace Sodu.ViewModel
{
    public enum CatalogDirection
    {
        Pre,
        Next,
        Current
    }

    public class BookContentPageViewModel : BasePageViewModel
    {
        #region 属性


        /// <summary>
        /// 缓存数据（url,pages ,content）
        /// </summary>
        public Dictionary<string, string> DicContentCache = new Dictionary<string, string>();
        public Dictionary<string, List<string>> DicPagesCache = new Dictionary<string, List<string>>();




        /// <summary>
        /// 是否正在加载目录页数据（目录，简介，封面，作者）
        /// </summary>
        private bool IsLoadingCatalogData { get; set; }

        public bool IsPreLoadingNextCatalog { get; set; }
        public bool IsPreLoadingPreCatalog { get; set; }
        public bool IsPreLoadingCatalog { get; set; }

        public BookCatalog NextCatalog { get; set; }
        public BookCatalog PreCatalog { get; set; }

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
            private set
            {
                Set(ref _currentCatalog, value);
            }
        }

        private string _currentCatalogContent;
        /// <summary>
        ///当前章节内容（未分页）
        /// </summary>
        public string CurrentCatalogContent
        {
            get { return _currentCatalogContent; }
            set
            {
                Set(ref _currentCatalogContent, value);
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

        private bool _isLocalBook = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsLocalBook
        {

            get
            {
                return _isLocalBook;
            }
            set
            {
                Set(ref _isLocalBook, value);
            }
        }

        private int _catalogCount;

        public int CatalogCount
        {
            get { return _catalogCount; }
            set
            {
                Set(ref _catalogCount, value);
            }
        }


        private bool _isRetry;
        /// <summary>
        ///是否需要重新加载
        /// </summary>
        public bool IsRetry
        {
            get { return _isRetry; }
            set { Set(ref _isRetry, value); }
        }


        #endregion

        #region 命令
        private ICommand _catalogCloseCommand;
        public ICommand CatalogCloseCommand => _catalogCloseCommand ?? (_catalogCloseCommand = new RelayCommand<object>(OnCatalogCloseCommand));



        private ICommand _catalogSelectedCommand;
        public ICommand CatalogSelectedCommand => _catalogSelectedCommand ?? (_catalogSelectedCommand = new RelayCommand<object>(OnCatalogSelectedCommand));


        private ICommand _retryCommand;
        public ICommand RetryCommand => _retryCommand ?? (_retryCommand = new RelayCommand<object>(OnRetryCommand));


        private ICommand _addToLocalShelfCommand;
        public ICommand AddToLocalShelfCommand => _addToLocalShelfCommand ?? (_addToLocalShelfCommand = new RelayCommand<object>(OnAddToLocalShelfCommand));


        #endregion

        #region 方法

        public BookContentPageViewModel()
        {
            InitBattery();
            InitTimer();
            InitSettingValue();
        }


        public void LoadData(Book book)
        {
            ResDeta();

            CurrentBook = book.Clone();

            var catalog = new BookCatalog()
            {
                CatalogName = book.LastReadChapterName,
                CatalogUrl = book.LastReadChapterUrl,
            };

            SetAddToLocalShelfButtonVisibility();

            SetCurrentContent(catalog);

            InitCatalogsData(catalog);
        }

        /// <summary>
        /// 设置添加到本地书架按钮是否显示
        /// </summary>
        private void SetAddToLocalShelfButtonVisibility()
        {
            Task.Run(() =>
            {
                if (CurrentBook == null)
                {
                    return;
                }

                var bookId = CurrentBook.BookId;

                var ifExist = ViewModelInstance.Instance.LocalBookPage.CheckBookExist(bookId);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsLocalBook = ifExist;
                });
            });
        }

        /// <summary>
        /// 设置当前章节内容，这是唯一入口
        /// </summary>
        /// <param name="catalog"></param>
        public async void SetCurrentContent(BookCatalog catalog)
        {
            try
            {
                if (catalog == null) { return; }

                CurrentCatalog = catalog;

                //  PreLoadPreAndNextCatalog();
                Debug.WriteLine("-----------开始获取章节数据");

                var value = await GetCatalogContent(catalog, true);


                Debug.WriteLine("-----------获取章节数据完成");


                if (value != null)
                {
                    CurrentCatalogContent = value.Item2;
                    CurrentBook.LastReadChapterName = catalog.CatalogName;
                    CurrentBook.LastReadChapterUrl = catalog.CatalogUrl;
                    UpdateDatabase();
                    IsRetry = false;
                }
                else
                {
                    IsRetry = true;
                    CurrentCatalogContent = "";
                }

                Messenger.Default.Send(CurrentCatalogContent, "CurrentCatalogContentChanged");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }
    
        /// <summary>
        /// 根据方向获取目录数据
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="isShowLoading"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string, BookCatalog>> GetCatalogDataByDirection(CatalogDirection dir, bool isShowLoading = false)
        {
            var catalog = GetCatalogByDirction(dir);
            if (catalog == null)
            {
                return null;
            }

            var value = await GetCatalogContent(catalog, isShowLoading);

            return new Tuple<List<string>, string, BookCatalog>(value.Item1, value.Item2, catalog);
        }

        /// <summary>
        ///  根据方向获取目录
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public BookCatalog GetCatalogByDirction(CatalogDirection dir)
        {
            if (CurrentCatalog == null || CurrentBook.CatalogList == null || CurrentBook.CatalogList.Count == 0)
            {
                return null;
            }

            var temp = CurrentBook.CatalogList.FirstOrDefault(p => p.CatalogUrl == CurrentCatalog.CatalogUrl);

            if (temp == null && dir == CatalogDirection.Next)
            {
                return null;
            }

            if (temp == null && dir == CatalogDirection.Current)
            {
                return CurrentCatalog;
            }

            var index = CurrentBook.CatalogList.IndexOf(temp);

            switch (dir)
            {
                case CatalogDirection.Current:

                    return temp;

                case CatalogDirection.Next:

                    if (index < CurrentBook.CatalogList.Count - 1)
                    {
                        return CurrentBook.CatalogList[index + 1];
                    }
                    break;

                case CatalogDirection.Pre:

                    if (index > 0)
                    {
                        return CurrentBook.CatalogList[index - 1];
                    }
                    break;
            }

            return null;
        }

        /// <summary>
        /// 获取目录内容入口
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="showLoading"></param>
        /// <returns></returns>
        public async Task<Tuple<List<string>, string>> GetCatalogContent(BookCatalog catalog, bool showLoading = false)
        {
            Tuple<List<string>, string> value = null;
            try
            {
                if (showLoading)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => { IsLoading = true; });
                }

                await Task.Run(async () =>
                {
                    try
                    {
                        if (CurrentBook.IsLocal || CurrentBook.IsTxt)
                        {
                            Debug.WriteLine("-----------开始获取本地-------------目录数据");

                            value = await GetLoaclBookCatalogContent(catalog);

                            Debug.WriteLine("-----------获取在线-------------目录数据完成");

                        }
                        else
                        {
                            Debug.WriteLine("-----------开始获取在线-------------章节数据");

                            value = await GetOnlineBookCatalogContent(catalog);

                            Debug.WriteLine("-----------获取在线-------------章节数据完成");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                    }
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + "\n" + e.StackTrace);
                value = null;
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                   IsLoading = false;
               });
            }
            return value;
        }

        /// <summary>
        /// 获取本地图书数据 Txt，本地缓存
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        private async Task<Tuple<List<string>, string>> GetLoaclBookCatalogContent(BookCatalog catalog)
        {
            string html = null;
            List<string> list = null;

            try
            {
                if (DicContentCache.ContainsKey(catalog.CatalogUrl))
                {
                    html = DicContentCache[catalog.CatalogUrl];
                }
                else
                {
                    if (CurrentBook.CatalogList != null)
                    {
                        var tempCatalog = CurrentBook.CatalogList.FirstOrDefault(p => p.CatalogUrl == catalog.CatalogUrl);

                        if (!string.IsNullOrEmpty(tempCatalog?.CatalogContent))
                        {
                            html = tempCatalog.CatalogContent;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("-----------检索sqlite数据库------------");

                        var temp = GetCatalogContentFormDb(catalog);

                        Debug.WriteLine("-----------检索完毕------------");

                        if (temp != null)
                        {
                            html = temp;
                        }
                        else
                        {
                            if (!CurrentBook.IsTxt)
                            {
                                html = await GetCatalogContentFromWeb(catalog);
                            }
                        }
                    }
                }

                if (html == null)
                {
                    return null;
                }

                list = GetCatalogPagesFormDicrionary(catalog.CatalogUrl);

                if (list != null)
                {
                    return new Tuple<List<string>, string>(list, html);
                }

                list = await SplitContentToPages(html);

                if (list == null || list.Count <= 0)
                {
                    return null;
                }

                DicContentCache.Add(catalog.CatalogUrl, html);
                DicPagesCache.Add(catalog.CatalogUrl, list);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                return null;
            }
            return new Tuple<List<string>, string>(list, html);
        }

        /// <summary>
        /// 获取在线章节 Online
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        private async Task<Tuple<List<string>, string>> GetOnlineBookCatalogContent(BookCatalog catalog)
        {
            string html = null;
            List<string> list = null;
            try
            {
                if (DicContentCache.ContainsKey(catalog.CatalogUrl))
                {
                    html = DicContentCache[catalog.CatalogUrl];
                }
                else
                {
                    html = await GetCatalogContentFromWeb(catalog);
                }

                if (html == null)
                {
                    return null;
                }

                list = GetCatalogPagesFormDicrionary(catalog.CatalogUrl);

                if (list != null)
                {
                    return new Tuple<List<string>, string>(list, html);
                }

                list = await SplitContentToPages(html);
                if (list == null || list.Count <= 0)
                {
                    return null;
                }

                DicContentCache.Add(catalog.CatalogUrl, html);
                DicPagesCache.Add(catalog.CatalogUrl, list);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                return null;
            }
            return new Tuple<List<string>, string>(list, html);
        }

        /// <summary>
        /// 获取在线数据具体实现方法
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        private async Task<string> GetCatalogContentFromWeb(BookCatalog catalog, int retryCount = 3)
        {
            var i = 0;
            IsCancleRequest = false;
            string html = null;
            while (i <= retryCount)
            {
                if (IsCancleRequest)
                {
                    Debug.WriteLine($"用户取消请求操作");
                    break;
                }
                html = await GetHtmlData(catalog.CatalogUrl, false, false);
                html = AnalisysSourceHelper.AnalisysCatalogContent(catalog.CatalogUrl, html);
                if (!string.IsNullOrEmpty(html))
                {
                    break;
                }

                Debug.WriteLine($"加载正文内容失败，第{i + 1}次尝试");
                i++;
            }

            if (string.IsNullOrEmpty(html))
            {
                Debug.WriteLine("加载正文内容失败，不再次尝试");
            }
            return html;
        }

        /// <summary>
        /// 获取本地数据库数据具体实现方法
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        private string GetCatalogContentFormDb(BookCatalog catalog)
        {
            string result = null;
            try
            {
                Debug.WriteLine($"-----------开始搜索:{catalog.CatalogUrl}------------");

                var tempCatalog = DbLocalBook.SelectBookCatalogById(AppDataPath.GetLocalBookDbPath(), CurrentBook.BookId,
                           catalog.CatalogUrl);

                Debug.WriteLine($"-----------搜索:{catalog.CatalogUrl}完成------------");


                result = tempCatalog?.CatalogContent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取目录页数据入口方法
        /// </summary>
        /// <param name="catalog"></param>
        public void InitCatalogsData(BookCatalog catalog)
        {
            Task.Run(() =>
            {
                if (CurrentBook == null || catalog?.CatalogUrl == null)
                {
                    return;
                }

                if (CurrentBook.IsLocal || CurrentBook.IsTxt)
                {
                    SetLocalBookCatalogsData(catalog);
                }
                else
                {
                    SetOnlineBookCatalogsData(AnalisysSourceHelper.GetCatalogPageUrl(catalog.CatalogUrl));
                }
            });
        }

        /// <summary>
        /// 获取在线所有目录数据具体实现方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="retryCount"></param>
        private async void SetOnlineBookCatalogsData(string url, int retryCount = 3)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return;
                }

                IsLoadingCatalogData = true;

                var i = 0;
                Tuple<List<BookCatalog>, string, string, string> value = null;
                while (i <= retryCount)
                {
                    value = await AnalisysSourceHelper.GetCatalogPageData(url, CurrentBook.BookId);
                    if (value != null)
                    {
                        break;
                    }
                    Debug.WriteLine($"加载目录失败，第{i + 1}次尝试");
                    i++;
                }

                if (value == null)
                {
                    Debug.WriteLine("加载目录失败，不再次尝试");
                    return;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (value.Item1 != null)
                    {
                        if (CurrentBook == null)
                        {
                            return;
                        }

                        CurrentBook.CatalogList = new List<BookCatalog>();
                        foreach (var bookCatalog in value.Item1)
                        {
                            CurrentBook.CatalogList.Add(bookCatalog);
                        }

                        CatalogCount = CurrentBook.CatalogList.Count;
                        var temp = CurrentBook.CatalogList.FirstOrDefault(p => p.CatalogUrl == CurrentCatalog.CatalogUrl);
                        if (temp != null)
                        {
                            CurrentCatalog = temp;
                            // PreLoadPreAndNextCatalog();
                        }

                    }
                    CurrentBook.Description = value.Item2;
                    CurrentBook.Cover = value.Item3;
                    CurrentBook.AuthorName = value.Item4;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                IsLoadingCatalogData = false;
            }
        }

        /// <summary>
        /// 获取本所有地数据库目录数据
        /// </summary>
        private void SetLocalBookCatalogsData(BookCatalog catalog)
        {
            Task.Run(() =>
            {
                IsPreLoadingCatalog = true;

                var catalogs = DbLocalBook.SelectBookCatalogsByBookId(AppDataPath.GetLocalBookDbPath(),
               CurrentBook.BookId);

                CurrentBook.CatalogList = catalogs;

                if (catalogs == null || catalogs.Count <= 0)
                {
                    return;
                }

                var temp = catalogs.LastOrDefault(p => p.CatalogUrl == catalog.CatalogUrl);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    CatalogCount = CurrentBook.CatalogList.Count;

                    if (temp != null)
                    {
                        CurrentCatalog = temp;
                    }
                });

                IsPreLoadingCatalog = false;
            });
        }

        /// <summary>
        /// 预加载上一章节下一章节
        /// </summary>
        private void PreLoadPreAndNextCatalog()
        {
            try
            {
                if (IsPreLoadingCatalog || CurrentBook?.CatalogList == null)
                {
                    return;
                }

                var tasks = new Task[2];

                tasks[0] = Task.Run(() =>
                {
                    PreLoadNextCatalog();
                });

                tasks[1] = Task.Run(() =>
                {
                    PreLoadPreCatalog();
                });

                Task.Factory.ContinueWhenAll(tasks, (c) =>
                {
                    IsPreLoadingCatalog = false;
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 预加载下一章节
        /// </summary>
        private async void PreLoadNextCatalog()
        {
            try
            {
                if (IsPreLoadingNextCatalog)
                {
                    return;
                }

                IsPreLoadingNextCatalog = true;

                if (CurrentBook == null)
                {
                    return;
                }
                var nextcatalog = GetCatalogByDirction(CatalogDirection.Next);

                if (nextcatalog == null)
                {
                    return;
                }
                await GetCatalogContent(nextcatalog);

                IsPreLoadingNextCatalog = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 预加载上一章节
        /// </summary>
        private async void PreLoadPreCatalog()
        {
            try
            {
                if (CurrentBook == null)
                {
                    return;
                }

                if (IsPreLoadingPreCatalog)
                {
                    return;
                }

                IsPreLoadingPreCatalog = true;

                var nextcatalog = GetCatalogByDirction(CatalogDirection.Pre);

                if (nextcatalog == null)
                {
                    return;
                }
                await GetCatalogContent(nextcatalog);
                IsPreLoadingPreCatalog = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }


        /// <summary>
        /// 更新历史纪录，更新本地图书记录
        /// </summary>
        private void UpdateDatabase()
        {
            try
            {
                if (CurrentBook.IsLocal || CurrentBook.IsOnline || CurrentBook.IsTxt)
                {

                    DbHelper.AddDbOperator(() =>
                    {
                        ViewModelInstance.Instance.LocalBookPage.InserOrUpdateBook(CurrentBook);
                    });
                }
                else
                {
                    DbHelper.AddDbOperator(() =>
                    {
                        ViewModelInstance.Instance.History.InserOrUpdateHistory(CurrentBook);
                    });
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 获取缓存内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetCatalogContentFormDicrionary(string url)
        {
            return DicContentCache.ContainsKey(url) ? DicContentCache[url] : null;
        }

        /// <summary>
        /// 获取缓存分页内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<string> GetCatalogPagesFormDicrionary(string url)
        {
            return DicPagesCache.ContainsKey(url) ? DicPagesCache[url] : null;
        }



        /// <summary>
        /// 添加本地书架在线版本
        /// </summary>
        /// <param name="obj"></param>
        public async void OnAddToLocalShelfCommand(object obj)
        {
            var dialog = new MessageDialog("是否添加本小说到本地收藏?", "收藏提示");

            dialog.Commands.Add(new UICommand("确定", cmd =>
            {
                if (!App.IsPro && ViewModelInstance.Instance.LocalBookPage.GetLocalBooksCount() >= 3)
                {
                    ToastHelper.ShowMessage("免费版本地书架只能收藏三本，专业版无限制.");
                    return;
                }

                CurrentBook.IsOnline = true;
                IsLocalBook = true;
                ViewModelInstance.Instance.LocalBookPage.InserOrUpdateBook(CurrentBook);

            }, commandId: 0));
            dialog.Commands.Add(new UICommand("取消", cmd =>
            {
            }, commandId: 1));

            //获取返回值
            await dialog.ShowAsync();
        }


        /// <summary>
        /// 点击目录列表切换当前章节内容
        /// </summary>
        /// <param name="obj"></param>
        public void OnCatalogSelectedCommand(object obj)
        {
            try
            {
                NavigationService.GoBack();
                SetCurrentContent(obj as BookCatalog);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");

            }
        }

        /// <summary>
        /// 点击重新加载按钮
        /// </summary>
        /// <param name="obj"></param>
        public async void OnRetryCommand(object obj)
        {
            IsRetry = false;
            await Task.Delay(50);
            SetCurrentContent(CurrentCatalog);
        }

        /// <summary>
        /// 关闭目录列表页
        /// </summary>
        /// <param name="obj"></param>
        public void OnCatalogCloseCommand(object obj)
        {
            NavigationService.GoBack();
        }




        #region 通用方法

        public override void OnBackCommand(object obj)
        {
            base.OnBackCommand(obj);
            ResDeta();
        }


        public void ResDeta()
        {
            IsLocalBook = false;

            PageIndex = 0;
            PageCount = 0;
            CatalogCount = 0;
            IsRetry = false;

            CurrentBook = null;
            CurrentCatalog = null;
            CurrentCatalogContent = null;

            DicContentCache.Clear();
            DicPagesCache.Clear();

            NextCatalog = null;
            PreCatalog = null;

            OnCancleHttpRequestCommand(null);
        }


        #endregion

        #endregion



        #region 分页相关

        private double ContainerWidth { get; set; } = 0.0;
        private double ContainerHeight { get; set; } = 0.0;

        public async void ResizePageContent()
        {
            if (string.IsNullOrEmpty(CurrentCatalogContent))
            {
                return;
            }

            Tuple<double, double> size = null;
            await App.RootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var content = NavigationService.ContentFrame.Content as BookContentPage;
                if (content == null)
                {
                    return;
                }
                size = content.GetControlSize();
            });

            if (Math.Abs(size.Item1 - ContainerWidth) < 0.0001 && Math.Abs(size.Item2 - ContainerHeight) < 0.0001)
            {
                return;
            }

            DicPagesCache.Clear();


            var pages = await SplitContentToPages(CurrentCatalogContent);

            if (pages == null)
            {
                return;
            }

            DicPagesCache.Add(CurrentCatalog.CatalogUrl, pages);

            if (PageIndex > pages.Count - 1)
            {
                PageIndex = pages.Count - 1;
            }

            PageCount = pages.Count;

            // Messenger.Default.Send(CurrentCatalogContent, "ContentTextChanged");
        }

        private async Task<List<string>> SplitContentToPages(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            List<string> list = null;
            await Task.Run(async () =>
          {
              Tuple<double, double> size = null;

              await App.RootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
             {
                 var content = NavigationService.ContentFrame.Content as BookContentPage;
                 if (content == null)
                 {
                     return;
                 }
                 size = content.GetControlSize();
             });

              if (size != null)
              {
                  list = GetSplitContentPages(html, size.Item1, size.Item2, FontSize, LineHeight);
              }
          });
            return list;
        }

        private List<string> SplitString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            List<string> strList = new List<string>();
            string[] lists = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lists.Count(); i++)
            {
                if (!string.IsNullOrEmpty(lists[i]) && !string.IsNullOrEmpty(lists[i].Trim()))
                {
                    strList.Add("　　" + lists[i].Trim());
                }
            }
            return strList;
        }

        private List<string> GetSplitContentPages(string html, double containerWidth, double containerHeight, double fontSize, double lineHeight)
        {
            var list = SplitString(html);

            if (list == null || list.Count == 0)
            {
                return null;
            }
            var paragraphs = list;

            int linesCount = (int)(containerHeight / lineHeight);
            int perLineCount = (int)(containerWidth / fontSize);


            if ((containerHeight % lineHeight) / lineHeight > 0.8)
            {
                linesCount = linesCount + 1;
            }
            List<string> pages = new List<string>();

            try
            {
                int i = 0;
                string tempPageContent = string.Empty;
                for (int j = 0; j < paragraphs.Count; j++)
                {
                    string str = paragraphs[j];
                    string lineStr = string.Empty;
                    var chars = str.ToArray();
                    var tempList = chars.ToList();

                    //将一段内容逐字符添加到内容中
                    for (int m = 0; m < tempList.Count; m++)
                    {
                        var word = tempList[m];
                        lineStr += word;
                        //换行
                        if (lineStr.Length == perLineCount || m == tempList.Count - 1)
                        {
                            //将一行内容添加到当前分页中
                            tempPageContent += lineStr + "\r";
                            lineStr = string.Empty;
                            i++;
                            //如果一页内容填满就添加下一页
                            if (i == linesCount)
                            {
                                pages.Add(tempPageContent);
                                tempPageContent = string.Empty;
                                i = 0;
                            }
                        }
                    }

                    if (j == paragraphs.Count - 1 && !string.IsNullOrEmpty(tempPageContent))
                    {
                        pages.Add(tempPageContent);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return pages;
        }

        #endregion


        #region 滚动切换章节

        public async void ScrollToSwitchCurrentCatalog(CatalogDirection dir)
        {
            if (IsLoading)
            {
                return;
            }
            var value = GetCatalogByDirction(dir);
            if (value != null)
            {
                SetCurrentContent(value);
            }
            else
            {
                if (IsLoadingCatalogData)
                {
                    ToastHelper.ShowMessage("目录数据加载中，请稍后");
                    return;
                }

                if (CurrentBook.CatalogList == null)
                {
                    ToastHelper.ShowMessage("没有目录数据");
                    return;
                }

                await Task.Delay(10);
                NavigationService.NavigateTo(typeof(CatalogPage));
            }
        }


        #endregion


        #region 设置相关

        private readonly Tuple<Brush, Brush> _nightModeColor = new Tuple<Brush, Brush>(new SolidColorBrush(Color.FromArgb(255, 12, 12, 12)), new SolidColorBrush(Color.FromArgb(255, 90, 90, 90)));


        private Tuple<Brush, Brush> _contentColor;
        /// <summary>
        ///  当前选中的颜色
        /// </summary>
        public Tuple<Brush, Brush> ContentColor
        {
            get
            {
                return _contentColor;
            }
            set
            {
                Set(ref _contentColor, value);
            }
        }

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
                AppSettingService.SetKeyValue(SettingKey.ContentColorIndex, index);

                if (!IsNightMode)
                {
                    ContentColor = value;
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
                    ContentColor = _nightModeColor;
                }
                else
                {
                    var index = AppSettingService.GetKeyValue(SettingKey.ContentColorIndex);
                    ContentColor = index == null ? ContentColors[0] : ContentColors[int.Parse(index.ToString())];
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
                AppSettingService.SetKeyValue(SettingKey.IsLandscape, value);
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
                AppSettingService.SetKeyValue(SettingKey.IsScroll, value);

            }
        }

        private ICommand _lineHeightCommand;
        public ICommand LineHeightCommand => _lineHeightCommand ?? (
                 _lineHeightCommand = new RelayCommand<object>(
                       (obj) =>
                       {
                           if (obj.ToString().Equals("-"))
                           {
                               LineHeight = LineHeight - 1;
                           }

                           if (obj.ToString().Equals("+"))
                           {
                               LineHeight = LineHeight + 1;
                           }

                           ResizePageContent();
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

                         ResizePageContent();
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
        public ICommand CatalogCommand => _catalogCommand ?? (
                 _catalogCommand = new RelayCommand<object>(
                       (obj) =>
                       {
                           if (IsLoadingCatalogData)
                           {
                               ToastHelper.ShowMessage("正在加载目录，请稍后");
                               return;
                           }

                           if (CurrentBook.CatalogList == null)
                           {
                               ToastHelper.ShowMessage("获取目录数据失败，请换个来源重试",false);
                               return;
                           }
                           NavigationService.NavigateTo(typeof(CatalogPage));
                       }));


        private ICommand _downloadCommand;
        public ICommand DownloadCommand => _downloadCommand ?? (
                 _downloadCommand = new RelayCommand<object>(
                       (obj) =>
                       {

                           if (IsLoadingCatalogData)
                           {
                               ToastHelper.ShowMessage("正在加载目录数据，请稍后");
                               return;
                           }
                           if (CurrentBook?.CatalogList == null || CurrentBook.CatalogList.Count == 0)
                           {
                               ToastHelper.ShowMessage("获取目录数据失败，无法缓存，请换个尝试换个来源");
                               return;
                           }

                           var ifExist = ViewModelInstance.Instance.LocalBookPage.CheckBookExist(CurrentBook.BookId);

                           if (ifExist)
                           {
                               ToastHelper.ShowMessage(CurrentBook.BookName + "已缓存在本地书架，无需再次缓存");
                               return;
                           }

                           var count = ViewModelInstance.Instance.LocalBookPage.GetLocalBooksCount();
                           if (!App.IsPro && count >= 3)
                           {
                               ToastHelper.ShowMessage("免费版本只能缓存三本小说，专业版无此限制");
                               return;
                           }

                           ViewModelInstance.Instance.DownloadCenter.AddDownItem(CurrentBook);

                       }));

        private void InitSettingValue()
        {
            //初始化字体颜色 背景颜色
            var index = AppSettingService.GetIntValue(SettingKey.ContentColorIndex);
            if (index < 0)
            {
                SelectedColor = ContentColors[0];
            }
            else
            {
                _selectedColor = ContentColors[int.Parse(index.ToString())];
            }

            //字体大小
            var fontSize = AppSettingService.GetIntValue(SettingKey.FontSize);
            if (fontSize < 0)
            {
                FontSize = 20;
            }
            else
            {
                _fontSize = fontSize;
            }

            //行高
            var lineHeight = AppSettingService.GetIntValue(SettingKey.LineHeight);
            if (lineHeight < 0)
            {
                LineHeight = 32;
            }
            else
            {
                _lineHeight = lineHeight;

            }

            //亮度
            var lightValue = AppSettingService.GetIntValue(SettingKey.LightValue);
            if (lightValue < 0)
            {
                LightValue = 100;
            }
            else
            {
                _lightValue = lightValue;

            }

            //夜间模式
            IsNightMode = AppSettingService.GetBoolKeyValue(SettingKey.IsNightMode);

            //横屏模式
            IsLandscape = AppSettingService.GetBoolKeyValue(SettingKey.IsLandscape);

            //滚动阅读
            IsScroll = AppSettingService.GetBoolKeyValue(SettingKey.IsScroll);

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
            if (PlatformHelper.IsMobile)
            {
                _battery = Battery.GetDefault();
                BatteryValue = string.Format("{0}", _battery.RemainingChargePercent);
                _battery.RemainingChargePercentChanged += _battery_RemainingChargePercentChanged;
            }

        }
        private void _battery_RemainingChargePercentChanged(object sender, object e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BatteryValue = string.Format("{0}", _battery.RemainingChargePercent);
            });
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
