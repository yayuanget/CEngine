using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CEngine
{
    public class Launcher
    {
        public static readonly Launcher instance = new Launcher();
        public TaskManager taskManager;
        public UIManager uiManager;

        private TimeSpan loginTimeSpan;

        public Launcher()
        {
            loginTimeSpan = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(8 * 3600);
            CDebug.LogWarning("Launcher " + loginTimeSpan.TotalSeconds);
        }

        public void StartUp(Transform uiRoot, Transform mask)
        {
            taskManager = TaskManager.instance;
            uiManager = UIManager.instance;
            uiManager.Init(uiRoot, mask);
        }

        public void Update()
        {
          
            taskManager.Update();
            uiManager.Update();
        }

        public void FixedUpdate()
        {
            uiManager.FixedUpdate();
        }

        public void Dispose()
        {
            uiManager.Dispose();
            taskManager.Dispose();
            TextureLoader.Instance.Dispose();
            GameObjectLoader.Instance.Dispose();
        }

      
    }
}