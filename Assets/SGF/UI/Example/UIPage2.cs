using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF;
using UnityEngine;
using SGF.UI.Framework;

namespace SGF.UI.Example
{
    public class UIPage2:UIPage
    {
        public void OnBtnOpenWnd1()
        {
            UIManager.Instance.OpenWindow("UIWnd1").onClose += OnWnd1Close;
        }

        private void OnWnd1Close(object arg)
        {
            this.Log("OnWnd1Close()");
        }

        public void OnBtnOpenWidget1()
        {
            UIManager.Instance.OpenWidget("UIWidget1");
        }
    }
}
