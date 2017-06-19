using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Entity;
using Sodu.Core.HtmlService;
using Sodu.Core.Util;
using Sodu.Service;

namespace Sodu.ViewModel
{
    public class OnlineBookShelfPageViewModel : BasePageViewModel
    {
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

            ToastHelper.ShowMessage(removeItem.BookName + "已从在线书架移除");
            Books.Remove(removeItem);
        }


        private ICommand _setHadReadCommand;
        public ICommand SetHadReadCommand => _setHadReadCommand ?? (_setHadReadCommand = new RelayCommand<object>(OnSetHadReadCommand));
        public void OnSetHadReadCommand(object obj)
        {
            var removeItem = obj as Book;
            if (removeItem == null)
            {
                return;
            }

            ToastHelper.ShowMessage(removeItem.BookName + "标为已读");
            removeItem.IsNew = false;
        }

        #endregion

        #region 方法

        public OnlineBookShelfPageViewModel()
        {
            Title = "在线书架";
        }

        public override void LoadData()
        {
            if (CookieHelper.CheckLogin() && Books?.Count == 0)
            {
                GetData();
            }
        }

        public async void GetData()
        {
            var url = WebPageUrl.BookShelfPage;
            var html = await GetHtmlData2(url);
            var list = ListPageDataHelper.GetBookShelftListFromHtml(html);
            if (list == null || list.Count == 0)
            {
                ToastHelper.ShowMessage("获取在线书架失败");
            }
            else
            {
                Books?.Clear();
                foreach (var book in list)
                {
                    book.IsNew = true;
                    Books?.Add(book);
                }
            }
        }



        public override void OnRefreshCommand(object obj)
        {
            if (IsLoading)
            {
                return;
            }
            GetData();
        }

        #endregion
    }
}
