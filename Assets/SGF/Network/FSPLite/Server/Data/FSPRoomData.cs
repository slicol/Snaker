using System;
using System.Collections.Generic;
using ProtoBuf;


namespace SGF.Network.FSPLite.Server.Data
{
	[ProtoContract]
    public class FSPRoomData
    {
		[ProtoMember(1)]
        public uint id;
		[ProtoMember(2)]
        public List<FSPPlayerData> players = new List<FSPPlayerData>();
    }


}
