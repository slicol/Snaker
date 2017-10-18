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



namespace SGF.Network.FSPLite.Server
{
    public class FSPSession
    {
        //========================================================================
        //---------------------------------------------------------
        //日志TAG'
        private string LOG_TAG = "FSPSession";
        

        //========================================================================
        //Session 的索引字段，可以用Sid也可以用Ip来索引
        public uint m_Sid;
        public uint Id{get{return m_Sid;}}

        private Action<FSPDataC2S> m_RecvListener;
        private KCPSocket m_Socket;
        private byte[] m_SendBuffer =  new byte[40960];
        private bool m_IsEndPointChanged = false;
        private IPEndPoint m_EndPoint;
        public IPEndPoint EndPoint
        {
            get { return m_EndPoint; }
            set
            {
                if (m_EndPoint == null || !m_EndPoint.Equals(value))
                {
                    m_IsEndPointChanged = true;
                }
                else
                {
                    m_IsEndPointChanged = false;
                }

                m_EndPoint = value;
            }
        }

        public bool IsEndPointChanged {get { return m_IsEndPointChanged; } }

        //========================================================================

        public FSPSession(uint sid,  KCPSocket socket)
        {
            m_Sid = sid;
            m_Socket = socket;
            LOG_TAG = "FSPSession<" + m_Sid.ToString("d4") + ">";
        }

        public virtual void Close()
        {
            if (m_Socket != null)
            {
                m_Socket.CloseKcp(EndPoint);
                m_Socket = null;
            }
        }

        //-------------------------------------------------------------------
        

        public void SetReceiveListener(Action<FSPDataC2S> listener)
        {
            m_RecvListener = listener;
        }


        public bool Send(FSPFrame frame)
        {
            if (m_Socket != null)
            {
                FSPDataS2C data = new FSPDataS2C();
                data.frames.Add(frame);
                int len = PBSerializer.NSerialize(data, m_SendBuffer);
                return m_Socket.SendTo(m_SendBuffer, len, EndPoint);
            }

            return false;
        }


        public void Receive(FSPDataC2S data)
        {
            if (m_RecvListener != null)
            {
                m_RecvListener(data);
            }
        }

    }
}

