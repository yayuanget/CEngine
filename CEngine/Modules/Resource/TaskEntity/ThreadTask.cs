using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Threading;

namespace CEngine
{
    /// <summary>
    /// Thread 的下载任务
    /// </summary>
    public sealed class ThreadTask : TaskBase<DataSet>
    {
        public ThreadTask(string url)
        {
            this.url = url;
            this.timeOut = 5;
            usedTime = 0;
        }

        public ThreadTask(string url, Action<DataSet> callback)
        {
            this.url = url;
            this.OnCompleteEvent = callback;
            this.timeOut = 5;
            usedTime = 0;
        }

        public ThreadTask(string url, string flag, string savePath, Action<DataSet> callback)
        {
            this.url = url;
            this.OnCompleteEvent = callback;
            this.flag = flag;
            this.timeOut = 5;
            this.savePath = savePath;
            usedTime = 0;
        }

        private string savePath = "";
        object locked = new object();
        private Thread thread;
        string dataPath = "";
        bool startLoad;
        bool isCompleted;
        int progressPercent;
        WebClient client;
        public override void Load()
        {
            client = new WebClient();
            progressPercent = 0;
            isCompleted = false;
            startLoad = false;
            dataPath = Application.dataPath;
            thread = new Thread(DoLoad);
            thread.Start();

        }

        private void DoLoad()
        {

            while (!startLoad)
            {
                lock (locked)
                {
                    startLoad = true;
                    try
                    {
                        //Load  
                        CDebug.Log("do load " + url);
                        client.Proxy = null;
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                        client.DownloadFileAsync(new System.Uri(url), savePath);
                        client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Completed);
                        CDebug.Log("savePath " + savePath);
                    }
                    catch (System.Exception ex)
                    {
                        CDebug.LogError(ex.Message);
                    }

                }
                Thread.Sleep(1);
            }

            CDebug.Log("ok ");
        }

        private void Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            CDebug.Log("Completed 1111" + url + " progressPercent " + progressPercent);
            if (progressPercent < 100)
            {
                thread.Abort();
                isCompleted = false;
            }
            else
                isCompleted = true;



        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            usedTime = 0;
            progressPercent = e.ProgressPercentage;
            //UnityEngine.CDebug.Log("e.ProgressPercentage " + e.ProgressPercentage + "   " + url);
            /*
            UnityEngine.CDebug.Log(string.Format("{0} MB's / {1} MB's",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00")));
            */
            //float value = (float)e.ProgressPercentage / 100f;

            //CDebug.Log("e.BytesReceived " + e.BytesReceived);

            //string value = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            //if (m_SyncEvent != null) m_SyncEvent(data);

            //if (e.ProgressPercentage == 100 && e.BytesReceived == e.TotalBytesToReceive)
            //{
            //    sw.Reset();

            //    data = new NotiData(NotiConst.UPDATE_DOWNLOAD, currDownFile);
            //    if (m_SyncEvent != null) m_SyncEvent(data);
            //}
        }

        public override bool Execute()
        {
            if (!isCompleted)
            {
                //if (progressPercent == 0)
                //{
                usedTime += Time.deltaTime;
                if (base.IsTimeout())
                {
                    CDebug.LogError("task timeout " + usedTime + " url " + url + " retry " + currTry);
                    ReTry();
                }
                //CDebug.Log("check time out " + usedTime);
                //}

            }
            else
            {
                CDebug.Log("thread load finish " + url);
                OnComplete();
            }

            return isCompleted;
        }

        public override void Dispose()
        {
            CDebug.LogError("thread Dispose " + url);
            thread.Abort();
            client.CancelAsync();
            client.Proxy = null;
            client.DownloadFileCompleted -= new System.ComponentModel.AsyncCompletedEventHandler(Completed);
            client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(ProgressChanged);
        }

        public override void OnComplete()
        {
            CDebug.Log("thread WebClient OnComplete ");
            base.OnComplete();
        }


    }
}