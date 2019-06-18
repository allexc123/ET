
using System;

using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIDiskComponentAwakeSystemq : AwakeSystem<UIDiskComponent>
    {
        public override void Awake(UIDiskComponent self)
        {
            self.Awake();
        }
    }
    [UIPanel(UIEnum.Disk, "UIDisk")]
    public class UIDiskComponent : UIBase
    {

        private GameObject big;
        private GameObject middle;
        private GameObject small;
        private GameObject begin;

        private Turntable tbBig;
        private Turntable tbMiddle;
        private Turntable tbSmall;



        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            this.big = rc.Get<GameObject>("big");
            this.middle = rc.Get<GameObject>("middle");
            this.small = rc.Get<GameObject>("small");
            this.begin = rc.Get<GameObject>("begin");
            begin.GetComponent<Button>().onClick.Add(onBegin);

            DiskComponet diskComponet = Game.Scene.GetComponent<DiskComponet>();
            List<string> bigIcons = diskComponet.GetBigIcons();
            for (int i = 0; i < bigIcons.Count; i++)
            {
                addIcon(this.big, 395, i, bigIcons[i]);
            }

            List<string> middleIcons = diskComponet.GetMiddleIcons();
            for (int i = 0; i < middleIcons.Count; i++)
            {
                addIcon(this.middle, 280, i, middleIcons[i]);
            }

            List<string> smallIcons = diskComponet.GetSmallIcons();
            for (int i = 0; i < smallIcons.Count; i++)
            {
                addIcon(this.small, 140, i, smallIcons[i]);
            }




            this.tbBig = ComponentFactory.Create<Turntable, GameObject>(this.big);
            //this.tbBig.Parent = this;
            this.tbMiddle = ComponentFactory.Create<Turntable, GameObject>(this.middle);
            //this.tbMiddle.Parent = this;
            this.tbSmall = ComponentFactory.Create<Turntable, GameObject>(this.small);
            //this.tbSmall.Parent = this;

        }

        public void addIcon(GameObject parent, float dist, int index, string icon)
        {
            Sprite sp = Game.Scene.GetComponent<UIComponent>().GetSprite("Icon", icon);

            GameObject go = new GameObject(sp.name);
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.parent = parent.transform;
            go.transform.position = new Vector3(0, 388, 0);


            RectTransform rectTrans = go.AddComponent<RectTransform>();
            rectTrans.SetParent(this.big.transform);
            rectTrans.anchoredPosition3D = Vector3.zero;
            rectTrans.localScale = Vector3.one;
            rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
            rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
            rectTrans.pivot = new Vector2(0.5f, 0.5f);
            rectTrans.offsetMin = Vector2.zero;
            rectTrans.offsetMax = Vector2.zero;
            float x = dist * Mathf.Sin(30 * index * Mathf.Deg2Rad);
            float y = dist * Mathf.Cos(30 * index * Mathf.Deg2Rad);
            rectTrans.localPosition = new Vector3(x, y, 0);
            rectTrans.localEulerAngles = new Vector3(0, 0, -30 * index);

            go.transform.localScale = Vector3.one;
            Image image = go.AddComponent<Image>();
            image.sprite = sp;
            image.SetNativeSize();

        }


        private void onBegin()
        {


            //SessionComponent.Instance.Session.Send(Opcode.S_DRAW, new DrawMsg() { });
            this.tbBig.RotateUp(1, 4);
            this.tbMiddle.RotateUp(2, 3);
            this.tbSmall.RotateUp(3, 1);

            
        }

        public void Wheel(int bigIndex, int middleIndex, int smallIndex, string rewardIcon)
        {

            //this.tbBig.begin(1, bigIndex);

            //this.tbMiddle.begin(2, middleIndex);

            //this.tbSmall.begin(3, smallIndex);

            //Game.Scene.GetComponent<UIComponent>().OpenPanelAsync(UIEnum.Reward);

            //UI ui = Game.Scene.GetComponent<UIComponent>().Get(UIEnum.Reward);
            //if (ui != null)
            //{
            //    UIRewardComponent uiRewardComponent = ui.GetComponent<UIRewardComponent>();
            //    uiRewardComponent.SetRewardIcon(rewardIcon);
            //}
        }





    }
}
