using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.Core.Extend;
using Sodu.Core.HtmlService;
using Sodu.Service;
using Sodu.View;
using Windows.UI.Xaml;

namespace Sodu.ViewModel
{
    public class DownloadItemViewModel : ViewModelBase
    {
        private DispatcherTimer _timer;

        public static object _obj = new object();
        public Book Book { get; set; }


        private int _totalCount;

        public int TotalCount
        {
            get { return _totalCount; }
            set { Set(ref _totalCount, value); }
        }

        private double _progress;

        public double Progress
        {
            get { return _progress; }
            set { Set(ref _progress, value); }
        }

        private int _completedCount;

        public int CompletedCount
        {
            get { return _completedCount; }
            set { Set(ref _completedCount, value); }
        }

        public bool IsDelete { get; set; }
        public bool IsCompleted { get; set; }

        private ICommand _deleteCommand;

        public ICommand DeleteCommand => _deleteCommand ?? (
                                             _deleteCommand = new RelayCommand<object>(
                                                 (obj) =>
                                                 {
                                                     IsDelete = true;
                                                 }));



        public DownloadItemViewModel(Book book)
        {
            var catalogs = book.CatalogList;
            Book = book.Clone();
            Book.CatalogList = catalogs;
            TotalCount = Book.CatalogList.Count;
        }

        public async void StartDownload()
        {
            try
            {
                var taskCount = App.IsPro ? 15 : 1;
                var groups = Book.CatalogList.Split<BookCatalog>(taskCount);

                var enumerable = groups as IEnumerable<BookCatalog>[] ?? groups.ToArray();
                if (groups == null || !enumerable.Any())
                {
                    return;
                }

                var tasks = new Task[enumerable.Length];

                _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
                _timer.Tick += Timer_Tick;
                _timer.Start();

                for (var i = 0; i < enumerable.Length; i++)
                {
                    if (IsDelete)
                    {
                        break;
                    }

                    tasks[i] = await Task.Factory.StartNew(async () =>
                    {
                        var catalogs = enumerable[i];
                        foreach (var bookCatalog in catalogs)
                        {
                            try
                            {
                                if (IsDelete)
                                {
                                    break;
                                }

                                bookCatalog.CatalogContent =
                                    await AnalisysSourceHelper.GetCatalogContent(bookCatalog.CatalogUrl);

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            finally
                            {
                                lock (_obj)
                                {
                                    _completedCount += 1;
                                    _progress = double.Parse(CompletedCount.ToString()) /
                                                double.Parse(TotalCount.ToString()) * 100;
                                }

                            }
                        }
                    });

                }

                await Task.Factory.ContinueWhenAll(tasks, (obj) =>
                {
                    if (CompletedCount < TotalCount - 10)
                    {
                        ToastHelper.ShowMessage(Book.BookName + "下载失败", false);
                        ViewModelInstance.Instance.DownloadCenter.RemoveDownItem(this);
                        return;
                    }
                    IsCompleted = true;
                    InsertOrUpdateBookCatalogs(Book);

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        _timer?.Stop();
                    });
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private void Timer_Tick(object sender, object e)
        {
            if (!IsCompleted && ViewModelInstance.Instance.DownloadCenter.IsFrameContent)
            {
                RaisePropertyChanged(() => Progress);
                RaisePropertyChanged(() => CompletedCount);
            }
        }



        public void InsertOrUpdateBookCatalogs(Book book)
        {
            var result = DbLocalBook.InsertOrUpdateBookCatalogs(AppDataPath.GetLocalBookDbPath(), book.CatalogList);

            if (!result)
            {
                ToastHelper.ShowMessage(book.BookName + "下载失败", false);
                ViewModelInstance.Instance.DownloadCenter.RemoveDownItem(this);
                return;
            }
            var value = DbLocalBook.InsertOrUpdatBook(AppDataPath.GetLocalBookDbPath(), book);

            if (value)
            {
                ToastHelper.ShowMessage(book.BookName + "下载成功，您可在本地书架中阅读");
                ViewModelInstance.Instance.DownloadCenter.RemoveDownItem(this);
                ViewModelInstance.Instance.LocalBookPage.InserOrUpdateBook(book);
            }
        }

    }

    public class DownloadCenterPageViewModel : ViewModelBase
    {

        public bool IsFrameContent { get; set; } = false;

        private ObservableCollection<DownloadItemViewModel> _downloadItems;

        /// <summary>
        ///列表项
        /// </summary>
        public ObservableCollection<DownloadItemViewModel> DownloadItems
        {
            get { return _downloadItems ?? (_downloadItems = new ObservableCollection<DownloadItemViewModel>()); }
            set { Set(ref _downloadItems, value); }
        }



        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ?? (_backCommand = new RelayCommand<object>(OnBackCommand));

        public void OnBackCommand(object obj)
        {
            NavigationService.GoBack();
        }

        private ICommand _removeCommand;

        public ICommand RemoveCommand
            => _removeCommand ?? (_removeCommand = new RelayCommand<DownloadItemViewModel>(RemoveDownItem));



        public void RemoveDownItem(DownloadItemViewModel item)
        {
            if (item != null)
            {
                item.IsDelete = true;
                var temp = DownloadItems.FirstOrDefault(p => p.Book.BookId == item.Book.BookId);

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    DownloadItems.Remove(temp);
                });
            }
        }


        public void AddDownItem(Book book)
        {
            var temp = DownloadItems.FirstOrDefault(p => p.Book.BookId == book.BookId);

            if (temp != null)
            {
                ToastHelper.ShowMessage(book.BookName + "已在缓存队列中");
                return;
            }

            var downloadItem = new DownloadItemViewModel(book);
            DownloadItems.Insert(0, downloadItem);
            downloadItem.StartDownload();

            ToastHelper.ShowMessage(book.BookName + "开始缓存，您可在设置-下载中心查看进度");
        }

    }
}
