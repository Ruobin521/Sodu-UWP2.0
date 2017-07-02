using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Newtonsoft.Json;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.Service;
using Sodu.View;

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
            var localItem = obj as LocalBookItemViewModel;
            if (localItem == null)
            {
                return;
            }

            Task.Run(() =>
            {
                var reslut = DbLocalBook.DeleteBook(AppDataPath.GetLocalBookDbPath(),
               localItem.CurrentBook.BookId);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (!reslut)
                    {
                        ToastHelper.ShowMessage($"{localItem.CurrentBook.BookName}删除失败，请重试", false);
                        return;
                    }
                    localItem.IsDeleted = true;
                    LocalBooks.Remove(localItem);
                });
            });
        }


        private ICommand _setHadReadCommand;
        public ICommand SetHadReadCommand => _setHadReadCommand ?? (_setHadReadCommand = new RelayCommand<object>(OnSetHadReadCommand));

        private void OnSetHadReadCommand(object obj)
        {
            var localBookl = obj as LocalBookItemViewModel;

            if (localBookl?.CurrentBook == null || (bool)!localBookl?.CurrentBook?.IsNew)
            {
                return;
            }
            localBookl.CurrentBook.IsNew = false;
            DbLocalBook.InsertOrUpdatBook(AppDataPath.GetLocalBookDbPath(), localBookl.CurrentBook);
        }

        private ICommand _addTxtCommand;
        public ICommand AddTxtCommand => _addTxtCommand ?? (_addTxtCommand = new RelayCommand<object>(OnAddTxtCommand));

        private async void OnAddTxtCommand(object obj)
        {
            ToastHelper.ShowMessage("点击添加Txt文件");

            FileOpenPicker openFile = new FileOpenPicker();
            openFile.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openFile.ViewMode = PickerViewMode.List;
            openFile.FileTypeFilter.Add(".txt");

            // 选取单个文件
            StorageFile file = await openFile.PickSingleFileAsync();
            if (file != null)
            {
                var str = "你所选择的文件是： " + file.Name;

                ToastHelper.ShowMessage(str);
            }
            else
            {
                var str = "打开文件操作被取消。";
                ToastHelper.ShowMessage(str);
            }
        }

        public override void OnItemClickCommand(object obj)
        {
            var localbook = obj as LocalBookItemViewModel;
            if (localbook?.CurrentBook == null)
            {
                return;
            }
            localbook.CurrentBook.IsLocal = true;
            localbook.CurrentBook.IsNew = false;
            NavigationService.NavigateTo(typeof(OnlineContentPage));
            ViewModelInstance.Instance.OnlineBookContent.ResDeta();
            ViewModelInstance.Instance.OnlineBookContent.LoadData(localbook.CurrentBook);
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
                "6.免费版本在本地书架中最多只能存三本。Pro版本无此限制" + "\n";


            if (!App.IsPro)
            {
                // HelpText +=  "6.本地书架暂不支持本地TXT文件阅读。" + "\n";

            }
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
            LocalBooks.Clear();
            Task.Run(async () =>
            {
                await Task.Delay(100);
                var list = DbLocalBook.GetBooks(AppDataPath.GetLocalBookDbPath());
                if (list == null || list.Count <= 0)
                {
                    return;
                }

                foreach (var book in list)
                {
                    var localVm = new LocalBookItemViewModel()
                    {
                        CurrentBook = book
                    };
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        LocalBooks.Add(localVm);
                    });

                    localVm.CheckUpdate();
                }
            });


        }

        public override void OnRefreshCommand(object obj)
        {
            if (LocalBooks.Any(p => p.IsUpdating == true))
            {
                ToastHelper.ShowMessage("正在更新数据，请稍后刷新");
                return;
            }

            GetLocalBookFromDb();
        }

        public void InserOrUpdateBook(Book book)
        {
            if (book == null)
            {
                return;
            }

            var temp = LocalBooks.FirstOrDefault(p => p.CurrentBook.BookId == book.BookId);
            DbLocalBook.InsertOrUpdatBook(AppDataPath.GetLocalBookDbPath(), book);

            if (temp != null)
            {
                LocalBooks.Remove(temp);
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                LocalBooks.Insert(0, new LocalBookItemViewModel(book));

            });
        }

        public int GetLocalBooksCount()
        {
            var count = DbLocalBook.GetBooksCount(AppDataPath.GetLocalBookDbPath());

            return count;
        }


        public bool CheckBookExist(string bookId)
        {
            return DbLocalBook.CheckBookExist(AppDataPath.GetLocalBookDbPath(), bookId);
        }

        #endregion



    }
}
