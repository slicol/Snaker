

using System;
using System.Collections.Generic;
using SGF;
using SGF.Network.FSPLite;
using SGF.Network.FSPLite.Client;
using SGF.Unity;
using Snaker.Game;
using Snaker.Game.Data;
using Snaker.Module.PVP.Data;
using Snaker.Service.User;
using Snaker.Service.UserManager.Data;
using UnityEngine;

namespace Snaker.Module.PVP
{

    /// <summary>
    ///  游戏逻辑
    /// </summary>
	public class PVPGame
	{
		public string LOG_TAG = "PVPGame";

		private FSPManager m_mgrFSP;
        private List<PlayerData> m_listPlayerData;
		private uint m_mainPlayerId;

		public event Action onMainPlayerDie;
		public event Action onGameEnd;
        private GameContext m_context;

		//--------------------------------------------------

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="param"></param>
		public void Start(PVPStartParam param)
		{
			Debuger.Log(LOG_TAG, "StartGame() param:{0}", param);

			
			UserData mainUserData = UserManager.Instance.MainUserData;
			m_listPlayerData = param.players;
			for (int i = 0; i < m_listPlayerData.Count; i++)
			{
				if (m_listPlayerData[i].userId == mainUserData.id)
				{
                    m_mainPlayerId = m_listPlayerData[i].id;
                    GameCamera.FocusPlayerId = m_mainPlayerId;
				}

                //注册玩家数据，为在帧同步过程中创建玩家提供数据
                //因为帧同步协议非常精简，不包含具体的玩家数据
				GameManager.Instance.RegPlayerData(m_listPlayerData[i]);
			}

			//启动游戏逻辑
			GameManager.Instance.CreateGame(param.gameParam);
			GameManager.Instance.onPlayerDie += OnPlayerDie;//有玩家死亡
            m_context = GameManager.Instance.Context;

			//启动帧同步逻辑
			m_mgrFSP = new FSPManager();
            m_mgrFSP.Start(param.fspParam, m_mainPlayerId);
			m_mgrFSP.SetFrameListener(OnEnterFrame);
			m_mgrFSP.onGameBegin += OnGameBegin;//游戏开始
            m_mgrFSP.onGameExit += OnGameExit;//有玩家退出
			m_mgrFSP.onRoundEnd += OnRoundEnd;//有玩家退出
            m_mgrFSP.onGameEnd += OnGameEnd;//游戏结束
            

			//初始化输入
			GameInput.Create();
			GameInput.OnVkey += OnVKey;

			//监听EnterFrame
			MonoHelper.AddFixedUpdateListener(FixedUpdate);

			GameBegin ();
		}


        /// <summary>
        /// 停止游戏 
        /// </summary>
		public void Stop()
		{
			Debuger.Log(LOG_TAG, "StopGame()");

			GameManager.Instance.ReleaseGame();

			MonoHelper.RemoveFixedUpdateListener(FixedUpdate);
			GameInput.Release();

			if (m_mgrFSP != null)
			{
				m_mgrFSP.Stop();
				m_mgrFSP = null;
			}

			onMainPlayerDie = null;
			onGameEnd = null;
            m_context = null;
		}


        /// <summary>
        /// 中止游戏
        /// </summary>
		public void GameExit()
		{
			Debuger.Log(LOG_TAG, "GameExit()");
            //因为PVP 模式中，还有其它玩家在玩，
            //所以应该只是让自己退出游戏
            m_mgrFSP.SendGameExit();
		}


		public void GameBegin()
		{
			Debuger.Log(LOG_TAG, "GameBegin()");
			m_mgrFSP.SendGameBegin();
		}

		private void OnGameBegin(int arg)
		{
			Debuger.Log(LOG_TAG, "OnGameBegin()");
			RoundBegin ();
		}

        /// <summary>
        /// 回合开始
        /// </summary>
		public void RoundBegin()
		{
			Debuger.Log(LOG_TAG, "RoundBegin()");
            m_mgrFSP.SendRoundBegin();
		}

