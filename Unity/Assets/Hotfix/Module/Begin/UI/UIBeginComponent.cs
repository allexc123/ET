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
        public void Awake()
        {
          
            ETModel.Game.Scene.AddComponent<ETModel.SessionComponent>();
            Game.Scene.AddComponent<SessionComponent>();

            Game.Scene.AddComponent<HeartbeatComponet>();
        }

    }
}
