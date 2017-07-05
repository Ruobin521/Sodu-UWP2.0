using System;
using GalaSoft.MvvmLight.Command;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.Service;
using Sodu.View;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;

namespace Sodu.ViewModel
{
    public class SearchPageViewModel : BasePageViewModel
    {

        private string _searchPara;
        /// <summary>
        /// 
        /// </summary>
        public string SearchPara
        {
            get { return _searchPara; }
            set { Set(ref _searchPara, value); }
        }

        public override async void OnSearchCommand(object obj)
        {
            if (string.IsNullOrEmpty(obj?.ToString().Trim()))
            {
                return;
            }
            try
            {
                IsLoading = true;
                var searchPara = obj.ToString();
                var uri = string.Format(SoduPageValue.BookSearchPage, WebUtility.UrlEncode(searchPara));
                var html = await GetHtmlData(uri, true, false);
                var books = ListPageDataHelper.GetSearchResultkListFromHtml(html);

                if (books == null || books.Count <= 0)
                {
                    ToastHelper.ShowMessage("无搜索结果");
                    return;
                }
                Books.Clear();
                foreach (var book in books)
                {
                    Books.Add(book);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public override void OnRefreshCommand(object obj)
        {
            if (IsLoading)
            {
                return;
            }
            OnSearchCommand(SearchPara);
        }


        public void ResetData()
        {
            
        }
    }
}
