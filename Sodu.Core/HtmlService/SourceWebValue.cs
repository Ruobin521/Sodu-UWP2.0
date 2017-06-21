using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.HtmlService
{
    public class SourceWebValue
    {
        /// 第七中文
        public const string Dqzw = "www.d7zy.com";

        /// <summary>
        /// 7度书屋
        /// </summary>
        public const string Qdsw = "www.7dsw.com";

        /// <summary>
        /// 第九中文网
        /// </summary>
        public const string Dijiuzww = "dijiuzww.com";

        /// <summary>
        /// 清风小说
        /// </summary>
        public const string Qfxs = "www.qfxs.cc";

        /// <summary>
        /// 窝窝小说网2
        /// </summary>
        //    string  wwxsw2 = "www.biquge120.com";
        /// <summary>
        /// 大海中文
        /// </summary>
        public const string Dhzw = "www.dhzw.com";

        /// <summary>
        /// 少年文学
        /// </summary>
        public const string Snwx = "www.snwx.com";

        /// <summary>
        /// 爱上中文
        /// </summary>
        public const string Aszw = "www.aszw8.com";

        /// <summary>
        /// 手牵手小说
        /// </summary>
        public const string Sqsxs = "www.sqsxs.com";

        /// <summary>
        /// 找书网
        /// </summary>
        public const string Zsw = "www.zhaodaoshu.com";

        /// <summary>
        /// 去笔趣阁
        /// </summary>
        public const string Xbiquge = "www.xbiquge.net";

        /// <summary>
        /// 古古
        /// </summary>
        public const string Ggxs = "www.55xs.com";

        /// <summary>
        /// 倚天中文
        /// </summary>
        //    string  ytzww = "www.ytzww.com";
        /// <summary>
        /// 书路小说
        /// </summary>
        public const string Shu6 = "www.shu6.cc";

        /// <summary>
        /// 风华居
        /// </summary>
        public const string Fenghuaju = "www.fenghuaju.cc";

        /// <summary>
        ///云来阁
        /// </summary>
        public const string Ylg = "www.yunlaige.com";

        /// <summary>
        ///4k中文
        /// </summary>
        public const string Fourkzw = "www.4kzw.com";

        /// <summary>
        ///幼狮书盟
        /// </summary>
        public const string Yssm = "www.youshishumeng.com";

        /// <summary>
        ///80小说
        /// </summary>
        //    string  su80 = "www.su80.net";
        /// <summary>
        ///木鱼哥
        /// </summary>
        public const string Myg = "www.muyuge.com";

        /// <summary>
        ///木鱼哥
        /// </summary>
        public const string Myg2 = "www.muyuge.net";

        /// <summary>
        ///VIVI小说网（顶点小说）
        /// </summary>
        //   string  vivi = "www.zkvivi.com";
        //   string  vivi = "www.zkvivi.com";
        /// <summary>
        ///轻语小说
        /// </summary>
        public const string Qyxs = "www.qingyuxiaoshuo.com";

        /// <summary>
        /// 乐文
        /// </summary>
        public const string Lww = "www.lwtxt.net";

        /// <summary>
        /// 去笔趣阁
        /// </summary>
        //   string  qbqg = "www.qbiquge.com";
        /// <summary>
        /// 笔铺阁
        /// </summary>
        //   string  bpg = "www.bipuge.com";
        /// <summary>
        /// 秋水轩
        /// </summary>
        public const string Qsx = "www.qiushuixuan.cc";

        /// <summary>
        /// 卓雅居
        /// </summary>
        public const string Zyj = "www.zhuoyaju.com";

        /// <summary>
        /// 81xs
        /// </summary>
        public const string Xs81 = "www.81xsw.com";

        /// <summary>
        /// 风云
        /// </summary>
        public const string Fyxs = "www.baoliny.com";

        /// <summary>
        /// 大书包
        /// </summary>
        public const string Dsb = "www.dashubao.cc";

        /// <summary>
        /// 漂流地
        /// </summary>
        public const string Pld = "piaoliudi.com";

        /// <summary>
        /// 齐鲁文学
        /// </summary>
        public const string Qlwx = "www.76wx.com";


        public static bool CheckUrl(string url)
        {
            var uri = new Uri(url);
            var host = uri.Host;
            return GetPropertyInfoArray().FirstOrDefault(p => p == host) != null;
        }

        private static List<string> GetPropertyInfoArray()
        {
            var values = new List<string>();
            try
            {
                var type = typeof(SourceWebValue);
                var props = type.GetFields();

                foreach (var pro in props)
                {
                    var temp = pro.GetValue(null);
                    if (temp != null)
                    {
                        values.Add(temp.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return values;
        }
    }
}
