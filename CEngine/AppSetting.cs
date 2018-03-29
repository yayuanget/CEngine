using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CEngine
{
    public class AppSetting
    {
        public static int appId = 100;
        public static Platform platform = Platform.UNITY_EDITOR;
        private static string downloadurl;

        public static bool isRemote
        {
            get { return ConfigManager.instance.GetBoolValue("isRemote"); }
        }

        public static bool localLua
        {
            get { return ConfigManager.instance.GetBoolValue("localLua"); }
        }

        /// <summary>
        /// 子游戏名称
        /// </summary>
        public static string chessName
        {
            get { return ConfigManager.instance.GetValue(AppSetting.appId.ToString()); }
        }

        public static string vercachePath
        {
            get
            {
                if (Application.isPlaying && isRemote)
                    return downloadurl;
                return localDataPath; //Application.streamingAssetsPath;

            }
            set
            {
                downloadurl = value;
            }
        }

        public static string rootPath
        {
            get
            {
                if (isRemote)
                    return persistentDataPath;

                return SourceDataPath;//localDataPath

            }
        }

        //public static string persistentDataPath
        //{
        //    get
        //    {
        //        string perDataPath = null;
        //        if (appId == 100)
        //            perDataPath = Application.persistentDataPath;
        //        else
        //            perDataPath = System.IO.Path.Combine(System.IO.Path.Combine(Application.persistentDataPath, "Chess"), ConfigManager.instance.GetValue(appId.ToString()));
        //        return perDataPath;
        //    }
        //}

        //改成指定目录加载
        public static string persistentDataPath
        {
            get
            {
                string rootpath = System.IO.Path.GetDirectoryName(Application.dataPath);
                //string localpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(rootpath), "PPgameCenter");
                string localpath = System.IO.Path.Combine(rootpath, "PPgameCenter");
                string perDataPath = null;
                if (appId == 100)
                    perDataPath = localpath;
                else
                    perDataPath = System.IO.Path.Combine(System.IO.Path.Combine(localpath, "Chess"), ConfigManager.instance.GetValue(appId.ToString()));
                return perDataPath;
            }
        }

        public static string localDataPath
        {
            get
            {
                string rootpath = System.IO.Path.GetDirectoryName(Application.dataPath);
                string localpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(rootpath), "LocalData");//"LocalData"
                string outpath = null;
                if (appId == 100)
                    outpath = localpath;
                else
                    outpath = System.IO.Path.Combine(System.IO.Path.Combine(localpath, "Chess"), ConfigManager.instance.GetValue(appId.ToString()));
                return outpath;
            }
        }

        public static string SourceDataPath
        {
            get
            {
                string rootpath = System.IO.Path.GetDirectoryName(Application.dataPath);
                string outpath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(rootpath), "SourceData");
                return outpath;
            }
        }

    }

    public enum Platform
    {
        UNITY_EDITOR = 0,
        UNITY_STANDALONE_WIN,
        UNITY_ANDROID,
        UNITY_IPHONE
    }
}
