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
using System.Net;
using System.Threading;
using SGF.Network.KCP;
using SGF.Network.RPCLite;

namespace SGF.Network.FSPLite.Server
{

    public class FSPServer:Singleton<FSPServer>
    {
        //===========================================================
        //---------------------------------------------------------
        //日志TAG'
        private string LOG_TAG_SEND = "FSPServer_Send";
        private string LOG_TAG_RECV = "FSPServer_Recv";
        private string LOG_TAG_MAIN = "FSPServer_Main";

        //===========================================================
        private FSPParam m_Param = new FSPParam();
        /// <summary>
        /// 帧间隔...
        /// </summary>
        private long FRAME_TICK_INTERVAL = 666666 ;
        private bool m_UseExternFrameTick = false;
        //===========================================================
        //基本数据

        //线程模块
        private Thread m_ThreadMain;
        private bool m_IsRunning = false;
        public bool IsRunning { get { return m_IsRunning; } }

        //Game通讯模块
        private KCPSocket m_GameSocket;
        //Room通讯模块
        private RPCService m_RoomRPC;


        //===========================================================
        //逻辑
        private long m_LogicLastTicks = 0;
        private long m_RealTicksAtStart = 0;
        //===========================================================
        //---------------------------------------------------------
        //Session管理
        //因为小型服务器不可能有海量Session，所以可以用List。
        private List<FSPSession> m_ListSession = new List<FSPSession>();
        //=========================================================

        //房间模块
        private FSPRoom m_Room;
        public FSPRoom Room { get { return m_Room; } }

        //战斗核心
        private FSPGame m_Game;
        public FSPGame Game { get { return m_Game; } }
        
        //------------------------------------------------------------

        #region 参数设置

        public void SetFrameInterval(int serverFrameInterval, int clientFrameRateMultiple)//MS
        {
            FRAME_TICK_INTERVAL = serverFrameInterval * 333333*30/1000;
            FRAME_TICK_INTERVAL = serverFrameInterval * 10000;
            m_Param.serverFrameInterval = serverFrameInterval;
            m_Param.clientFrameRateMultiple = clientFrameRateMultiple;
        }

        public void SetServerTimeout(int serverTimeout)
        {
            m_Param.serverTimeout = serverTimeout;
        }

        public int GetFrameInterval()//MS
        {
            return (int)(FRAME_TICK_INTERVAL/10000);
        }

        public bool UseExternFrameTick
        {
            get { return m_UseExternFrameTick; }
            set { m_UseExternFrameTick = value; }
        }

        public FSPParam GetParam()
        {
            m_Param.host = GameIP;
            m_Param.port = GamePort;
            return m_Param.Clone();
        }

        public int RealtimeSinceStartupMS
        {
            get
            {
                long dt = DateTime.Now.Ticks - m_RealTicksAtStart;
                return (int)(dt/10000);
            }
        }

        #endregion

        //------------------------------------------------------------
        #region 通讯参数

        public string GameIP
        {
            get { return m_GameSocket != null ? m_GameSocket.SelfIP : ""; }
        }

        public int GamePort
        {
            get { return m_GameSocket != null ? m_GameSocket.SelfPort : 0; }
        }


        public string RoomIP
        {
            get { return m_RoomRPC != null ? m_RoomRPC.SelfIP:""; }
        }

        public int RoomPort
        {
            get { return m_RoomRPC != null ? m_RoomRPC.SelfPort:0; }
        }

        #endregion


        //---------------------------------------------------------
        #region Session管理
        private FSPSession GetSession(uint sid)
        {
            FSPSession s = null;
            lock (m_ListSession)
            {
                for (int i = 0; i < m_ListSession.Count; i++)
                {
                    if (m_ListSession[i].Id == sid)
                    {
                        return m_ListSession[i];
                    }
                }
            }
            return null;
        }

        internal FSPSession AddSession(uint sid)
        {
            FSPSession s = GetSession(sid);
            if (s != null)
            {
                Debuger.LogWarning(LOG_TAG_MAIN, "AddSession() SID已经存在 = " + sid);
                return s;
            }
            Debuger.Log(LOG_TAG_MAIN, "AddSession() SID = " + sid);

            s = new FSPSession(sid, m_GameSocket);

            lock (m_ListSession)
            {
                m_ListSession.Add(s);
            }
            return s;
        }

        internal void DelSession(uint sid)
        {
            Debuger.Log(LOG_TAG_MAIN, "DelSession() sid = {0}", sid);

            lock (m_ListSession)
            {
                for (int i = 0; i < m_ListSession.Count; i++)
                {
                    if (m_ListSession[i].Id == sid)
                    {
                        m_ListSession[i].Close();
                        m_ListSession.RemoveAt(i);
                        return;
                    }
                }
            }
        }



        private void DelAllSession()
        {
            Debuger.Log(LOG_TAG_MAIN,"DelAllSession()");
            lock (m_ListSession)
            {
                for (int i = 0; i < m_ListSession.Count; i++)
                {
                    m_ListSession[i].Close();
                }
                m_ListSession.Clear();
            }

        }

