using System;
using System.IO;
using Reign;
using SGF;
using SGF.Codec;
using SGF.Module.Framework;
using SGF.UI.Framework;
using Snaker.Module;
using UnityEngine;
using UnityEngine.UI;

namespace Snaker.UI.PVP
{
	public class UIRoomFindWnd:UIWindow
	{
		public InputField inputIPPort;
	    public Text txtScanResult;

        private Texture2D m_TexImage;
        private string m_ImageInfo = "";

		protected override void OnOpen(object arg = null)
		{
			base.OnOpen(arg);
		}

		public void OnBtnOK()
		{
			this.Close (inputIPPort.text);
		}

		public void OnBtnCancel()
		{
			this.Close (null);
		}

	    void Update()
	    {
            txtScanResult.text = m_ImageInfo;
	    }

	    public void OnBtnScan()
	    {
	        m_TexImage = null;

#if UNITY_EDITOR
            m_ImageInfo = "正在打开相册";
            StreamManager.LoadFileDialog(FolderLocations.Pictures, 512, 512, 
                new string[] { ".png", ".jpg", ".jpeg" }, OnImageLoaded);
#else
            m_ImageInfo = "正在打开摄像机";
            StreamManager.LoadCameraPicker(CameraQuality.Med, 512, 512, OnImageLoaded);
#endif
        }

        private void OnImageLoaded(Stream stream, bool succeeded)
        {
            if (!succeeded)
            {
                m_ImageInfo = "打开 摄像机 失败！";
                this.LogError("OnImageLoaded()" + m_ImageInfo);
                

                if (stream != null)
                {
                    stream.Dispose();
                }
                return;
            }

            try
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                var newImage = new Texture2D(512, 512);
                newImage.LoadImage(data);
                newImage.Apply();
                m_TexImage = newImage;

                string content = QRCodeUtils.DecodeFromImage(m_TexImage);
                if (string.IsNullOrEmpty(content))
                {
                    m_ImageInfo = "二维码解析失败！";    
                }
                else
                {
                    m_ImageInfo = "二维码解析成功！"; 
                }

                inputIPPort.text = content;
            }
            catch (Exception e)
            {
                m_ImageInfo = "Error:" + e.Message;
                this.LogError("OnImageLoaded() " + m_ImageInfo);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

        }
	}
}
