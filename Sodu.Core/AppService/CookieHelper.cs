using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Sodu.Core.HtmlService;

namespace Sodu.Core.Util
{
    public class CookieHelper
    {
        public static void SetCookie(string url, bool isRember)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(url));
            HttpCookie cookie = null;
            foreach (var cookieItem in cookieCollection)
            {
                if (cookieItem.Name == "sodu_user")
                {
                    cookie = cookieItem;
                }
            }

            if (cookie == null)
            {
                return;
            }
            if (isRember)
            {
                //设置cookie存活时间，如果为null，则表示只在一个会话中生效。
                cookie.Expires = new DateTimeOffset(DateTime.Now.AddDays(365));
                filter.CookieManager.SetCookie(cookie, false);
            }
            else
            {
                filter.CookieManager.DeleteCookie(cookie);
            }

        }
        public static bool CheckLogin()
        {
            var filter = new HttpBaseProtocolFilter();
            var cookieCollection = filter.CookieManager.GetCookies(new Uri(SoduPageValue.HomePage));
            var cookieItem = cookieCollection.FirstOrDefault(p => p.Name.Equals("sodu_user"));
            return cookieItem != null;
        }

    }


}
