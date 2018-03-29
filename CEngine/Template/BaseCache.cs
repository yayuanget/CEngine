using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CEngine
{
    /// <summary>
    /// 对象池基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCache<T>
    {
        public Dictionary<string, T> pool = new Dictionary<string, T>();

        public virtual void Init()
        {

        }

        public virtual void Add(string key, T t)
        {
            if (pool.ContainsKey(key))
                return;

            pool.Add(key, t);
        }

        public virtual bool Update(string key, T t)
        {
            if (!pool.ContainsKey(key))
                return false;

            pool[key] = t;
            return true;
        }

        public virtual void Remove(string key)
        {
            if (!pool.ContainsKey(key))
                return;

            pool.Remove(key);
        }

        public virtual bool Contains(string key)
        {
            return pool.ContainsKey(key);
        }

        public virtual T Get(string key)
        {
            if (!pool.ContainsKey(key))
            {
                return default(T);
            }

            return pool[key];

        }

        /// <summary>
        /// 释放字典
        /// </summary>
        public virtual void Release()
        {
            pool.Clear();
        }
    }


    /// <summary>
    /// 引用计数
    /// </summary>
    public class RefCounter : IRefCounter
    {
        public RefCounter(){}

        public int refCount { get; private set; }

        public void Retain()
        {
            refCount++;
        }

        public void Release()
        {
            refCount--;
            if (refCount == 0)
            {
                OnZeroRef();
            }
        }

        public virtual void OnZeroRef()
        {
            
        }
    }
}
