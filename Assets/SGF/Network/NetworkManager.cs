using UnityEngine;
using System.Collections;
using SGF.Module.Framework;

namespace SGF.Network
{
	public class NetworkManager :ServiceModule<NetworkManager>
	{
		public void Init()
		{
			IPUtils.CheckSelfIPAddress ();
		}
	}
}

