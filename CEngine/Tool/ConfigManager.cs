using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CEngine
{

    public class ConfigManager
    {
        public readonly static ConfigManager instance = new ConfigManager();

        private TextAsset configData;
        private Dictionary<string, string> configs;
        private string path = "Version/Config";
        private string resourcePath;
        private string fullPath;
        private bool isExist
        {
            get
            {
                CDebug.Log("Config fullPath " + fullPath);
                return System.IO.File.Exists(fullPath);
            }
        }

        public ConfigManager()
        {
            resourcePath = System.IO.Path.Combine(Application.dataPath, "Resources");
            fullPath = System.IO.Path.Combine(resourcePath, path + ".txt");
            configs = new Dictionary<string, string>();
        }

        public void SetConfig(string key, string value)
        {
            if (!configs.ContainsKey(key))
                configs.Add(key, "");

            configs[key] = value;
        }

        public string GetValue(string key)
        {
            if (!configs.ContainsKey(key))
            {
                CDebug.LogError("get config error key not found " + key);
                return "";
            }

            return configs[key];
        }

        public bool GetBoolValue(string key)
        {
            if (!configs.ContainsKey(key))
            {
                CDebug.LogError("get config error key not found " + key);
                return false;
            }

            return bool.Parse(configs[key]);
        }

        public void Parse()
        {
            if (AppSetting.platform == Platform.UNITY_EDITOR)
            {
                if (!isExist)
                    return;
            }

            configData = TaskManager.LoadInLocal<TextAsset>(path);
            CDebug.Log("Parse configData " + configData.text);
            string[] strs = configData.text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < strs.Length; i++)
            {
                if (string.IsNullOrEmpty(strs[i]) || strs[i].Contains("--"))
                    continue;

                CDebug.Log("config index " + i + " " + strs[i]);
                string[] tmp = strs[i].Split(':');
                if (!configs.ContainsKey(tmp[0]))
                {
                    configs.Add(tmp[0], tmp[1]);
                }

            }
        }

        public void Save()
        {
            string tmp = "";
            var enumrator = configs.GetEnumerator();
            while (enumrator.MoveNext())
            {
                string configStr = enumrator.Current.Key + ":" + enumrator.Current.Value + "\r\n";
                tmp += configStr;
            }

            CDebug.Log("ConfigManager.Save " + tmp);
            System.IO.File.WriteAllText(fullPath, tmp);
        }

    }
}
