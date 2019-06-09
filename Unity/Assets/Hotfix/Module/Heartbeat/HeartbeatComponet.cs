using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{

    [ObjectSystem]
    public class HeartbeatComponetAwakeSystem : AwakeSystem<HeartbeatComponet>
    {
        public override void Awake(HeartbeatComponet self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class HeartbeatComponetUpdateSystem : UpdateSystem<HeartbeatComponet>
    {
        public override void Update(HeartbeatComponet self)
        {
            self.Update();
        }
    }

    public class HeartbeatComponet :Component
    {
        private float lastTime = 0;
        public void Awake()
        {

        }

        public void Update()
        {
            this.lastTime += Time.deltaTime;

            if (this.lastTime <  5)
            {
                return;
            }
            ETModel.Session session = ETModel.SessionComponent.Instance.Session;

            if (session == null || session.IsDisposed)
            {
                // 创建一个ETModel层的Session,并且保存到ETModel.SessionComponent中
                session = ETModel.Game.Scene.GetComponent<NetOuterComponent>().Create("192.168.1.3:10001");
                ETModel.SessionComponent.Instance.Session = session;

                // 创建一个ETHotfix层的Session, 并且保存到ETHotfix.SessionComponent中
                SessionComponent.Instance.Session = ComponentFactory.Create<Session, ETModel.Session>(session);

                SessionComponent.Instance.Session.Send(Opcode.Login, new Login() { DriveId = "ddafdaf" });
            }

            SessionComponent.Instance.Session.Send(Opcode.S_HEARTBEAT, new HeartbeatMsg() { });

            this.lastTime = 0;

        }
    }
    
}