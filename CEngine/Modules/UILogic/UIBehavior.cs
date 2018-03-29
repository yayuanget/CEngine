using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using CEngine.Tool;

namespace CEngine
{
    public class UIBehavior : IUIBehavior
    {
        public Transform view { get; set; }
        public Transform panel { get; set; }
        public UILoader loader { get; set; }
        public UILayer uiLayer { get; set; }
        public UISetting setting { get; set; }
        public UIExitEvent exitEvent { get; set; }

        public bool isEnable
        {
            get { return view != null && view.gameObject.activeInHierarchy; }
        }

        public string uiName { get { return setting.uiName; } set { } }

        private bool isLock;
        //打开界面调用一次onShow()
        private bool isOpened;

        //卸载任务列表
        public List<Action> unloadTask = new List<Action>();

        public UIBehavior() { }

        public UIBehavior(UISetting setting)
        {
            this.setting = setting;
            loader = new UILoader(this);
            uiLayer = UIManager.instance.GetLayer(setting.uiLayer);
        }

        private void SetMask()
        {
            GameObject mask = UIManager.instance.mask.gameObject;

            if (mask == null)
                return;

            bool inUse = uiLayer.IsMaskInUse();

            CDebug.Log("SetMask mask inUse " + inUse);
            if (!setting.needMask && !inUse)
            {
                mask.SetActive(false);
                return;
            }
            else
            {
                if (setting.needMask)
                {
                    var canvas = mask.GetComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = uiLayer.index + GetOrder() - 5;
                }
                mask.transform.SetSiblingIndex(panel.transform.GetSiblingIndex() - 1);
                mask.SetActive(true);
            }

            CDebug.Log("SetMask index " + uiLayer.index);
        }

        public int GetOrder()
        {
            int order = 0;
            switch (setting.uiLayer)
            {
                case 0:
                    order = 0;
                    break;
                case 1:
                    order = uiLayer.index + 50;
                    break;
                case 2:
                    order = uiLayer.index + 100;
                    break;
                default:
                    break;
            }

            return order;
        }

        public void Load(Callback onload)
        {
            loader.Load(onload);
        }

        public void Open()
        {
            //CDebug.Log("UIMould.Open -> panel GetInstanceID " + this.panel.GetInstanceID());
            Start();
            if (!isOpened)
            {
                isOpened = true;
                OnShow();
                AddListenter();
            }
        }

        public void OnHide()
        {
            for (int i = 0; i < unloadTask.Count; i++)
            {
                unloadTask[i].Invoke();
            }
            unloadTask.Clear();

            isOpened = false;
            RemoveListenter();
            if (panel != null)
                panel.gameObject.SetActive(false);
        }

        public virtual void Awake()
        {
            uiLayer.index += 5;
            CDebug.LogError("uiLayer.index " + uiLayer.index + " uiName " + uiName);
            CDebug.Log("UIMould Awake " + setting.uiName );
        }

        /// <summary>
        /// 添加监听事件
        /// </summary>
        public virtual void AddListenter()
        {

        }

        /// <summary>
        /// 移除监听事件
        /// </summary>
        public virtual void RemoveListenter()
        {

        }

        /// <summary>
        /// 每次进入界面都会调用
        /// </summary>
        public virtual void Start()
        {
            isLock = false;
            panel.gameObject.SetActive(true);
            //if (setting.uiLayer == 1)

            uiLayer.SetOrder(panel.gameObject);
            SetMask();
        }

        /// <summary>
        /// 界面第一次打开调用一次
        /// </summary>
        public virtual void OnShow()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }

        public void ImageGray(Image img, bool isGray)
        {
            if (img == null)
            {
                CDebug.LogError("UIMould Error ImageGray img is null ");
                return;
            }
            if (img.material == null || !img.material.shader.name.Equals("UI/Default Gray"))
            {
                Shader grayMat = Shader.Find("UI/Default Gray");
                if (grayMat == null)
                {
                    CDebug.LogError("UIMould Error can not found shader UI/Default Gray ");
                    return;
                }
                img.material = new Material(grayMat);
                unloadTask.Add(() => { GameObject.Destroy(img.material); });
            }

            img.material.SetFloat("_Gray", isGray ? 1 : 0);
        }

        /// <summary>
        /// 硬件返回
        /// </summary>
        public virtual void HardwareEsc()
        {

        }

        //处理点击事件
        public virtual void OnClick(GameObject go)
        {

        }

        public virtual void OnPressed(GameObject go)
        {
            if (uiLayer.isLoading)
                return;
        }

        public virtual void OnPressReleased(GameObject go)
        {
            if (uiLayer.isLoading)
                return;
        }

        //处理 OnToggleValueChange事件 
        public virtual void OnToggleValueChange(GameObject go, bool isOn)
        {
            if (uiLayer.isLoading)
                return;

        }

