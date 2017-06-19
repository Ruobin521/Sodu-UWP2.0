using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Entity;
using Sodu.Core.Util;
using Sodu.View;

namespace Sodu.ViewModel
{
    public class BasePageViewModel : ViewModelBase
    {
        public HttpHelper Http { get; set; }

        private ObservableCollection<Book> _books;
        /// <summary>
        ///列表项
        /// </summary>
        public ObservableCollection<Book> Books
        {
            get
            {
                return _books ?? (_books = new ObservableCollection<Book>());
            }
            set { Set(ref _books, value); }
        }


        private bool _isLoading;
        /// <summary>
        ///正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }


        private int _pageIndex;
        /// <summary>
        ///当前页面索引
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set { Set(ref _pageIndex, value); }
        }


        private int _pageCount;
        /// <summary>
        ///页面数量
        /// </summary>
        public int PageCount
        {
            get { return _pageCount; }
            set { Set(ref _pageCount, value); }
        }


        private string _title;
        /// <summary>
        ///title
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }


        /// <summary>
        /// 下拉刷新
        /// </summary>
        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new RelayCommand<object>(OnRefreshCommand));
        public virtual void OnRefreshCommand(object obj)
        {
        }


        /// <summary>
        /// 上拉加载
        /// </summary>
        private ICommand _pullToLoadCommand;
        public ICommand PullToLoadCommand => _pullToLoadCommand ?? (_pullToLoadCommand = new RelayCommand<object>(OnPullToLoadCommand));
        public virtual void OnPullToLoadCommand(object obj)
        {

        }


        /// <summary>
        ///  点击列表项
        /// </summary>
        private ICommand _itemClickCommand;
        public ICommand ItemClickCommand => _itemClickCommand ?? (_itemClickCommand = new RelayCommand<object>(OnItemClickCommand));
        public virtual void OnItemClickCommand(object obj)
        {


        }

        /// <summary>
        ///搜索
        /// </summary>
        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new RelayCommand<object>(OnSearchCommand));
        public void OnSearchCommand(object obj)
        {
            NavigationService.NavigateTo(typeof(SearchPage));
        }

        internal async Task<string> GetHtmlData(string url)
        {
            string html = null;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsLoading = true;
            });
            try
            {
                Http = new HttpHelper();
                html = await Http.WebRequestGet(url, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                html = null;
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsLoading = false;
                });
            }
            return html;
        }

        internal async Task<string> GetHtmlData2(string url)
        {
            string html = null;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                IsLoading = true;
            });
            try
            {
                Http = new HttpHelper();
                html = await Http.HttpClientGetRequest(url, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                html = null;
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsLoading = false;
                });
            }
            return html;
        }



        /// <summary>
        /// 加载数据
        /// </summary>
        public virtual void LoadData()
        {


        }
    }
}
