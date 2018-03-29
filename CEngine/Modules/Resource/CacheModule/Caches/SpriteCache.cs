using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CEngine
{
    public class SpriteCache : BaseCache<SpriteRefCounter>,ICache<Sprite>
    {
        public readonly static SpriteCache Instance = new SpriteCache();

        public SpriteCache()
        {
            Init();
        }

        public void AddCache(string key, Sprite sprite)
        {
           if(!base.Contains(key))
                base.Add(key, new SpriteRefCounter(sprite));

            IRefCounter counter = base.Get(key);
            counter.Retain();
            //CDebug.LogError("SpriteCache Retain key " + key + " ref " + counter.refCount);
        }

        public Sprite GetCache(string key)
        {
            if (!base.Contains(key))
                return null;

            SpriteRefCounter counter = base.Get(key);
            counter.Retain();
            //CDebug.LogError("SpriteCache Retain key " + key + " ref " + counter.refCount);

            return counter.sprite;
        }


        public void UnloadCache(string key)
        {
            if (!base.Contains(key))
                return;

            IRefCounter counter = base.Get(key);
            counter.Release();
            //CDebug.LogError("SpriteCache Release key " + key + " ref " + counter.refCount);
            
            if (counter.refCount == 0)
            {
                base.Remove(key);
            }
        }
    }

    public sealed class SpriteRefCounter : RefCounter
    {
        public SpriteRefCounter(Sprite sprite) 
        {
            this.sprite = sprite;  
        }

        public Sprite sprite;

        public override void OnZeroRef()
        {
            base.OnZeroRef();

            if (sprite != null)
                Object.Destroy(sprite);
        }

    }
}

