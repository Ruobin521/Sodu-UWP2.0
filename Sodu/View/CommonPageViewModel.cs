using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.Core.Util;
using Sodu.Service;
using Sodu.ViewModel;

namespace Sodu.View
{
    public class CommonPageViewModel : ViewModelBase
    {
        #region 属性
        public bool IsCancleRequest { get; set; }

        public HttpHelper Http { get; set; }


        private bool _isLoading;
        /// <summary>
        ///正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }


        #endregion

        #region 方法

        internal async Task<string> GetHtmlData(string url, bool isShowLoading, bool time)
        {
            string html = null;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (isShowLoading)
                {
                    IsLoading = true;
                }
            });
            try
            {
                Http = new HttpHelper();
                html = await Http.WebRequestGet(url, time);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                html = null;
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (isShowLoading)
                    {
                        IsLoading = false;
                    }
                });
            }
            return html;
        }

        internal async Task<string> GetHtmlData2(string url, bool isShowLoading, bool time)
        {
            string html = null;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (isShowLoading)
                {
                    IsLoading = true;
                }
            });
            try
            {
                Http = new HttpHelper();
                html = await Http.HttpClientGetRequest(url, time);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                html = null;
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (isShowLoading)
                    {
                        IsLoading = false;
                    }
                });
            }
            return html;
        }


        public async void HideStatusBar(bool isContent = false)
        {
            if (PlatformHelper.IsMobileDevice)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = isContent ? Color.FromArgb(255, 25, 25, 25) : Contants.ConstantValue.AppMainColor;
                await statusBar.HideAsync();
            }
        }

        public async void ShowStatusBar(bool isContent = false)
        {
            if (PlatformHelper.IsMobileDevice)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = isContent ? Color.FromArgb(255, 25, 25, 25) : Contants.ConstantValue.AppMainColor;
                await statusBar.ShowAsync();
            }
        }

        #endregion

        #region 命令

        /// <summary>
        /// 添加至书架
        /// </summary>
        private ICommand _addToOnlineShelfCommand;
        public ICommand AddToOnlineShelfCommand => _addToOnlineShelfCommand ?? (_addToOnlineShelfCommand = new RelayCommand<object>(OnAddToOnlineShelfCommand));

        public async void OnAddToOnlineShelfCommand(object obj)
        {
            var book = obj as Book;
            if (book == null)
            {
                return;
            }

            if (!ViewModelInstance.Instance.Main.IsLogin)
            {
                ToastHelper.ShowMessage("您尚未登陆，请登录后操作");
                return;
            }
            if (ViewModelInstance.Instance.OnLineBookShelf.Books.FirstOrDefault(p => p.BookId == book.BookId) != null)
            {
                ToastHelper.ShowMessage("您已添加该小说至在线书架");
                return;
            }

            var url = string.Format(SoduPageValue.AddToShelfPage, book.BookId);
            var html = await GetHtmlData2(url, false, false);

            if (html.Contains("{\"success\":true}"))
            {
                var temp = book.Clone();
                temp.LastReadChapterName = temp.NewestChapterName;
                var result = DbBookShelf.InsertOrUpdateBook(AppDataPath.GetAppCacheDbPath(), temp, AppSettingService.GetKeyValue(SettingKey.UserName) as string);
                ViewModelInstance.Instance.OnLineBookShelf.Books.Insert(0, temp);
                ToastHelper.ShowMessage("添加到在线书架成功");
            }
            else
            {
                ToastHelper.ShowMessage(book.BookName + " 添加至个人书架失败");
            }
        }


        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ?? (_backCommand = new RelayCommand<object>(OnBackCommand));

        public virtual void OnBackCommand(object obj)
        {
            NavigationService.GoBack();
        }

        private ICommand _cancleHttpRequestCommand;
        public ICommand CancleHttpRequestCommand => _cancleHttpRequestCommand ?? (_cancleHttpRequestCommand = new RelayCommand<object>(OnCancleHttpRequestCommand));

        public virtual void OnCancleHttpRequestCommand(object obj)
        {
            if (Http != null)
            {
                IsCancleRequest = true;
                Http.HttpClientCancleRequest();
                Http.HttpClientCancleRequest2();
            }
        }


        /// <summary>
        ///  点击列表项
        /// </summary>
        private ICommand _itemClickCommand;
        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<object>(OnItemClickCommand));

        public virtual async void OnItemClickCommand(object obj)
        {
            NavigationService.NavigateTo(typeof(UpdateCatalogPage));
            await Task.Delay(100);
            ViewModelInstance.Instance.UpdateCatalog.LoadData(obj);

            if (ViewModelInstance.Instance.Main.IsLogin && AppSettingService.GetBoolKeyValue(SettingKey.IsAutoAddToOnlineShelf))
            {
                OnAddToOnlineShelfCommand(obj);
            }
        }

        #endregion

    }
}
