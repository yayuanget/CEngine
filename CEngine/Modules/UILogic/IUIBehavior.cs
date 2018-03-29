using System;
using UnityEngine;

namespace CEngine
{
    public interface IUIBehavior : IEquatable<IUIBehavior>
    {
        string uiName { get; set; }
        Transform view { get; set; }
        Transform panel { get; set; }
        UILoader loader { get; set; }
        UILayer uiLayer { get; set; }
        UISetting setting { get; set; }
        bool isEnable { get; }
        UIExitEvent exitEvent { get; set; }

        int GetOrder(); //次序
        void Load(Callback onload);
        void Awake();
        void Open();
        void Start();
        void OnShow();
        void AddListenter();
        void RemoveListenter();
        void Update();
        void FixedUpdate();
        void HardwareEsc();
        void OnClick(GameObject obj);
        void OnLock();
        void OnHide();
        void OnExit();
        void Dispose();
        //设置退出事件 在下个界面加载完时
        void SetExitEvent(string nextUIName);

    }
}

