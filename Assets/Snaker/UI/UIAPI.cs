using Snaker.UI.Common;
using SGF.UI.Framework;

namespace Snaker
{
    public static class UIAPI
    {
        /// <summary>
        /// 对MsgBox的调用封装
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="btnText">如果有多个按钮，用|分割，例如：确定|取消|关闭</param>
        /// <param name="onCloseEvent"></param>
        /// <returns></returns>
        public static UIWindow ShowMsgBox(string title, string content, string btnText, UIWindow.CloseEvent onCloseEvent = null)
        {
            UIMsgBox.UIMsgBoxArg arg = new UIMsgBox.UIMsgBoxArg();
            arg.content = content;
            arg.title = title;
            arg.btnText = btnText;
            UIWindow wnd = UIManager.Instance.OpenWindow(UIDef.UIMsgBox, arg);

            if (wnd != null && onCloseEvent != null)
            {
                wnd.onClose += closeArg =>
                {
                    onCloseEvent(closeArg);
                };
            }

            return wnd;
        }
    }
}
