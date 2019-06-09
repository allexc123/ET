using System;
using System.Collections.Generic;

namespace ETModel
{
	[ObjectSystem]
	public class MessageDispatcherComponentAwakeSystem : AwakeSystem<MessageDispatcherComponent>
	{
		public override void Awake(MessageDispatcherComponent t)
		{
			t.Awake();
		}
	}

	[ObjectSystem]
	public class MessageDispatcherComponentLoadSystem : LoadSystem<MessageDispatcherComponent>
	{
		public override void Load(MessageDispatcherComponent self)
		{
			self.Load();
		}
	}

	/// <summary>
	/// 消息分发组件
	/// </summary>
	public class MessageDispatcherComponent : Component
	{
		private readonly Dictionary<int, List<IMHandler>> handlers = new Dictionary<int, List<IMHandler>>();
        private readonly Dictionary<int, object> typeMessages = new Dictionary<int, object>();

        public void Awake()
		{
			this.Load();
		}

		public void Load()
		{
			this.handlers.Clear();

			List<Type> types = Game.EventSystem.GetTypes(typeof(MessageHandlerAttribute));

			foreach (Type type in types)
			{
				object[] attrs = type.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
				if (attrs.Length == 0)
				{
					continue;
				}

				IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
				if (iMHandler == null)
				{
					Log.Error($"message handle {type.Name} 需要继承 IMHandler");
					continue;
				}

				Type messageType = iMHandler.GetMessageType();
                MessageHandlerAttribute attribute = attrs[0] as MessageHandlerAttribute;
                int opcode = attribute.Opcode;

                if (opcode == 0)
				{
					Log.Error($"消息opcode为0: {messageType.Name}");
					continue;
				}
				this.RegisterHandler(opcode, iMHandler);
                this.typeMessages.Add(opcode, Activator.CreateInstance(messageType));
            }
		}

		public void RegisterHandler(int opcode, IMHandler handler)
		{
			if (!this.handlers.ContainsKey(opcode))
			{
				this.handlers.Add(opcode, new List<IMHandler>());
			}
			this.handlers[opcode].Add(handler);
		}

		public void Handle(Session session, int opcode, object message)
		{
			List<IMHandler> actions;
			if (!this.handlers.TryGetValue(opcode, out actions))
			{
				Log.Error($"消息没有处理: {opcode} {JsonHelper.ToJson(message)}");
				return;
			}
			
			foreach (IMHandler ev in actions)
			{
				try
				{
					ev.Handle(session, message);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}
		}
        public object GetInstance(int opcode)
        {
            return this.typeMessages[opcode];
        }

        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
		}
	}
}