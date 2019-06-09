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
		public float acc;
		public float maxSpeed;
		public float decRoll;
	}
}
