
using UnityEngine;
namespace CEngine
{
    /// <summary>
    /// 视图代理类的基础类
    /// </summary>
    public class ViewMediator : MonoBehaviour
    {
        /// <summary>
        /// 登录视图组件的gameobject对象
        /// </summary>
        public GameObject view;
        void Awake()
        {
            this.dataInit();
            this.addListeners();
        }

        void OnDestroy()
        {
            this.removeListeners();
        }

        void Start()
        {
            this.viewInit();
        }

        void OnEnable()
        {
            //this.viewInit();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            pauseHandle(pauseStatus);
        }

        /// <summary>
        /// 视图内部数据的初始化
        /// </summary>
        virtual internal void dataInit() { }

        /// <summary>
        /// 视图初始化
        /// </summary>
        virtual internal void viewInit() { }

        /// <summary>
        /// 添加登录视图所关心的所有事件的监听
        /// </summary>
        virtual internal void addListeners() { }

        virtual internal void removeListeners() { }

        virtual internal void pauseHandle(bool pauseStatus) { }


        /// <summary>
        /// 关闭视图 方便直接作为关闭事件的监听回调
        /// </summary>
        virtual internal void do_close_handler()
        {
            view.SetActive(false);
        }

        /// <summary>
        /// 打开视图 方便直接作为关闭事件的监听回调
        /// </summary>
        virtual internal void do_open_handler()
        {
            view.SetActive(true);
        }

    }
}

