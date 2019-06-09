namespace ETModel
{
	public class MessageHandlerAttribute : BaseAttribute
	{
		public int Opcode { get; }


		public MessageHandlerAttribute(int Opcode)
		{
			this.Opcode = Opcode;
		}
	}
}