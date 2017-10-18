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



namespace SGF.Network.FSPLite.Server
{
    public class FSPPlayer
    {
        //---------------------------------------------------------

        private uint m_id;
        public uint Id { get { return m_id; } }
        private FSPSession m_Session = null;
        private bool m_HasAuth = false;
        private Action<FSPPlayer, FSPVKey> m_RecvListener;
        
        private int m_LastAddFrameId = 0;
        private int m_LastRecvTime = 0;
        private int m_Timeout = 0;
        
        private Queue<FSPFrame> m_FrameCache = null;
        public bool WaitForExit = false;
        //---------------------------------------------------------

        public FSPPlayer(uint playerId, int timeout, FSPSession session, Action<FSPPlayer, FSPVKey> listener)
        {
            m_id = playerId;
            m_Timeout = timeout;
            m_Session = session;
            m_Session.SetReceiveListener(OnSessionReceive);
            m_RecvListener = listener;
            WaitForExit = false;
            m_FrameCache = new Queue<FSPFrame>();
        }

        public void Dispose()
        {
            m_Session = null;
            m_FrameCache.Clear();
        }


        public bool HasAuth
        {
            get { return m_HasAuth; }
        }

        public void SetAuth(int auth)
        {
            //这里暂时不做真正的鉴权，只是让流程完整
            m_HasAuth = true;
        }

        public void ClearRound()
        {
            m_FrameCache.Clear();
            m_LastAddFrameId = 0;
        }

        private void OnSessionReceive(FSPDataC2S data)
        {
            m_LastRecvTime = FSPServer.Instance.RealtimeSinceStartupMS;

            if (m_Session.IsEndPointChanged)
            {
                m_HasAuth = false;
            }

            if (m_RecvListener != null)
            {
                for (int i = 0; i < data.vkeys.Count; i++)
                {
                    m_RecvListener(this, data.vkeys[i]);
                }
                
            }
        }


        public uint Sid
        {
            get
            {
                if (m_Session != null)
                {
                    return m_Session.Id;
                }
                return 0;
            }
        }

        //---------------------------------------------------------
        public int SendToClient(FSPFrame frame)
        {
            if (frame != null)
            {
                if (!m_FrameCache.Contains(frame))
                {
                    m_FrameCache.Enqueue(frame);
                }
            }

            while (m_FrameCache.Count > 0)
            {
                if (TryAddToSession(m_FrameCache.Peek()))
                {
                    m_FrameCache.Dequeue();
                }
                else
                {
                    return -1;
                }
            }

            return 1;
        }

        private bool TryAddToSession(FSPFrame frame)
        {
            if (frame.frameId != 0 && frame.frameId <= m_LastAddFrameId)
            {
                //已经Add过了
                return true;
            }

            if (m_Session != null)
            {
                if (m_Session.Send(frame))
                {
                    m_LastAddFrameId = frame.frameId;
                    return true;
                }
            }

            return false;
        }


        //---------------------------------------------------------------
        /// <summary>
        /// 判断是否掉线
        /// </summary>
        /// <returns></returns>
        public bool IsLose()
        {
            if (m_Timeout <= 0)
            {
                return false;
            }

            int dt = FSPServer.Instance.RealtimeSinceStartupMS - m_LastRecvTime;
            return dt > m_Timeout;
        }
    }
}

