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
    public class FSPGame
    {

        //---------------------------------------------------------
        //日志TAG'
        public string LOG_TAG = "FSPGame";

        //---------------------------------------------------------
        //基本数据
        private FSPParam m_FSPParam;

        /// <summary>
        /// 最大支持的玩家数：31
        /// 因为用来保存玩家Flag的Int只有31位有效位可用，不过31已经足够了
        /// </summary>
        private const int MaxPlayerNum = 31;

        //游戏状态
        private FSPGameState m_State;
        private int m_StateParam1;
        private int m_StateParam2;

        public FSPGameState State{get { return m_State; }}
        public int StateParam1 { get { return m_StateParam1; } }
        public int StateParam2 { get { return m_StateParam2; } }

        
        //---------------------------------------------------------
        //Player的VKey标识
        private int m_GameBeginFlag = 0;
        private int m_RoundBeginFlag = 0;
        private int m_ControlStartFlag = 0;
        private int m_RoundEndFlag = 0;
        private int m_GameEndFlag = 0;
        

        //Round标志
        private int m_CurRoundId = 0;
        public int CurrentRoundId { get { return m_CurRoundId; } }
        //---------------------------------------------------------
        //帧列表
        private int m_CurFrameId = 0;
        public int CurrentFrameId { get { return m_CurFrameId; } }

        //当前帧
        private FSPFrame m_LockedFrame = new FSPFrame();

        //玩家列表
        private List<FSPPlayer> m_ListPlayer = new List<FSPPlayer>();
        //等待删除的玩家
        private List<FSPPlayer> m_ListPlayersExitOnNextFrame = new List<FSPPlayer>();

		//有一个玩家退出游戏
		public Action<uint> onGameExit;

		//游戏真正结束
		public Action<int> onGameEnd;

        //=========================================================
        //延迟GC缓存
        public static bool UseDelayGC = false;
        private List<object> m_ListObjectsForDelayGC = new List<object>(); 

        //---------------------------------------------------------
        public void Create(FSPParam param)
        {
            Debuger.Log(LOG_TAG, "Create()");
            m_FSPParam = param;
            m_CurRoundId = 0;

            ClearRound();
            SetGameState(FSPGameState.Create);

        }


        public void Dispose()
        {
            SetGameState(FSPGameState.None);
            for (int i = 0; i < m_ListPlayer.Count; i++)
            {
                FSPPlayer player = m_ListPlayer[i];
				FSPServer.Instance.DelSession(player.Sid);
                player.Dispose();
            }
            m_ListPlayer.Clear();
            m_ListObjectsForDelayGC.Clear();
            GC.Collect();
			onGameExit = null;
			onGameEnd = null;

            Debuger.Log(LOG_TAG, "Dispose()");
        }

        //---------------------------------------------------------
        public bool AddPlayer(uint playerId, uint sid)
        {
            Debuger.Log(LOG_TAG, "AddPlayer() playerId:{0}, sid:{1}", playerId, sid);

            if (m_State != FSPGameState.Create)
            {
                Debuger.LogError(LOG_TAG, "AddPlayer() 当前状态下无法AddPlayer! State = {0}", m_State);
                return false;
            }

            FSPPlayer player = null;
            for (int i = 0; i < m_ListPlayer.Count; i++)
            {
                player = m_ListPlayer[i];
                if (player.Id == playerId)
                {
                    Debuger.LogWarning(LOG_TAG, "AddPlayer() PlayerId已经存在！用新的替代旧的! PlayerId = " + playerId);
                    m_ListPlayer.RemoveAt(i);
                    FSPServer.Instance.DelSession(player.Sid);
                    player.Dispose();
                    break;
                }
            }

            if (m_ListPlayer.Count >= MaxPlayerNum)
            {
                Debuger.LogError(LOG_TAG, "AddPlayer() 已经达到最大玩家数了! MaxPlayerNum = {0}", MaxPlayerNum);
                return false;
            }

            FSPSession session = FSPServer.Instance.AddSession(sid);
            player = new FSPPlayer(playerId, m_FSPParam.serverTimeout, session, OnPlayerReceive);
            m_ListPlayer.Add(player);

            return true;
        }

 
        private FSPPlayer GetPlayer(uint playerId)
        {
            FSPPlayer player = null;
            for (int i = 0; i < m_ListPlayer.Count; i++)
            {
                player = m_ListPlayer[i];
                if (player.Id == playerId)
                {
                    return player;
                }
            }
            return null;
        }

        internal int GetPlayerCount()
        {
            return m_ListPlayer.Count;
        }

        public List<FSPPlayer> GetPlayerList()
        {
            return m_ListPlayer;
        }

        //---------------------------------------------------------
        //收到客户端Player的Cmd
        private void OnPlayerReceive(FSPPlayer player, FSPVKey cmd)
        {
            //防止GC
            if (UseDelayGC)
            {
                m_ListObjectsForDelayGC.Add(cmd);
            }

            HandleClientCmd(player, cmd);
        }


        //---------------------------------------------------------
        
        /// <summary>
        /// 处理来自客户端的 Cmd
        /// 对其中的关键VKey进行处理
        /// 并且收集业务VKey
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cmd"></param>
        protected virtual void HandleClientCmd(FSPPlayer player, FSPVKey cmd)
        {
            uint playerId = player.Id;

            //处理鉴权
            if (!player.HasAuth)
            {
                Debuger.Log(LOG_TAG,"HandleClientCmd() hasAuth = false! Wait AUTH!");
                if (cmd.vkey == FSPVKeyBase.AUTH)
                {
                    Debuger.Log(LOG_TAG, "HandleClientCmd() AUTH, playerId={0}", playerId);
                    player.SetAuth(cmd.args[0]);
                }
                return;
            }
            

            switch (cmd.vkey)
            {
                case FSPVKeyBase.GAME_BEGIN:
                    {
                        Debuger.Log(LOG_TAG, "HandleClientCmd() GAME_BEGIN, playerId = {0}, cmd = {1}", playerId, cmd);
                        SetFlag(playerId, ref m_GameBeginFlag, "m_GameBeginFlag");
                        break;
                    }
                case FSPVKeyBase.ROUND_BEGIN:
                    {
                        Debuger.Log(LOG_TAG, "HandleClientCmd() ROUND_BEGIN, playerId = {0}, cmd = {1}", playerId, cmd);
                        SetFlag(playerId, ref m_RoundBeginFlag, "m_RoundBeginFlag");
                        break;
                    }
                case FSPVKeyBase.CONTROL_START:
                    {
                        Debuger.Log(LOG_TAG, "HandleClientCmd() CONTROL_START, playerId = {0}, cmd = {1}", playerId, cmd);
                        SetFlag(playerId, ref m_ControlStartFlag, "m_ControlStartFlag");
                        break;
                    }
                case FSPVKeyBase.ROUND_END:
                    {
                        Debuger.Log(LOG_TAG, "HandleClientCmd() ROUND_END, playerId = {0}, cmd = {1}", playerId, cmd);
                        SetFlag(playerId, ref m_RoundEndFlag, "m_RoundEndFlag");
                        break;
                    }
                case FSPVKeyBase.GAME_END:
                    {
                        Debuger.Log(LOG_TAG, "HandleClientCmd() GAME_END, playerId = {0}, cmd = {1}", playerId, cmd);
                        SetFlag(playerId, ref m_GameEndFlag, "m_GameEndFlag");
                        break;
                    }
                case FSPVKeyBase.GAME_EXIT:
                    {
						Debuger.Log(LOG_TAG, "HandleClientCmd() GAME_EXIT, playerId = {0}, cmd = {1}", playerId, cmd);
                        HandleGameExit(playerId, cmd);
                        break;
                    }
                default:
                    {
                        Debuger.Log(LOG_TAG, "HandleClientCmd() playerId = {0}, cmd = {1}",playerId, cmd);
                        AddCmdToCurrentFrame(playerId, cmd);
                        break;
                    }
            }


        }



        protected void AddCmdToCurrentFrame(uint playerId, FSPVKey cmd)
        {
            cmd.playerId = playerId;
            m_LockedFrame.vkeys.Add(cmd);
        }

        protected void AddCmdToCurrentFrame(int vkey, int arg = 0)
        {
            FSPVKey cmd = new FSPVKey();
            cmd.vkey = vkey;
            cmd.args = new int[] { arg};
            cmd.playerId = 0;
            AddCmdToCurrentFrame(0, cmd);
        }

        private void HandleGameExit(uint playerId, FSPVKey cmd)
        {
            AddCmdToCurrentFrame(playerId, cmd);
            FSPPlayer player = GetPlayer(playerId);

            if (player != null)
            {
                player.WaitForExit = true;

				if (onGameExit != null) 
				{
					onGameExit (player.Id);
				}
            }
        }




        //---------------------------------------------------------
        /// <summary>
        /// 驱动游戏状态
        /// </summary>
        public void EnterFrame()
        {
            for (int i = 0; i < m_ListPlayersExitOnNextFrame.Count; i++)
            {
                FSPPlayer player = m_ListPlayersExitOnNextFrame[i];
                FSPServer.Instance.DelSession(player.Sid);
                player.Dispose();
            }
            m_ListPlayersExitOnNextFrame.Clear();

            //在这里处理状态
            HandleGameState();

            //经过上面状态处理之后，有可能状态还会发生变化
            if (m_State == FSPGameState.None)
            {
                return;
            }

            if (m_LockedFrame.frameId != 0 || !m_LockedFrame.IsEmpty())
            {
                //将当前帧扔给Player
                for (int i = 0; i < m_ListPlayer.Count; i++)
                {
                    FSPPlayer player = m_ListPlayer[i];
                    player.SendToClient(m_LockedFrame);
                    if (player.WaitForExit)
                    {
                        m_ListPlayersExitOnNextFrame.Add(player);
                        m_ListPlayer.RemoveAt(i);
                        --i;
                    }
                }
            }

            //0帧每个循环需要额外清除掉再重新统计
            if (m_LockedFrame.frameId == 0)
            {
                m_LockedFrame = new FSPFrame();
                //防止GC
                if (UseDelayGC)
                {
                    m_ListObjectsForDelayGC.Add(m_LockedFrame);
                }
            }


            //在这个阶段，帧号才会不停往上加
            if (m_State == FSPGameState.RoundBegin || m_State == FSPGameState.ControlStart)
            {
                m_CurFrameId++;
                m_LockedFrame = new FSPFrame();
                m_LockedFrame.frameId = m_CurFrameId;
                //防止GC
                if (UseDelayGC)
                {
                    m_ListObjectsForDelayGC.Add(m_LockedFrame);
                }
            }
        }


        /// <summary>
        /// 检测游戏是否异常结束
        /// </summary>
        private bool CheckGameAbnormalEnd()
        {
            //判断还剩下多少玩家，如果玩家少于2，则表示至少有玩家主动退出
            if (m_ListPlayer.Count < 2)
            {
                //直接进入GameEnd状态
                SetGameState(FSPGameState.GameEnd, (int)FSPGameEndReason.AllOtherExit);
                AddCmdToCurrentFrame(FSPVKeyBase.GAME_END, (int)FSPGameEndReason.AllOtherExit);
                return true;
            }

            // 检测玩家在线状态
            for (int i = 0; i < m_ListPlayer.Count; i++)
            {
                FSPPlayer player = m_ListPlayer[i];
                if (player.IsLose())
                {
                    m_ListPlayer.RemoveAt(i);
                    FSPServer.Instance.DelSession(player.Sid);
                    player.Dispose();
                    --i;
                }
            }

            //判断还剩下多少玩家，如果玩家少于2，则表示有玩家掉线了
            if (m_ListPlayer.Count < 2)
            {
                //直接进入GameEnd状态
                SetGameState(FSPGameState.GameEnd, (int)FSPGameEndReason.AllOtherLost);
                AddCmdToCurrentFrame(FSPVKeyBase.GAME_END, (int)FSPGameEndReason.AllOtherLost);
                return true;
            }

            return false;
        }


        //设置状态机
        protected void SetGameState(FSPGameState state, int param1 = 0, int param2 = 0)
        {
            Debuger.Log(LOG_TAG,"SetGameState() >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            Debuger.Log(LOG_TAG, "SetGameState() {0} -> {1}, param1 = {2}, param2 = {3}", m_State, state, param1, param2);
            Debuger.Log(LOG_TAG, "SetGameState() <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            m_State = state;
            m_StateParam1 = param1;
            m_StateParam2 = param2;
        }


        //---------------------------------------------------------
        //Tick 状态机驱动
        private void HandleGameState()
        {
            switch (m_State)
            {
                case FSPGameState.None:
                    {
                        //进入这个状态的游戏，马上将会被回收
                        //这里是否要考虑session中的所有消息都发完了？
                        break;
                    }
                case FSPGameState.Create: //游戏刚创建，未有任何玩家加入, 这个阶段等待玩家加入
                    {
                        OnState_Create();
                        break;
                    }
                case FSPGameState.GameBegin: //游戏开始，等待RoundBegin
                    {
                        OnState_GameBegin();
                        break;
                    }
                case FSPGameState.RoundBegin: //回合已经开始，开始加载资源等，等待ControlStart
                    {
                        OnState_RoundBegin();
                        break;
                    }
                case FSPGameState.ControlStart: //在这个阶段可操作，这时候接受游戏中的各种行为包，并等待RoundEnd
                    {
                        OnState_ControlStart();
                        break;
                    }
                case FSPGameState.RoundEnd: //回合已经结束，判断是否进行下一轮，即等待RoundBegin，或者GameEnd
                    {
                        OnState_RoundEnd();
                        break;
                    }
                case FSPGameState.GameEnd://游戏结束
                    {
                        OnState_GameEnd();
                        break;
                    }
                default:
                    break;
            }
        }


        //---------------------------------------------------------
        #region 一系列状态处理函数

        
        /// <summary>
        /// 游戏创建状态
        /// 只有在该状态下，才允许加入玩家
        /// 当所有玩家都发VKey.GameBegin后，进入下一个状态
        /// </summary>
        protected virtual int OnState_Create()
        {
            //如果有任何一方已经鉴权完毕，则游戏进入GameBegin状态准备加载
            if (IsFlagFull(m_GameBeginFlag))
            {
                ResetRoundFlag();
                SetGameState(FSPGameState.GameBegin);
				AddCmdToCurrentFrame(FSPVKeyBase.GAME_BEGIN);
                return 0;
            }
            return 0;
        }

        /// <summary>
        /// 游戏开始状态
        /// 在该状态下，等待所有玩家发VKey.RoundBegin，或者 判断玩家是否掉线
        /// 当所有人都发送VKey.RoundBegin，进入下一个状态
        /// 当有玩家掉线，则从FSPGame中删除该玩家：
        ///     判断如果只剩下1个玩家了，则直接进入GameEnd状态，否则不影响游戏状态
        /// </summary>
        protected virtual int OnState_GameBegin()
        {
            if (CheckGameAbnormalEnd())
            {
                return 0;
            }
            
            if (IsFlagFull(m_RoundBeginFlag))
            {
                SetGameState(FSPGameState.RoundBegin);
                IncRoundId();
                AddCmdToCurrentFrame(FSPVKeyBase.ROUND_BEGIN, m_CurRoundId);

                return 0;
            }

            return 0;
        }

        /// <summary>
        /// 回合开始状态
        /// （这个时候客户端可能在加载资源）
        /// 在该状态下，等待所有玩家发VKey.ControlStart， 或者 判断玩家是否掉线
        /// 当所有人都发送VKey.ControlStart，进入下一个状态
        /// 当有玩家掉线，则从FSPGame中删除该玩家：
        ///     判断如果只剩下1个玩家了，则直接进入GameEnd状态，否则不影响游戏状态
        /// </summary>
        protected virtual int OnState_RoundBegin()
        {
            if (CheckGameAbnormalEnd())
            {
                return 0;
            }

            if (IsFlagFull(m_ControlStartFlag))
            {
                SetGameState(FSPGameState.ControlStart);
                AddCmdToCurrentFrame(FSPVKeyBase.CONTROL_START);
                return 0;
            }

            return 0;
        }


        /// <summary>
        /// 可以开始操作状态
        /// （因为每个回合可能都会有加载过程，不同的玩家加载速度可能不同，需要用一个状态统一一下）
        /// 在该状态下，接收玩家的业务VKey， 或者 VKey.RoundEnd，或者VKey.GameExit
        /// 当所有人都发送VKey.RoundEnd，进入下一个状态
        /// 当有玩家掉线，或者发送VKey.GameExit，则从FSPGame中删除该玩家：
        ///     判断如果只剩下1个玩家了，则直接进入GameEnd状态，否则不影响游戏状态
        /// </summary>
        protected virtual int OnState_ControlStart()
        {
            if (CheckGameAbnormalEnd())
            {
                return 0;
            }

            if (IsFlagFull(m_RoundEndFlag))
            {
                SetGameState(FSPGameState.RoundEnd);
                ClearRound();
                AddCmdToCurrentFrame(FSPVKeyBase.ROUND_END, m_CurRoundId);
                return 0;
            }

            return 0;
        }


        /// <summary>
        /// 回合结束状态
        /// （大部分游戏只有1个回合，也有些游戏有多个回合，由客户端逻辑决定）
        /// 在该状态下，等待玩家发送VKey.GameEnd，或者 VKey.RoundBegin（如果游戏不只1个回合的话）
        /// 当所有人都发送VKey.GameEnd，或者 VKey.RoundBegin时，进入下一个状态
        /// 当有玩家掉线，则从FSPGame中删除该玩家：
        ///     判断如果只剩下1个玩家了，则直接进入GameEnd状态，否则不影响游戏状态
        /// </summary>
        protected virtual int OnState_RoundEnd()
        {
            if (CheckGameAbnormalEnd())
            {
                return 0;
            }


            //这是正常GameEnd
            if (IsFlagFull(m_GameEndFlag))
            {
                SetGameState(FSPGameState.GameEnd, (int)FSPGameEndReason.Normal);
                AddCmdToCurrentFrame(FSPVKeyBase.GAME_END, (int)FSPGameEndReason.Normal);
                return 0;
            }


            if (IsFlagFull(m_RoundBeginFlag))
            {
                SetGameState(FSPGameState.RoundBegin);
                IncRoundId();
                AddCmdToCurrentFrame(FSPVKeyBase.ROUND_BEGIN, m_CurRoundId);
                return 0;
            }


            return 0;
        }


        protected virtual int OnState_GameEnd()
        {
            //到这里就等业务层去读取数据了 
			if (onGameEnd != null) 
			{
				onGameEnd (m_StateParam1);
				onGameEnd = null;
			}
            return 0;
        }

        public bool IsGameEnd()
        {
            return m_State == FSPGameState.GameEnd;
        }

        #endregion


        //--------------------------------------------------------------------
        //Round处理函数
        private int ClearRound()
        {
            m_LockedFrame = new FSPFrame();
            m_CurFrameId = 0;
            
            ResetRoundFlag();

            for (int i = 0; i < m_ListPlayer.Count; i++)
            {
                if (m_ListPlayer[i] != null)
                {
                    m_ListPlayer[i].ClearRound();
                }
            }

            return 0;
        }

        private void ResetRoundFlag()
        {
            m_RoundBeginFlag = 0;
            m_ControlStartFlag = 0;
            m_RoundEndFlag = 0;
            m_GameEndFlag = 0;
        }

        private void IncRoundId()
        {
            ++m_CurRoundId;
        }




        //--------------------------------------------------------------------
        #region Player 状态标志工具函数

        private void SetFlag(uint playerId, ref int flag, string flagname)
        {
            flag |= (0x01 << ((int)playerId - 1));
            Debuger.Log(LOG_TAG, "SetFlag() player = {0}, flag = {1}", playerId, flagname);
        }

        private void ClsFlag(int playerId, ref int flag, string flagname)
        {
            flag &= (~(0x01 << (playerId - 1)));
        }

        public bool IsAnyFlagSet(int flag)
        {
            return flag != 0;
        }

        public bool IsFlagFull(int flag)
        {
			if (m_ListPlayer.Count > 1)
			{
				for (int i = 0; i < m_ListPlayer.Count; i++)
				{
					FSPPlayer player = m_ListPlayer[i];
					int playerId = (int)player.Id;
					if ((flag & (0x01 << (playerId - 1))) == 0)
					{
						return false;
					}
				}
				return true;
			}
			
			return false;
            
        }


        #endregion


    }
}

