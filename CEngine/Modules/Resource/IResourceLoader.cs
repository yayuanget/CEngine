using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CEngine
{
    interface IResourceLoader<T>
    {
        string m_path { get;  }
        Dictionary<string, List<Callback>> unloadTask { get; set; }
        void Load(string tag, string sourceName,  Callback<T> callback);
        void AddToUnloadTask(string tag, Callback callback);
        void Dispose(string key);
        void Dispose(); 
    }
}
