using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Sodu.Core.HtmlService;

namespace Sodu.ViewModel
{
    public class RankPageViewModel : BasePageViewModel
    {
        public RankPageViewModel()
        {
            PageCount = 8;
            Title = "排行榜";
          
        }

        public override void LoadData()
        {
            if (Books == null || Books.Count == 0)
            {
                GetData(1);
            }
        }

        public async void GetData(int pageIndex)
        {
            PageIndex = pageIndex;
            var url = WebPageUrl.GetRankListPage(pageIndex.ToString());

            var html = await GetHtmlData(url);
            var list = ListPageDataHelper.GetRankListFromHtml(html);
            if (list == null)
            {
                Console.WriteLine("排行榜数据获取失败");
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

        public override void OnItemClickCommand(object obj)
        {

        }
    }
}
