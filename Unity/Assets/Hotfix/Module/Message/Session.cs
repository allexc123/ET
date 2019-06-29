using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
	[ObjectSystem]
	public class SessionAwakeSystem : AwakeSystem<Session, ETModel.Session>
	{
		public override void Awake(Session self, ETModel.Session session)
		{
			self.session = session;
			SessionCallbackComponent sessionComponent = self.session.AddComponent<SessionCallbackComponent>();
			sessionComponent.MessageCallback = (s, opcode, pid, memoryStream) => { self.Run(s, opcode, pid, memoryStream); };
			sessionComponent.DisposeCallback = s => { self.Dispose(); };

            self.Pid = -1;
		}
	}

	/// <summary>
	/// 用来收发热更层的消息
	/// </summary>
	public class Session: Entity
	{
		public ETModel.Session session;

		public  long Pid { get; set; }

        private readonly Dictionary<int, Action<object>> requestCallback = new Dictionary<int, Action<object>>();



        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			base.Dispose();

			this.session.Dispose();
		}

		public void Run(ETModel.Session s, int opcode,long pid, MemoryStream memoryStream)
		{
            Log.Debug($"opcode = {opcode}  pid = {pid}");

            object instance = Game.Scene.GetComponent<MessageDispatcherComponent>().GetInstance(opcode);
            object message = this.session.Network.MessagePacker.DeserializeFrom(instance, memoryStream);

            
            Action<object> action;
            if (this.requestCallback.TryGetValue(opcode, out action))
            {
                
                this.requestCallback.Remove(opcode);
                action(message);
            }
            Game.Scene.GetComponent<MessageDispatcherComponent>().Handle(session,opcode, message);

            //OpcodeTypeComponent opcodeTypeComponent = Game.Scene.GetComponent<OpcodeTypeComponent>();
            //object instance = opcodeTypeComponent.GetInstance(opcode);
            //object message = this.session.Network.MessagePacker.DeserializeFrom(instance, memoryStream);

            //if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
            //{
            //	Log.Msg(message);
            //}

            //Game.Scene.GetComponent<MessageDispatcherComponent>().Handle(session, new MessageInfo(opcode, message));


        }

		

		public void Send(int opcode, object message)
		{
            long pid = this.Pid;
			this.session.Send(opcode, pid, message);
		}

        public Task<T> Call<T>(int opCode, object message, int responseOpCode)
        {
           
            var tcs = new TaskCompletionSource<T>();
            try
            {
                this.requestCallback.Add(responseOpCode,(response)=>
                {
                    try
                    {
                        tcs.SetResult((T)response);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(new Exception($"Call Error: {opCode}", e));

                    }

                });
                Game.Scene.GetComponent<MessageDispatcherComponent>().RegisterMessageType(responseOpCode, typeof(T));
            }
            catch(Exception e)
            {
                Log.Error(e);
            }
           
           
            
            this.Send(opCode, message);
            return tcs.Task;
        }
	}
}
