using UnityEngine;
using System.Collections;
using System;

namespace CEngine
{

    /// <summary>
    /// www 的下载任务
    /// </summary>
    public sealed class WWWTask : TaskBase<DataSet>
    {
        public WWWTask(string url)
        {
            this.url = url;
            this.timeOut = 5;
            usedTime = 0;
        }

        public WWWTask(string url, Action<DataSet> callback)
        {
            this.url = url;
            this.OnCompleteEvent = callback;
            this.timeOut = 5;
            usedTime = 0;
        }

        public WWWTask(string url, string flag, Action<DataSet> callback)
        {
            this.url = url;
            this.OnCompleteEvent = callback;
            this.flag = flag;
            this.timeOut = 5;
            usedTime = 0;
        }

        public override void Load()
        {
            resource = new DataSet();
            resource.www = new WWW(GetUrl());
            //从本地加载要加 "file:///"
            preProgress = 0;
            //CDebug.Log("----------BeginLoad " + load_url);
        }

        private float preProgress;
        public override bool Execute()
        {
            if (WWWCache.Instance.Contains(url))
            {
                //CDebug.LogError(" WWWCache contains onfinish  " + url);
                resource = new DataSet(WWWCache.Instance.Get(url));
                OnComplete();
                return true;
            }

            if (!string.IsNullOrEmpty(resource.www.error))
                CDebug.LogError(resource.www.error + " url " + url);

            if (!resource.www.isDone)
            {
                //CDebug.LogError(" check is down url " + url + " progress " + resource.www.progress);

                if (resource.www.progress == preProgress)
                {
                    preProgress = resource.www.progress;
                    usedTime += Time.deltaTime;
                    if (base.IsTimeout())
                    {
                        CDebug.LogError("task timeout " + usedTime + " url " + url + " retry " + currTry);
                        base.ReTry();
                    }
                    //CDebug.Log("check time out " + usedTime);
                }

                return false;
            }
            else
            {

                //CDebug.Log("downTask  load down " + url);
                //if (url.Contains(".ab"))
                if (url.Contains("allRes.byte"))
                    resource.bytes = resource.www.bytes;
                else
                    resource.bytes = base.Decrypt(resource.www.bytes);
                //else
                //    resource.bytes = resource.www.bytes;
                OnComplete();
                return true;
            }


        }

        public override void Dispose()
        {
            resource.www.Dispose();
            if (resource.www.texture != null)
            {
                UnityEngine.Object.Destroy(resource.www.texture);
            }
        }

        public override void OnComplete()
        {
            //CDebug.Log("loadFinish url " + url);
            if (resource != null)
            {
                WWWCache.Instance.Add(url, resource.www);
                //CDebug.Log("WWWCache .add " + url);
            }

            base.OnComplete();
        }

        private string GetUrl()
        {
            string load_url = "";
            switch (AppSetting.platform)
            {
                case Platform.UNITY_EDITOR:
                case Platform.UNITY_STANDALONE_WIN:
                    load_url = "file:///" + url;
                    break;
                case Platform.UNITY_ANDROID:
                    load_url = url;
                    break;
                case Platform.UNITY_IPHONE://IOS下未验证
                    break;
                default:
                    break;
            }

            return load_url;
        }

    }
}