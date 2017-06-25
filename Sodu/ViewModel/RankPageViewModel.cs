using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Sodu.Core.HtmlService;
using Sodu.Service;

namespace Sodu.ViewModel
{
    public class RankPageViewModel : BasePageViewModel
    {

        #region 命令



        #endregion


        public RankPageViewModel()
        {
            PageCount = 8;
            Title = "排行榜";
        }

        public override void LoadData(object obj = null)
        {
            if (Books == null || Books.Count == 0)
            {
                GetData(1);
            }
        }

        public async void GetData(int pageIndex)
        {
            PageIndex = pageIndex;
            var url = SoduPageValue.GetRankListPage(pageIndex.ToString());

            var html = await GetHtmlData(url, true, true);
            var list = ListPageDataHelper.GetRankListFromHtml(html);
            if (list == null)
            {
                ToastHelper.ShowMessage("排行榜数据获取失败");
            }
            else
            {
                if (pageIndex == 1)
                {
                    Books = list;
                }
                else
                {
                    foreach (var item in list)
                    {
                        Books.Add(item);
                    }
                }
                Title = $"排行榜({PageIndex}/{PageCount})";
            }
        }


        public override void OnRefreshCommand(object obj)
        {
            GetData(1);
        }


        public override void OnPullToLoadCommand(object obj)
        {
            if (PageIndex == PageCount || IsLoading)
            {
                return;
            }
            GetData(PageIndex + 1);
        }

    }
}
