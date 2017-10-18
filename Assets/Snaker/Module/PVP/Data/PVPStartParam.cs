using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;
using SGF.Network.FSPLite;
using Snaker.Game.Data;


namespace Snaker.Module.PVP.Data
{

	[ProtoContract]
	public class PVPStartParam
	{
		[ProtoMember(1)] public FSPParam fspParam = new FSPParam();
		[ProtoMember(2)] public GameParam gameParam = new GameParam();
        [ProtoMember(3)] public List<PlayerData> players = new List<PlayerData>();

		public override string ToString()
		{
			XmlSerializer xs = new XmlSerializer(typeof(PVPStartParam));
			StringBuilder sb = new StringBuilder();
			TextWriter tw = new StringWriter(sb);
			xs.Serialize(tw, this);
			tw.Flush();
			return sb.ToString();
		}
	}
}
