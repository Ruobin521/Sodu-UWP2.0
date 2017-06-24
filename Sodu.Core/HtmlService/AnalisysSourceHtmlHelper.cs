using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sodu.Core.Entity;

namespace Sodu.Core.HtmlService
{
    public class AnalisysSourceHtmlHelper
    {
        public static object AnalisysHtml(string url, string html, AnalisysType type, string bookName = null)
        {
            var uri = new Uri(url);
            var host = uri.Host;
            switch (host)
            {
                case SourceWebValue.Qlwx:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"content\">.*?</div>");
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageUrl)
                    {
                        var value = GetCatalogPageUrlCommon(url);
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageData)
                    {
                        var baseUrl = "http://" + host;
                        var value = GetCatalogPageDataCommon(url, baseUrl, html,
                            htmlRegex: "<dl>.*?</dl>",
                            catalogRegex: "<dd><a href=\"(.*?)\">(.*?)</a></dd>",
                            introRegex: "<div id=\"intro\">(.*?)</div",
                            coverRegex: "<div id=\"fmimg\">.*?<img.*?src=\"(.*?)\".*?/>",
                            authorRegex: "<p>作.*?者：(.*?)</p>");
                        return value;
                    }
                    break;


                default:
                    return null;
            }
            return null;
        }

        /// <summary>
        /// 替换html标签
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ReplaceSymbol(string html)
        {
            html = Regex.Replace(html, "<br.*?/>", "\n");
            html = Regex.Replace(html, "<script.*?</script>", "");
            html = Regex.Replace(html, "&nbsp;", " ");
            html = Regex.Replace(html, "<p.*?>", "\n");
            html = Regex.Replace(html, "<.*?>", "");
            html = html.Replace("&lt;/script&gt;", "");
            html = html.Replace("&lt;/div&gt;", "");
            html = html.Replace(" ", "");
            return html;
        }

        /// <summary>
        /// 获取html正文内容通用方法
        /// </summary>
        /// <param name="html"></param>
        /// <param name="regexStr"></param>
        /// <returns></returns>
        private static string GetContentFromHtmlCommon(string html, string regexStr)
        {
            try
            {
                string result = null;
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                var match = Regex.Match(html, regexStr, RegexOptions.IgnoreCase);
                if (match == null)
                {
                    return null;
                }
                result = match.ToString();
                result = Regex.Replace(result, "阅读本书最新章节请到.*?敬请记住我们最新网址.*?m", "");
                result = ReplaceSymbol(result);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// 获取目录页地址通用方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetCatalogPageUrlCommon(string url)
        {
            string result = url.Substring(0, url.LastIndexOf('/') + 1);

            return result;
        }

        /// <summary>
        /// 获取目录页数据通用方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static Tuple<List<BookCatalog>, string, string, string> GetCatalogPageDataCommon(string baseUrl, string coverBaseUrl, string html, string htmlRegex, string catalogRegex, string introRegex, string coverRegex, string authorRegex)
        {
            string result = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");


            //目录
            List<BookCatalog> catalogs = null;
            var match1 = Regex.Match(html, htmlRegex, RegexOptions.IgnoreCase);
            if (match1 == null)
            {
                catalogs = null;
            }
            else
            {
                var collection = Regex.Matches(match1.ToString(), catalogRegex);
                if (collection != null && collection.Count > 0)
                {
                    int i = 1;
                    catalogs = new List<BookCatalog>();
                    foreach (Match tempMatch in collection)
                    {
                        if (tempMatch.Groups.Count < 3)
                        {
                            continue;
                        }
                        var bookCatalog = new BookCatalog();
                        bookCatalog.Index = i;

                        bookCatalog.CatalogUrl = baseUrl + tempMatch.Groups[1];
                        bookCatalog.CatalogName = tempMatch.Groups[2].ToString();
                        i++;
                        catalogs.Add(bookCatalog);
                    }
                }
            }


            //简介
            string intro = null;
            var match2 = Regex.Match(html, introRegex, RegexOptions.IgnoreCase);
            if (match2 != null && match2.Groups.Count >= 2)
            {
                intro = match2.Groups[1].ToString();
                intro = ReplaceSymbol(intro);
            }

            //封面
            string cover = null;
            var match3 = Regex.Match(html, coverRegex, RegexOptions.IgnoreCase);
            if (match3 != null && match3.Groups.Count >= 2)
            {
                cover = coverBaseUrl + match3.Groups[1];
            }


            //作者
            string author = null;
            var match4 = Regex.Match(html, authorRegex, RegexOptions.IgnoreCase);
            if (match4 != null && match4.Groups.Count >= 2)
            {
                author = match4.Groups[1].ToString();
            }

            return new Tuple<List<BookCatalog>, string, string, string>(catalogs, intro, cover, author);
        }
    }
}
