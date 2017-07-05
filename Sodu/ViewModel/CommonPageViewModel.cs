using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
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



        #endregion

        #region 命令

        /// <summary>
        /// 添加至书架
        /// </summary>
        private ICommand _addToOnlineShelfCommand;
        public ICommand AddToOnlineShelfCommand => _addToOnlineShelfCommand ?? (_addToOnlineShelfCommand = new RelayCommand<object>(OnAddToOnlineShelfCommand));

        public async void OnAddToOnlineShelfCommand(object obj)
        {
            if (!App.IsPro)
            {
                ToastHelper.ShowMessage("这是专业版功能，购买专业版即可使用");
                return;
            }

            var book = obj as Book;
            if (book == null)
            {
                return;
            }

            if (!ViewModelInstance.Instance.Main.IsLogin && ViewModelInstance.Instance.Setting.IsPro)
            {
                ToastHelper.ShowMessage("您尚未登陆，请登录后操作");
                return;
            }
            if (ViewModelInstance.Instance.OnLineBookShelf.Books.FirstOrDefault(p => p.BookId == book.BookId) != null)
            {
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
                ToastHelper.ShowMessage($"成功添加{book.BookName}到在线书架");
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

        public virtual void OnItemClickCommand(object obj)
        {
            ViewModelInstance.Instance.UpdateCatalog.ResetData();

            NavigationService.NavigateTo(typeof(UpdateCatalogPage));

            ViewModelInstance.Instance.UpdateCatalog.LoadData(obj);

            if (ViewModelInstance.Instance.Main.IsLogin && AppSettingService.GetBoolKeyValue(SettingKey.IsAutoAddToOnlineShelf))
            {
                OnAddToOnlineShelfCommand(obj);
            }
        }


        private ICommand _createTitleCommand;
        public ICommand CreateTitleCommand => _createTitleCommand ?? (_createTitleCommand = new RelayCommand<object>(OnCreatTileCommand));


        public async void OnCreatTileCommand(object obj)
        {
            if (!App.IsPro)
            {
                ToastHelper.ShowMessage("这是专业版功能，购买专业版即可使用");
                return;
            }

            Book book = null;

            if (obj is LocalBookItemViewModel)
            {
                book = (obj as LocalBookItemViewModel)?.CurrentBook;
            }
            else if (obj is Book)
            {
                book = (obj as Book);
            }

            if (book == null)
            {
                return;
            }

            //磁贴ID
            var tileid = book.BookId;
            //磁贴展示名称
            var displayName = book.BookName;
            //点击磁贴传回的参数

            var str = JsonConvert.SerializeObject(book);

            var titleEntity = new TitleEntity();

            titleEntity.TtitleId = book.BookId;

            titleEntity.BookJosn = str;

            if (book.IsLocal || book.IsOnline || book.IsTxt)
            {
                titleEntity.BookType = "1";
            }
            else
            {
                titleEntity.BookType = "0";
            }

            var args = JsonConvert.SerializeObject(titleEntity);

            //磁贴的路径
            var logourl =   new Uri("ms-appx:///Assets/Square150x150Logo.scale-150.png");

            //磁贴的大小
            var size = TileSize.Square150x150;
            //创建磁贴对象
            var tile = new SecondaryTile(tileid, displayName, args, logourl, size);
            //让磁贴显示展示名
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            //创建磁贴，返回bool值
            bool b = await tile.RequestCreateAsync();
            if (b)
            {
                ToastHelper.ShowMessage($"{book.BookName}磁贴创建成功");
            }
            else
            {
                ToastHelper.ShowMessage($"{book.BookName}磁贴创建失败",false);
            }
        }

        #endregion

    }
}
