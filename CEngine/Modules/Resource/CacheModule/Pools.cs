using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CEngine
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class CachePoolFactory
    {
        private static Dictionary<string, CacheBase> caches = new Dictionary<string, CacheBase>();
        
        /// <summary>
        /// 取得一个对象池 若没有则自动创建
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static CacheBase GetPool(string key)
        {
            if (!caches.ContainsKey(key))
                caches.Add(key, new CacheBase());

            return caches[key];
        }

        public static void Dispose()
        {
            var enumrator = caches.GetEnumerator();
            while (enumrator.MoveNext())
            {
                enumrator.Current.Value.Clear();
            }
        }
    }

    public class CacheBase
    {
        public List<GameObject> Pool;

        public CacheBase()
        {
            Init();
        }

        public void Init()
        {
            Pool = new List<GameObject>();
        }

        public virtual void Add(GameObject go)
        {
            Pool.Add(go);
        }

        public virtual void Remove(GameObject go)
        {
            Pool.Remove(go);
        }

        public virtual void RemoveLast()
        {
            Texture2D.Destroy(Pool[Pool.Count - 1]);
            Pool.RemoveAt(Pool.Count - 1);
            CDebug.Log("RemoveLast " + Pool.Count);
        }

        public virtual void Clear()
        {
            for (int i = Pool.Count - 1; i >= 0; i--)
            {
                if (Pool[i] != null)
                    Texture2D.Destroy(Pool[i]);
                Pool.RemoveAt(i);
            }
        }

    }
}
