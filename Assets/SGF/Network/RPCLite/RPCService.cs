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
using System.Linq;
using System.Net;
using System.Reflection;
using SGF.Network.KCP;



namespace SGF.Network.RPCLite
{
    public class RPCService
    {
        private string LOG_TAG = "RPCService";
        public delegate void CustomRPC(object[] args, IPEndPoint targetAddress);

        //KCPSocket
        private KCPSocket m_Socket;
        private bool m_IsRunning = false;

        


        private Dictionary<string, RPCMethodHelper> m_MapRPCBind;
        //=================================================================================
        #region 构造和析构

        public RPCService(int port = 0)
        {
            m_MapRPCBind = new Dictionary<string, RPCMethodHelper> ();

            //创建Socket
            m_Socket = new KCPSocket(port, 1);
            m_Socket.AddReceiveListener(OnReceive);
            m_Socket.EnableBroadcast = true;
            m_IsRunning = true;

            port = m_Socket.SelfPort;
            LOG_TAG = LOG_TAG + "[" + port + "]";
            Debuger.Log(LOG_TAG, "RPCSocket() port:{0}", port);
        }


        public virtual void Dispose()
        {
            Debuger.Log(LOG_TAG, "Dispose()");

            m_IsRunning = false;

            if (m_Socket != null)
            {
                m_Socket.Dispose();
                m_Socket = null;
            }

            m_MapRPCBind.Clear ();
            
        }

        #endregion
        //=================================================================================
        public IPEndPoint SelfEndPoint { get { return m_Socket.SelfEndPoint; } }
        public int SelfPort { get { return m_Socket.SelfPort; } }
        public string SelfIP { get { return m_Socket.SelfIP; } }

        //=================================================================================


        //=================================================================================
        #region 主线程驱动

        public void RPCTick()
        {
            if (m_IsRunning)
            {
                m_Socket.Update();
            }
        }
        #endregion


        //=================================================================================
        //接收
        #region 消息接收处理: ACK, SYN, Broadcast
        private void OnReceive(byte[] buffer, int size, IPEndPoint remotePoint)
        {
            try
            {
                var msg = PBSerializer.NDeserialize<RPCMessage>(buffer);
                HandleRPCMessage(msg, remotePoint);
            }
            catch (Exception e)
            {
                Debuger.LogError(LOG_TAG, "OnReceive()->HandleMessage->Error:" + e.Message + "\n" + e.StackTrace);
            }
        }


        private void HandleRPCMessage(RPCMessage msg, IPEndPoint target)
        {
            MethodInfo mi = this.GetType().GetMethod(msg.name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null)
            {
                Debuger.Log(LOG_TAG, "HandleRPCMessage() DefaultRPC:{0}, Target:{1}", msg.name, target);
                try
                {
                    var args = msg.args.ToList();
                    args.Add(target);
                    
                    mi.Invoke(this, BindingFlags.NonPublic, null, args.ToArray(), null);
                }
                catch (Exception e)
                {
                    Debuger.LogError(LOG_TAG, "HandleRPCMessage() DefaultRPC<" + msg.name + ">响应出错:" + e.Message + "\n" + e.StackTrace + "\n");
                }
            }
            else
            {
                OnBindingRPCInvoke(msg, target);
            }
        }


        private void OnBindingRPCInvoke(RPCMessage msg, IPEndPoint target)
        {
            if (m_MapRPCBind.ContainsKey(msg.name))
            {
                Debuger.Log(LOG_TAG, "OnBindingRPCInvoke() RPC:{0}, Target:{1}", msg.name, target);

                RPCMethodHelper rpc = m_MapRPCBind[msg.name];


                try
                {
                    rpc.Invoke(msg.args, target);
                }
                catch (Exception e)
                {
                    Debuger.LogError(LOG_TAG, "OnBindingRPCInvoke() RPC<" + msg.name + ">响应出错:" + e.Message + "\n" + e.StackTrace + "\n");
                }

            }
            else
            {
                Debuger.LogError(LOG_TAG, "OnBindingRPCInvoke() 收到未知的RPC:{0}", msg.name);
            }
        }
        

        #endregion

        //=================================================================================
        //发送
        #region 消息发送处理
        //发送SYN

