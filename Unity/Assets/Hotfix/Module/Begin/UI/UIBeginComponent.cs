using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIBeginComponentAwakeSystemq : AwakeSystem<UIBeginComponent>
    {
        public override void Awake(UIBeginComponent self)
        {
            self.Awake();
        }
    }
    [UIPanel(UIEnum.Begin, "UIBegin")]
    public class UIBeginComponent : UIBase
    {

        private GameObject begin;

        public void Awake()
        {
            ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();


            this.begin = rc.Get<GameObject>("BeginBut");
            begin.GetComponent<Button>().onClick.Add(onBegin);




            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>();
            Game.Scene.AddComponent<SessionComponent>();

            Game.Scene.AddComponent<HeartbeatComponet>();


        }

        private void onBegin()
        {
            if (SessionComponent.Instance.Session == null)
            {
                return;
            }
            SessionComponent.Instance.Session.Send(Opcode.Login, new Login() { DriveId = "ddafdaf" });
        }
    }
}
