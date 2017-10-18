using System;
using System.Collections.Generic;
using ProtoBuf;


namespace SGF.Network.FSPLite.Server.Data
{
	[ProtoContract]
    public class FSPGameStartParam
    {
		[ProtoMember(1)]
        public FSPParam fspParam = new FSPParam();
		[ProtoMember(2)]
        public List<FSPPlayerData> players = new List<FSPPlayerData>(); 
		[ProtoMember(3)]
        public byte[] customGameParam;
    }
}
