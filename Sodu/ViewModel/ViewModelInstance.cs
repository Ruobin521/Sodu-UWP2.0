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

        private OnlineBookShelfPageViewModel _onLineBookShelf;
        public OnlineBookShelfPageViewModel OnLineBookShelf
        {
            get { return _onLineBookShelf ?? (_onLineBookShelf = new OnlineBookShelfPageViewModel()); }
            set
            {
                Set(ref _onLineBookShelf, value);
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
    }
}
