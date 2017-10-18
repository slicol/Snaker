using Snaker.Service.UserManager.Data;
using SGF.Module.Framework;
using SGF.UI.Framework;
using Snaker.Service.User;

namespace Snaker.Module
{
    /// <summary>
    /// 实现登录相关的逻辑
    /// 比较关键的逻辑有：断线重连
    /// </summary>
    public class LoginModule:BusinessModule
    {
        protected override void Show(object arg)
        {
            UIManager.Instance.OpenPage(UIDef.UILoginPage);
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        public void Login(uint id, string name, string pwd)
        {
            //由于我们暂时没有服务端，所以这里并不真正向服务器发协议
            //而是假设已经收到服务端登录成功的协议
            UserData ud = new UserData();
            ud.id = id;
            ud.name = name;
            ud.defaultSnakeId = 1;

            //假设登录成功了
            OnLoginSuccess(ud);

        }

        private void OnLoginSuccess(UserData ud)
        {
            UserManager.Instance.UpdateMainUserData(ud);

            AppConfig.Value.mainUserData = UserManager.Instance.MainUserData;
            AppConfig.Save();

            //将登录成功事件通知给整个游戏
            GlobalEvent.onLogin.Invoke(true);

            //将UI切换到主城
            UIManager.Instance.EnterMainPage();
        }

        
    }
}
