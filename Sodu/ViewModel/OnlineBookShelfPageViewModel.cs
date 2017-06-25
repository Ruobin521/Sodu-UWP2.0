using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.Core.Util;
using Sodu.Service;

namespace Sodu.ViewModel
{
    public class OnlineBookShelfPageViewModel : BasePageViewModel
    {
        private bool IsInit { get; set; }

        #region 命令
        /// <summary>
        ///  
        /// </summary>
        private ICommand _removeCommand;
        public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new RelayCommand<object>(OnRemoveCommand));

        /// <summary>
        /// 移除书架
        /// </summary>
        /// <param name="obj"></param>
        public void OnRemoveCommand(object obj)
        {
            var removeItem = obj as Book;
            if (removeItem == null)
            {
                return;
            }
            RemoveItemFromShelf(removeItem);
        }




        private ICommand _setHadReadCommand;
        public ICommand SetHadReadCommand => _setHadReadCommand ?? (_setHadReadCommand = new RelayCommand<object>(OnSetHadReadCommand));
        public void OnSetHadReadCommand(object obj)
        {
            var item = obj as Book;
            if (item == null || !item.IsNew)
            {
                return;
            }
            item.IsNew = false;
            item.LastReadChapterName = item.NewestChapterName;
            DbBookShelf.InsertOrUpdateBook(AppDataPath.GetAppCacheDbPath(), item, AppSettingService.GetUserId());
        }

        #endregion

        #region 方法

        public OnlineBookShelfPageViewModel()
        {
            Title = "在线书架";

            HelpText =
                "您的书架空空如也，在排行或者热门推荐中向左滑动添加几本吧..." + "\n\n" +
                "使用提示：" + "\n" +
                "1.在线书架是主要是为了追更，如果想连续阅读，请使用本地书架功能。" + "\n" +
                "2.手机用户可以在当前列表项左右滑动进行相关操作，PC用户可点击鼠标右键进行相关操作。" + "\n" +
                "3.在排行或热门推荐页面向左滑动(手机用户)或点击鼠标右键(PC用户)选择相应操作即可添加所选项至在线书架。";

            //     GetCacheData();
        }

        public override void LoadData(object obj = null)
        {
            if (IsInit)
            {
                return;
            }
            IsInit = true;
            GetData();
        }

        public void GetCacheData()
        {
            if (!CookieHelper.CheckLogin())
            {
                return;
            }

            Task.Run(() =>
            {
                if (string.IsNullOrEmpty(AppSettingService.GetUserId()))
                {
                    return;
                }
                var books = DbBookShelf.GetBooks(AppDataPath.GetAppCacheDbPath(), AppSettingService.GetUserId());
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    books.ForEach(p => Books.Add(p));
                });
            });
        }

        public async void GetData()
        {
            if (!CookieHelper.CheckLogin() || IsLoading)
            {
                return;
            }
            try
            {
                IsLoading = true;

                var html = await GetHtmlData2(SoduPageValue.BookShelfPage, false, true);
                var list = ListPageDataHelper.GetBookShelftListFromHtml(html);
                if (list == null || list.Count == 0)
                {
                    if (html != null && html.Contains(SoduPageValue.BookShelfPage))
                    {
                        Books = null;
                        ToastHelper.ShowMessage("您的在线书架为空");
                    }
                    else
                    {
                        ToastHelper.ShowMessage("获取在线书架数据失败");
                    }
                }
                else
                {
                    var result = await CompareWithLocalCache(list.ToList());
                    Books?.Clear();
                    foreach (var book in list)
                    {
                        Books?.Add(book);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> CompareWithLocalCache(List<Book> list)
        {
            var result = false;
            await Task.Run(() =>
             {
                 try
                 {
                     if (list == null || list.Count == 0)
                     {
                         return;
                     }

                     var localCathes = DbBookShelf.GetBooks(AppDataPath.GetAppCacheDbPath(), AppSettingService.GetUserId());
                     if (localCathes == null || localCathes.Count == 0)
                     {
                         DbBookShelf.InsertOrUpdateBooks(AppDataPath.GetAppCacheDbPath(), list, AppSettingService.GetUserId());
                         return;
                     }

                     foreach (var book in list)
                     {
                         var item = localCathes.FirstOrDefault(b => b.BookId == book.BookId);
                         if (item == null)
                         {
                             continue;
                         }

                         book.IsNew = !ReplaceChar(book.NewestChapterName).Equals(ReplaceChar(item.LastReadChapterName));
                         book.LastReadChapterName = item.LastReadChapterName;
                     }
                     result = true;
                     DbBookShelf.InsertOrUpdateBooks(AppDataPath.GetAppCacheDbPath(), list, AppSettingService.GetUserId());
                 }
                 catch (Exception ex)
                 {
                     Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                     result = false;
                 }
             });
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

        public override void OnRefreshCommand(object obj)
        {
            if (IsLoading)
            {
                return;
            }
            GetData();
        }

        public override void OnItemClickCommand(object obj)
        {
            if (obj != null)
            {
                OnSetHadReadCommand(obj);
            }
            base.OnItemClickCommand(obj);
        }


        private async void RemoveItemFromShelf(Book item)
        {
            var url = SoduPageValue.BookShelfPage + "?id=" + item.BookId;
            var html = await GetHtmlData2(url, false, false);
            if (html.Contains("取消收藏成功"))
            {
                ToastHelper.ShowMessage(item.BookName + "已从在线书架移除");
                Books.Remove(item);
                DbBookShelf.RemoveBook(AppDataPath.GetAppCacheDbPath(), item, AppSettingService.GetUserId());
            }
            else
            {
                ToastHelper.ShowMessage(item.BookName + "移除失败");
            }
        }


        public void ResetData()
        {
            Books.Clear();
            IsInit = false;
        }

        #endregion
    }
}
