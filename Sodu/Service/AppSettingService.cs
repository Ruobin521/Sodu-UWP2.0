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
        //总共八种颜色可选，记录选中的index
        ContentColorIndex,
        //是否为夜间模式
        IsNightMode,
        //横屏
        IsLandscape,
        //行高
        LineHeight,
        //亮度
        LightValue,
        //阅读模式（分页动画，滚动）
        IsScroll,

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
