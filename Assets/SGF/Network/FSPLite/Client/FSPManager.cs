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



namespace SGF.Network.FSPLite.Client
{
    public class FSPManager
    {
        public string LOG_TAG = "FSPManager";

        private FSPParam m_Param;
        private FSPClient m_Client;
        private FSPFrameController m_FrameCtrl;
        private DictionaryEx<int, FSPFrame> m_FrameBuffer;
        private bool m_IsRunning = false;

        private FSPGameState m_GameState = FSPGameState.None;
        public FSPGameState GameState { get { return m_GameState; } }

        private int m_CurrentFrameIndex = 0;
        private int m_ClientLockedFrame = 0;
        private Action<int, FSPFrame> m_FrameListener;
        
        //本地表现
        private uint m_MinePlayerId = 0;
        private FSPFrame m_NextLocalFrame;

		public event Action<int> onGameBegin;
		public event Action<int> onRoundBegin;
        public event Action<int> onControlStart; 
		public event Action<int> onRoundEnd;
		public event Action<int> onGameEnd;
        public event Action<uint> onGameExit; 
        

        public void Start(FSPParam param, uint playerId)
        {
            m_Param = param;
            m_MinePlayerId = playerId;
            LOG_TAG = "FSPManager[" + playerId + "]";

            if (!m_Param.useLocal)
            {
                m_ClientLockedFrame = m_Param.clientFrameRateMultiple - 1;

                m_Client = new FSPClient();
                m_Client.SetSessionId((ushort) param.sid);
                m_Client.SetFSPAuthInfo(param.authId);
                m_Client.Connect(param.host, param.port);
                m_Client.SetFSPListener(OnFSPListener);
                m_Client.VerifyAuth();
            }
            else
            {
                m_ClientLockedFrame = param.maxFrameId;
            }

            m_FrameCtrl = new FSPFrameController();
            m_FrameCtrl.Start(param);

            m_FrameBuffer = new DictionaryEx<int, FSPFrame>();

            m_IsRunning = true;
            m_GameState = FSPGameState.Create;
            m_CurrentFrameIndex = 0;

        }

        public void Stop()
        {
            m_GameState = FSPGameState.None;

            if (m_Client != null)
            {
                m_Client.Close();
                m_Client = null;
            }

            if (m_FrameCtrl != null)
            {
                m_FrameCtrl.Close();
                m_FrameCtrl = null;
            }

            m_FrameListener = null;
            m_FrameBuffer.Clear();
            m_IsRunning = false;

            onGameBegin = null;
			onRoundBegin = null;
            onControlStart = null;
            onGameEnd = null;
			onRoundEnd = null;
        }





        /// <summary>
        /// 设置帧数据的监听
        /// </summary>
        /// <param name="listener"></param>
        public void SetFrameListener(Action<int, FSPFrame> listener)
        {
            m_FrameListener = listener;
        }

        /// <summary>
        /// 监听来自FSPClient的帧数据
        /// </summary>
        /// <param name="frame"></param>
        private void OnFSPListener(FSPFrame frame)
        {
            AddServerFrame(frame);
        }

        /// <summary>
        /// 由外界驱动
        /// </summary>
        public void EnterFrame()
        {
            if (!m_IsRunning)
            {
                return;
            }

            if (!m_Param.useLocal)
            {
                m_Client.EnterFrame();

                int speed = m_FrameCtrl.GetFrameSpeed(m_CurrentFrameIndex);
                while (speed > 0)
                {
                    if (m_CurrentFrameIndex < m_ClientLockedFrame)
                    {
                        m_CurrentFrameIndex++;
                        FSPFrame frame = m_FrameBuffer[m_CurrentFrameIndex];
                        ExecuteFrame(m_CurrentFrameIndex,frame);
                        
                    }

                    speed--;
                }

                //这里可能加预表现
            }
            else
            {
                if (m_ClientLockedFrame == 0 || m_CurrentFrameIndex < m_ClientLockedFrame)
                {
                    m_CurrentFrameIndex++;
                    FSPFrame frame = m_FrameBuffer[m_CurrentFrameIndex];
                    ExecuteFrame(m_CurrentFrameIndex, frame);
                    
                }
            }
        }


        /// <summary>
        /// 执行每一帧
        /// </summary>
        /// <param name="frameId"></param>
        /// <param name="frame"></param>
        private void ExecuteFrame(int frameId, FSPFrame frame)
        {

            //优先处理流程VKey
            if (frame != null && frame.vkeys != null)
            {
                for (int i = 0; i < frame.vkeys.Count; i++)
                {
                    FSPVKey cmd = frame.vkeys[i];

                    if (cmd.vkey == FSPVKeyBase.GAME_BEGIN)
                    {
                        Handle_GameBegin(cmd.args[0]);
                    }
                    else if (cmd.vkey == FSPVKeyBase.ROUND_BEGIN) 
					{
						Handle_RoundBegin (cmd.args[0]);
					}
                    else if (cmd.vkey == FSPVKeyBase.CONTROL_START)
                    {
                        Handle_ControlStart(cmd.args[0]);
                    }
                    else if (cmd.vkey == FSPVKeyBase.ROUND_END) 
					{
						Handle_RoundEnd (cmd.args[0]);
					} 
					else if (cmd.vkey == FSPVKeyBase.GAME_END)
					{
						Handle_GameEnd (cmd.args[0]);
					}
                    else if (cmd.vkey == FSPVKeyBase.GAME_EXIT)
                    {
                        Handle_GameExit(cmd.playerId);
                    }
                }
            }

            //再将其它VKey抛给外界处理
            if (m_FrameListener != null)
            {
                m_FrameListener(frameId, frame);
            }
        }



