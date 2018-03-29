using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CEngine
{
    /// <summary>
    /// 界面加载器
    /// </summary>
    public class UILoader
    {
        public IUIBehavior behavior;
        public bool isComplete;

        public UILoader(UIBehavior behavior)
        {
            this.behavior = behavior;
        }

        public virtual void Load(Callback onNavComplete)
        {
            CDebug.Log("UILoader.Load -> uiName " + behavior.setting.uiName + " panel == null  " + (behavior.panel == null));
            if (behavior.panel != null)
            {
                isComplete = true;
                if (onNavComplete != null)
                {
                    onNavComplete();
                    onNavComplete = null;
                }
            }
            else
            {
                isComplete = false;

                string path = "";
                path = AppSetting.chessName + "UIPrefab/" + behavior.setting.uiName;
                CDebug.Log("UILoader.Load path with chess -> " + path);
                Callback<GameObject> onload = new Callback<GameObject>((go) =>
                {
                    OnLoadComplete(go);
                    if (onNavComplete != null)
                    {
                        onNavComplete();
                        onNavComplete = null;
                    }
                });

                GameObjectLoader.Instance.Load(behavior.setting.uiName, path, onload);
            }

        }

        private void OnLoadComplete(GameObject obj)
        {
            CDebug.Log("UILoader.DoFinish -> viewName " + behavior.setting.uiName);
            obj.transform.parent = UIManager.instance.uiRoot;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            var rt = obj.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, 0);
            //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
            //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            //rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);

            obj.name = behavior.setting.uiName;
            behavior.panel = obj.transform;
            behavior.view = behavior.panel.Find("view");

            //CDebug.Log("UILoader.DoFinish -> panel GetInstanceID " + uiMould.panel.GetInstanceID());
            //CDebug.Log("UILoader.DoFinish -> view GetInstanceID " + uiMould.view.GetInstanceID());
            //state = LayoutState.loadDown;
            //uiMould.Open();

            isComplete = true;
            behavior.Awake();

        }

        /// <summary>
        /// 卸载该界面的依赖项
        /// </summary>
        public virtual void Dispose()
        {
            CDebug.Log("UILoader.Dispose -> uiloader " + behavior.setting.uiName);
            GameObjectLoader.Instance.Dispose(behavior.setting.uiName);
        }

    }
}