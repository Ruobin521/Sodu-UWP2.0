using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Core.Util;
using Sodu.ViewModel;

namespace Sodu.Service
{
    public enum SettingKey
    {
        UserName,
        IsAutoAddToOnlineShelf,
        IsAutoUpdateLocalShelf,
        IsDownloadOnWaan,

        //字体大小 
        FontSize,
        //字体颜色
        TextColor,
        //背景颜色
        ContentBackground,
        //阅读模式（日间，夜间）
        ReadContentMode,
        //横屏
        IsHorizontal,
        //行高
        LineHeight,
        //亮度
        LightValue,

    }

    public class AppSettingService
    {
        private const string ContainerName = "SudoSettings";

        public static void SetKeyValue(SettingKey key, object value)
        {
            SettingHelper.SetContainerValue(ContainerName, key.ToString(), value?.ToString());
        }


        public static object GetKeyValue(SettingKey key)
        {
            var value = SettingHelper.GetValueByContainer(ContainerName, key.ToString());
            return value;
        }

        public static string GetUserId()
        {
            var value = SettingHelper.GetValueByContainer(ContainerName, SettingKey.UserName.ToString());
            return value?.ToString();
        }


    }
}
