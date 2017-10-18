
using SGF;
using Snaker;
using UnityEngine;

public class Example_MsgBox : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{

	    //UIAPI.ShowMsgBox("", "无标题的MsgBox", "确定");
        //UIAPI.ShowMsgBox("我是对话框", "有标题的MsgBox", "确定|取消");
        UIAPI.ShowMsgBox("我是对话框", "有标题的MsgBox", "确定|取消|我是按钮", OnMsgBoxClick);
	}

    private void OnMsgBoxClick(object arg)
    {
        this.LogWarning("ButtonIndex:{0}", arg);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
