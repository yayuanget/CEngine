using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CEngine
{
    public class SoundCache : BaseCache<SoundRefCounter>,ICache<AudioClip>
    {
        public readonly static SoundCache Instance = new SoundCache();

        public SoundCache()
        {
            Init();
        }

        public void AddCache(string key, AudioClip clip)
        {
            if (!base.Contains(key))
                base.Add(key, new SoundRefCounter(clip));

            IRefCounter counter = base.Get(key);
            counter.Retain();
        }

        public AudioClip GetCache(string key)
        {
            if (!base.Contains(key))
                return null;

            SoundRefCounter counter = base.Get(key);
            counter.Retain();

            return counter.clip;
        }

        public void UnloadCache(string key)
        {
            if (!base.Contains(key))
                return;

            IRefCounter counter = base.Get(key);
            counter.Release();

            if (counter.refCount == 0)
            {
                base.Remove(key);
            }
        }
    }

    public sealed class SoundRefCounter :RefCounter
    {
        public SoundRefCounter(AudioClip clip)
        {
            this.clip = clip;
        }

        public AudioClip clip;

        public override void OnZeroRef()
        {
            base.OnZeroRef();

            if (clip != null)
            {
                Object.Destroy(clip);
            }
        }
    }
}


