using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.ClientH |  AppType.ClientM | AppType.Gate | AppType.Map))]
	public partial class DiskConfigCategory : ACategory<DiskConfig>
	{
	}

	public class DiskConfig: IConfig
	{
		public long Id { get; set; }
		public int spinNum;
		public int spinTime;
		public int itemNum;
		public int cw;
	}
}
