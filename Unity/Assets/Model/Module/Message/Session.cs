using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ETModel
{
	[ObjectSystem]
	public class SessionAwakeSystem : AwakeSystem<Session, AChannel>
	{
		public override void Awake(Session self, AChannel b)
		{
			self.Awake(b);
		}
	}

	public sealed class Session : Entity
	{
		private AChannel channel;

		//private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();

		public NetworkComponent Network
		{
			get
			{
				return this.GetParent<NetworkComponent>();
			}
		}

		public int Error
		{
			get
			{
				return this.channel.Error;
			}
			set
			{
				this.channel.Error = value;
			}
		}

		public void Awake(AChannel aChannel)
		{
			this.channel = aChannel;
			//this.requestCallback.Clear();
			long id = this.Id;
			channel.ErrorCallback += (c, e) =>
			{
				this.Network.Remove(id); 
			};
			channel.ReadCallback += this.OnRead;
		}
		
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			this.Network.Remove(this.Id);

			base.Dispose();
			
			//foreach (Action<IResponse> action in this.requestCallback.Values.ToArray())
			//{
			//	action.Invoke(new ResponseMessage { Error = this.Error });
			//}

			//int error = this.channel.Error;
			//if (this.channel.Error != 0)
			//{
			//	Log.Trace($"session dispose: {this.Id} ErrorCode: {error}, please see ErrorCode.cs!");
			//}
			
			this.channel.Dispose();
			
			//this.requestCallback.Clear();
		}

		public void Start()
		{
			this.channel.Start();
		}

		public IPEndPoint RemoteAddress
		{
			get
			{
				return this.channel.RemoteAddress;
			}
		}

		public ChannelType ChannelType
		{
			get
			{
				return this.channel.ChannelType;
			}
		}

		public MemoryStream Stream
		{
			get
			{
				return this.channel.Stream;
			}
		}

		public void OnRead(MemoryStream memoryStream)
		{
			try
			{
				this.Run(memoryStream);
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		private void Run(MemoryStream memoryStream)
		{
			memoryStream.Seek(0, SeekOrigin.Begin);
			int opcode = BitConverter.ToInt32(memoryStream.GetBuffer(), Packet.OpcodeIndex);
            long pid = BitConverter.ToInt64(memoryStream.GetBuffer(), Packet.IdIndex);

            memoryStream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
            this.GetComponent<SessionCallbackComponent>().MessageCallback.Invoke(this, opcode, pid, memoryStream);
            return;

//#if !SERVER
//            if (OpcodeHelper.IsClientHotfixMessage(opcode))
//			{
//				this.GetComponent<SessionCallbackComponent>().MessageCallback.Invoke(this, opcode, memoryStream);
//				return;
//			}
//#endif
			
			//object message;
			//try
			//{
			//	OpcodeTypeComponent opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>();
			//	object instance = opcodeTypeComponent.GetInstance(opcode);
			//	message = this.Network.MessagePacker.DeserializeFrom(instance, memoryStream);
				
			//	if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
			//	{
			//		Log.Msg(message);
			//	}
			//}
			//catch (Exception e)
			//{
			//	// 出现任何消息解析异常都要断开Session，防止客户端伪造消息
			//	Log.Error($"opcode: {opcode} {this.Network.Count} {e}, ip: {this.RemoteAddress}");
			//	this.Error = ErrorCode.ERR_PacketParserError;
			//	this.Network.Remove(this.Id);
			//	return;
			//}

			//if (!(message is IResponse response))
			//{
			//	this.Network.MessageDispatcher.Dispatch(this, opcode, message);
			//	return;
			//}
			
			//Action<IResponse> action;
			//if (!this.requestCallback.TryGetValue(response.RpcId, out action))
			//{
			//	throw new Exception($"not found rpc, response message: {StringHelper.MessageToStr(response)}");
			//}
			//this.requestCallback.Remove(response.RpcId);

			//action(response);
		}

		public void Send(int opcode, long pid, object message)
		{
			if (this.IsDisposed)
			{
				throw new Exception("session已经被Dispose了");
			}
			
            //Log.Msg(message);

            MemoryStream stream = this.Stream;

			
			stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);

            this.Network.MessagePacker.SerializeTo(message, stream);



            channel.Send(opcode, pid, stream);
		}

	}
}