        private void SendMessage(IPEndPoint target, RPCMessage msg)
        {
            byte[] buffer = PBSerializer.NSerialize(msg);
            m_Socket.SendTo(buffer, buffer.Length, target);
        }

        private void SendMessage(List<IPEndPoint> listTargets, RPCMessage msg)
        {
            byte[] buffer = PBSerializer.NSerialize(msg);

            for (int i = 0; i < listTargets.Count; i++)
            {
                IPEndPoint target = listTargets[i];
                if (target != null)
                {
                    m_Socket.SendTo(buffer, buffer.Length, target);
                }
            }
        }

        private void SendBroadcast(int beginPort, int endPort, RPCMessage msg)
        {
            byte[] buffer = PBSerializer.NSerialize(msg);

            for (int i = beginPort; i < endPort; i++)
            {
                m_Socket.SendTo(buffer, buffer.Length, new IPEndPoint(IPAddress.Broadcast, i));
            }
        }

        #endregion


        //=================================================================================
        //RPC 调用 
        public void RPC(IPEndPoint target, string name, params object[] args)
        {
            Debuger.Log(LOG_TAG, "RPC() 1对1调用, name:{0}, target:{1}", name, target);

            RPCMessage msg = new RPCMessage();
            msg.name = name;
            msg.args = args;
            SendMessage(target, msg);
            
        }

        public void RPC(List<IPEndPoint> listTargets, string name, params object[] args)
        {
            Debuger.Log(LOG_TAG, "RPC() 1对多调用, Begin, msg:{0}", name);

            RPCMessage msg = new RPCMessage();
            msg.name = name;
            msg.args = args;
            SendMessage(listTargets, msg);

            Debuger.Log(LOG_TAG, "RPC() 1对多调用, End!");
        }

        public void RPC(int beginPort, int endPort, string name, params object[] args)
        {
            Debuger.Log(LOG_TAG, "RPC() 广播调用, PortRange:{0}-{1}, Begin, msg:{2}",  beginPort, endPort, name);

            RPCMessage msg = new RPCMessage();
            msg.name = name;
            msg.args = args;
            SendBroadcast(beginPort, endPort, msg);
        }

        public void RPC(RPCService target, string name, params object[] args)
        {
            RPCMessage msg = new RPCMessage();
            msg.name = name;
            msg.args = args;
            target.HandleRPCMessage(msg, null);
        }

        //==========================================================================

        public void Bind(string name, RPCMethod rpc)
        {
            RPCMethodHelper helper = new RPCMethodHelper();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0>(string name, RPCMethod<T0> rpc)
        {
            RPCMethodHelper<T0> helper = new RPCMethodHelper<T0>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1>(string name, RPCMethod<T0, T1> rpc)
        {
            RPCMethodHelper<T0, T1> helper = new RPCMethodHelper<T0, T1>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2>(string name, RPCMethod<T0, T1, T2> rpc)
        {
            RPCMethodHelper<T0, T1, T2> helper = new RPCMethodHelper<T0, T1, T2>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2, T3>(string name, RPCMethod<T0, T1, T2, T3> rpc)
        {
            RPCMethodHelper<T0, T1, T2, T3> helper = new RPCMethodHelper<T0, T1, T2, T3>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2, T3, T4>(string name, RPCMethod<T0, T1, T2, T3, T4> rpc)
        {
            RPCMethodHelper<T0, T1, T2, T3, T4> helper = new RPCMethodHelper<T0, T1, T2, T3, T4>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2, T3, T4, T5>(string name, RPCMethod<T0, T1, T2, T3, T4, T5> rpc)
        {
            RPCMethodHelper<T0, T1, T2, T3, T4, T5> helper = new RPCMethodHelper<T0, T1, T2, T3, T4, T5>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2, T3, T4, T5, T6>(string name, RPCMethod<T0, T1, T2, T3, T4, T5, T6> rpc)
        {
            RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6> helper = new RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2, T3, T4, T5, T6, T7>(string name, RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7> rpc)
        {
            RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7> helper = new RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string name, RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8> rpc)
        {
            RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8> helper = new RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }

        public void Bind<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string name, RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> rpc)
        {
            RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> helper = new RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>();
            m_MapRPCBind[name] = helper;
            helper.method = rpc;
        }
        

    }
}
