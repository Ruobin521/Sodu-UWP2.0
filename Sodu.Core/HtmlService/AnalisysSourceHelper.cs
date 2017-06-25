using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Core.Entity;
using Sodu.Core.Util;

namespace Sodu.Core.HtmlService
{
    public enum AnalisysType
    {
        Content,
        CatalogPageUrl,
        CatalogPageData
    }

    public class AnalisysSourceHelper
    {
        public static async Task<string> GetHtmlByUrl(string url)
        {
            var html = await new HttpHelper().WebRequestGet(url);
            return html;
        }


        /// <summary>
        /// 解析目录页地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCatalogPageUrl(string url)
        {
            var catalogUrl = AnalisysSourceHtmlHelper.AnalisysHtml(url, null, type: AnalisysType.CatalogPageUrl);
            return catalogUrl?.ToString();
        }

        /// <summary>
        /// 解析正文内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetCatalogContent(string url)
        {
            var html = await GetHtmlByUrl(url);
            html = AnalisysSourceHtmlHelper.AnalisysHtml(url, html, AnalisysType.Content)?.ToString();
            return html;
        }

        public static string AnalisysCatalogContent(string url, string html)
        {
            html = AnalisysSourceHtmlHelper.AnalisysHtml(url, html, AnalisysType.Content)?.ToString();
            return html;
        }
        //catalogs:[BookCatalog]?, introduction:String?,author:String?, cover:String?)
        /// <summary>
        /// 解析目录页数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns>目录列表，简介，封面地址,作者</returns>
        public static async Task<Tuple<List<BookCatalog>, string, string, string>> GetCatalogPageData(string url)
        {
            var html = await GetHtmlByUrl(url);

            var value = AnalisysSourceHtmlHelper.AnalisysHtml(url, html, AnalisysType.CatalogPageData);

            return value as Tuple<List<BookCatalog>, string, string, string>;
        }
    }
}
