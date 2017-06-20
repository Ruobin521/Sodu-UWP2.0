using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Sodu.Core.Entity;

namespace Sodu.Core.HtmlService
{
    public class ListPageDataHelper
    {
        public static List<Book> GetHotAndRecommendList(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var list = new List<Book>();
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                //整体
                var match = Regex.Match(html, "<div class=\"main-head\">.*?<table");

                if (match == null)
                {
                    return null;
                }

                var matches = match.ToString().Split(new[] { "<div class=\"main-head\">" }, StringSplitOptions.RemoveEmptyEntries);
                if (matches.Length < 3)
                {
                    return null;
                }

                try
                {
                    if (matches[2] != null && !string.IsNullOrEmpty(matches[2]))
                    {
                        var hotList = CommonGetEntityList(matches[2]);

                        if (hotList != null)
                        {
                            list.AddRange(hotList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                try
                {
                    if (matches[1] != null && !string.IsNullOrEmpty(matches[1]))
                    {
                        var commandList = CommonGetEntityList(matches[1]);
                        if (commandList != null)
                        {
                            list.AddRange(commandList);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            catch (Exception)
            {
                list = null;
            }
            return list;
        }
        public static ObservableCollection<Book> GetRankListFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            var matches = Regex.Matches(html, "<div class=\"main-html\".*?<div style=\"width:88px;float:left;\">.*?</div>");
            if (matches.Count == 0)
            {
                return null;
            }
            var list = new ObservableCollection<Book>();

            for (int i = 0; i < matches.Count; i++)
            {
                var tEntity = new Book();
                try
                {
                    var match = Regex.Match(matches[i].ToString(), "<a href=\"javascript.*?</a>");

                    tEntity.BookName = Regex.Match(match.ToString(), "(?<=addToFav\\(.*?').*?(?=')").ToString();
                    tEntity.UpdateCatalogUrl = Regex.Match(matches[i].ToString(), "(?<=<a href=\").*?(?=\">.*?</a>)").ToString();
                    tEntity.BookId = Regex.Match(match.ToString(), "(?<=id=\").*?(?=\")").ToString().Replace("a", "");
                    tEntity.NewestChapterName = Regex.Match(matches[i].ToString(), "(?<=<a href.*?>).*?(?=</a>)", RegexOptions.RightToLeft).ToString();
                    Match match2 = Regex.Match(matches[i].ToString(), "(<div.*?>).*?(?=</div>)", RegexOptions.RightToLeft);
                    tEntity.UpdateTime = Regex.Replace(match2.ToString(), "<.*?>", "");

                    if (matches[i].ToString().Contains("trend-"))
                    {
                        var value = Regex.Match(matches[i].ToString(), "<small class=\"trend-(.*?)\">(.*?)</small>", RegexOptions.RightToLeft);

                        if (value.Groups[1].ToString().Equals("up"))
                        {
                            tEntity.RankChangeValue = value.Groups[2].ToString();
                        }
                        else
                        {
                            tEntity.RankChangeValue = "-" + value.Groups[2];
                        }
                    }
                    else
                    {
                        tEntity.RankChangeValue = "-";
                    }
                    tEntity.NewestChapterName = DealWithChapterName(tEntity.NewestChapterName);
                    tEntity.LastReadChapterName = DealWithChapterName(tEntity.LastReadChapterName);

                    list.Add(tEntity);
                }
                catch
                {
                    return null;
                }
            }
            return list;
        }
        public static List<Book> GetUpdatePageBookList(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            ObservableCollection<Book>[] listArray = new ObservableCollection<Book>[3];
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            //更新列表
            Match t_string = Regex.Match(html, "<form name=\"form2\".*?</form>");
            if (t_string != null && !string.IsNullOrWhiteSpace(t_string.ToString()))
            {
                html = html.Replace(t_string.ToString(), "");
            }
            var updateList = CommonGetEntityList(html);
            return updateList;
        }
        public static ObservableCollection<Book> GetBookShelftListFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            //个人书架
            ObservableCollection<Book> t_list = new ObservableCollection<Book>();

            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?class=\"clearSc\".*?</div>");
            if (matches.Count == 0)
            {
                t_list = null;
                return t_list;
            }

            Book t_entity;
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i] == null) continue;
                t_entity = new Book();
                try
                {
                    MatchCollection divmatches = Regex.Matches(matches[i].ToString(), "<div.*?</div>");
                    t_entity.BookName = Regex.Replace(divmatches[0].ToString(), "<.*?>", "");
                    t_entity.NewestChapterName = Regex.Replace(divmatches[1].ToString(), "<.*?>", "");
                    t_entity.LastReadChapterName = Regex.Replace(divmatches[1].ToString(), "<.*?>", "");

                    t_entity.UpdateTime = Regex.Replace(divmatches[2].ToString(), "<.*?>", "");
                    t_entity.UpdateCatalogUrl = Regex.Match(divmatches[0].ToString(), "(?<=<a href=\").*?(?=\")").ToString();
                    t_entity.BookId = Regex.Match(divmatches[3].ToString(), "(?<=id=).*?(?=\")").ToString();

                    t_entity.NewestChapterName = DealWithChapterName(t_entity.NewestChapterName);
                    t_entity.LastReadChapterName = DealWithChapterName(t_entity.LastReadChapterName);

                    t_list.Add(t_entity);
                }
                catch
                {
                    return null;
                }
            }
            return t_list;
        }
        public static List<Book> GetSearchResultkListFromHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match t_string = Regex.Match(html, "<form name=\"form2\".*?</form>");

            if (t_string != null && !string.IsNullOrWhiteSpace(t_string.ToString()))
            {
                html = html.Replace(t_string.ToString(), "");
            }
            var t_list = GetSearchEntityList(html);
            return t_list;
        }
        public static List<Book> CommonGetEntityList(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var t_list = new List<Book>();
            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?class=xt1.*?</div>");
            //MatchCollection matches = Regex.Matches(html, "<div style=\"width:188px;float:left;\">.*?</div></div>");
            if (matches.Count == 0)
            {
                t_list = null;
                return t_list;
            }

            Book t_entity;
            for (int i = 0; i < matches.Count; i++)
            {
                t_entity = new Book();

                try
                {
                    Match match = Regex.Match(matches[i].ToString(), "<div style=\"width:482px;float:left;\">.*?</div>");
                    t_entity.BookName = Regex.Match(match.ToString(), "(?<=alt=\").*?(?=\")").ToString();
                    t_entity.UpdateCatalogUrl = Regex.Match(match.ToString(), "(?<=<a href=\").*?(?=\")").ToString();
                    t_entity.BookId = Regex.Match(matches[i].ToString(), "(?<=.*?id=\").*?(?=\")").ToString();
                    t_entity.NewestChapterName = Regex.Replace(match.ToString(), "<.*?>", "");
                    Match match2 = Regex.Match(matches[i].ToString(), "(?<=<.*?class=xt1>).*?(?=</div>)");
                    t_entity.UpdateTime = match2.ToString();
                    t_entity.NewestChapterName = DealWithChapterName(t_entity.NewestChapterName);
                    t_entity.LastReadChapterName = DealWithChapterName(t_entity.LastReadChapterName);
                    t_list.Add(t_entity);
                }
                catch
                {
                    t_list = null;
                    return t_list;
                }

            }
            return t_list;
        }