        //添加点击事件
        public void AddClickEvent(object _go)
        {
            AddClick(_go);
        }

        //添加点击事件
        public void AddClickEvent(object _go, string tag)
        {
            CDebug.Log("UIMould AddClickEvent " + (_go == null) + " tag " + tag);
            AddClick(_go);
        }



        /// <summary>
        /// 自动获取点击的对象 GameObject or MonoBehaviour or Transform
        /// </summary>
        /// <param name="_go"></param>
        /// <returns></returns>
        public GameObject GetClickObject(object _go)
        {
            GameObject go;
            go = _go as GameObject;
            if (go == null)
            {
                MonoBehaviour mono = _go as MonoBehaviour;
                if (mono != null)
                    go = mono.gameObject;
            }

            if (go == null)
            {
                Transform tran = _go as Transform;
                if (tran != null)
                    go = tran.gameObject;
            }

            return go;
        }

        /// <summary>
        /// 添加点击事件
        /// </summary>
        /// <param name="_go"></param>
        public virtual void AddClick(object _go)
        {
            GameObject go = GetClickObject(_go);

            if (go == null)
            {
                CDebug.Log("UIMould.AddClickEvent -> AddClickEventError go is null");
                return;
            }

            if (go.GetComponent<Button>())
            {
                //CDebug.Log("UIMould.AddClickEvent -> go.name " + go.name);
                Button btn = go.GetComponent<Button>();
                btn.onClick.AddListener(() => { OnClick(go); });
                unloadTask.Add(() => { btn.onClick.RemoveAllListeners(); });
            }
            else if (go.GetComponent<Toggle>())
            {
                Toggle toggle = go.GetComponent<Toggle>();

                toggle.onValueChanged.AddListener((isOn) =>
                {
                    OnToggleValueChange(go, isOn);
                });
                unloadTask.Add(() => { toggle.onValueChanged.RemoveAllListeners(); });
            }
            else
            {
                CDebug.Log("UIMould.AddClickEvent -> Error go is null");
                return;
            }
        }


        //加载贴图
        public void AddTextureTask(string path, Image image)
        {
            m_AddTextureTask(path, image, false);
        }

        public void AddTextureTask(string path, Image image, bool nativeSize)
        {
            m_AddTextureTask(path, image, nativeSize);
        }

        public void AddEffectTask(string path, Action<GameObject> callback)
        {
            GameObjectLoader.Instance.Load(setting.uiName, path, (go) => { callback(go); });
        }

        //卸载
        public virtual void Dispose()
        {
            uiLayer.index -= 5;
            //CDebug.Log("Dispose uiLayer.index " + uiLayer.index + " uiName " + uiName);
            isOpened = false;
            panel = null;

            TaskManager.instance.Dispose(setting.uiName);
            TextureLoader.Instance.Dispose(setting.uiName);
            GameObjectLoader.Instance.Dispose(setting.uiName);
            
            loader.Dispose();
            
        }

        //若UIAction为Lock 执行该方法
        public virtual void OnLock()
        {

            isLock = true;
        }

        //界面退出时
        public virtual void OnExit()
        {
            //Debug.LogError(uiName + " -- " + exitEvent);
            switch (exitEvent)
            {
                case UIExitEvent.hide:
                    OnHide();
                    break;
                case UIExitEvent.destroy:
                    OnHide();
                    Dispose();
                    break;
                case UIExitEvent.lockui:
                    OnLock();
                    break;

                default:
                    break;
            }
        }

        //当前UI退出前 根据下个ui名字决定退出动作
        public virtual void SetExitEvent(string nextName)
        {

        }

        public bool Equals(IUIBehavior other)
        {
            if (other == null)
                return false;

            return setting.uiName.Equals(other.setting.uiName);
        }

        private void m_AddTextureTask(string path, Image image, bool nativeSize)
        {
            if (image == null)
            {
                CDebug.LogError("UIMould m_AddTextureTask image is null ");
                return;
            }

            if (image.sprite != null && image.mainTexture != null && image.mainTexture.name == path)
            {
                //CDebug.LogError("AddTextureTask return ");
                return;
            }

            unloadTask.Add(new Action(() =>
            {
                if (image != null)
                {
                    image.sprite = null;
                    Color c = image.color;
                    c.a = 0;
                    image.color = c;
                }
            }));

            Callback<Sprite> onload = (sprite) =>
            {
                image.sprite = sprite;
                image.mainTexture.name = path;
                if (nativeSize)
                    image.SetNativeSize();
                Color color = new Color(image.color.r, image.color.g, image.color.b, 1);
                image.color = Color.Lerp(image.color, color, 1f);
            };

            TextureLoader.Instance.Load(setting.uiName, path, onload);
        }


    }
}






