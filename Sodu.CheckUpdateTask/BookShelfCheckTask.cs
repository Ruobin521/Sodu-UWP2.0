using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using Sodu.Core.Entity;
using Sodu.DataService;

namespace Sodu.CheckUpdateTask
{
    public sealed class BookShelfCheckTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            var reslult = await CheckUpdateOperation();

            if (reslult)
            {
                SendToast();
            }

            deferral.Complete();
        }

        private static IAsyncOperation<bool> CheckUpdateOperation()
        {
            return CheckUpdate().AsAsyncOperation();
        }

        private static async Task<bool> CheckUpdate()
        {
            bool reslut;
            try
            {
                var service = new OnlineBookShelfDataService();
                var onlineList = await service.GetOnlineData();
                var localList = service.GetCacheData();
                reslut = service.CompareWithLocalCache(onlineList, localList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                reslut = false;
            }
            return reslut;
        }

        public static void SendToast()
        {
            var t = Windows.UI.Notifications.ToastTemplateType.ToastImageAndText01;
            var content = Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(t);
            //需要using Windows.Data.Xml.Dom;  
            XmlNodeList xml = content.GetElementsByTagName("text");
            xml[0].AppendChild(content.CreateTextNode("您的在线书架有更新，别忘了追更哦~"));

            XmlNodeList image = content.GetElementsByTagName("image");
            ((XmlElement)image[0]).SetAttribute("src", "ms-appx:///Assets/Icon/head.png");


            //需要using Windows.UI.Notifications;  
            Windows.UI.Notifications.ToastNotification toast = new ToastNotification(content);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }



    }
}
