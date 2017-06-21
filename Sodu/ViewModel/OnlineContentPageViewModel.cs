using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;

namespace Sodu.ViewModel
{
    public class OnlineContentPageViewModel : ViewModelBase
    {
        #region 属性
        /// <summary>
        /// 缓存数据（url,pages ,content）
        /// </summary>
        public Dictionary<string, Tuple<List<string>, string>> DicContentCache = new Dictionary<string, Tuple<List<string>, string>>();

        /// <summary>
        /// 当前目录项
        /// </summary>
        public BookCatalog CurrentCatalog { get; set; }

        /// <summary>
        /// 当前小说
        /// </summary>
        public Book CurrentBook { get; set; }


        private ObservableCollection<BookCatalog> _bookCatalogs;
        /// <summary>
        ///目录列表
        /// </summary>
        public ObservableCollection<BookCatalog> BookCatalogs
        {
            get
            {
                return _bookCatalogs ?? (_bookCatalogs = new ObservableCollection<BookCatalog>());
            }
            set { Set(ref _bookCatalogs, value); }
        }



        private List<string> _contentPages;
        /// <summary>
        ///当前章节分页内容
        /// </summary>
        public List<string> ContentPages
        {
            get
            {
                return _contentPages ?? (_contentPages = new List<string>());
            }
            set { Set(ref _contentPages, value); }
        }


        private string _contentText;
        /// <summary>
        ///当前章节内容（未分页）
        /// </summary>
        public string ContentText
        {
            get { return _contentText; }
            set
            {
                Set(ref _contentText, value);
            }
        }

        #endregion


        #region 方法


        public void LoadData(Book book)
        {
            CurrentBook = book;
            var catalog = new BookCatalog()
            {
                CatalogName = book.NewestChapterName,
                CatalogUrl = book.NewestChapterUrl,
            };
            CurrentCatalog = catalog;

            SetContent(catalog);
        }

        private async void SetContent(BookCatalog catalog)
        {
            if (DicContentCache.ContainsKey(catalog.CatalogUrl))
            {
                var value = DicContentCache[catalog.CatalogUrl];
                if (value?.Item1 != null && value.Item1.Count > 0)
                {
                    ContentPages = DicContentCache[catalog.CatalogUrl].Item1;
                    ContentText = DicContentCache[catalog.CatalogUrl].Item2;
                    return;
                }
            }

            if (CurrentBook.IsLocal)
            {
                var html = GetCatalogContentFormDb(catalog);
                ContentText = html;
                ContentPages = SplitContentToPages(html);
                DicContentCache.Add(catalog.CatalogUrl, new Tuple<List<string>, string>(ContentPages, html));
            }
            else
            {
                var html = await GetCatalogContentFormWeb(catalog);
                ContentText = html;
                ContentPages = SplitContentToPages(html);
                DicContentCache.Add(catalog.CatalogUrl, new Tuple<List<string>, string>(ContentPages, html));
            }
        }


        public async Task<string> GetCatalogContentFormWeb(BookCatalog catalog)
        {
            var html = await SourceHtmlHelper.GetCatalogContent(catalog.CatalogUrl);

            return html;
        }


        public string GetCatalogContentFormDb(BookCatalog catalog)
        {
            return null;
        }


        private List<string> SplitContentToPages(string html)
        {


            return null;
        }

        #endregion

    }
}
