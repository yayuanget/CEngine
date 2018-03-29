using UnityEngine;
using System.Collections;

namespace CEngine
{
    public class LayoutName
    {
        public static readonly string loginPanel = "loginPanel";
        public static readonly string mainPanel = "mainPanel";
        public static readonly string gamePanel = "gamePanel";
    }

    //UI行为
    public enum UIExitEvent
    {
        none = 0,
        hide,
        destroy,
        lockui
    }
}

