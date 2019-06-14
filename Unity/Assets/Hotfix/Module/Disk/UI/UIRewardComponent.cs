
using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIRewardComponentAwakeSystemq : AwakeSystem<UIRewardComponent>
    {
        public override void Awake(UIRewardComponent self)
        {
            self.Awake();
        }
    }
    [UIPanel(UIEnum.Reward, "UIReward")]
    public class UIRewardComponent : Component
    {

        private GameObject reward;
        private GameObject phoneNumber;
        private GameObject confirm;

       

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            this.reward = rc.Get<GameObject>("reward");
            this.phoneNumber = rc.Get<GameObject>("phoneNumber");
            this.confirm = rc.Get<GameObject>("confirm");

            this.confirm.GetComponent<Button>().onClick.Add(OnReward);


        }

        public void SetRewardIcon(string icon)
        {

            Sprite sp = Game.Scene.GetComponent<UIComponent>().GetSprite("Icon", icon);

            Image image = this.reward.GetComponent<Image>();
            image.sprite = sp;
        }

        public void OnReward()
        {
            string phoneNumber = this.phoneNumber.GetComponent<InputField>().text;
            SessionComponent.Instance.Session.Send(Opcode.S_REWARD, new RewardMsg() { PhoneNumber = phoneNumber});
            Game.Scene.GetComponent<UIComponent>().Close(UIEnum.Reward);
        }





    }
}
