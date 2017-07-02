using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Sodu.Core.Config;

namespace Sodu.Core.Util
{
    public class DownloadCoverHelper
    {

        public static async void DownloadCover(string folder, string imageName, string url)
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Debug.WriteLine(url);
                if (File.Exists(Path.Combine(folder, imageName)))
                {
                    File.Delete(Path.Combine(folder, imageName));
                }
            }
        }


        public static async void SaveHttpImage(string folder, string fileName, string url,Action action = null)
        {
            try
            {
                Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();

                IBuffer buffer = await http.GetBufferAsync(new Uri(url));

                BitmapImage img = new BitmapImage();

                using (IRandomAccessStream stream = new InMemoryRandomAccessStream())

                {
                    await stream.WriteAsync(buffer);
                    stream.Seek(0);
                    await img.SetSourceAsync(stream);
                    var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder);
                    StorageFile file = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                    DataReader read = new DataReader(stream.GetInputStreamAt(0));

                    await read.LoadAsync((uint)stream.Size);

                    byte[] temp = new byte[stream.Size];

                    read.ReadBytes(temp);

                    await FileIO.WriteBytesAsync(file, temp);

                    action?.Invoke();
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Debug.WriteLine(url);
                if (File.Exists(Path.Combine(folder, fileName)))
                {
                    File.Delete(Path.Combine(folder, fileName));
                }
            }
        }
    }
}