        #region 对基础流程VKey的处理

        public void SendGameBegin()
        {
            SendFSP(FSPVKeyBase.GAME_BEGIN,0);
        }

        private void Handle_GameBegin(int arg)
        {
            m_GameState = FSPGameState.GameBegin;
            if (onGameBegin != null)
            {
                onGameBegin(arg);
            }
        }

        public void SendRoundBegin()
        {
            SendFSP(FSPVKeyBase.ROUND_BEGIN, 0);
        }

        private void Handle_RoundBegin(int arg)
        {
            m_GameState = FSPGameState.RoundBegin;
            m_CurrentFrameIndex = 0;

            if (!m_Param.useLocal)
            {
                m_ClientLockedFrame = m_Param.clientFrameRateMultiple - 1;
            }
            else
            {
                m_ClientLockedFrame = m_Param.maxFrameId;
            }

            m_FrameBuffer.Clear();

            if (onRoundBegin != null)
            {
                onRoundBegin(arg);
            }
        }

        public void SendControlStart()
        {
            SendFSP(FSPVKeyBase.CONTROL_START, 0);
        }
        private void Handle_ControlStart(int arg)
        {
            m_GameState = FSPGameState.ControlStart;
            if (onControlStart != null)
            {
                onControlStart(arg);
            }
        }

        public void SendRoundEnd()
        {
            SendFSP(FSPVKeyBase.ROUND_END, 0);
        }
        private void Handle_RoundEnd(int arg)
        {
            m_GameState = FSPGameState.RoundEnd;
            if (onRoundEnd != null)
            {
                onRoundEnd(arg);
            }
        }

        public void SendGameEnd()
        {
            SendFSP(FSPVKeyBase.GAME_END, 0);
        }
        private void Handle_GameEnd(int arg)
        {
            m_GameState = FSPGameState.GameEnd;
            if (onGameEnd != null)
            {
                onGameEnd(arg);
            }
        }


        public void SendGameExit()
        {
            SendFSP(FSPVKeyBase.GAME_EXIT,0);
        }

        private void Handle_GameExit(uint playerId)
        {
            if (onGameExit != null)
            {
                onGameExit(playerId);
            }
        }


        #endregion




        private void AddServerFrame(FSPFrame frame)
        {


            if (frame.frameId <= 0)
            {
                ExecuteFrame(frame.frameId,frame);
                return;
            }

            frame.frameId = frame.frameId * m_Param.clientFrameRateMultiple;
            m_ClientLockedFrame = frame.frameId + m_Param.clientFrameRateMultiple - 1;

            m_FrameBuffer.Add(frame.frameId, frame);
            m_FrameCtrl.AddFrameId(frame.frameId);
        }



        /// <summary>
        /// 给外界用来发送FSPVkey的
        /// </summary>
        /// <param name="vkey"></param>
        /// <param name="arg"></param>
        public void SendFSP(int vkey, int arg = 0)
        {
            if (!m_IsRunning)
            {
                return;
            }

            if (!m_Param.useLocal)
            {
                m_Client.SendFSP(vkey, arg, m_CurrentFrameIndex);
            }
            else
            {
                SendFSPLocal(vkey, arg);
            }
        }



        /// <summary>
        /// 用于本地兼容，比如打PVE的时候，也可以用帧同步兼容
        /// </summary>
        /// <param name="vkey"></param>
        /// <param name="arg"></param>
        private void SendFSPLocal(int vkey, int arg = 0)
        {
            Debuger.Log(LOG_TAG, "SendFSPLocal() vkey={0}, arg={1}", vkey, arg);
            int nextFrameId = m_CurrentFrameIndex + 1;
            if (m_NextLocalFrame == null || m_NextLocalFrame.frameId != nextFrameId)
            {
                m_NextLocalFrame = new FSPFrame();
                m_NextLocalFrame.frameId = nextFrameId;
                m_NextLocalFrame.vkeys = new List<FSPVKey>();

                m_FrameBuffer.Add(m_NextLocalFrame.frameId, m_NextLocalFrame);
            }

            FSPVKey cmd = new FSPVKey();
            cmd.vkey = vkey;
            cmd.args = new int[] {arg};
            cmd.playerId = m_MinePlayerId;

            m_NextLocalFrame.vkeys.Add(cmd);

            
        }


        //======================================================================

        public string ToDebugString()
        {
            string str = "";
            if (m_FrameCtrl != null)
            {
                str += ("NewestFrameId:" + m_FrameCtrl.NewestFrameId) + "; ";
                str += ("PlayedFrameId:" + m_CurrentFrameIndex) + "; ";
                str += ("IsInBuffing:" + m_FrameCtrl.IsInBuffing) + "; ";
                str += ("IsInSpeedUp:" + m_FrameCtrl.IsInSpeedUp) + "; ";
                str += ("FrameBufferSize:" + m_FrameCtrl.FrameBufferSize) + "; ";
            }

            if (m_Client != null)
            {
                str += m_Client.ToDebugString();
            }

            return str;
        }
    }
}
