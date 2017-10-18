using ProtoBuf;

namespace SGF.Network.FSPLite.Server.Data
{
	[ProtoContract]
    public class FSPPlayerData
    {
		[ProtoMember(1)]
        public uint id;
		[ProtoMember(2)]
        public string name;
		[ProtoMember(3)]
        public uint userId;
		[ProtoMember(4)]
        public uint sid;
		[ProtoMember(5)]
        public bool isReady;
		[ProtoMember(6)]
        public byte[] customPlayerData;


		public override string ToString ()
		{
			return string.Format ("[FSPPlayerData] id:{0}, name:{1}, userId:{2}, sid:{3}, isReady:{4}", id, name, userId, sid, isReady);
		}
    }
}
