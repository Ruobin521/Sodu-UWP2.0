using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                // 少年文学
                case SourceWebValue.Snwx:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"BookText\">.*?</div>");
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
                            catalogsRegex: "<div id=\"list\">.*?</div>",
                            catalogRegex: "<dd><a href=\"(.*?)\".*?>(.*?)</a></dd>",
                            introRegex: "<div class=\"intro\">(.*?)</div>",
                            coverRegex: "<div id=\"fmimg\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<i>作者：(.*?)</i>");
                        return value;
                    }
                    break;

                // 爱上中文
                case SourceWebValue.Aszw:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"contents\".*?</div>");
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
                        var value = GetCatalogPageDataCommon(url, "", html,
                            catalogsRegex: "<head>.*?</html>",
                            catalogRegex: "<td class=\"L\".*?href=\"(.*?)\".*?>(.*?)</a></td>",
                            introRegex: "<div class=\"js\">(.*?)<p><b>",
                            coverRegex: "<div class=\"pic\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<i>作者：(.*?)</i>");
                        return value;
                    }
                    break;

                // 七度
                case SourceWebValue.Qdsw:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"BookText\">.*?</div>");
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
                            catalogsRegex: "<div id=\"list\">.*?</div>",
                            catalogRegex: "<dd><a href=\"(.*?)\".*?>(.*?)</a></dd>",
                            introRegex: "<div class=\"intro\">(.*?)</div>",
                            coverRegex: "<div id=\"fmimg\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<i>作者：(.*?)</i>");
                        return value;
                    }
                    break;


                // 云来阁
                case SourceWebValue.Ylg:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"content\">.*?<div class=\"bottomlink tc\">");
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
                            catalogsRegex: "<table.*?</table>",
                            catalogRegex: "<a href=\"(.*?)\">(.*?)</a>",
                            introRegex: "<meta property=\"og:description\" content=\"(.*?)\"/>",
                            coverRegex: "<meta property=\"og:image\" content=\"(.*?)\"/>",
                            authorRegex: "<meta property=\"og:novel:author\" content=\"(.*?)\"/>");
                        return value;
                    }
                    break;

                // 古古 55xs
                case SourceWebValue.Ggxs:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<dd id=\"contents\".*?</dd>");
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
                        var value = GetCatalogPageDataCommon(baseUrl, baseUrl, html,
                            catalogsRegex: "<table.*?class=\"list\">.*?</table>",
                            catalogRegex: "<td><a href=\"(.*?)\".*?>(.*?)</a></td>",
                            introRegex: "<div class=\"msgarea\">(.*?)</p>",
                            coverRegex: "<div class=\"img1\"><img src=\"(.*?)\".*?</div>",
                            authorRegex: "<a href=\"/modules/article/authorarticle.php\\?author=.*?>(.*?)</a>");
                        return value;
                    }
                    break;


                // 风云
                case SourceWebValue.Fyxs:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<p id=\"?content\"?.*?</p>");
                        value = value.Replace("", "");
                        value = Regex.Replace(value, "【无弹窗小说网.*?www.baoliny.com】", "");
                        value = Regex.Replace(value, "【风云小说阅读网.*?www.baoliny.com】", "");
                        value = Regex.Replace(value, "【最新章节阅读.*?www.baoliny.com】", "");
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageUrl)
                    {
                        var value = GetCatalogPageUrlCommon(url) + "index.html";
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageData)
                    {
                        var baseUrl = "http://" + host;
                        var value = GetCatalogPageDataCommon("", "", html,
                            catalogsRegex: "<div class=\"readerListShow\".*?</div>",
                            catalogRegex: "<td.*?href=\"(.*?)\".*?>(.*?)</a></td>",
                            introRegex: "<p align=\"center\">.*?<p>(.*?)</p>",
                            coverRegex: "",
                            authorRegex: "②作者(.*?)所写");
                        return value;
                    }
                    break;



                // 第九中文
                case SourceWebValue.Dijiuzww:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"?content\"?.*?</div>");
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
                            catalogsRegex: "正文</dt>.*?</div>",
                            catalogRegex: "<dd><a href=\"/(.*?)\">(.*?)</a></dd>",
                            introRegex: "<div id=\"intro\">.*?</p>",
                            coverRegex: "<div id=\"fmimg\"><script.*?src=\"/(.*?)\">",
                            authorRegex: "<p>作&nbsp;&nbsp;&nbsp;&nbsp;者：(.*?)</p>");
                        return value;
                    }
                    break;



                // 大海中文
                case SourceWebValue.Dhzw:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"BookText\">.*?</div>");
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
                            catalogsRegex: "<div id=\"list\">.*?</div>",
                            catalogRegex: "<dd><a href=\"(.*?)\".*?>(.*?)</a></dd>",
                            introRegex: "<div class=\"intro\">(.*?)</div>",
                            coverRegex: "<div id=\"fmimg\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<i>作者：(.*?)</i>");
                        return value;
                    }
                    break;


                // 手牵手
                case SourceWebValue.Sqsxs:
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
                            catalogsRegex: "<div id=\"list\">.*?</div>",
                            catalogRegex: "<dd><a href=\"(.*?)\".*?>(.*?)</a></a></dd>",
                            introRegex: "<div id=\"intro\">(.*?)</div>",
                            coverRegex: "<div id=\"fmimg\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<meta property=\"og:novel:author\" content=\"(.*?)\"/>");
                        return value;
                    }
                    break;

                // 风华居
                case SourceWebValue.Fenghuaju:
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
                        var value = GetCatalogPageDataCommon(baseUrl, baseUrl, html,
                            catalogsRegex: "正文</dt>.*?</div>",
                            catalogRegex: "<dd><a href=\"(.*?)\">(.*?)</a></dd>",
                            introRegex: "<div id=\"intro\">(.*?)</div>",
                            coverRegex: "<div id=\"fmimg\"><img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<p>作&nbsp;&nbsp;&nbsp;&nbsp;者：(.*?)");
                        return value;
                    }
                    break;



                // 木鱼哥
                case SourceWebValue.Myg:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<p class=\"vote\">.*?</div>");

                        value = value.Replace("，请用搜索引擎各种任你观看", "")
                            .Replace("【一秒钟记住本站：muyuge.com 木鱼哥】", "")
                            .Replace("【木鱼哥 温馨提示：如果您喜欢本书，请按 回车键 回到目录页，并使用 分享按钮 分享到微博和空间，举手之劳，分享越多更新更给力！】", "");
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
                        var value = GetCatalogPageDataCommon("", "", html,
                            catalogsRegex: "<div id=\"xslist\">.*?</div>",
                            catalogRegex: "<li><a href=\"(.*?)\".*?>(.*?)</a></li>",
                            introRegex: "<p>&nbsp;&nbsp;&nbsp;&nbsp;(.*?)</p>",
                            coverRegex: "<div id=\"fmimg\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "</h1>&nbsp;&nbsp;&nbsp;&nbsp;(.*?)/著</div>");
                        return value;
                    }
                    break;



                // 乐文
                case SourceWebValue.Lww:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"txtright\">.*?<span id=\"endtips\">");
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageUrl)
                    {
                        var value = GetLwCatalogPageUrl(url);
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageData)
                    {
                        var baseUrl = "http://" + host;
                        var value = GetCatalogPageDataCommon(baseUrl, baseUrl, html,
                            catalogsRegex: "<h2 class=\"bookTitle\">.*?<div id=\"uyan_frame\">",
                            catalogRegex: "<a href=\"(.*?)\">(.*?)</a>",
                            introRegex: "<div class=\"reBook borderF\">(.*?)</div>",
                            coverRegex: "<div style=\"width:600px; padding:5px\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<p>作者：(.*?)&nbsp;&nbsp;&nbsp;</p>");
                        return value;
                    }
                    break;




                // 卓雅居
                case SourceWebValue.Zyj:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"?content\"?.*?</div>");
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageUrl)
                    {
                        var value = GetZyjCatalogPageUrl(url);
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageData)
                    {
                        var baseUrl = "http://" + host;
                        var value = GetCatalogPageDataCommon(url, baseUrl, html,
                            catalogsRegex: "正文</dt>.*?</div>",
                            catalogRegex: "<dd><a href=\"/(.*?)\">(.*?)</a></dd>",
                            introRegex: "<div id=\"intro\">.*?</p>",
                            coverRegex: "<div id=\"fmimg\"><script.*?src=\"/(.*?)\">",
                            authorRegex: "<p>作&nbsp;&nbsp;&nbsp;&nbsp;者：(.*?)</p>");
                        return value;
                    }
                    break;

                // 81中文
                case SourceWebValue.Xs81:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"?content\"?.*?</div>");
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
                            catalogsRegex: "<div id=\"list\">.*?</div>",
                            catalogRegex: "<dd><a href=\"(.*?)\">(.*?)</a></dd>",
                            introRegex: "<div id=\"intro\">(.*?)</div>",
                            coverRegex: "<div id=\"fmimg\"><img.*?src=\"(.*?)\".*?/>",
                            authorRegex: "<p>作&nbsp;&nbsp;&nbsp;&nbsp;者：(.*?)</p>");
                        return value;
                    }
                    break;



                // 大书包
                case SourceWebValue.Dsb:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div class=\"hr101\">.*?<span id=\"endtips\">");
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageUrl)
                    {
                        var value = GetDsbCatalogPageUrl(url);
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageData)
                    {
                        var baseUrl = "http://" + host;
                        var value = GetCatalogPageDataCommon(url, "", html,
                            catalogsRegex: "<h2 class=\"bookTitle\">.*?<div id=\"uyan_frame\">",
                            catalogRegex: "<a href=\"(.*?)\">(.*?)</a>",
                            introRegex: "<div class=\"reBook borderF\">(.*?)</div>",
                            coverRegex: "<div style=\"width:600px; padding:5px\">.*?<img.*?src=\"(.*?)\".*?>",
                            authorRegex: "<p>作者：(.*?)&nbsp;&nbsp;&nbsp;</p>");
                        return value;
                    }
                    break;


                // 漂流地
                case SourceWebValue.Pld:
                    if (type == AnalisysType.Content)
                    {
                        var value = GetContentFromHtmlCommon(html, "<div id=\"BookText\">.*?</div>");
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageUrl)
                    {
                        var value = GetCatalogPageUrlCommon(url);
                        return value;
                    }
                    if (type == AnalisysType.CatalogPageData)
                    {
                        var baseUrl = "https://" + host;
                        var value = GetCatalogPageDataCommon(baseUrl, baseUrl, html,
                            catalogsRegex: "<dl class=\"chapterlist\">.*?</dl>",
                            catalogRegex: "<dd><a href=\"(.*?)\">(.*?)</a></dd>",
                            introRegex: "<p class=\"book-intro\">(.*?)</p>",
                            coverRegex: "<div class=\"book-img\">.*?<img.*?src=\"(.*?)\".*?/>",
                            authorRegex: "<p>作.*?者：(.*?)</p>");
                        return value;
                    }
                    break;

                //齐鲁文学
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
                            catalogsRegex: "<dl>.*?</dl>",
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
            html = html.Replace("  ", "　");
            html = html.Replace("\n\n", "\n");
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
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
        /// 获取乐闻目录页地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetLwCatalogPageUrl(string url)
        {
            //http://www.lwtxt.net/html/49440_21322790.html
            var index1 = url.LastIndexOf("/", StringComparison.Ordinal);
            var index2 = url.LastIndexOf("_", StringComparison.Ordinal);
            var bookId = url.Substring(index1 + 1, index2 - index1 + 1);
            var result = "http://www.lwtxt.net/modules/article/reader.php?aid=" + bookId;
            return result;
        }

        /// <summary>
        /// 卓雅居
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetZyjCatalogPageUrl(string url)
        {
            //   http://www.zhuoyaju.com/9/9598/41253188.html

            var uri = new Uri(url);
            var bookId = uri.Segments[2];
            var result = $"http://www.zhuoyaju.com/book/{bookId}.html";
            return result;
        }


        private static string GetDsbCatalogPageUrl(string url)
        {
            //http://www.dashubao.cc/html/57509_22892059.html
            // http://www.dashubao.cc/modules/article/reader.php?aid=57509

            var index1 = url.LastIndexOf("/", StringComparison.Ordinal);
            var index2 = url.LastIndexOf("_", StringComparison.Ordinal);
            var bookId = url.Substring(index1 + 1, index2 - index1 - 1);
            var result = "http://www.dashubao.cc/modules/article/reader.php?aid=" + bookId;
            return result;
        }

        /// <summary>
        /// 获取目录页数据通用方法
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static Tuple<List<BookCatalog>, string, string, string> GetCatalogPageDataCommon(string baseUrl, string coverBaseUrl, string html, string catalogsRegex, string catalogRegex, string introRegex, string coverRegex, string authorRegex)
        {
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            //目录
            List<BookCatalog> catalogs = null;
            var match1 = Regex.Match(html, catalogsRegex, RegexOptions.IgnoreCase);
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
