using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CEngine
{
    public class UIGuideLayer : UILayer
    {
        public static readonly UIGuideLayer instance = new UIGuideLayer();

        public UIGuideLayer()
        {

        }

        public override void NavBack(Callback onNavComplete)
        {
            //base.NavBack();
            int count = behaviors.Count;
            if (count <= 0)
                return;

            if (count == 1)
            {
                NavBackLast();
            }
            else
            {
                var final = behaviors[count - 2];

                CDebug.Log("UIManager.NavBack -> RemoveInvalid uiMoulds.RemoveAt " + next.setting.uiName + " " + behaviors.Count);
                base.NavTo(final.setting.uiName);
            }
        }

        public void NavBackAll()
        {
            if (behaviors.Count <= 0)
                return;

            int total = behaviors.Count;
            for (int i = 0; i < total; i++)
            {
                this.NavBack(null);
            }
        }

        private void NavBackLast()
        {
            next = behaviors[0];
            curr = next;
            curr.exitEvent = UIExitEvent.destroy;

            if (!UIManager.instance.IsMaskInUse())
                UIManager.instance.mask.gameObject.SetActive(false);
            curr.OnExit();
            curr = null;

            behaviors.Clear();
        }
    }

}

