using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CEngine
{
    public interface IRefCounter
    {
        int refCount { get; }

        void Retain();
        void Release();

        void OnZeroRef();
    }

    public interface ICache<T>
    {
        void AddCache(string key, T obj);
        T GetCache(string key);
        void UnloadCache(string key);

    }
}
