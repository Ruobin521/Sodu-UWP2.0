using Sodu.Core.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Core.AppService;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;

namespace Sodu.DataService
{
    public class OnlineBookShelfDataService
    {
        public HttpHelper Http { get; set; }

        public List<Book> GetCacheData()
        {
            List<Book> books = null;
            if (string.IsNullOrEmpty(AppSettingService.GetUserId()))
            {
                return null;
            }
            books = DbBookShelf.GetBooks(AppDataPath.GetAppCacheDbPath(), AppSettingService.GetUserId());
            return books;
        }

        public async Task<List<Book>> GetOnlineData()
        {
            try
            {
                var html = await GetHtmlData(SoduPageValue.BookShelfPage, true);
                var list = ListPageDataHelper.GetBookShelftListFromHtml(html)?.ToList();
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                return null;
            }
        }

        private async Task<string> GetHtmlData(string url, bool time)
        {
            string html = null;
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Http = new HttpHelper();
                html = await Http.HttpClientGetRequest(url, time);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                html = null;
            }
            return html;
        }

        public bool CompareWithLocalCache(List<Book> onlineList,List<Book> localList )
        {
            var result = false;
            try
            {
                if (onlineList == null || onlineList.Count == 0)
                {
                    return false;
                }
                if (localList == null || localList.Count == 0)
                {
                    return false;
                }

                var userId = AppSettingService.GetUserId();
               
                foreach (var book in onlineList)
                {
                    var item = localList.FirstOrDefault(b => b.BookId == book.BookId);
                    if (item == null)
                    {
                        continue;
                    }

                    book.IsNew = !ReplaceChar(book.NewestChapterName).Equals(ReplaceChar(item.LastReadChapterName));
                    book.LastReadChapterName = item.LastReadChapterName;
                }
                if (onlineList.Any(p => p.IsNew))
                {
                    result = true;
                }
                DbBookShelf.InsertOrUpdateBooks(AppDataPath.GetAppCacheDbPath(), onlineList, userId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                result = false;
            }
            return result;
        }

        public string ReplaceChar(string str)
        {
            var temp = str.Replace(" ", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("【", "")
                .Replace("】", "")
                .Replace("，", "")
                .Replace("。", "")
                .Replace("《", "")
                .Replace("》", "")
                .Replace("？", "")
                .Replace("?", "")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("卷", "")
                .Replace(" ", "")
                .Replace("正文", "");
            return temp;
        }
    }
}
