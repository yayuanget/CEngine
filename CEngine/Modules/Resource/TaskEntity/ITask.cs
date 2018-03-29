using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
namespace CEngine
{

    public interface ITask
    {
        string url { get; set; }
        string flag { get; set; }
        bool isComplete { get; set; }
        bool isFailed { get; set; }

        void Load();
        void ReTry();
        bool IsTimeout();
        bool Execute();
        void OnComplete();
        void Dispose();

    }

    public class TaskBase<T> : ITask, IEquatable<ITask>
    {
        public T resource;
        public const int reTry = 3;
        public int currTry;
        public float timeOut;
        public float usedTime;

        private string m_url;
        public string url { get { return m_url; } set { m_url = value; } }

        public string m_flag;
        public string flag { get { return m_flag; } set { m_flag = value; } }

        public bool isComplete { get; set; }
        public bool isFailed { get; set; }

        public Action<T> OnCompleteEvent;

        public virtual void Load()
        {
            isFailed = false;
            isComplete = false;
            CDebug.Log("BeginLoad " + url);
        }

        public virtual void ReTry()
        {
            Dispose();
            usedTime = 0;

            if (currTry < reTry)
            {
                CDebug.LogError("reTry " + currTry + " url " + url);
                currTry++;
                Load();
            }
            else
            {
                isFailed = true;
                CDebug.LogError("资源下载失败 " + currTry);

            }

        }

        //true 执行完毕
        public virtual bool Execute()
        {
            return false;
        }

        public virtual bool IsTimeout()
        {
            return usedTime > timeOut;
        }

        public virtual void Dispose()
        {

        }

        public virtual void OnComplete()
        {
            isComplete = true;
            if (OnCompleteEvent == null)
                return;

            OnCompleteEvent(resource);
            OnCompleteEvent = null;
        }

        public bool Equals(ITask other)
        {
            return other.url.Equals(this.url);
        }

        public Byte[] Decrypt(byte[] bytes)
        {
            Byte[] bs = bytes;
            if (AppSetting.isRemote)
                StringEncryption.Encypt(ref bs);
            return bs;
        }
    }


    public class DataSet
    {
        public WWW www;
        public byte[] bytes;

        public DataSet() { }

        public DataSet(WWW www)
        {
            this.www = www;
            bytes = www.bytes;
        }

        public DataSet(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public void Dispose()
        {
            this.bytes = null;
            if (www != null)
                www.Dispose();
        }

    }
}