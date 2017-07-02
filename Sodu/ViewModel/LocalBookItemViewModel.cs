using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.Core.Extend;
using Sodu.Core.HtmlService;
using Sodu.Service;

namespace Sodu.ViewModel
{
    public class LocalBookItemViewModel : ViewModelBase
    {
        public bool IsDeleted { get; set; }
        public bool IsUpdating { get; set; }

        public static object _obj = new object();

        private string _needUpdateCount;
        public string NeedUpdateCount
        {
            get
            {
                return _needUpdateCount;
            }
            set
            {
                Set(ref _needUpdateCount, value);
            }
        }

        private List<BookCatalog> NeedUpdateCatalogs { get; set; }


        private Book _currentBook;
        public Book CurrentBook
        {
            get
            {
                return _currentBook;
            }
            set
            {
                Set(ref _currentBook, value);
            }
        }


        public LocalBookItemViewModel()
        {

        }

        public LocalBookItemViewModel(Book book)
        {
            CurrentBook = book;
        }

        public void CheckUpdate()
        {
            if (CurrentBook.IsTxt)
            {
                return;
            }

            if (CurrentBook.IsOnline)
            {
                Task.Run(async () =>
                {
                    try
                    {

                        var catalogPageUrl = AnalisysSourceHelper.GetCatalogPageUrl(CurrentBook.NewestChapterUrl);
                        var catalogData = await AnalisysSourceHelper.GetCatalogPageData(catalogPageUrl, CurrentBook.BookId);

                        var lastWebCatalog = catalogData?.Item1?.LastOrDefault();

                        if (lastWebCatalog == null)
                        {
                            return;
                        }

                        if (lastWebCatalog.CatalogUrl == CurrentBook.NewestChapterUrl)
                        {
                            return;
                        }
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            CurrentBook.IsNew = true;
                            CurrentBook.NewestChapterName = lastWebCatalog.CatalogName;
                            CurrentBook.NewestChapterUrl = lastWebCatalog.CatalogUrl;
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }).ContinueWith((result) =>
                {
                    if (IsDeleted)
                    {
                        return;
                    }
                    DbLocalBook.InsertOrUpdatBook(AppDataPath.GetLocalBookDbPath(), CurrentBook);
                });
            }

            else
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var localCatalogs = DbLocalBook.SelectBookCatalogsByBookId(AppDataPath.GetLocalBookDbPath(),
                   CurrentBook.BookId);

                        var localLastCatalog = localCatalogs.LastOrDefault();

                        if (localLastCatalog != null)
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                CurrentBook.NewestChapterName = localLastCatalog.CatalogName;
                                CurrentBook.NewestChapterUrl = localLastCatalog.CatalogUrl;
                            });
                        }

                        var catalogPageUrl = AnalisysSourceHelper.GetCatalogPageUrl(CurrentBook.NewestChapterUrl);
                        var catalogData = await AnalisysSourceHelper.GetCatalogPageData(catalogPageUrl, CurrentBook.BookId);

                        var lastWebCatalog = catalogData?.Item1?.LastOrDefault();

                        if (lastWebCatalog == null)
                        {
                            return;
                        }

                        if (localCatalogs == null || localCatalogs.Count == 0)
                        {
                            NeedUpdateCatalogs = catalogData.Item1;
                        }

                        if (localLastCatalog == null)
                        {
                            return;
                        }

                        if (lastWebCatalog.CatalogUrl == localLastCatalog.CatalogUrl)
                        {
                            return;
                        }

                        var tempCatalog = catalogData.Item1.LastOrDefault(p => p.CatalogUrl == localLastCatalog.CatalogUrl);
                        var tempList = catalogData.Item1.Skip(tempCatalog.Index).ToList();
                        NeedUpdateCatalogs = tempList;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NeedUpdateCount = NeedUpdateCatalogs == null ? "" : NeedUpdateCatalogs?.Count.ToString();
                        });
                    }
                }).ContinueWith((result) =>
                {
                    if (IsDeleted)
                    {
                        return;
                    }

                    if (NeedUpdateCatalogs == null || NeedUpdateCatalogs.Count == 0)
                    {
                        return;
                    }

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NeedUpdateCount = NeedUpdateCatalogs.Count.ToString();

                        CurrentBook.IsNew = true;
                        CurrentBook.NewestChapterName = NeedUpdateCatalogs.LastOrDefault()?.CatalogName;
                        CurrentBook.NewestChapterUrl = NeedUpdateCatalogs.LastOrDefault()?.CatalogUrl;
                        DbLocalBook.InsertOrUpdatBook(AppDataPath.GetLocalBookDbPath(), CurrentBook);
                    });

                    StartUpdate();
                });
            }


        }

        public async void StartUpdate()
        {
            try
            {
                var taskCount = 3;
                var groups = NeedUpdateCatalogs.Split<BookCatalog>(taskCount);
                var enumerable = groups as IEnumerable<BookCatalog>[] ?? groups.ToArray();
                if (groups == null || !enumerable.Any())
                {
                    return;
                }
                IsUpdating = true;

                var tasks = new Task[enumerable.Length];
                for (var i = 0; i < enumerable.Length; i++)
                {
                    if (IsDeleted)
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
                                if (IsDeleted)
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
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                    {
                                        NeedUpdateCount = (int.Parse(NeedUpdateCount) - 1).ToString();
                                    });
                                }
                            }
                        }
                    });

                }

                await Task.Factory.ContinueWhenAll(tasks, (obj) =>
                {
                    var reslut = DbLocalBook.InsertOrUpdateBookCatalogs(AppDataPath.GetLocalBookDbPath(),
                        NeedUpdateCatalogs);

                    if (reslut)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NeedUpdateCount = "";
                        });


                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsUpdating = false;
            }

        }

    }
}
