using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CEngine
{
    public class WWWCache : BaseCache<WWW>
    {
        public readonly static WWWCache Instance = new WWWCache();

        public override void Release()
        {
            var enumrator = pool.GetEnumerator();
            while(enumrator.MoveNext())
            {
                var value = enumrator.Current.Value;
                if (value == null)
                    continue;
                value.Dispose();
                CDebug.Log("wwwCache dispose enumrator.Current.key " + enumrator.Current.Key);
            }
            base.Release();
        }
    }

    public class BytesCache : BaseCache<byte[]>
    {
        public readonly static BytesCache Instance = new BytesCache();

        public override void Release()
        {
            var enumrator = pool.GetEnumerator();
            while (enumrator.MoveNext())
            {
                var value = enumrator.Current.Value;
                if (value == null)
                    continue;

                CDebug.Log("bytesCache dispose enumrator.Current.key " + enumrator.Current.Key);
                value = null;
            }
            base.Release();
        }
    }

  

    public class MainSenceCache : BaseCache<GameObject>
    {
        public readonly static MainSenceCache Instance = new MainSenceCache();
    }

    public class FontCache : BaseCache<AssetBundle>
    {
        public readonly static FontCache Instance = new FontCache();
    }

    public class GameObjectCache : BaseCache<List<GameObject>>
    {
        public readonly static GameObjectCache Instance = new GameObjectCache();

        public GameObjectCache()
        {
            Init();
        }
        public void Add(string key, GameObject obj)
        {
            key = key.ToLower();
            var list = Get(key);
            if (list == null)
            {
                list = new List<GameObject>();
                list.Add(obj);
                Add(key, list);
            }
            else
            {
                list.Add(obj);
                Update(key, list);
            }
        }

        public override void Remove(string url)
        {
            List<GameObject> objs = null;
            url = url.ToLower();
            if (!pool.TryGetValue(url, out objs)) return;
            pool.Remove(url);

            if (objs != null)
            {
                //CDebug.Log(objs.Count);
                for (int i = 0; i < objs.Count; i++)
                {
                    GameObject.Destroy(objs[i]);
                }
                objs = null;
            }

        }

        public GameObject GetCanUse(string url)
        {
            GameObject res = null;
            List<GameObject> objs = null;
            url = url.ToLower();
            //CDebug.Log(url );
            if (!pool.TryGetValue(url, out objs)) return null;
            // pool.Remove(url);
            //CDebug.Log(objs);
            if (objs != null)
            {
                for (int i = 0; i < objs.Count; i++)
                {
                    if (objs[i] != null && !objs[i].activeInHierarchy)
                        res = objs[i];
                }
            }
            return res;
        }
    }

    

}
