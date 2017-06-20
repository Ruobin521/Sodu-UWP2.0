using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.Config
{
    public class AppDataPath
    {

        /// <summary>
        /// 数据缓存数据库(在线书架，历史纪录)
        /// </summary>
        public const string AppCacheDbName = "AppCache.db";

        /// <summary>
        /// 本地缓存小说数据库
        /// </summary>
        public const string LocalBookDbName = "LoacalBook.db";

        /// <summary>
        /// 封面缓存文件夹
        /// </summary>
        public const string BookCoverFolderName = "Images";

        /// <summary>
        /// 数据库文件夹
        /// </summary>
        public const string DatabaseFolderName = "Database";


        public static string GetDbFolderPath()
        {
            var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DatabaseFolderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetBookCoverFolderPath()
        {
            var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, BookCoverFolderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public static string GetAppCacheDbPath()
        {
            var path = Path.Combine(GetDbFolderPath(), AppCacheDbName);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            return path;
        }

        public static string GetLocalBookDbPath()
        {
            var path = Path.Combine(GetDbFolderPath(), LocalBookDbName);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            return path;
        }



        public static string GetBookCoverPath(string bookid)
        {
            var path = Path.Combine(GetBookCoverFolderPath(), bookid + ".jpg");
            return !File.Exists(path) ? null : path;
        }

    }
}
