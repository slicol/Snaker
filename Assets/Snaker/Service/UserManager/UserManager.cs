using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snaker.Service.UserManager.Data;
using SGF.Module.Framework;

namespace Snaker.Service.User
{
    public class UserManager:ServiceModule<UserManager>
    {
        private UserData m_mainUserData;
        public UserData MainUserData { get { return m_mainUserData; } }

        public void Init()
        {
            CheckSingleton();
        }


        /// <summary>
        /// 通过登录等逻辑来更新用户数据
        /// </summary>
        /// <param name="data"></param>
        public void UpdateMainUserData(UserData data)
        {
            m_mainUserData = data;
        }

    }
}
