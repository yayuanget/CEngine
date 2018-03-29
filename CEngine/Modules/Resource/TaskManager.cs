using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CEngine
{
    public class TaskManager
    {
        public readonly static TaskManager instance = new TaskManager();

        public const string FRAMETASK = "FRAMETASK";
        public const string DOWNTASK = "DOWNTASK";
        public const string THREADTASK = "THREADTASK";

        private List<ITask> tasks;
        private List<ITask> runner;
        private Dictionary<string, List<ITask>> delayCalls;
        private int parallels;//任务并行数

        private bool haveTask { get { return tasks.Count > 0; } }
        private bool lessThanParallels { get { return runner.Count < parallels; } }

        public TaskManager()
        {
            parallels = 100;
            tasks = new List<ITask>();
            runner = new List<ITask>();
            delayCalls = new Dictionary<string, List<ITask>>();
        }

        //本地资源路径 加载的时候以这个为准
        public Dictionary<string, LoadPath> localResPaths = new Dictionary<string, LoadPath>();

        public bool ContainsPath(string path)
        {
            return localResPaths.ContainsKey(path);
        }

        public LoadPath GetPath(string path)
        {
            if (!AppSetting.isRemote)
                return new LoadPath(AppSetting.rootPath, path);

            if (!localResPaths.ContainsKey(path))
            {
                CDebug.LogError("TextureLoader GetLoadPath -> Error path is not Contains in localResPaths " + path);
                return null;
            }

            return localResPaths[path];
        }

        #region  同步方式加载资源
        //<summary>
        //载入素材 从AssetBundle
        //</summary>
        public static AssetBundle LoadBundleInLocal(string url)
        {
            byte[] stream = null;
            AssetBundle bundle = null;
            CDebug.Log("LoadBundle url " + url);
            stream = File.ReadAllBytes(url);
            bundle = AssetBundle.LoadFromMemory(stream); //关联数据的素材绑定
            return bundle;
        }

        public static Texture LoadTextureInLocal(string url)
        {
            byte[] stream = null;
            CDebug.Log("LoadTexture url " + url);
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            stream = File.ReadAllBytes(url);
            tex.LoadImage(stream);

            return tex;
        }

        public static byte[] LoadFromCacheDirect(string path)
        {
            //CDebug.Log(path);
            using (System.IO.Stream s = System.IO.File.OpenRead(path))
            {
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                //s.Close();
                return b;
            }
        }
        #endregion

        //异步加载资源


        /// <summary>
        /// 向资源管理器中添加一个任务
        /// </summary>
        private void AddTask(ITask task)
        {
            //CDebug.Log("AddDownTask" + task.url + " " + BytesCache.Instance.Contains(task.url));

            if (BytesCache.Instance.Contains(task.url))
            {
                //CDebug.Log("ResourceManager.AddDownTask -> BytesCache.Instance.Contains " + task.url);
                task.OnComplete();
                return;
            }
            else if (WWWCache.Instance.Contains(task.url))
            {
                //CDebug.Log("ResourceManager.AddDownTask -> WWWCache.Instance.Contains " + task.url);
                task.OnComplete();
                return;
            }

            if (tasks.Contains(task))
            {
                //task.finish(null);
                //task = tasks[tasks.IndexOf(task)];
                if (!delayCalls.ContainsKey(task.url))
                    delayCalls.Add(task.url, new List<ITask>());

                delayCalls[task.url].Add(task);
                //CDebug.LogError("this task is in loading " + task.url + " delayCalls cnt " + delayCalls.Count);
                return;
            }


            //CDebug.Log("add downtask " + task.flag + " " + tasks.Contains(task));
            tasks.Add(task);
            //CDebug.Log("AddDownTask " + task.url);
        }

        /// <summary>
        /// 向资源管理器中添加一个任务
        /// </summary>
        public void AddTask(LoadPath loadPath, string flag, Action<DataSet> finish)
        {
            ITask task;

            if (loadPath.usewww)
                task = new WWWTask(loadPath.path, flag, finish);
            else
                task = new FrameTask(loadPath.path, flag, finish);

            AddTask(task);
        }

        public void Update()
        {
            while (haveTask && lessThanParallels)
            {
                ITask task = tasks[0];

                // CDebug.Log("runner add " + dt.flag + " " + dt.url); 
                runner.Add(task);
                task.Load();
                tasks.RemoveAt(0);
                //CDebug.Log(tasks.Count + "  ");
            }


            //CDebug.Log("runner.Count " + runner.Count);
            for (int i = 0; i < runner.Count; i++)
            {
                ITask task = runner[i];
                if (task.isFailed)
                {
                    CDebug.LogError("ResourceManager.Update -> error task.isFailed " + task.url);
                    continue;
                }

                if (task.Execute())
                {
                    ExecuteDeleyCalls(task);
                }
            }

            //CDebug.Log(tasks.Count + " " + runner.Count + " " + delayCalls.Count);

        }

        /// <summary>
        /// 处理重复加载的任务
        /// </summary>
        public void ExecuteDeleyCalls(ITask task)
        {
            if (delayCalls.ContainsKey(task.url))
            {
                var delays = delayCalls[task.url];
                for (int i = 0; i < delays.Count; i++)
                {
                    delays[i].OnComplete();
                }
                delays.Clear();
                delayCalls.Remove(task.url);

            }
            runner.Remove(task);
        }

        /// <summary>
        /// 对正在进行的 flag 任务进行中断
        /// </summary>
        /// <param name="flag"></param>
        public void Dispose(string flag)
        {
            CDebug.Log("ResourceManager.Dispose -> flag " + flag);
            for (int i = runner.Count - 1; i >= 0; i--)
            {
                if (runner[i].flag.Equals(flag))
                {
                    runner[i].Dispose();
                    runner.RemoveAt(i);
                }

            }

            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                if (tasks[i].flag.Equals(flag))
                {
                    if (!tasks[i].isComplete)
                        tasks.RemoveAt(i);

                    CDebug.Log(tasks.Count + " ----- ");
                }
            }

            List<string> needRemove = new List<string>();
            var enumrator = delayCalls.GetEnumerator();
            while (enumrator.MoveNext())
            {
                var curr = enumrator.Current;
                if (curr.Value.Count == 0)
                    continue;

                if (curr.Value[0].flag.Equals(flag))
                {
                    needRemove.Add(curr.Key);
                    //CDebug.Log("needRemove add " + flag);
                }

            }

            for (int i = needRemove.Count - 1; i >= 0; i--)
            {
                delayCalls.Remove(needRemove[i]);
                //CDebug.Log("delayCalls remove " + needRemove[i]);
            }

            CDebug.Log(tasks.Count + " " + runner.Count + " " + delayCalls.Count + " " + flag);//
        }

        /// <summary>
        /// 载入素材 从本地Recource中
        /// </summary>
        public static T LoadInLocal<T>(string path)
        {
            object t = Resources.Load(path);
            return (T)t;
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        public void Dispose()
        {
            Dispose(THREADTASK);
            CDebug.Log("ResourceManager.OnDestroy -> ~ResourceManager was destroy!");
        }
    }
}




