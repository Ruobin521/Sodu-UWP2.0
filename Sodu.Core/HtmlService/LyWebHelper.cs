using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.HtmlService
{
    public class LyWebHelper
    {
        /// 第七中文
        public string Dqzw { get; } = "www.d7zy.com";

        /// <summary>
        /// 7度书屋
        /// </summary>
        public string Qdsw { get; } = "www.7dsw.com";

        /// <summary>
        /// 第九中文网
        /// </summary>
        public string Dijiuzww { get; } = "dijiuzww.com";

        /// <summary>
        /// 清风小说
        /// </summary>
        public string Qfxs { get; } = "www.qfxs.cc";

        /// <summary>
        /// 窝窝小说网2
        /// </summary>
        //    string  wwxsw2 = "www.biquge120.com";
        /// <summary>
        /// 大海中文
        /// </summary>
        public string Dhzw { get; } = "www.dhzw.com";

        /// <summary>
        /// 少年文学
        /// </summary>
        public string Snwx { get; } = "www.snwx.com";


        /// <summary>
        /// 爱上中文
        /// </summary>
        public string Aszw { get; } = "www.aszw8.com";

        /// <summary>
        /// 手牵手小说
        /// </summary>
        public string Sqsxs { get; } = "www.sqsxs.com";

        /// <summary>
        /// 找书网
        /// </summary>
        public string Zsw { get; } = "www.zhaodaoshu.com";

        /// <summary>
        /// 去笔趣阁
        /// </summary>
        public string Xbiquge { get; } = "www.xbiquge.net";

        /// <summary>
        /// 古古
        /// </summary>
        public string Ggxs { get; } = "www.55xs.com";

        /// <summary>
        /// 倚天中文
        /// </summary>
        //    string  ytzww = "www.ytzww.com";
        /// <summary>
        /// 书路小说
        /// </summary>
        public string Shu6 { get; } = "www.shu6.cc";

        /// <summary>
        /// 风华居
        /// </summary>
        public string Fenghuaju { get; } = "www.fenghuaju.cc";

        /// <summary>
        ///云来阁
        /// </summary>
        public string Ylg { get; } = "www.yunlaige.com";

        /// <summary>
        ///4k中文
        /// </summary>
        public string Fourkzw { get; } = "www.4kzw.com";

        /// <summary>
        ///幼狮书盟
        /// </summary>
        public string Yssm { get; } = "www.youshishumeng.com";

        /// <summary>
        ///80小说
        /// </summary>
        //    string  su80 = "www.su80.net";
        /// <summary>
        ///木鱼哥
        /// </summary>
        public string Myg { get; } = "www.muyuge.com";

        /// <summary>
        ///木鱼哥
        /// </summary>
        public string Myg2 { get; } = "www.muyuge.net";

        /// <summary>
        ///VIVI小说网（顶点小说）
        /// </summary>
        //   string  vivi = "www.zkvivi.com";
        //   string  vivi = "www.zkvivi.com";
        /// <summary>
        ///轻语小说
        /// </summary>
        public string Qyxs { get; } = "www.qingyuxiaoshuo.com";

        /// <summary>
        /// 乐文
        /// </summary>
        public string Lww { get; } = "www.lwtxt.net";

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
        public string Qsx { get; } = "www.qiushuixuan.cc";

        /// <summary>
        /// 卓雅居
        /// </summary>
        public string Zyj { get; } = "www.zhuoyaju.com";

        /// <summary>
        /// 81xs
        /// </summary>
        public string Xs81 { get; } = "www.81xsw.com";

        /// <summary>
        /// 风云
        /// </summary>
        public string Fyxs { get; } = "www.baoliny.com";

        /// <summary>
        /// 大书包
        /// </summary>
        public string Dsb { get; } = "www.dashubao.cc";

        /// <summary>
        /// 漂流地
        /// </summary>
        public string Pld { get; } = "piaoliudi.com";

        /// <summary>
        /// 齐鲁文学
        /// </summary>
        public string Qlwx { get; } = "www.76wx.com";


        public static bool CheckUrl(string url)
        {
            return GetPropertyInfoArray().FirstOrDefault(p => p == url) != null;
        }

        private static List<string> GetPropertyInfoArray()
        {
            var values = new List<string>();
            try
            {
                var type = typeof(LyWebHelper);
                var obj = Activator.CreateInstance(type);
                PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var pro in props)
                {
                    var temp = pro.GetValue(obj, null);
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
