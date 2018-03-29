using UnityEngine;
using System.Collections;
using System;
namespace CEngine
{
    public class SingletonBase<T> where T : class, new()
    {
        private static T s_instance = null;
        public static T Instance
        {
            get
            {
                if (SingletonBase<T>.s_instance == null)
                {
                    SingletonBase<T>.CreateInstance();
                }
                return s_instance;
            }
        }
        protected SingletonBase()
        {
        }

        public static void CreateInstance()
        {
            if (SingletonBase<T>.s_instance == null)
            {
                SingletonBase<T>.s_instance = Activator.CreateInstance<T>();
                (SingletonBase<T>.s_instance as SingletonBase<T>).Init();
            }
        }

        public static void DestroyInstance()
        {
            if (SingletonBase<T>.s_instance != null)
            {
                (SingletonBase<T>.s_instance as SingletonBase<T>).UnInit();
                SingletonBase<T>.s_instance = null;
            }
        }

        public static bool HasInstance() { return (s_instance != null); }

        public virtual void Init()
        {
        }

        public virtual void UnInit()
        {
        }

    }
}

