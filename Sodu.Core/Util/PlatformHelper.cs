using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.Util
{
    public class PlatformHelper
    {
        public enum Platform
        {
            IsMobile,
            IsPc
        }

        public static Platform CurrentPlatform => GetPlatform();
        public static bool IsMobile => GetPlatform() == Platform.IsMobile;

        private static Platform GetPlatform()
        {
            var api = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (api.Equals("Windows.Desktop"))
            {
                return Platform.IsPc;
            }
            if (api.Equals("Windows.Mobile"))
            {
                return Platform.IsMobile;
            }
            return Platform.IsPc;
        }
    }
}
