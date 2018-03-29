using UnityEngine;
using System.Collections;
using System;
using System.Threading;

namespace CEngine
{
    /// <summary>
    /// 加载本地文件
    /// </summary>
    public sealed class FrameTask : TaskBase<DataSet>
    {
        public FrameTask(string url)
        {
            this.url = url;
            this.timeOut = 5;
            usedTime = 0;
        }

        public FrameTask(string url, Action<DataSet> callback)
        {
            this.url = url;
            this.OnCompleteEvent = callback;
            this.timeOut = 5;
            usedTime = 0;
        }

        public FrameTask(string url, string flag, Action<DataSet> callback)
        {
            this.url = url;
            this.OnCompleteEvent = callback;
            this.flag = flag;
            this.timeOut = 5;
            usedTime = 0;
        }

        public override void Load()
        {
            //CDebug.LogError(" framtask BeginLoad " + url);

            resource = new DataSet();
            //var thread = new Thread(delegate ()
            //{
            resource.bytes = LoadFromCacheDirect(url);
            OnComplete();
            //    ThreadCallback callback = new ThreadCallback();
            //    callback.name = url;
            //    callback.callback = () => { OnComplete(); };
            //    ThreadCallbackManager.GetInstance().Add(callback);
            //});

            //thread.IsBackground = true;
            //thread.Start();


        }

        public override bool Execute()
        {
            return true;
        }

        private static object obj = new object();
        public byte[] LoadFromCacheDirect(string path)
        {

            lock (obj)
            {
                path = path.Replace("\\", "/");
                //CDebug.Log("LoadFromCacheDirect " + path);
                try
                //  if (System.IO.File.Exists(path))
                {
                    using (System.IO.Stream s = System.IO.File.OpenRead(path))
                    {
                        byte[] b = new byte[s.Length];
                        //CDebug.Log(b.Length);
                        s.Read(b, 0, (int)s.Length);
                        s.Close();
                        //if (path.Contains(".ab"))
                        return base.Decrypt(b);
                        //else
                        //    return b;
                    }
                }
                catch (Exception e)
                {
                    CDebug.LogError("---------unExit " + path + " " + e.ToString());
                    return null;
                }
            }

        }

        public override void Dispose()
        {
            //CDebug.Log("task dispose --------------");
            resource.bytes = null;
        }

        public override void OnComplete()
        {
            //CDebug.LogError("loadFinish " + url + " resource is null ?  " + (resource == null));
            if (resource != null)
            {
                BytesCache.Instance.Add(url, resource.bytes);
                //CDebug.LogError("BytesCache .add " + url);
            }

            if (resource == null)
                resource = new DataSet(BytesCache.Instance.Get(url));

            base.OnComplete();
        }
    }
}
