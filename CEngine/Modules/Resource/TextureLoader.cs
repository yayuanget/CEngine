using CEngine.Tool;
using System.Collections.Generic;
using UnityEngine;

namespace CEngine
{
    public class TextureLoader : IResourceLoader<Sprite>
    {
        public static readonly TextureLoader Instance = new TextureLoader();
        public string m_path { get { return "textures/"; } }
        public Dictionary<string, List<Callback>> unloadTask { get; set; }

        public TextureLoader()
        {
            unloadTask = new Dictionary<string, List<Callback>>();
        }

        public void Load(string tag, string sourceName, Callback<Sprite> callback)
        {
            var path = m_path + sourceName;
            var loadPath = TaskManager.instance.GetPath(path);

            if (SpriteCache.Instance.Contains(loadPath.path))
            {
                var sprite = SpriteCache.Instance.GetCache(loadPath.path);
                TextureCache.Instance.GetCache(loadPath.path);
                AddToUnloadTask(tag, () =>
                {
                    SpriteCache.Instance.UnloadCache(loadPath.path);
                    TextureCache.Instance.UnloadCache(loadPath.path);
                });

                if (callback != null)
                {
                    callback(sprite);
                    callback = null;
                }
                return;
            }

            Callback<DataSet> onload = (data) =>
            {
                OnLoad(tag, loadPath, data, callback);
            };

            TaskManager.instance.AddTask(loadPath, "TextureLoader", (data) => { onload(data); });
        }

        public void OnLoad(string tag, LoadPath loadPath, DataSet data, Callback<Sprite> callback)
        {
            CDebug.Log("load texture success " + loadPath.path);
            Texture2D texture2d = null;
            if (TextureCache.Instance.Contains(loadPath.path))
                texture2d = TextureCache.Instance.Get(loadPath.path).texture2d;
            else
                texture2d = ByteConvert.BytesToTexture2D(data.bytes);

            texture2d.wrapMode = TextureWrapMode.Clamp;
            Sprite sprite = ByteConvert.CreateImage(texture2d);
            TextureCache.Instance.AddCache(loadPath.path, texture2d);
            SpriteCache.Instance.AddCache(loadPath.path, sprite);

            Callback unload = () =>
            {
                TextureCache.Instance.UnloadCache(loadPath.path);
                SpriteCache.Instance.UnloadCache(loadPath.path);
            };

            AddToUnloadTask(tag, unload);

            if (callback != null)
            {
                callback(sprite);
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
