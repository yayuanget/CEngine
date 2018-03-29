using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CEngine
{

    public class AssetBundleCache : BaseCache<ABRefCounter> ,ICache<AssetBundle>
    {
        public readonly static AssetBundleCache Instance = new AssetBundleCache();

        public AssetBundleCache()
        {
            Init();
        }

        public void AddCache(string key, AssetBundle bundle)
        {
            if (!base.Contains(key))
                base.Add(key, new ABRefCounter(bundle));

            IRefCounter counter = base.Get(key);
            counter.Retain();

            //CDebug.LogError("tex " + key + " add ref " + base.Get(key).refCnt);
        }

        /// <summary>
        /// 取assetbundle缓存 并增加引用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AssetBundle GetCache(string key)
        {
            if (!base.Contains(key))
                return null;

            ABRefCounter counter = base.Get(key) ;
            counter.Retain();

            return counter.bundle;

        }

        public void UnloadCache(string key)
        {
            if (!base.Contains(key))
                return;

            IRefCounter counter = base.Get(key);
            counter.Release();

            if(counter.refCount == 0)
            {
                base.Remove(key);
            }
        }

        
        
    }

    public sealed class ABRefCounter : RefCounter
    {
        public ABRefCounter(AssetBundle bundle)
        {
            this.bundle = bundle;
        }

        public AssetBundle bundle;

        public override void OnZeroRef()
        {
            base.OnZeroRef();

            if (bundle != null)
                bundle.Unload(true);
        }
    }
}
