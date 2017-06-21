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

    public class SourceHtmlHelper
    {
        private static readonly HttpHelper Http = new HttpHelper();

        public static async Task<string> GetHtmlByUrl(string url)
        {
            var html = await Http.WebRequestGet(url);
            return html;
        }


        /// <summary>
        /// 解析目录页地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetCatalogPageUrl(string url)
        {
            var catalogUrl = AnalisysSourceWebHtmlHelper.AnalisysHtml(url, null, type: AnalisysType.CatalogPageUrl);
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
            html = AnalisysSourceWebHtmlHelper.AnalisysHtml(url, html, AnalisysType.Content)?.ToString();
            return html;
        }

        //catalogs:[BookCatalog]?, introduction:String?,author:String?, cover:String?)
        /// <summary>
        /// 解析目录页数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns>目录列表，简介，作者，封面地址</returns>
        public static Tuple<List<BookCatalog>, string, string, string> GetCatalogPageData(string url)
        {

            return new Tuple<List<BookCatalog>, string, string, string>(null, null, null, null);
        }
    }
}
