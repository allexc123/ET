using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(Opcode.C_DRAW)]
	public class RewardHandler : AMHandler<DrawResultMsg>
	{
        protected override void Run(ETModel.Session session, DrawResultMsg message)
        {

            UI ui = Game.Scene.GetComponent<UIComponent>().Get(UIEnum.Disk);
            if (ui != null) {
                UIDiskComponent uiDiskComponent = ui.GetComponent<UIDiskComponent>();
                uiDiskComponent.Wheel(message.BigIndxe, message.MiddleIndex, message.SmallIndex, message.RewardIcon);
            }
            //Game.Scene.GetComponent<RewardComponet>().RewardIcon = message.RewardIcon;


            //Game.Scene.GetComponent<UIComponent>().OpenPanelAsync(UIEnum.Disk);
            //Game.Scene.GetComponent<UIComponent>().Remove(UIEnum.Begin);

        }
	}
}