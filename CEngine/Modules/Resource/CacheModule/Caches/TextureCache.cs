using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CEngine
{
    public class TextureCache : BaseCache<TextureRefCounter>,ICache<Texture2D>
    {
        public readonly static TextureCache Instance = new TextureCache();

        public TextureCache()
        {
            Init();
        }

        public void AddCache(string key, Texture2D texture2d)
        {
            if (!base.Contains(key))
                base.Add(key, new TextureRefCounter(texture2d));

            IRefCounter counter = base.Get(key);
            counter.Retain();
            //CDebug.LogError("TextureCache Retain key " + key + " ref " + counter.refCount);
        }

        public Texture2D GetCache(string key)
        {
            if (!base.Contains(key))
                return null;

            TextureRefCounter counter = base.Get(key);
            counter.Retain();
            //CDebug.LogError("TextureCache Retain key " + key + " ref " + counter.refCount);

            return counter.texture2d;
        }

        public void UnloadCache(string key)
        {
            if (!base.Contains(key))
                return;

            IRefCounter counter = base.Get(key);
            counter.Release();
            //CDebug.LogError("TextureCache Release key " + key + " ref " + counter.refCount);

            if (counter.refCount == 0)
            {
                base.Remove(key);
            }
        }
    }

    public sealed class TextureRefCounter : RefCounter
    {
        public TextureRefCounter(Texture2D texture2d)
        {
            this.texture2d = texture2d;
        }

        public Texture2D texture2d;

        public override void OnZeroRef()
        {
            base.OnZeroRef();

            if (texture2d != null)
                Object.Destroy(texture2d);
        }

    }
}

