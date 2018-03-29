using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
namespace CEngine
{

    public class AssetBundleLoad
    {
        public static AssetBundleLoad Instance = new AssetBundleLoad();

        private Queue<AssetBundleTask> tasks = new Queue<AssetBundleTask>();
        private Queue<AssetBundleTask> unloadtasks = new Queue<AssetBundleTask>();
        public Action OnFinish;

        private bool start; //开始任务
        public int total = 0; //任务总数
        public int curr = 0; //当前完成数

        public void Start()
        {
            start = true;
        }

        public void Reset()
        {
            start = false;
            total = 0;
            curr = 0;
            tasks.Clear();
            OnFinish = null;
            unloadtasks.Clear();
        }

        public void AddTask(AssetBundleTask task)
        {
            if (tasks.Contains(task))
                return;

            tasks.Enqueue(task);
        }

        public void DoLoad()
        {
            if (!start)
                return;
            //开启所有异步任务
            Begin();
            //检测结束
            Checking();
        }


        private void Checking()
        {
            if (!Check())
                return;

            if (OnFinish == null)
                return;

            OnFinish();
            OnFinish = null;

            //Reset();

        }

        private bool Check()
        {
            return start && (curr == total);
        }

        private void Begin()
        {
            while (tasks.Count > 0)
            {
                AssetBundleTask task = tasks.Dequeue();
                unloadtasks.Enqueue(task);
                task.Begin();
            }
        }

        private void UnLoad()
        {
            while (unloadtasks.Count > 0)
            {
                unloadtasks.Dequeue().UnLoad();
            }
            Reset();
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        /// <param name="abName">Resources下相对路径</param>
        public void Load(string abName, Action<AssetBundle> OnFinish)
        {
            AssetBundleTask task = new AssetBundleTask(abName, OnFinish);
            task.Begin();

        }
        //public static int finishNum = 0;
        //int requestNum = 0;
        //public static int unloadNum = 0;
        public void Load(AssetBundleTask task)
        {
            //requestNum++;
            //CDebug.LogError("requestNum " + requestNum);
            //AssetBundleTask task = new AssetBundleTask(abName, OnFinish);
            task.Begin();

        }
    }

    public class AssetBundleTask : IEquatable<AssetBundleTask>
    {
        public AssetBundle mainFirst;
        public List<string> depends;
        public AssetBundle cache;
        public GameObject cacheObject;
        //Resources下相对路径
        public string abName;
        public Action<AssetBundle> OnFinish;
        public bool isFinished;

        public string resourceRoot
        {
            get
            {
                return platform.platformStr + "/";
            }
        }

        public LoadPath mainFirstPath
        {
            get
            {
                //return resourceRoot + platform.platformString + ".ab";
                // "file:///"
                if (!AppSetting.isRemote)
                {
                    return new LoadPath(AppSetting.rootPath, resourceRoot + platform.platformStr + ".ab");
                }
                else
                {
                    return TaskManager.instance.localResPaths[platform.platformStr + "/" + platform.platformStr + ".ab"];
                }
                    
            }
        }

        public string flag = "";

        TaskManager resMgr;

        public AssetBundleTask(string abName, Action<AssetBundle> OnFinish)
        {
            //CDebug.Log("new AssetBundleTask " + abName);
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeStamp = Convert.ToInt64(ts.TotalSeconds).ToString();

            flag = abName + "_ts_" + timeStamp + "_" + Guid.NewGuid();
            // CDebug.Log("new AssetBundleTask  " + flag);
            this.OnFinish = OnFinish;
            this.abName = abName.ToLower() + ".ab";
            resMgr = TaskManager.instance;
        }

        //public void Init(string abName, Action<AssetBundle> OnFinish)
        //{
        //    this.OnFinish = OnFinish;
        //    this.abName = abName;
        //    resMgr = InitGame.resourceManager;

        //}

        public void Begin()
        {
            isFinished = false;
            LoadMainFirst();
        }

        private void LoadMainFirst()
        {
            if (AssetBundleCache.Instance.Contains(mainFirstPath.path))
            {
                mainFirst = AssetBundleCache.Instance.GetCache(mainFirstPath.path);
                if (mainFirst == null)
                    CDebug.LogError("mainFirst is null " + mainFirstPath);

                LoadDepends();
            }
            else
            {
                resMgr.AddTask(mainFirstPath, flag, (CEngine.DataSet data) =>
                {
                    if (AssetBundleCache.Instance.Contains(mainFirstPath.path))
                    {
                        mainFirst = AssetBundleCache.Instance.GetCache(mainFirstPath.path);
                    }
                    else
                    {
                        mainFirst = AssetBundle.LoadFromMemory(data.bytes);
                        AssetBundleCache.Instance.AddCache(mainFirstPath.path, mainFirst);
                    }

                    if (mainFirst == null)
                        CDebug.LogError("mainFirst is null " + mainFirstPath.path);

                    LoadDepends();

                });
            }


        }

