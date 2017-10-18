using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF;
using Snaker.Service.UserManager.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SGF.UI.Framework;

namespace Snaker.UI.Common
{
    public class UIMsgBox:UIWindow
    {
        public class UIMsgBoxArg
        {
            public string title = "";
            public string content = "";
            public string btnText;//"确定|取消|关闭"
        }

        private UIMsgBoxArg m_arg;
        public Text txtContent;
        public UIBehaviour ctlTitle;
        public Button[] buttons;
        

        protected override void OnOpen(object arg = null)
        {
            base.OnOpen(arg);
            m_arg = arg as UIMsgBoxArg;
            txtContent.text = m_arg.content;
            string[] btnTexts = m_arg.btnText.Split('|');

            UIUtils.SetChildText(ctlTitle, m_arg.title);
            UIUtils.SetActive(ctlTitle, !string.IsNullOrEmpty(m_arg.title));

            float btnWidth = 200;
            float btnStartX =  (1- btnTexts.Length) * btnWidth / 2;
            
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i < btnTexts.Length)
                {
                    UIUtils.SetButtonText(buttons[i], btnTexts[i]);
                    UIUtils.SetActive(buttons[i], true);
                    Vector3 pos = buttons[i].transform.localPosition;
                    pos.x = btnStartX + i * btnWidth;
                    buttons[i].transform.localPosition = pos;
                }
                else
                {
                    UIUtils.SetActive(buttons[i], false);
                }
            }


        }

        public void OnBtnClick(int btnIndex)
        {
            Button btn = buttons[btnIndex];
            this.Close(btnIndex);
        }
    }
}
