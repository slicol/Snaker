using SGF.Module.Framework;
using SGF.UI.Framework;
using Snaker.Module;
using UnityEngine.UI;
using SGF.Codec;
using UnityEngine;

namespace Snaker.UI.Host
{
    public class UIHostWnd:UIWindow
    {
		public InputField txtRoomIPPort;
        private HostModule m_module;
		public RawImage imgRoomQR;

        protected override void OnOpen(object arg = null)
        {
            base.OnOpen(arg);
            txtRoomIPPort.text = "";
			m_module = ModuleManager.Instance.GetModule(ModuleDef.HostModule) as HostModule;

			ClearRoomInfo ();
			UpdateRoomInfo ();
        }

		protected override void OnClose (object arg)
		{
			base.OnClose (arg);
			m_module = null;
		}

        public void OnBtnStartServer()
        {
			txtRoomIPPort.text = "";
            m_module.StartServer();
			UpdateRoomInfo ();
        }

        public void OnBtnStopServer()
        {
			m_module.CloseServer();
			txtRoomIPPort.text = "";
			ClearRoomInfo ();
        }

		private void UpdateRoomInfo()
		{
			if (m_module.GetRoomPort () > 0)
			{
				string ipport = m_module.GetRoomIP () + ":" + m_module.GetRoomPort ();
				txtRoomIPPort.text = ipport;

				imgRoomQR.enabled = true;
				Texture2D tex = QRCodeUtils.EncodeToImage (ipport, 256, 256);
				imgRoomQR.texture = tex;
			}
		}

		private void ClearRoomInfo()
		{
			txtRoomIPPort.text = "";
			imgRoomQR.enabled = false;
		}

    }
}
