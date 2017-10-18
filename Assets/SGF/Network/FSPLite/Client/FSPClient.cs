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
using System.Net;
using SGF.Network.KCP;
using SGF.Unity;


namespace SGF.Network.FSPLite.Client
{
    public class FSPClient
    {
        //===========================================================
        public delegate void FSPTimeoutListener(FSPClient target, int val);
        
        //===========================================================
        //日志
        public string LOG_TAG_SEND = "FSPClient_Send";
        public string LOG_TAG_RECV = "FSPClient_Recv";
        public string LOG_TAG_MAIN = "FSPClient_Main";
        public string LOG_TAG = "FSPClient";

        //===========================================================
        //基本数据
        
        //线程模块
        private bool m_IsRunning = false;
        
        //基础通讯模块
        private KCPSocket m_Socket;
        private string m_Host;
        private int m_Port;
        private IPEndPoint m_HostEndPoint = null;
        private ushort m_SessionId = 0;
        
        //===========================================================

        //===========================================================
        //接收逻辑
        private Action<FSPFrame> m_RecvListener;
        private byte[] m_TempRecvBuf = new byte[10240];
        
        //发送逻辑
        private bool m_EnableFSPSend = true;
        private int m_AuthId;
        private FSPDataC2S m_TempSendData = new FSPDataC2S();
        private byte[] m_TempSendBuf = new byte[128];
        
        private bool m_WaitForReconnect = false;
        private bool m_WaitForSendAuth = false;

        //===========================================================
        //===========================================================
        //------------------------------------------------------------
        #region 构造与析构
        public FSPClient()
        {

        }

        public void Close()
        {
            Debuger.Log(LOG_TAG_MAIN, "Close()");
            Disconnect();
            m_RecvListener = null;
            m_WaitForReconnect = false;
            m_WaitForSendAuth = false;
        }


        #endregion


        //------------------------------------------------------------
        #region 设置通用参数

        public void SetSessionId(ushort sid)
        {
            LOG_TAG_MAIN = "FSPClient_Main<" + sid.ToString("d4") + ">";
            LOG_TAG_SEND = "FSPClient_Send<" + sid.ToString("d4") + ">";
            LOG_TAG_RECV = "FSPClient_Recv<" + sid.ToString("d4") + ">";
            LOG_TAG = LOG_TAG_MAIN;

            m_SessionId = sid;
            m_TempSendData = new FSPDataC2S();
            m_TempSendData.vkeys.Add(new FSPVKey());
            m_TempSendData.sid = sid;
        }



        #endregion

        //------------------------------------------------------------
        #region 设置FSP参数

        public void SetFSPAuthInfo(int authId)
        {
            Debuger.Log(LOG_TAG_MAIN, "SetFSPAuthInfo() " + authId);
            m_AuthId = authId;
        }

        public void SetFSPListener(Action<FSPFrame> listener)
        {
            m_RecvListener = listener;
        }

        #endregion

        //------------------------------------------------------------

    #region 基础连接函数

        public bool IsRunning { get { return m_IsRunning; } }

        public void VerifyAuth()
        {
            m_WaitForSendAuth = false;
            SendFSP(FSPVKeyBase.AUTH, m_AuthId, 0);
        }

        public void Reconnect()
        {
            Debuger.Log(LOG_TAG_MAIN, "Reconnect() 重新连接");
            m_WaitForReconnect = false;

            Disconnect();
            Connect(m_Host, m_Port);
            VerifyAuth();
        }

        public bool Connect(string host, int port)
        {
            if (m_Socket != null)
            {
                Debuger.LogError(LOG_TAG_MAIN, "Connect() 无法建立连接，需要先关闭上一次连接！");
                return false;
            }

            Debuger.Log(LOG_TAG_MAIN, "Connect() 建立基础连接， host = {0}, port = {1}", (object)host, port);

            m_Host = host;
            m_Port = port;

            try
            {
                //获取Host对应的IPEndPoint
                Debuger.Log(LOG_TAG_MAIN, "Connect() 获取Host对应的IPEndPoint");
                m_HostEndPoint = IPUtils.GetHostEndPoint(m_Host, m_Port);
                if (m_HostEndPoint == null)
                {
                    Debuger.LogError(LOG_TAG_MAIN, "Connect() 无法将Host解析为IP！");
                    Close();
                    return false;
                }
                Debuger.Log(LOG_TAG_MAIN, "Connect() HostEndPoint = {0}", m_HostEndPoint.ToString());

                m_IsRunning = true;

                //创建Socket
                Debuger.Log(LOG_TAG_MAIN, "Connect() 创建UdpSocket, AddressFamily = {0}", m_HostEndPoint.AddressFamily);
                m_Socket = new KCPSocket(0, 1);
                //m_Socket.Connect(m_HostEndPoint);
                m_Socket.AddReceiveListener(m_HostEndPoint, OnReceive);

            }
            catch (Exception e)
            {
                Debuger.LogError(LOG_TAG_MAIN, "Connect() " + e.Message + e.StackTrace);
                Close();
                return false;
            }


            return true;
        }

        private void Disconnect()
        {
            Debuger.Log(LOG_TAG_MAIN, "Disconnect()");



            m_IsRunning = false;

            if (m_Socket != null)
            {
                m_Socket.Dispose();
                m_Socket = null;
            }


            m_HostEndPoint = null;
        }




        #endregion


        //------------------------------------------------------------

        #region Receive

        private void OnReceive(byte[] buffer, int size, IPEndPoint remotePoint)
        {
            FSPDataS2C data = PBSerializer.NDeserialize<FSPDataS2C>(buffer);
            
            if (m_RecvListener != null)
            {
                for (int i = 0; i < data.frames.Count; i++)
                {
                    m_RecvListener(data.frames[i]);
                }
                
            }
        }


        #endregion


        //------------------------------------------------------------

        #region Send

        public bool SendFSP(int vkey, int arg, int clientFrameId)
        {
            if (m_IsRunning)
            {
                FSPVKey cmd = m_TempSendData.vkeys[0];
                cmd.vkey = vkey;
                cmd.args = new int[] { arg};
                cmd.clientFrameId = (uint)clientFrameId;
                
                int len = PBSerializer.NSerialize(m_TempSendData, m_TempSendBuf);
                
                return m_Socket.SendTo(m_TempSendBuf, len, m_HostEndPoint);
            }
            return false;
        }

        #endregion  


        //------------------------------------------------------------
        public void EnterFrame()
        {
            if (!m_IsRunning)
            {
                return;
            }

            m_Socket.Update();


            if (m_WaitForReconnect)
            {
                if (NetCheck.IsAvailable())
                {
                    Reconnect();
                }
                else
                {
                    Debuger.Log(LOG_TAG_MAIN, "EnterFrame() 等待重连，但是网络不可用！");
                }
            }

            if (m_WaitForSendAuth)
            {
                VerifyAuth();
            }
        }

      

        public string ToDebugString()
        {
            string str = "";
            return str;
        }
    }
}