        /// <summary>
        /// 回合结束
        /// </summary>
		public void RoundEnd()
		{
			Debuger.Log(LOG_TAG, "RoundEnd()");
            m_mgrFSP.SendRoundEnd();
		}

		private void OnRoundEnd(int arg)
		{
			Debuger.Log(LOG_TAG, "OnRoundEnd()");
			GameEnd ();
		}

		public void GameEnd()
		{
			Debuger.Log(LOG_TAG, "GameEnd()");
			m_mgrFSP.SendGameEnd();
		}


        /// <summary>
        /// 创建玩家
        /// </summary>
		public void CreatePlayer()
		{
			Debuger.Log(LOG_TAG, "CreatePlayer()");
			m_mgrFSP.SendFSP(GameVKey.CreatePlayer);
		}


        /// <summary>
        /// 重生玩家
        /// </summary>
		public void RebornPlayer()
		{
			Debuger.Log(LOG_TAG, "RebornPlayer()");

			m_mgrFSP.SendFSP(GameVKey.CreatePlayer);
		}
        

		//--------------------------------------------------
        /// <summary>
        /// 来自GameInput的输入
        /// </summary>
        /// <param name="vkey"></param>
        /// <param name="arg"></param>
		private void OnVKey(int vkey, float arg)
		{
			m_mgrFSP.SendFSP(vkey, (int)(arg * 10000));
		}

        /// <summary>
        /// 驱动游戏循环
        /// </summary>
		private void FixedUpdate()
		{
			m_mgrFSP.EnterFrame();
		}

		private void OnEnterFrame(int frameId, FSPFrame frame)
		{
			GameManager.Instance.EnterFrame(frameId);

			if (frame != null && frame.vkeys != null)
			{
				for (int i = 0; i < frame.vkeys.Count; i++)
				{
					FSPVKey cmd = frame.vkeys[i];
					GameManager.Instance.InputVKey(cmd.vkey, ((float)cmd.args[0]) / 10000.0f, cmd.playerId);
				}
			}

            CheckTimeEnd();
		}

        /// <summary>
        /// 检测是否限时结束
        /// </summary>
		private void CheckTimeEnd()
		{
			if (IsTimelimited)
			{
				if (GetRemainTime() <= 0)
				{
					RoundEnd();
				}
			}
		}


        /// <summary>
        /// 是否限时模式
        /// </summary>
		public bool IsTimelimited
		{
			get
			{
				return m_context.param.mode == GameMode.TimelimitPVP;
			}
		}

        /// <summary>
        /// 如果是限时模式，还剩下多少时间
        /// </summary>
        /// <returns></returns>
		public int GetRemainTime()
		{
			if (m_context.param.mode == GameMode.TimelimitPVP)
			{
				return (int)(m_context.param.limitedTime - m_context.currentFrameIndex * 0.033333333);
			}
			return 0;
		}

        /// <summary>
        /// 游戏已经经过多少时间
        /// </summary>
        /// <returns></returns>
		public int GetElapsedTime()
		{
			return (int)(m_context.currentFrameIndex * 0.033333333f);
		}


		//--------------------------------------------------

        /// <summary>
        /// 有玩家死了
        /// </summary>
        /// <param name="playerId"></param>
		private void OnPlayerDie(uint playerId)
		{
            //当死亡的自己时
			if (m_mainPlayerId == playerId)
			{
				if (onMainPlayerDie != null)
				{
					onMainPlayerDie();
				}
				else
				{
					this.LogError("OnPlayerDie() onMainPlayerDie == null!");
				}
			}
		}


        /// <summary>
        /// 当有玩家退出时
        /// </summary>
        /// <param name="playerId"></param>
        private void OnGameExit(uint playerId)
        {
            //当退出的是自己时
            if (m_mainPlayerId == playerId)
            {
                if (onGameEnd != null)
                {
                    onGameEnd();
                }                
            }

            
        }

        private void OnGameEnd(int arg)
        {
            if (onGameEnd != null)
            {
                onGameEnd();
            }
        }
	}
}
