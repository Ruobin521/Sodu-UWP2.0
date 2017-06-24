using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Sodu.Core.Config;

namespace Sodu.Core.Util
{
    public class DownloadCoverHelper
    {
        public static async  void DownloadCover(string folder,string imageName,string url)
        {
            try
            {
                var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder);
                StorageFile file = null;
                if (!File.Exists(imageName))
                {
                    file = await storageFolder.CreateFileAsync(imageName, CreationCollisionOption.ReplaceExisting);
                }
                file = await storageFolder.GetFileAsync(imageName);
                using (var stream = await file.OpenReadAsync())
                {
                    if (stream.Size <= 0)
                    {
                        var tmpfile = await storageFolder.CreateFileAsync(imageName, CreationCollisionOption.ReplaceExisting);
                        var http = new HttpClient();
                        var data = await http.GetByteArrayAsync(url);
                        await FileIO.WriteBytesAsync(tmpfile, data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

    }
}
