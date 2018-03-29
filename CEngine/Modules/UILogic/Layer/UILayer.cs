using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace CEngine
{
    //UI逻辑控制
    public class UILayer : BaseModel<IUIBehavior>
    {
        private Dictionary<string, IUIBehavior> datas = new Dictionary<string, IUIBehavior>();
        public List<IUIBehavior> behaviors;
        public IUIBehavior curr;
        public IUIBehavior next;
        public int index = 0;
        private readonly string rootName = "mainPanel";

        public bool isLoading
        {
            get { return next != null && !next.loader.isComplete; }
        }

        public bool haveLayers
        {
            get { return curr != null; }
        }

        public UILayer()
        {
            behaviors = base.GetAllData();
        }

        //界面注册
        public void Regist(UISetting setting, IUIBehavior behavior)
        {
            if (datas.ContainsKey(setting.uiName))
            {
                CDebug.LogError("UIManager.RegistLayout -> UI重复注册 " + setting.uiName);
                return;
            }

            datas.Add(setting.uiName, behavior);
        }

        public void Load(Callback onNavComplete, bool isNavBack = false)
        {
            Callback onload = new Callback(() =>
            {
                //CDebug.Log("UILayer.Load -> load ui successed " + next.uiName + " isNavBack " + isNavBack);
                if (curr != null)
                {
                    //退出事件待处理
                    //Debug.LogError("!!! nextUI " + next.uiName + " " + next.setting.preAction + " "+ isNavBack);
                    if (!isNavBack)
                    {
                        curr.exitEvent = (UIExitEvent)Enum.Parse(typeof(UIExitEvent), next.setting.preAction);
                        if (curr.panel != null)
                        {
                            curr.SetExitEvent(next.setting.uiName);
                            curr.OnExit();
                        }
                    }
                }

                curr = next;
                base.Update(curr);
                curr.Open();

                if (onNavComplete != null)
                {
                    onNavComplete();
                    onNavComplete = null;
                }

                Sort();
            });

            next.Load(onload);
        }

        /// <summary>
        /// 导航到下一个界面
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="callback"></param>
        public void NavTo(string uiName, Callback onNavComplete = null, bool isNavBack = false)
        {
            //CDebug.Log("UILayer.NavTo ->  uiName " + uiName);
            var final = GetUIBehavior(uiName);
            if (final == null)
            {
                CDebug.LogError("uiName is not define in ui.tsv");
                return;
            }

            if (isLoading)
            {
                if (final.Equals(next))
                    return;
                else
                    next.Dispose();
            }

            if (final.Equals(curr))
            {
                //CDebug.Log(layoutName + " " + curr.uiName);
                if (onNavComplete != null)
                {
                    onNavComplete();
                    onNavComplete = null;
                }
                return;
            }

            //回滚到目标界面
            if (behaviors.Contains(final))
            {
                //Debug.Log(" behaviors.Contains " + final.uiName);
                int index = behaviors.FindIndex((item) => { return final.Equals(item); });
                for (int i = behaviors.Count - 1; i > index; i--)
                {
                    if (behaviors[i].panel != null)
                    {
                        behaviors[i].exitEvent = UIExitEvent.destroy;
                        behaviors[i].OnExit();
                    }

                    behaviors.RemoveAt(i);
                }
            }

            next = final;
            Load(onNavComplete, isNavBack);

        }

        /// <summary>
        /// 回到上个界面
        /// </summary>
        public virtual void NavBack(Callback onNavComplete)
        {
            //CDebug.Log("UIManager.NavBack -> " + next == null ? "null" : next.uiName);
            int count = behaviors.Count;
            if (count <= 1)
                return;

            var final = behaviors[count - 2];
            NavTo(final.setting.uiName, onNavComplete, true);

        }

        private void Sort()
        {
            if (behaviors.Count <= 1)
                return;

            if (behaviors[0].setting.uiName.Equals(rootName))
                return;

            int index = behaviors.FindIndex((item) =>
            {
                return item.uiName.Equals(rootName);
            });

            if (index == -1)
                return;

            var root = behaviors[index];

            List<IUIBehavior> tmps = new List<IUIBehavior>();
            tmps.Add(root);
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (behaviors[i].Equals(root))
                    continue;

                if (i < index)
                    continue;

                tmps.Add(behaviors[i]);
            }

            this.behaviors = base._datas = tmps;

        }

        private IUIBehavior GetUIBehavior(string key)
        {
            if (datas.ContainsKey(key))
                return datas[key];

            return null;
        }

        public void Update()
        {
            if (isLoading)
                return;

            for (int i = 0; i < behaviors.Count; i++)
            {
                var ui = behaviors[i];
                if (ui == null)
                    continue;

                if (!ui.isEnable)
                    continue;

                if (!ui.setting.needUpdate)
                    continue;

                ui.Update();
            }

        }

        public void FixedUpdate()
        {
            if (isLoading)
                return;

            for (int i = 0; i < behaviors.Count; i++)
            {
                var ui = behaviors[i];
                if (ui == null)
                    continue;

                if (!ui.isEnable)
                    continue;

                if (!ui.setting.needUpdate)
                    continue;

                ui.FixedUpdate();
            }
        }

        public void SetOrder(GameObject panel)
        {
            if (curr != null && curr.setting.uiName == LayoutName.mainPanel)
                index = 0;

            Canvas canvas = null;
            GraphicRaycaster raycaster = null;

            canvas = panel.GetComponent<Canvas>();
            raycaster = panel.GetComponent<GraphicRaycaster>();

            if (canvas == null)
                canvas = panel.AddComponent<Canvas>();

            if (raycaster == null)
                raycaster = panel.AddComponent<GraphicRaycaster>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = this.index + curr.GetOrder();

            //CDebug.Log("set order " + canvas.sortingOrder + " " + this.index + " uiName " + curr.uiName);

            Renderer[] renders = panel.GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renders)
            {
                render.sortingOrder = this.index;
            }

        }

        public bool IsMaskInUse()
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                var ui = behaviors[i];
                if (!ui.setting.needMask)
                    continue;

                if (ui.panel != null && ui.view.gameObject.activeInHierarchy)
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            index = 0;
            curr = null;
            next = null;
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnExit();
            }
            behaviors.Clear();
            datas.Clear();
        }
    }

}



