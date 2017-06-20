using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Core.HtmlService;

namespace Sodu.ViewModel
{
    public class HotAndRecommendPageViewModel : BasePageViewModel
    {
        public HotAndRecommendPageViewModel()
        {
            Title = "热门推荐";

        }


        public override void LoadData()
        {
            if (Books == null || Books.Count == 0)
            {
                GetData();
            }
        }

        public async void GetData()
        {
            var url = WebPageUrl.HomePage;
            var html = await GetHtmlData(url,true,true);
            var list = ListPageDataHelper.GetHotAndRecommendList(html);
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("排行榜数据获取失败");
            }
            else
            {
                Books?.Clear();
                foreach (var book in list)
                {
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


    }
}
