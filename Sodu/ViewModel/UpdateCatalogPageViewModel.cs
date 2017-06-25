using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.View;

namespace Sodu.ViewModel
{
    public class UpdateCatalogPageViewModel : BasePageViewModel
    {
        #region  属性

        private Book _currentBook;
        /// <summary>
        /// 当前小说
        /// </summary>
        public Book CurrentBook
        {
            get { return _currentBook; }
            set { Set(ref _currentBook, value); }
        }


        #endregion


        #region  命令

        #endregion


        #region  方法


        public UpdateCatalogPageViewModel()
        {

        }


        private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {


        }


        public override void LoadData(object obj = null)
        {
            if (IsLoading)
            {
                return;
            }

            if ((obj as Book) == null)
            {
                return;
            }

            ResetData();

            CurrentBook = (Book)obj;

            Title = CurrentBook.BookName;

            if (CurrentBook == null || Books.Count == 0)
            {
                GetData(1, CurrentBook.UpdateCatalogUrl);
            }
        }

        public async void GetData(int pageIndex, string url)
        {
            try
            {
                if (IsLoading)
                {
                    return;
                }

                IsLoading = true;


                if (PageCount != -1 && pageIndex > PageCount)
                {
                    return;
                }

                if (string.IsNullOrEmpty(url))
                {
                    return;
                }

                PageIndex = pageIndex;

                if (pageIndex > 1)
                {
                    url = url.Insert(url.Length - 5, "_" + pageIndex);
                }

                var html = await GetHtmlData(url, false, true);
                var list = ListPageDataHelper.GetBookUpdateChapterList(html);
                if (list == null)
                {
                    Debug.WriteLine("排行榜数据获取失败");
                }
                else
                {
                    var match = Regex.Match(html, @"(?<=总计.*?记录.*?共).*?(?=页)");
                    if (match != null)
                    {
                        try
                        {
                            PageCount = Convert.ToInt32(match.ToString().Trim());
                        }
                        catch (Exception)
                        {
                            PageCount = 1;
                        }
                    }
                    if (pageIndex == 1)
                    {
                        Books.Clear();

                        foreach (var item in list)
                        {
                            item.BookName = CurrentBook.BookName;
                            item.BookId = CurrentBook.BookId;
                            Books.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in list)
                        {
                            item.BookName = CurrentBook.BookName;
                            item.BookId = CurrentBook.BookId;
                            Books.Add(item);
                        }
                    }

                    Title = $"{CurrentBook.BookName}({PageIndex}/{PageCount})";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw;
            }
            finally
            {
                await Task.Delay(100);
                IsLoading = false;
            }
        }


        public override void OnRefreshCommand(object obj)
        {
            if (CurrentBook == null)
            {
                return;
            }
            GetData(1, CurrentBook.UpdateCatalogUrl);
        }


        public override void OnPullToLoadCommand(object obj)
        {
            if (PageIndex == PageCount || IsLoading)
            {
                return;
            }

            if (CurrentBook == null)
            {
                return;
            }

            GetData(PageIndex + 1, CurrentBook.UpdateCatalogUrl);
        }

        public override void OnItemClickCommand(object obj)
        {
            if ((obj as Book) == null)
            {
                return;
            }

            NavigationService.NavigateTo(typeof(OnlineContentPage));
            var vm = new OnlineContentPageViewModel();
            ViewModelInstance.Instance.OnlineBookContent = vm;
            vm.LoadData((Book)obj);
        }


        public void ResetData()
        {
            Books.Clear();
            PageIndex = -1;
            PageCount = -1;
            Title = null;
            CurrentBook = null;
        }


        #endregion


    }
}
