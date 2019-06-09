using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(Opcode.C_HEARTBEAT)]
	public class HeartbeatHandler : AMHandler<HeartbeatResultMsg>
	{
        protected override void Run(ETModel.Session session, HeartbeatResultMsg message)
        {

            Log.Debug($"{message.ServerTime}");

        }
	}
}