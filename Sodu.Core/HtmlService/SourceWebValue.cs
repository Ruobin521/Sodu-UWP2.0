using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.HtmlService
{
    public class SourceWebValue
    {

        /// <summary>
        /// 少年文学
        /// </summary>
        public const string Snwx = "www.snwx.com";

        /// <summary>
        /// 爱上中文
        /// </summary>
        public const string Aszw = "www.aszw.org";



        /// <summary>
        /// 风华居
        /// </summary>
        public const string Fenghuaju = "www.fenghuaju.cc";

        /// <summary>
        ///80小说（暂不处理）
        /// </summary>
        //    string  su80 = "www.su80.org";

        /// <summary>
        ///木鱼哥
        /// </summary>
        public const string Myg = "www.muyuge.com";

      
        /// <summary>
        /// 乐文
        /// </summary>
        public const string Lww = "www.lwtxt.net";

        /// <summary>
        /// 卓雅居
        /// </summary>
        public const string Zyj = "www.zhuoyaju.com";

        /// <summary>
        /// 81xs
        /// </summary>
        public const string Xs81 = "www.81xsw.com";


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

        ///// <summary>
        ///// 手牵手小说(挂了)
        ///// </summary>
        //public const string Sqsxs = "www.sqsxs.com";

        ///// <summary>
        ///// 大海中文（挂了）
        ///// </summary>
        //public const string Dhzw = "www.dhzw.com";

        ///// <summary>
        /////第七中文（挂了）
        ///// </summary>
        //public const string Dqzw = "www.d7zy.com";

        ///// <summary>
        ///// 7度书屋（挂了）
        ///// </summary>
        //public const string Qdsw = "www.7dsw.com";

        ///// <summary>
        ///// 第九中文网（挂了）
        ///// </summary>
        //public const string Dijiuzww = "dijiuzww.com";

        ///// <summary>
        ///// 清风小说（挂了）
        ///// </summary>
        //public const string Qfxs = "www.qfxs.cc";

        /// <summary>
        /// 窝窝小说网2（挂了）
        /// </summary>
        //    string  wwxsw2 = "www.biquge120.com";

        ///// <summary>
        ///// 找书网（挂了）
        ///// </summary>
        //public const string Zsw = "www.zhaodaoshu.com";

        ///// <summary>
        ///// 去笔趣阁（挂了）
        ///// </summary>
        //public const string Xbiquge = "www.xbiquge.net";

        ///// <summary>
        ///// 古古（挂了）
        ///// </summary>
        //public const string Ggxs = "www.55xs.com";

        /// <summary>
        /// 倚天中文（挂了）
        /// </summary>
        //    string  ytzww = "www.ytzww.com";

        ///// <summary>
        ///// 书路小说（挂了）
        ///// </summary>
        //public const string Shu6 = "www.shu6.cc";

        ///// <summary>
        /////云来阁(打不开)
        ///// </summary>
        //public const string Ylg = "www.yunlaige.com";

        ///// <summary>
        /////4k中文(挂了）
        ///// </summary>
        //public const string Fourkzw = "www.4kzw.com";

        ///// <summary>
        /////幼狮书盟(挂了)
        ///// </summary>
        //public const string Yssm = "www.youshishumeng.com";

        ///// <summary>
        /////轻语小说（挂了）
        ///// </summary>
        //public const string Qyxs = "www.qingyuxiaoshuo.com";

        ///// <summary>
        ///// 秋水轩（挂了）
        ///// </summary>
        //public const string Qsx = "www.qiushuixuan.cc";

        ///// <summary>
        ///// 风云(挂了)
        ///// </summary>
        //public const string Fyxs = "www.baoliny.com";

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
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
            return values;
        }
    }
}
