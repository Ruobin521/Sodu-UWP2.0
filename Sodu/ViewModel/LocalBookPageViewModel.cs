using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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
using Sodu.Core.Util;
using Sodu.Service;
using Sodu.View;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

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

        private bool IsInit { get; set; }

        #endregion

        #region 命令


        /// <summary>
        /// 删除
        /// </summary>
        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand<object>(OnDeleteCommand));

        private void OnDeleteCommand(object obj)
        {
            if (IsLoading)
            {
                ToastHelper.ShowMessage("加载中，请稍后");
                return;
            }
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
            if (IsLoading)
            {
                return;
            }

            FileOpenPicker openFile = new FileOpenPicker();
            openFile.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openFile.ViewMode = PickerViewMode.List;
            openFile.FileTypeFilter.Add(".txt");

            // 选取单个文件
            var file = await openFile.PickSingleFileAsync();

            if (file != null)
            {
                LoadTxtFile(file);
            }
        }


        private void LoadTxtFile(StorageFile file)
        {
            Task.Run(async () =>
            {
                try
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IsLoading = true;
                    });

                    string txt;
                    using (Stream stream = await file.OpenStreamForReadAsync())
                    {
                        using (StreamReader read = new StreamReader(stream, Encoding.GetEncoding("GBK")))
                        {
                            txt = read.ReadToEnd();
                        }
                    }
                    //@"(第?\w*章\s\w{1,20}[\(【（]?\w{1,20}[\)】）]?\n)"
                       var matches1 = Regex.Matches(txt, @"(?<title>第?\w*章\s\w{1,20}[！]?\s?[--，]?\w{1,20}\(?（?\w{1,20}\)?）?)", RegexOptions.Compiled);
                    var matches = Regex.Matches(txt, @"(第?\w*章\s\w{1,20}[\(【（]?\w{1,20}[\)】）]?)", RegexOptions.Compiled);


                    if (matches.Count <= 0)
                    {
                        ToastHelper.ShowMessage("文件解析失败。");
                        return;
                    }

                    Debug.WriteLine(matches.Count);

                    var catalogs = new List<BookCatalog>();

                    var bookId = Guid.NewGuid().ToString();

                    for (int i = 0; i < matches.Count; i++)
                    {
                        var currentMatch = matches[i];
                        if (currentMatch == null)
                        {
                            continue; ;
                        }


                        if (i == 0 && !string.IsNullOrEmpty(txt.Substring(0, currentMatch.Index)?.Trim()))
                        {
                            var temp = new BookCatalog
                            {
                                BookId = bookId,
                                Index = 0,
                                CatalogName = "前言",
                                CatalogContent = txt.Substring(0, currentMatch.Index),
                                CatalogUrl = "0"
                            };
                            catalogs.Add(temp);
                        }

                        var catalog = new BookCatalog
                        {
                            CatalogName = currentMatch.ToString(),
                            BookId = bookId,
                            Index = i + 1
                        };
                        if (i == matches.Count - 1)
                        {
                            catalog.CatalogContent = txt.Substring(currentMatch.Index, txt.Length - currentMatch.Index);
                            catalog.CatalogUrl = currentMatch.Index.ToString();
                            catalog.CatalogUrl = (i + 1).ToString();
                        }
                        else
                        {
                            var nextMatch = matches[i + 1];
                            catalog.CatalogContent = txt.Substring(currentMatch.Index, nextMatch.Index - currentMatch.Index);
                            catalog.CatalogUrl = (i + 1).ToString();
                        }
                        catalog.CatalogContent = catalog.CatalogContent?.Replace("\r\n\r\n", "\r\n");

                        catalogs.Add(catalog);

                    }

                    var book = new Book();
                    book.IsTxt = true;
                    book.BookId = bookId;
                    book.BookName = file.DisplayName;
                    book.LastReadChapterName = catalogs.FirstOrDefault().CatalogName;
                    book.LastReadChapterUrl = catalogs.FirstOrDefault().CatalogUrl;
                    book.NewestChapterName = catalogs.LastOrDefault().CatalogName;
                    book.NewestChapterUrl = catalogs.LastOrDefault().CatalogUrl;
                    book.AuthorName ="某位大神";
                    book.Description = "不管三七二十一，就是好看。";
                    book.LyWeb = "本地TXT文档";

                    DbLocalBook.InsertOrUpdateBookCatalogs(AppDataPath.GetLocalBookDbPath(), catalogs);

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.InserOrUpdateBook(book);

                        IsLoading = false;
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        IsLoading = false;
                    });
                }

            });
        }



        public override void OnItemClickCommand(object obj)
        {
            var localbook = obj as LocalBookItemViewModel;
            if (localbook?.CurrentBook == null)
            {
                return;
            }
            if (localbook.CurrentBook.IsNew)
            {
                OnSetHadReadCommand(localbook);
            }

            NavigationService.NavigateTo(typeof(OnlineContentPage));
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
                "3.下载或添加完毕后即可在此处点击阅读。" + "\n" +
                "4.手机用户向左滑动即可删除当前项，向右滑动可标记为已读。" + "\n" +
                "5.PC用户点击鼠标右键进行相关不操作。" + "\n" +
                "6.免费版本在本地书架中最多只能存三本。Pro版本无此限制。" + "\n" +
                "7.Pro版本点击加号即可添加本地Txt文件阅读。" + "\n";

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

            IsLoading = true;

            Task.Run(async () =>
            {
                await Task.Delay(1);
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
                    DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                    {
                        LocalBooks.Add(localVm);
                        await Task.Delay(10);
                    });

                    localVm.CheckUpdate();
                }

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
              {
                  IsLoading = false;
              });

                IsInit = true;
            });

        }

        public override void OnRefreshCommand(object obj)
        {
            if (IsLoading)
            {
                return;
            }

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

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (temp != null)
                {
                    LocalBooks.Remove(temp);
                }
                LocalBooks.Insert(0, new LocalBookItemViewModel(book));
            });

            if (!IsInit)
            {
                GetLocalBookFromDb();
            }
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
