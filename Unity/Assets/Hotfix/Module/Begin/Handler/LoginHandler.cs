using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(Opcode.loginResult)]
	public class LoginHandler : AMHandler<LoginResult>
	{
        protected override void Run(ETModel.Session session, LoginResult message)
        {
            DiskComponet diskComponet= Game.Scene.GetComponent<DiskComponet>();

            Log.Debug($" playerId = {message.PlayerId}");

            SessionComponent.Instance.Session.Pid = message.PlayerId;

            diskComponet.Clear();
            for (int i = 0; i < message.Bigs.count; i++)
            {
                diskComponet.AddBigIcon(message.Bigs[i]);
            }
            for (int i = 0; i < message.Middles.count; i++)
            {
                diskComponet.AddMiddleIcon(message.Middles[i]);
            }
            for (int i = 0; i < message.Smalls.count; i++)
            {
                diskComponet.AddSmallIcon(message.Smalls[i]);
            }

            Game.Scene.GetComponent<UIComponent>().OpenPanelAsync(UIEnum.Disk);
            Game.Scene.GetComponent<UIComponent>().Close(UIEnum.Begin);

        }
	}
}