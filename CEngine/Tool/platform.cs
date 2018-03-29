using System;
using System.Collections.Generic;
using System.Text;

namespace CEngine
{
    public class platform
    {
        public static string platformString
        {
            get
            {
                switch (AppSetting.platform)
                {
                    case Platform.UNITY_EDITOR:
                    case Platform.UNITY_STANDALONE_WIN:
                        return "win";
                    case Platform.UNITY_ANDROID:
                        return "android";
                    case Platform.UNITY_IPHONE:
                        return "ios";
                    default:
                        return "unknown";
                }
            }

        }

        public static string platformStr
        {
            get
            {
                string str = "";
                if (AppSetting.appId != 100)
                    str = platformString + "_" + ConfigManager.instance.GetValue(AppSetting.appId.ToString());
                else
                    str = platformString;
                return str;
            }
        }
        public static string platformStringInEditor
        {
            get { return "win"; }
        }
    }
}

