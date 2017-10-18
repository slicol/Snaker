using SGF.Module.Framework;
using SGF.UI.Framework;

namespace Snaker.Module
{
    public class HomeModule:BusinessModule
    {
        public void TryReLogin()
        {
            //作一些判断，是否可以重新登录
            UIManager.Instance.OpenPage(UIDef.UILoginPage);
        }

		public void OpenModule(string name, object arg)
        {
		    switch (name)
		    {
                case ModuleDef.PVEModule:
                case ModuleDef.PVPModule:
                    ModuleManager.Instance.ShowModule(name, arg);
                    break;
                default:
                    UIAPI.ShowMsgBox(name, "模块正在开发中...", "确定");
                    break;
		    }
        }
    }
}
