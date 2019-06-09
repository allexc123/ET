using System;
using System.IO;

namespace ETModel
{
	public enum ParserState
	{
		HeadSize,
		PacketBody
	}
	
	public static class Packet
	{
        public const int OpcodeIndex = 0;
        public const int IdIndex = 4;
        public const int PacketSizeIndex = 12;
        public const int MessageIndex = 16;

        public const int OpcodeLength = 4;
        public const int IdLength = 8;
        public const int PacketSizeLength = 4;

        public const int HeadSizeLength = 16;


    }

	public class PacketParser
	{
        private int opcode;
        private long pid;
        private int packetSize;
        private readonly CircularBuffer buffer;
		
		private ParserState state;
		public MemoryStream memoryStream;
		private bool isOK;

		public PacketParser(CircularBuffer buffer, MemoryStream memoryStream)
		{
			
			this.buffer = buffer;
			this.memoryStream = memoryStream;
		}

		public bool Parse()
		{
			if (this.isOK)
			{
				return true;
			}

			bool finish = false;
			while (!finish)
			{
				switch (this.state)
				{
					case ParserState.HeadSize:
						if (this.buffer.Length < Packet.HeadSizeLength)
						{
							finish = true;
						}
						else
						{
							this.buffer.Read(this.memoryStream.GetBuffer(), 0, Packet.HeadSizeLength);
                            if (BitConverter.IsLittleEndian) {
                                Array.Reverse(this.memoryStream.GetBuffer(), Packet.OpcodeIndex, Packet.OpcodeLength);
                                Array.Reverse(this.memoryStream.GetBuffer(), Packet.IdIndex, Packet.IdLength);
                                Array.Reverse(this.memoryStream.GetBuffer(), Packet.PacketSizeIndex, Packet.PacketSizeLength);
                            }


                            this.opcode = BitConverter.ToInt32(this.memoryStream.GetBuffer(), Packet.OpcodeIndex);
                            this.pid = BitConverter.ToInt64(this.memoryStream.GetBuffer(), Packet.IdIndex);
                            this.packetSize = BitConverter.ToInt32(this.memoryStream.GetBuffer(), Packet.PacketSizeIndex);

							this.state = ParserState.PacketBody;
						}
						break;
					case ParserState.PacketBody:
						if (this.buffer.Length < this.packetSize)
						{
							finish = true;
						}
						else
						{


                            this.memoryStream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
							this.memoryStream.SetLength(this.packetSize + Packet.HeadSizeLength);

							byte[] bytes = this.memoryStream.GetBuffer();
							this.buffer.Read(bytes, Packet.MessageIndex, this.packetSize);
							this.isOK = true;
							this.state = ParserState.HeadSize;
							finish = true;


                        }
						break;
				}
			}
			return this.isOK;
		}

		public MemoryStream GetPacket()
		{
			this.isOK = false;
			return this.memoryStream;
		}
	}
}