        #endregion

        //------------------------------------------------------------

        #region 启动
        public bool Start(int port)
        {
            if (m_IsRunning)
            {
                Debuger.LogWarning(LOG_TAG_MAIN,"Start() 不能重复创建启动Server！");
                return false;
            }
            Debuger.Log(LOG_TAG_MAIN,"Start()  port = {0}", port);

            DelAllSession();

            try
            {
                m_LogicLastTicks = DateTime.Now.Ticks;
                m_RealTicksAtStart = m_LogicLastTicks;

                //创建Game Socket
                m_GameSocket = new KCPSocket(0,1);
                m_GameSocket.AddReceiveListener(OnReceive);
                m_IsRunning = true;

                //一个简单通用的房间模块
                m_Room = new FSPRoom();
                m_Room.Create();
                m_RoomRPC = m_Room;

                //创建线程
                Debuger.Log(LOG_TAG_MAIN,"Start()  创建服务器线程");
                m_ThreadMain = new Thread(Thread_Main) { IsBackground = true };
                m_ThreadMain.Start();

            }
            catch (Exception e)
            {
                Debuger.LogError(LOG_TAG_MAIN,"Start() " + e.Message);
                Close();
                return false;
            }

            //当用户直接用UnityEditor上的停止按钮退出游戏时，会来不及走完整的析构流程。
            //这里做一个监听保护
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playmodeStateChanged -= OnEditorPlayModeChanged;
            UnityEditor.EditorApplication.playmodeStateChanged += OnEditorPlayModeChanged;
#endif
            return true;
        }


#if UNITY_EDITOR
        private void OnEditorPlayModeChanged()
        {
            if (UnityEngine.Application.isPlaying == false)
            {
                UnityEditor.EditorApplication.playmodeStateChanged -= OnEditorPlayModeChanged;
                Close();
            }
        }
#endif

        public void Close()
        {
            Debuger.Log(LOG_TAG_MAIN, "Close()");

            m_IsRunning = false;

            if (m_Game != null)
            {
                m_Game.Dispose();
                m_Game = null;
            }

            if (m_Room != null)
            {
                m_Room.Dispose();
                m_Room = null;
                m_RoomRPC = null;
            }

            if (m_GameSocket != null)
            {
                m_GameSocket.Dispose();
                m_GameSocket = null;
            }

            if (m_ThreadMain != null)
            {
                m_ThreadMain.Interrupt();
                m_ThreadMain = null;
            }

            DelAllSession();
        }

        #endregion


		#region Game Logic

		public FSPGame StartGame()
		{
			if (m_Game != null) 
			{
				m_Game.Dispose ();
			}
			m_Game = new FSPGame();
			m_Game.Create(m_Param);
			return m_Game;
		}
			
		public void StopGame()
		{
			if (m_Game != null) 
			{
				m_Game.Dispose ();
				m_Game = null;
			}
		}

		#endregion


        #region 接收线程
        //------------------------------------------------------------

        private void OnReceive(byte[] buffer, int size, IPEndPoint remotePoint)
        {
            FSPDataC2S data = PBSerializer.NDeserialize<FSPDataC2S>(buffer);
            
            FSPSession session = GetSession(data.sid);
            if (session == null)
            {
                Debuger.LogWarning(LOG_TAG_RECV, "DoReceive() 收到一个未知的SID = " + data.sid);
                //没有这个玩家，不理它的数据
                return;
            }
            this.Log("DoReceive() Receive Buffer, SID={0}, IP={1}, Size={2}", session.Id, remotePoint, buffer.Length);

            session.EndPoint = remotePoint;
            session.Receive(data);
        }

        #endregion



        //------------------------------------------------------------
        #region 主循环线程
        private void Thread_Main()
        {
            Debuger.Log(LOG_TAG_MAIN, "Thread_Main() Begin ......");

            while (m_IsRunning)
            {
                try
                {
                    DoMainLoop();
                }
                catch (Exception e)
                {
                    Debuger.LogError(LOG_TAG_MAIN, "Thread_Main() " + e.Message + "\n" + e.StackTrace);
                    Thread.Sleep(10);
                }
            }

            Debuger.Log(LOG_TAG_MAIN, "Thread_Main() End!");
        }


        //------------------------------------------------------------
        private void DoMainLoop()
        {
            long nowticks = DateTime.Now.Ticks;
            long interval = nowticks - m_LogicLastTicks;

            if (interval > FRAME_TICK_INTERVAL)
            {
                m_LogicLastTicks = nowticks - (nowticks % FRAME_TICK_INTERVAL);

                if (!m_UseExternFrameTick)
                {
                    EnterFrame();
                }
            }
        }
        #endregion


        public void EnterFrame()
        {
            if (m_IsRunning)
            {
                m_GameSocket.Update();
                m_RoomRPC.RPCTick();

				if (m_Game != null) 
				{
					m_Game.EnterFrame ();
				}
            }
        }


    }


}

    
