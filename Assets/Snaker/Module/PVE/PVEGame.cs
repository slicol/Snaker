using System;
using SGF;
using SGF.Unity;
using Snaker.Game;
using Snaker.Game.Data;
using Snaker.Service.User;

namespace Snaker.Module.PVE
{
    public class PVEGame
    {
        private uint m_mainPlayerId = 1;
        private int m_frameIndex = 0;
		private bool m_pause = false;
		public event Action onMainPlayerDie;
		public event Action onGameEnd;
		private GameContext m_context;


        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="param"></param>
		public void Start(GameParam param)
		{
			GameManager.Instance.CreateGame(param);
            GameManager.Instance.onPlayerDie += OnPlayerDie;
			m_context = GameManager.Instance.Context;

			PlayerData pd = new PlayerData();
			pd.id = m_mainPlayerId;
			pd.userId = UserManager.Instance.MainUserData.id;
			pd.snakeData.id = 3;
			pd.snakeData.length = 50;
			pd.ai = 1;
			GameManager.Instance.RegPlayerData(pd);

			//初始化输入
			GameInput.Create();
			GameInput.OnVkey += OnVKey;


			//监听EnterFrame
			MonoHelper.AddFixedUpdateListener(FixedUpdate);

            GameCamera.FocusPlayerId = m_mainPlayerId;
		}


        /// <summary>
        /// 停止游戏
        /// </summary>
		public void Stop()
		{
            MonoHelper.RemoveFixedUpdateListener(FixedUpdate);

            GameInput.Release();

			GameManager.Instance.ReleaseGame ();

			onGameEnd = null;
			onMainPlayerDie = null;
			m_context = null;
		}

        /// <summary>
        /// 暂停游戏
        /// </summary>
		public void Pause()
		{
			m_pause = true;
		}


        /// <summary>
        /// 恢复游戏
        /// </summary>
		public void Resume()
		{
			m_pause = false;
		}


        /// <summary>
        /// 中止游戏
        /// </summary>
		public void Terminate()
		{
			Pause();

			if (onGameEnd != null)
			{
				onGameEnd();
			}
		}

        /// <summary>
        /// 创建玩家
        /// </summary>
		public void CreatePlayer()
		{
			GameManager.Instance.InputVKey(GameVKey.CreatePlayer, 0, m_mainPlayerId);
			
		}

        /// <summary>
        /// 重生玩家
        /// </summary>
		public void RebornPlayer()
		{
			CreatePlayer();
		}


		//--------------------------------------------------
        /// <summary>
        /// 收到来自GameInput的输入
        /// </summary>
        /// <param name="vkey"></param>
        /// <param name="arg"></param>
		private void OnVKey(int vkey, float arg)
		{
			GameManager.Instance.InputVKey(vkey, arg, m_mainPlayerId);
		}

        /// <summary>
        /// 驱动游戏逻辑循环
        /// </summary>
		private void FixedUpdate()
		{
			if (m_pause)
			{
				return;
			}


			m_frameIndex++;

			GameManager.Instance.EnterFrame(m_frameIndex);

			CheckTimeEnd ();
		}

        /// <summary>
        /// 检测游戏是否限时结束
        /// </summary>
		private void CheckTimeEnd()
		{
			if (IsTimelimited)
			{
				if (GetRemainTime() <= 0)
				{
					Terminate();
				}
			}
		}

        /// <summary>
        /// 是否为限时模式
        /// </summary>
		public bool IsTimelimited
		{
			get
			{
				return m_context.param.mode == GameMode.TimelimitPVE;
			}
		}

        /// <summary>
        /// 如果是限时模式，还剩下多少时间
        /// </summary>
        /// <returns></returns>
		public int GetRemainTime()
		{
			if (m_context.param.mode == GameMode.TimelimitPVE)
			{
				return (int)(m_context.param.limitedTime - m_context.currentFrameIndex * 0.033333333);
			}
			return 0;
		}


        /// <summary>
        /// 游戏经过了多少时间
        /// </summary>
        /// <returns></returns>
		public int GetElapsedTime()
		{
			return (int)(m_context.currentFrameIndex * 0.033333333f);
		}


		//--------------------------------------------------
        /// <summary>
        /// 当玩家死亡时，进行处理
        /// </summary>
        /// <param name="playerId"></param>
		private void OnPlayerDie(uint playerId)
		{
			if (m_mainPlayerId == playerId)
			{
				Pause();

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


    }
}
