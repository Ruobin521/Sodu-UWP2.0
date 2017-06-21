using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;

namespace Sodu.ViewModel
{
    public class LocalBookPageViewModel : BasePageViewModel
    {

        #region 属性

        private ObservableCollection<LocalBookItemViewModel> _localBooks;
        /// <summary>
        ///列表项
        /// </summary>
        public ObservableCollection<LocalBookItemViewModel> LocalBooks
        {
            get
            {
                return _localBooks ?? (_localBooks = new ObservableCollection<LocalBookItemViewModel>());
            }
            set { Set(ref _localBooks, value); }
        }

        #endregion

        #region 命令


        /// <summary>
        /// 删除
        /// </summary>
        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand<object>(OnDeleteCommand));

        private void OnDeleteCommand(object obj)
        {

        }

        /// <summary>
        /// 删除
        /// </summary>
        private ICommand _setHadReadCommand;
        public ICommand SetHadReadCommand => _setHadReadCommand ?? (_setHadReadCommand = new RelayCommand<object>(OnSetHadReadCommand));

        private void OnSetHadReadCommand(object obj)
        {
            var localBookl = obj as LocalBookItemViewModel;

            if (localBookl?.CurrentBook != null)
            {
                localBookl.CurrentBook.IsNew = false;
            }
        }


        #endregion

        #region 方法

        public LocalBookPageViewModel()
        {
            HelpText =
                "使用帮助：" + "\n" +
                "1.点击阅读正文菜单中的“缓存”按钮即可缓存全部章节内容,可在“设置-下载中心”中查看下载进度。" + "\n" +
                "2.点击阅读正文右下方红色按钮，添加该小说至本地书架进行在线阅读。" + "\n" +
                "3.下载或添加完毕后即可在此处点击阅读" + "\n" +
                "4.手机用户向左滑动即可删除当前项，向右滑动可标记为已读。" + "\n" +
                "5.PC用户点击鼠标右键进行相关不操作" + "\n" +
                "6.本地书架暂不支持本地TXT文件阅读。";
        }


        public override void LoadData(object obj = null)
        {
            if (IsLoading || LocalBooks.Count > 0)
            {
                return;
            }

            GetLocalBookFromDb();
        }


        private void GetLocalBookFromDb()
        {

            Task.Run(async () =>
            {
                await Task.Delay(100);
                var list = DbLocalBook.GetBooks(AppDataPath.GetLocalBookDbPath());
                if (list == null || list.Count <= 0)
                {
                    return;
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foreach (var book in list)
                    {
                        var localVm = new LocalBookItemViewModel()
                        {
                            CurrentBook = book
                        };
                        LocalBooks.Add(localVm);
                    }
                });
            });

        }

        public override void OnRefreshCommand(object obj)
        {

        }

        #endregion



    }
}
