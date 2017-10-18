


using SGF.Module.Framework;
using SGF.Time;
using SGF.UI.Framework;
using Snaker.Module;
using Snaker.Module.PVE;
using Snaker.Service.User;
using Snaker.UI;
using UnityEngine.UI;

namespace Snaker.PVE.UI
{
    public class UIPVEGamePage:UIPage
    {
		public Text txtUserInfo;
		public Button btnReady;
		public Text txtTimeInfo;

        private PVEGame m_game; 

        /// <summary>
        /// 当UI打开时
        /// </summary>
        /// <param name="arg"></param>
		protected override void OnOpen (object arg)
		{
			base.OnOpen (arg);

            //需要监听游戏状态
            PVEModule module = ModuleManager.Instance.GetModule(ModuleDef.PVEModule) as PVEModule;
            m_game = module.GetCurrentGame();
			m_game.onMainPlayerDie += OnMainPlayerDie;
			m_game.onGameEnd += OnGameEnd;

			txtUserInfo.text = UserManager.Instance.MainUserData.name;
			txtTimeInfo.text = "";

		}

		protected override void OnClose (object arg)
		{
			m_game = null;
			base.OnClose (arg);
		}


        /// <summary>
        /// 玩家在进入游戏场景后，需要主动点“Ready”才能创建玩家
        /// </summary>
		public void OnBtnReady()
		{
			UIUtils.SetActive(btnReady, false);
            m_game.CreatePlayer();


		}

        /// <summary>
        /// 暂停按钮
        /// </summary>
		public void OnBtnPauseGame()
		{
			m_game.Pause();

            UIAPI.ShowMsgBox("暂停", "是否退出这一局游戏？","确定退出|继续游戏",(arg) => 
            {
                if((int)arg == 0)
                {
                    m_game.Terminate();
                }
                else
                {
                    m_game.Resume();
                }
            });
			
        }

        /// <summary>
        /// 当主玩家死时
        /// </summary>
		private void OnMainPlayerDie()
		{
            m_game.Pause();

			UIAPI.ShowMsgBox("死亡！！！", "是否重生继续游戏？", "确定退出|继续游戏", (arg) =>
			{
				if ((int)arg == 0)
				{
                    //中止游戏
					m_game.Terminate();
				}
				else
				{
                    //恢复和重生游戏
					m_game.Resume();
                    m_game.RebornPlayer();
				}
			});

		}


        /// <summary>
        /// 当游戏结束时
        /// </summary>
		private void OnGameEnd()
		{
			m_game = null;

			UIAPI.ShowMsgBox("游戏结束", "显示游戏积分...", "确定", (arg) =>
			{
                UIManager.Instance.GoBackPage();
			});
			
		}


        /// <summary>
        /// 在战斗UI上实时显示游戏时间
        /// </summary>
		void Update()
		{
			if (m_game != null) 
			{
				int time;
				if (m_game.IsTimelimited) 
				{
					time = m_game.GetRemainTime ();//second

				} 
				else 
				{
					time = m_game.GetElapsedTime ();
				}

				txtTimeInfo.text = TimeUtils.GetTimeString ("%hh:%mm:%ss", time);
			}
		}

    }
}
