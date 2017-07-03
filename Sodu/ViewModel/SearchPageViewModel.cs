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
    public class SearchPageViewModel : CommonPageViewModel
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

        #endregion


        #region 命令
        /// <summary>
        /// 搜索
        /// </summary>
        private ICommand _searchfCommand;
        public ICommand SearchCommand => _searchfCommand ?? (_searchfCommand = new RelayCommand<object>(OnSearchCommand));

        private async void OnSearchCommand(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString().Trim()))
            {
                return;
            }
            var searchPara = obj.ToString();
            var uri = string.Format(SoduPageValue.BookSearchPage, WebUtility.UrlEncode(searchPara));
            var html = await GetHtmlData(uri,true,false);
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
        #endregion
    }
}
