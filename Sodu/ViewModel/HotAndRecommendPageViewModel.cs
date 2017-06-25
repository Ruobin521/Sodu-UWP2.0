using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Core.HtmlService;
using Sodu.Service;

namespace Sodu.ViewModel
{
    public class HotAndRecommendPageViewModel : BasePageViewModel
    {
        public HotAndRecommendPageViewModel()
        {
            Title = "热门推荐";

        }


        public override void LoadData(object obj = null)
        {
            if (Books == null || Books.Count == 0)
            {
                GetData();
            }
        }

        public async void GetData()
        {
            var url = SoduPageValue.HomePage;
            var html = await GetHtmlData(url, true, true);
            var list = ListPageDataHelper.GetHotAndRecommendList(html);
            if (list == null || list.Count == 0)
            {
                ToastHelper.ShowMessage("热门推荐数据获取失败");
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
