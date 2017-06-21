using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.HtmlService
{
    public class SoduPageValue
    {
        /// <summary>
        /// 首页地址
        /// </summary>
        public const string HomePage = "http://www.sodu.cc/";

        /// <summary>
        /// 登录页
        /// </summary>
        public const string LoginPostPage = "http://www.sodu.cc/handler/login.html";

        /// <summary>
        /// 注销页面
        /// </summary>
        public const string LogoutPage = "http://www.sodu.cc/logout.html?callback=http://www.sodu.cc/home.html";

        //注册页面post
        public const string RegisterPostPage = "http://www.sodu.cc/handler/reg.html";

        /// <summary>
        /// 我的书架
        /// </summary>
        public const string BookShelfPage = "http://www.sodu.cc/home.html";

        /// <summary>
        /// 搜索地址
        /// </summary>
        public const string BookSearchPage = "http://www.sodu.cc/result.html?searchstr={0}";

        /// <summary>
        /// 排行榜地址
        /// </summary>
        public const string BookRankListPage = "http://www.sodu.cc/top.html";

        public const string BookRankListPage2 = "http://www.sodu.cc/top_{0}.html";
        /// <summary>
        /// 最新更新
        /// </summary>
        public const string LatUpdateBookkListPage = "http://www.sodu.cc/map.html";

        public const string LatUpdateBookkListPage2 = "http://www.sodu.cc/map_{0}.html";

        ///添加至书架
        public const string AddToShelfPage = "http://www.sodu.cc/handler/home.html?bid={0}";

        public static string GetRankListPage(string pageIndex = null)
        {
            if (pageIndex == null || pageIndex.Equals("1"))
            {
                return BookRankListPage;
            }
            else
            {
                return string.Format(BookRankListPage2, pageIndex);
            }
        }
    }
}
