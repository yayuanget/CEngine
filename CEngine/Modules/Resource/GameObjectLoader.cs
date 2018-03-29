using CEngine.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CEngine
{
    public class GameObjectLoader : IResourceLoader<GameObject>
    {
        public static readonly GameObjectLoader Instance = new GameObjectLoader();
        public string m_path { get { return ""; } }
        public Dictionary<string, List<Callback>> unloadTask { get; set; }

        public GameObjectLoader()
        {
            unloadTask = new Dictionary<string, List<Callback>>();
        }

        public void Load(string tag, string sourceName, Callback<GameObject> callback)
        {
            var path = sourceName;
            //若是小游戏
            if (AppSetting.appId != 100)
                path = ConfigManager.instance.GetValue(AppSetting.appId.ToString()).ToLower() + "/" + path;

            GameObject go = null;
            go = GameObjectCache.Instance.GetCanUse(tag + path + ".ab");
            if (go != null)
            {
                callback(go);
                return;
            }

            Callback<GameObject> onload = (obj) =>
            {
                go = GameObject.Instantiate(obj);
                GameObjectCache.Instance.Add(tag + path + ".ab", go);

                Callback unload = () =>
                {
                    GameObjectCache.Instance.Remove(tag + path + ".ab");
                };

                AddToUnloadTask(tag, unload);

                if (go != null && callback != null)
                {
                    callback(go);
                    callback = null;
                }

            };

            if (!AppSetting.isRemote)
                LoadInternal(path, onload);
            else
                LoadRemote(path, onload, (task) =>
                {
                    AddToUnloadTask(tag, () => { task.UnLoad(); });
                });
        }

        public void AddToUnloadTask(string tag, Callback callback)
        {
            if (!unloadTask.ContainsKey(tag))
                unloadTask.Add(tag, new List<Callback>());

            unloadTask[tag].Add(callback);
        }

        private void LoadInternal(string path, Callback<GameObject> onload)
        {
            GameObject go = ResourceLoadInLocal<GameObject>(path);
            if (go == null)
            {
                CDebug.LogError("error resource not found local path " + path);
                return;
            }

            if (onload != null)
            {
                onload(go);
                onload = null;
            }

        }

        private void LoadRemote(string path, Callback<GameObject> onload, Callback<AssetBundleTask> getTask)
        {
            AssetBundleTask task = new AssetBundleTask(path, (ab) =>
            {
                if (ab == null)
                {
                    CDebug.LogError("error resource not found (ab is null) remote path " + path);
                    return;
                }

                object mainAsset = ab.LoadAsset("assets/resources/" + path + ".prefab");
                GameObject go = mainAsset as GameObject;

                if (go == null)
                {
                    CDebug.LogError("error resource not found (go is null) remote path " + path);
                    return;
                }

                if (onload != null)
                {
                    onload(go);
                    onload = null;
                }

                //object mainAsset = ab.LoadAssetAsync("assets/resources/" + path + ".prefab");
                //AppFacade.InitGame.StartCoroutine(LoadAssetAsyncCoroutine(ab, "assets/resources/" + path + ".prefab", (asset) =>
                //{
                //    if (asset != null)
                //       finish(asset);
                //}));
            });
            AssetBundleLoad.Instance.Load(task);

            if (getTask != null)
                getTask(task);
        }



        static IEnumerator LoadAssetAsyncCoroutine(AssetBundle ab, string name, Action<GameObject> callback)
        {
            AssetBundleRequest request = ab.LoadAssetAsync(name);

            // 等待加载完成
            while (!request.isDone)
            {
                yield return false;
            }
            var obj = request.asset as GameObject;
            callback(obj);   // 回调上层
        }


        public T ResourceLoadInLocal<T>(string path)
        {
            object obj = Resources.Load(path);
            return (T)obj;
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
