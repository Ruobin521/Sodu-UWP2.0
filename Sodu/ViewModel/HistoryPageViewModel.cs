using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Sodu.Core.Config;
using Sodu.Core.DataBase;
using Sodu.Core.Entity;
using Sodu.View;
using GalaSoft.MvvmLight.Threading;

namespace Sodu.ViewModel
{
    public class HistoryPageViewModel : BasePageViewModel
    {
        #region 命令


        /// <summary>
        ///清空所有
        /// </summary>
        private ICommand _clearAllCommand;
        public ICommand ClearAllCommand => _clearAllCommand ?? (_clearAllCommand = new RelayCommand<object>(OnClearAllCommand));

        public void OnClearAllCommand(object obj)
        {
            Task.Run(() =>
            {
                var result = DbHistory.ClearBooks(AppDataPath.GetAppCacheDbPath());

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (result)
                    {
                        Books.Clear();
                    }
                });

            });
        }


        #endregion

        #region 方法

        public HistoryPageViewModel()
        {

        }

        public void LoadData()
        {
            Task.Run(() =>
            {
                var histoies = DbHistory.GetBooks(AppDataPath.GetAppCacheDbPath());

                if (histoies != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Books.Clear();
                        histoies.ForEach(p => Books.Add(p));
                    });
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        Books.Clear();
                    });
                }
            });

        }

        public void InserOrUpdateHistory(Book bookPara)
        {
            if (bookPara == null)
            {
                return;
            }
            var book = bookPara.Clone();

            Task.Run(() =>
            {
                DbHelper.AddDbOperator(new Action(() =>
                {
                    DbHistory.InsertOrUpdatHistory(AppDataPath.GetAppCacheDbPath(), book);
                }));

                var temp = Books.FirstOrDefault(p => p.BookId == book.BookId);
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (temp != null)
                    {
                        Books.Remove(temp);
                    }
                    Books.Insert(0, book);
                });
            });

        }


        public override void OnItemClickCommand(object obj)
        {
            if (obj == null)
            {
               return;
            }

            NavigationService.NavigateTo(typeof(BookContentPage));

            ViewModelInstance.Instance.BookContent.LoadData(obj as Book);
        }

        #endregion
    }
}
