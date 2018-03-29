using CEngine.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace CEngine
{
    public class SettingLoader : IResourceLoader<string>
    {
        public static readonly SettingLoader Instance = new SettingLoader();
        public string m_path { get { return "setting/"; } }
        public Dictionary<string, List<Callback>> unloadTask { get; set; }

        public SettingLoader()
        {
            unloadTask = new Dictionary<string, List<Callback>>();
        }

        public void Load(string tag, string sourceName, Callback<string> callback)
        {
            var path = m_path + sourceName;
            var loadPath = TaskManager.instance.GetPath(path);

            if (StringCache.Instance.Contains(loadPath.path))
            {
                var str = StringCache.Instance.Get(loadPath.path);

                if (callback != null)
                {
                    callback(str);
                    callback = null;
                }
                return;
            }

            Callback<DataSet> onload = (data) =>
            {
                OnLoad(tag, loadPath, data, callback);
            };

            TaskManager.instance.AddTask(loadPath, "SettingLoader", (data) => { onload(data); });
        }

        public void OnLoad(string tag, LoadPath loadPath, DataSet data, Callback<string> callback)
        {
            CDebug.Log("load texture success " + loadPath.path);
            string str = "";
            if (StringCache.Instance.Contains(loadPath.path))
                str = StringCache.Instance.Get(loadPath.path);
            else
            {
                str = ByteConvert.BytesToString(data.bytes);
                StringCache.Instance.Add(loadPath.path, str);
            }

            Callback unload = () =>
            {
                str = "";
            };

            AddToUnloadTask(tag, unload);

            if (callback != null)
            {
                callback(str);
                callback = null;
            }
        }

        public void AddToUnloadTask(string tag, Callback callback)
        {
            if (!unloadTask.ContainsKey(tag))
                unloadTask.Add(tag, new List<Callback>());

            unloadTask[tag].Add(callback);
        }

        public void Dispose(string tag)
        {
            if (!unloadTask.ContainsKey(tag))
                return;

            var tasks = unloadTask[tag];
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Invoke();
            }

            tasks.Clear();
        }

        public void Dispose()
        {
            var enumrator = unloadTask.GetEnumerator();
            while (enumrator.MoveNext())
            {
                var tag = enumrator.Current.Key;
                Dispose(tag);
            }
            unloadTask.Clear();
        }
    }
}