        private void LoadDepends()
        {
            AssetBundleManifest manifest = (AssetBundleManifest)mainFirst.LoadAsset("AssetBundleManifest");
            //获取依赖文件列表;
            string[] dependNames = manifest.GetAllDependencies(abName);

            if (dependNames.Length == 0)
                LoadCache();

            //CDebug.Log("!!!!!!!!!!!!!!!!!!!!!!!dependNames.Length " + dependNames.Length);
            //foreach (var item in dependNames)
            //{
            //    CDebug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!dependName " + item);
            //}

            depends = new List<string>();

            for (int index = 0; index < dependNames.Length; index++)
            {
                //加载所有的依赖文件;
                //string path = resourceRoot + dependNames[index];
                LoadPath localPath;
                if (!AppSetting.isRemote)
                {
                    localPath = new LoadPath(AppSetting.rootPath, resourceRoot + dependNames[index]);
                }
                else
                {
                    //CDebug.Log(platform.platformStr + "/" + dependNames[index]);
                    localPath = TaskManager.instance.localResPaths[platform.platformStr + "/" + dependNames[index]];
                }


                //CDebug.LogError(dependNames[index]);

                if (AssetBundleCache.Instance.Contains(localPath.path))
                {
                    AssetBundleCache.Instance.GetCache(localPath.path);
                    depends.Add(localPath.path);
                    if (depends.Count == dependNames.Length)
                    {
                        //CDebug.Log("load depends finished 1" + abName);
                        LoadCache();
                    }
                }
                else
                {
                    resMgr.AddTask(localPath, flag, (CEngine.DataSet data) =>
                    {
                        if (AssetBundleCache.Instance.Contains(localPath.path))
                        {
                            AssetBundleCache.Instance.GetCache(localPath.path);
                            depends.Add(localPath.path);
                            if (depends.Count == dependNames.Length)
                            {
                                //CDebug.Log("load depends finished 2" + abName);
                                LoadCache();
                            }
                        }
                        else
                        {
                            AssetBundle ab = AssetBundle.LoadFromMemory(data.bytes);
                            depends.Add(localPath.path);
                            AssetBundleCache.Instance.AddCache(localPath.path, ab);
                            if (depends.Count == dependNames.Length)
                            {
                                //CDebug.Log("load depends finished 3" + abName);
                                LoadCache();
                            }
                        }

                    });
                }
            }


        }

        LoadPath cachePath;
        private void LoadCache()
        {
            //string path = resourceRoot + abName;
            //CDebug.Log(platform.platformStr + "/" + abName);

            if (!AppSetting.isRemote)
                cachePath = new LoadPath(AppSetting.rootPath, resourceRoot + abName);
            else
            {
                if (!TaskManager.instance.localResPaths.ContainsKey(platform.platformStr + "/" + abName))
                {
                    CDebug.LogError("---------- 资源不存在 " + platform.platformStr + "/" + abName);

                    return;
                }
                else
                    cachePath = TaskManager.instance.localResPaths[platform.platformStr + "/" + abName];
            }


            if (AssetBundleCache.Instance.Contains(cachePath.path))
            {
                cache = AssetBundleCache.Instance.GetCache(cachePath.path);
                LoadFinish();
            }
            else
            {
                resMgr.AddTask(cachePath, flag, (CEngine.DataSet bundle) =>
                {
                    //CDebug.Log(bundle == null);
                    //CDebug.Log("Path.GetFileNameWithoutExtension(abName) " + Path.GetFileNameWithoutExtension(abName));
                    if (AssetBundleCache.Instance.Contains(cachePath.path))
                    {
                        cache = AssetBundleCache.Instance.GetCache(cachePath.path);
                    }
                    else
                    {
                        cache = AssetBundle.LoadFromMemory(bundle.bytes);

                        AssetBundleCache.Instance.AddCache(cachePath.path, cache);

                        //CDebug.Log(cache == null);
                    }
                    LoadFinish();
                    //if (cache != null)
                    //{
                    //    cacheObject = cache.LoadAsset(Path.GetFileNameWithoutExtension(abName)) as GameObject;

                    //    CDebug.Log("load cache " + abName + " finished " + (cacheObject == null));
                    //    //Instantiate(cache);
                    //    //DateTime t2 = DateTime.Now;

                    //    //CDebug.Log(" Time2:" + (t2 - t1).TotalMilliseconds);//+ " Time3:" + (t3 - t2).TotalMilliseconds
                    //}
                });
            }


        }


        private void LoadFinish()
        {
            isFinished = true;
            //AssetBundleLoad.finishNum++;
            //CDebug.LogError( "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! abName " + abName + flag + " " + (OnFinish == null) + " " +(cache == null) );//"finishNum " + AssetBundleLoad.finishNum +
            if (OnFinish != null)
            {
                OnFinish.Invoke(cache);
                OnFinish = null;
            }
        }


        public void UnLoad()
        {
            OnFinish = null;
            resMgr.Dispose(flag);

            //AssetBundleCache.Instance.Unload(mainFirstPath);

            if (mainFirst != null)
            {
                AssetBundleManifest manifest = (AssetBundleManifest)mainFirst.LoadAsset("AssetBundleManifest");
                //CDebug.Log(manifest == null);

                //获取依赖文件列表;
                //string[] dependNames = manifest.GetAllDependencies(abName);
                if (depends != null)
                {
                    for (int i = 0; i < depends.Count; i++)
                    {
                        AssetBundleCache.Instance.UnloadCache(depends[i]);
                    }
                }

                if (cache != null)
                {
                    // AssetBundleLoad.unloadNum++;
                    AssetBundleCache.Instance.UnloadCache(cachePath.path);
                    // CDebug.LogError("unloadnum " + AssetBundleLoad.unloadNum);
                }

            }


        }

        public bool Equals(AssetBundleTask other)
        {
            return this.abName == other.abName;
        }
    }
}