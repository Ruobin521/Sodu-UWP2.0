using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using Sodu.Core.Entity;
using Sodu.View;

namespace Sodu.ViewModel

{
    public class BasePageViewModel : CommonPageViewModel
    {
        #region 属性

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


        private int _pageIndex;
        /// <summary>
        ///当前页面索引
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set { Set(ref _pageIndex, value); }
        }


        private int _pageCount = -1;
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


        private string _helpText;
        /// <summary>
        ///title
        /// </summary>
        public string HelpText
        {
            get { return _helpText; }
            set { Set(ref _helpText, value); }
        }


        #endregion

        #region 命令

        /// <summary>
        /// 下拉刷新
        /// </summary>
        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new RelayCommand<object>(OnRefreshCommand));

        /// <summary>
        /// 上拉加载
        /// </summary>
        private ICommand _pullToLoadCommand;
        public ICommand PullToLoadCommand => _pullToLoadCommand ?? (_pullToLoadCommand = new RelayCommand<object>(OnPullToLoadCommand));


        /// <summary>
        ///搜索
        /// </summary>
        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ?? (_searchCommand = new RelayCommand<object>(OnSearchCommand));


        #endregion

        #region 已实现方法
        public void OnSearchCommand(object obj)
        {
            NavigationService.NavigateTo(typeof(SearchPage));
        }


        #endregion

        #region 未实现方法（需要子类重写）

        /// <summary>
        /// 初始化时加载数据
        /// </summary>
        public virtual void LoadData(object obj = null)
        {

        }

        public virtual void OnPullToLoadCommand(object obj)
        {

        }

        public virtual void OnRefreshCommand(object obj)
        {
        }

       
        #endregion
    }
}
