using UnityEngine;
using System.Collections;
using SGF.UI.Component;
using UnityEngine.UI;
using SGF.Network.FSPLite.Server.Data;
using SGF.UI.Framework;
using UnityEngine.EventSystems;

namespace Snaker.PVP.UI
{
	public class CtlRoomPlayerItem : UIListItem
	{
		public Text m_txtPlayerInfo;
		public UIBehaviour m_ctlReady;

		private FSPPlayerData m_data;

		public override void UpdateItem(int index, object data)
		{ 
			m_data = data as FSPPlayerData;
			if (m_data != null) 
			{
				m_txtPlayerInfo.text = "["+m_data.id + "] " + m_data.name + "(UID:"+m_data.userId+") ";
				UIUtils.SetActive (m_ctlReady, m_data.isReady);
			}
		}

		void OnGUI()
		{
			if (m_data != null) 
			{
				UIUtils.SetActive (m_ctlReady, m_data.isReady);
			}
		}

	}

}