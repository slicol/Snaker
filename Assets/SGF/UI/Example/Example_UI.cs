using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF.UI.Framework;
using UnityEngine;

namespace SGF.UI.Example
{
    public class Example_UI : MonoBehaviour
    {
        void Start()
        {
            Debuger.EnableLog = true;
            UIManager.Instance.Init("ui/Example/");
            UIManager.MainPage = "UIPage1";
            UIManager.Instance.EnterMainPage();

        }


    }
}