        public static List<Book> GetSearchEntityList(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var t_list = new List<Book>();
            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?</div></div>");
            //MatchCollection matches = Regex.Matches(html, "<div style=\"width:188px;float:left;\">.*?</div></div>");
            if (matches.Count == 0)
            {
                t_list = null;
                return t_list;
            }

            Book t_entity;
            for (int i = 0; i < matches.Count; i++)
            {
                t_entity = new Book();

                try
                {
                    Match match1 = Regex.Match(matches[i].ToString(), "<a href.*?>(.*?)<.*?onclick.*?addToFav\\((.*?), \'(.*?)\'\\)");
                    t_entity.BookName = match1.Groups[1].ToString();
                    t_entity.BookId = match1.Groups[2].ToString();
                    Match match = Regex.Match(matches[i].ToString(), "<div style=\"width:482px;float:left;\">.*?</div>");
                    t_entity.UpdateCatalogUrl = Regex.Match(match.ToString(), "(?<=<a href=\").*?(?=\")").ToString();
                    t_entity.NewestChapterName = Regex.Replace(match.ToString(), "<.*?>", "");
                    t_entity.LastReadChapterName = Regex.Replace(match.ToString(), "<.*?>", "");
                    Match match2 = Regex.Match(matches[i].ToString(), "(?<=<.*?class=xt1>).*?(?=</div>)");
                    t_entity.UpdateTime = match2.ToString();
                    t_entity.NewestChapterName = DealWithChapterName(t_entity.NewestChapterName);
                    t_entity.LastReadChapterName = DealWithChapterName(t_entity.LastReadChapterName);
                    t_list.Add(t_entity);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            return t_list;
        }
        public static List<Book> GetBookUpdateChapterList(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            List<Book> list = new List<Book>();
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?class=\"xt1.*?</div>");

            foreach (var item in matches)
            {
                MatchCollection matches2 = Regex.Matches(item.ToString(), "<a href.*?</a>");
                Book t_entity = new Book();
                t_entity.NewestChapterUrl = Regex.Match(matches2[0].ToString(), "(?<=&chapterurl=).*?(?=\")").ToString();

                //bool value = WebSet.CheckUrl(t_entity.NewestChapterUrl);
                //if (!value)
                //{
                //    continue;
                //}
                t_entity.NewestChapterName = Regex.Match(matches2[0].ToString(), "(?<=alt=\").*?(?=\")").ToString();

                //  t_entity.ChapterName = Regex.Replace(matches2[0].ToString(), "<.*?>", "").ToString();
                t_entity.LyWeb = Regex.Replace(matches2[1].ToString(), "<.*?>", "");

                Match match2 = Regex.Match(item.ToString(), "(?<=<.*?class=\"xt1\">).*?(?=</div>)");
                t_entity.UpdateTime = match2.ToString();

                t_entity.NewestChapterName = DealWithChapterName(t_entity.NewestChapterName);
                t_entity.LastReadChapterName = DealWithChapterName(t_entity.LastReadChapterName);

                list.Add(t_entity);
            }

            return list;

        }

        private static string DealWithChapterName(string chapterName)
        {
            if (string.IsNullOrEmpty(chapterName))
            {
                return chapterName;
            }
            chapterName = chapterName.Replace("【卓雅居全文字秒更】", "").Replace("target=_blank", "");

            return chapterName;
        }


    }
}
