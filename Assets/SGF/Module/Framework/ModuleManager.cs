////////////////////////////////////////////////////////////////////
//                            _ooOoo_                             //
//                           o8888888o                            //
//                           88" . "88                            //
//                           (| ^_^ |)                            //
//                           O\  =  /O                            //
//                        ____/`---'\____                         //
//                      .'  \\|     |//  `.                       //
//                     /  \\|||  :  |||//  \                      //
//                    /  _||||| -:- |||||-  \                     //
//                    |   | \\\  -  /// |   |                     //
//                    | \_|  ''\---/''  |   |                     //
//                    \  .-\__  `-`  ___/-. /                     //
//                  ___`. .'  /--.--\  `. . ___                   //
//                ."" '<  `.___\_<|>_/___.'  >'"".                //
//              | | :  `- \`.;`\ _ /`;.`/ - ` : | |               //
//              \  \ `-.   \_ __\ /__ _/   .-` /  /               //
//        ========`-.____`-.___\_____/___.-`____.-'========       //
//                             `=---='                            //
//        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^      //
//            佛祖保佑       无BUG        不修改                   //
////////////////////////////////////////////////////////////////////
/*
* 描述：
* 作者：slicol
*/

using System;
using System.Collections.Generic;
using SGF;

namespace SGF.Module.Framework
{
    public class ModuleManager : ServiceModule<ModuleManager>
    {
        class MessageObject
        {
            public string target;
            public string msg;
            public object[] args;
        }

        /// <summary>
        /// 已创建的模块列表
        /// </summary>
        private Dictionary<string, BusinessModule> m_mapModules;

        /// <summary>
        /// 当目标模块未创建时，缓存的消息对象
        /// </summary>
        private Dictionary<string, List<MessageObject>> m_mapCacheMessage;

        /// <summary>
        /// 当目标模块未创建时，预监听的事件
        /// </summary>
        private Dictionary<string, EventTable> m_mapPreListenEvents;

        /// <summary>
        /// 用于模块反射的域
        /// </summary>
        private string m_domain;


        public ModuleManager()
        {
            m_mapModules = new Dictionary<string, BusinessModule>();
            m_mapCacheMessage = new Dictionary<string, List<MessageObject>>();
            m_mapPreListenEvents = new Dictionary<string, EventTable>();
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="domain">业务模块所在的域</param>
        public void Init(string domain = "")
        {
            CheckSingleton();
            m_domain = domain;
        }

        /// <summary>
        /// 通过类型创建一个业务模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        private T CreateModule<T>(object args = null) where T : BusinessModule
        {
            return (T)CreateModule(typeof(T).Name, args);
        }

        /// <summary>
        /// 通过名字创建一个业务模块
        /// 先通过名字反射出Class，如果不存在
        /// 则通过扫描Lua文件目录加载LuaModule
        /// </summary>
        /// <param name="name">业务模块的名字</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public BusinessModule CreateModule(string name, object args = null)
        {
            this.Log("CreateModule() name = " + name + ", args = " + args);

            if (m_mapModules.ContainsKey(name))
            {
                this.LogError("CreateModule() The Module<{0}> Has Existed!");
                return null;
            }

            BusinessModule module = null;
            Type type = Type.GetType(m_domain + "." + name);
            if (type != null)
            {
                module = Activator.CreateInstance(type) as BusinessModule;
            }
            else
            {
                module = new LuaModule(name);
            }
            m_mapModules.Add(name, module);

            //处理预监听的事件
            if (m_mapPreListenEvents.ContainsKey(name))
            {
                EventTable mgrEvent = null;
                mgrEvent = m_mapPreListenEvents[name];
                m_mapPreListenEvents.Remove(name);

                module.SetEventTable(mgrEvent);
            }

            module.Create(args);

            //处理缓存的消息
            if (m_mapCacheMessage.ContainsKey(name))
            {
                List<MessageObject> list = m_mapCacheMessage[name];
                for (int i = 0; i < list.Count; i++)
                {
                    MessageObject msgobj = list[i];
                    module.HandleMessage(msgobj.msg, msgobj.args);
                }
                m_mapCacheMessage.Remove(name);
            }

            return module;
        }


        /// <summary>
        /// 释放一个由ModuleManager创建的模块
        /// 遵守谁创建谁释放的原则
        /// </summary>
        /// <param name="module"></param>
        public void ReleaseModule(BusinessModule module)
        {
            if (module != null)
            {
                if (m_mapModules.ContainsKey(module.Name))
                {
                    this.Log("ReleaseModule() name = " + module.Name);
                    m_mapModules.Remove(module.Name);
                    module.Release();
                }
                else
                {
                    this.LogError("ReleaseModule() 模块不是由ModuleManager创建的！ name = " + module.Name);
                }
            }
            else
            {
                this.LogError("ReleaseModule() module = null!");
            }

        }

        /// <summary>
        /// 释放所有模块
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var @event in m_mapPreListenEvents)
            {
                @event.Value.Clear();
            }
            m_mapPreListenEvents.Clear();

            m_mapCacheMessage.Clear();

            foreach (var module in m_mapModules)
            {
                module.Value.Release();
            }
            m_mapModules.Clear();
        }


        /// <summary>
        /// 通过名字获取一个Module
        /// 如果未创建过该Module，则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BusinessModule GetModule(string name)
        {
            if (m_mapModules.ContainsKey(name))
            {
                return m_mapModules[name];
            }
            return null;
        }

        /// <summary>
        /// 显示业务模块的默认UI
        /// </summary>
        /// <param name="name"></param>
        public void ShowModule(string name, object arg = null)
        {
			SendMessage(name, "Show", arg);
        }

        /// <summary>
        /// 向指定的模块发送消息
        /// </summary>
        /// <param name="target"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void SendMessage(string target, string msg, params object[] args)
        {
            SendMessage_Internal(target, msg, args);
        }


        private void SendMessage_Internal(string target, string msg, object[] args)
        {
            BusinessModule module = GetModule(target);
            if (module != null)
            {
                module.HandleMessage(msg, args);
            }
            else
            {
                //将消息缓存起来
                List<MessageObject> list = GetCacheMessageList(target);
                MessageObject obj = new MessageObject();
                obj.target = target;
                obj.msg = msg;
                obj.args = args;
                list.Add(obj);

                this.LogWarning("SendMessage() target不存在！将消息缓存起来! target:{0}, msg:{1}, args:{2}", target, msg, args);
            }
        }

        private List<MessageObject> GetCacheMessageList(string target)
        {
            List<MessageObject> list = null;
            if (!m_mapCacheMessage.ContainsKey(target))
            {
                list = new List<MessageObject>();
                m_mapCacheMessage.Add(target, list);
            }
            else
            {
                list = m_mapCacheMessage[target];
            }
            return list;
        }



        /// <summary>
        /// 监听指定模块的指定事件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ModuleEvent Event(string target, string type)
        {
            ModuleEvent evt = null;
            BusinessModule module = GetModule(target);
            if (module != null)
            {
                evt = module.Event(type);
            }
            else
            {
                //预创建事件
                EventTable table = GetPreEventTable(target);
                evt = table.GetEvent(type);
                this.LogWarning("Event() target不存在！将预监听事件! target:{0}, event:{1}", target, type);
            }
            return evt;
        }

        private EventTable GetPreEventTable(string target)
        {
            EventTable table = null;
            if (!m_mapPreListenEvents.ContainsKey(target))
            {
                table = new EventTable();
                m_mapPreListenEvents.Add(target, table);
            }
            else
            {
                table = m_mapPreListenEvents[target];
            }
            return table;
        }


    }
}
