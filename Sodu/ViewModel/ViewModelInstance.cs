using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.ViewModel
{
    public class ViewModelInstance : BasePageViewModel
    {
        private static readonly object SynObject = new object();

        private static ViewModelInstance _instance;
        public static ViewModelInstance Instance
        {
            get
            {
                lock (SynObject)
                {
                    return _instance ?? (_instance = new ViewModelInstance());
                }
            }
        }

        private MainViewModel _main;
        public MainViewModel Main
        {
            get { return _main ?? (_main = new MainViewModel()); }
            set
            {
                Set(ref _main, value);
            }
        }

        private OnlineBookShelfPageViewModel _onLineBookShelf;
        public OnlineBookShelfPageViewModel OnLineBookShelf
        {
            get { return _onLineBookShelf ?? (_onLineBookShelf = new OnlineBookShelfPageViewModel()); }
            set
            {
                Set(ref _onLineBookShelf, value);
            }
        }

        private RankPageViewModel _rank;
        public RankPageViewModel Rank
        {
            get { return _rank ?? (_rank = new RankPageViewModel()); }
            set
            {
                Set(ref _rank, value);
            }
        }

        private HotAndRecommendPageViewModel _hotAndRecommend;
        public HotAndRecommendPageViewModel HotAndRecommend
        {
            get { return _hotAndRecommend ?? (_hotAndRecommend = new HotAndRecommendPageViewModel()); }
            set
            {
                Set(ref _hotAndRecommend, value);
            }
        }


        private LocalBookPageViewModel _localBookPage;
        public LocalBookPageViewModel LocalBookPage
        {
            get { return _localBookPage ?? (_localBookPage = new LocalBookPageViewModel()); }
            set
            {
                Set(ref _localBookPage, value);
            }
        }



        private SettingPageViewModel _setting;
        public SettingPageViewModel Setting
        {
            get { return _setting ?? (_setting = new SettingPageViewModel()); }
            set
            {
                Set(ref _setting, value);
            }
        }


        private SearchPageViewModel _search;
        public SearchPageViewModel Search
        {
            get { return _search ?? (_search = new SearchPageViewModel()); }
            set
            {
                Set(ref _search, value);
            }
        }


        private UpdateCatalogPageViewModel _updateCatalog;
        public UpdateCatalogPageViewModel UpdateCatalog
        {
            get { return _updateCatalog ?? (_updateCatalog = new UpdateCatalogPageViewModel()); }
            set
            {
                Set(ref _updateCatalog, value);
            }
        }



        private BookContentPageViewModel _bookContent;
        public BookContentPageViewModel  BookContent
        {
            get { return _bookContent ?? (_bookContent = new BookContentPageViewModel()); }
            set
            {
                Set(ref _bookContent, value);
            }
        }

        private HistoryPageViewModel _history;
        public HistoryPageViewModel History
        {
            get { return _history ?? (_history = new HistoryPageViewModel()); }
            set
            {
                Set(ref _history, value);
            }
        }

        private DownloadCenterPageViewModel _downloadCenter;
        public DownloadCenterPageViewModel DownloadCenter
        {
            get { return _downloadCenter ?? (_downloadCenter = new DownloadCenterPageViewModel()); }
            set
            {
                Set(ref _downloadCenter, value);
            }
        }
    }
}
