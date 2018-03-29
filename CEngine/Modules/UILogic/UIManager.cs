using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CEngine
{
    public class UIManager
    {
        public readonly static UIManager instance = new UIManager();

        public Transform uiRoot;
        public Transform mask;
        public UIBaseLayer uiBaseLayer;
        public UIOverLayer uiOverLayer;
        public UIGuideLayer uiGuideLayer;

        public void Init(Transform uiRoot, Transform mask)
        {
            this.uiRoot = uiRoot;
            this.mask = mask;
            uiBaseLayer = UIBaseLayer.instance;
            uiOverLayer = UIOverLayer.instance;
            uiGuideLayer = UIGuideLayer.instance;
        }

        public void Update()
        {
            uiBaseLayer.Update();
            uiOverLayer.Update();
            uiGuideLayer.Update();

            HardwareEsc();
        }

        public void FixedUpdate()
        {
            uiBaseLayer.FixedUpdate();
            uiOverLayer.FixedUpdate();
            uiGuideLayer.FixedUpdate();
        }

        public void NavTo(string layoutName, Callback onNavComplete = null)
        {
            CDebug.Log("UIManager.NavTo " + layoutName);
            if (!UISettings.instance.Contains(layoutName))
            {
                CDebug.LogError("UI.tsv not contains layout " + layoutName);
                return;
            }

            UISetting setting = UISettings.instance.Get(layoutName);
            UILayer layer = GetLayer(setting.uiLayer);
            layer.NavTo(layoutName, onNavComplete);
        }

        public void NavBack()
        {
            if (uiOverLayer.haveLayers)
                uiOverLayer.NavBack(null);
            else
            {
                uiBaseLayer.NavBack(null);
                uiGuideLayer.NavBackAll();
            }
                
        }

        public void NavBackAllOverLayer()
        {
            UIOverLayer.instance.NavBackAll();
        }

        public UILayer GetLayer(int layer)
        {
            switch (layer)
            {
                case 0:
                    return uiBaseLayer;
                case 1:
                    return uiOverLayer;
                case 2:
                    return uiGuideLayer;
                default:
                    break;
            }

            return uiBaseLayer;
        }

        public bool IsMaskInUse()
        {
            if (uiGuideLayer.IsMaskInUse() || uiOverLayer.IsMaskInUse() || uiBaseLayer.IsMaskInUse())
                return true;

            return false;
        }

        public void HardwareEsc()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (uiOverLayer.haveLayers)
                    uiOverLayer.curr.HardwareEsc();
                else
                {
                    uiBaseLayer.curr.HardwareEsc();
                    uiGuideLayer.NavBackAll();
                }
                    
            }
        }

        public void Dispose()
        {
            uiBaseLayer.Dispose();
            uiOverLayer.Dispose();
            uiGuideLayer.Dispose();
        }

    }

}
