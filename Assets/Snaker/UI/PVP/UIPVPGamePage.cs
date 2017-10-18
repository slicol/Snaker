
using SGF.Module.Framework;
using SGF.Time;
using SGF.UI.Framework;
using Snaker.Module;
using Snaker.Module.PVP;
using Snaker.Service.User;
using UnityEngine.UI;

namespace Snaker.PVP.UI
{
	public class UIPVPGamePage : UIPage
	{
		public Text txtUserInfo;
		public Button btnReady;
		public Text txtTimeInfo;

		private PVPGame m_game;

		protected override void OnOpen(object arg)
		{
			base.OnOpen(arg);

			PVPModule module = ModuleManager.Instance.GetModule(ModuleDef.PVPModule) as PVPModule;
			m_game = module.GetGame();
			m_game.onMainPlayerDie += OnMainPlayerDie;
			m_game.onGameEnd += OnGameEnd;

			txtUserInfo.text = UserManager.Instance.MainUserData.name;
			txtTimeInfo.text = "";

		}

		protected override void OnClose(object arg)
		{
			m_game = null;
			base.OnClose(arg);
		}



		public void OnBtnReady()
		{
			UIUtils.SetActive(btnReady, false);

			m_game.CreatePlayer();
		}



		private void OnMainPlayerDie()
		{
			UIAPI.ShowMsgBox("死亡！！！", "是否重生继续游戏？", "确定退出|继续游戏", (arg) =>
			{
				if ((int)arg == 0)
				{
					m_game.GameExit();
				}
				else
				{
					m_game.RebornPlayer();
				}
			});

		}

		private void OnGameEnd()
		{
			m_game = null;

			UIAPI.ShowMsgBox("游戏结束", "显示游戏积分...", "确定", (arg) =>
			{
				UIManager.Instance.GoBackPage();
			});

		}



		void Update()
		{
			if (m_game != null)
			{
				int time = 0;
				if (m_game.IsTimelimited)
				{
					time = m_game.GetRemainTime();//second

				}
				else
				{
					time = m_game.GetElapsedTime();
				}

				txtTimeInfo.text = TimeUtils.GetTimeString("%hh:%mm:%ss", time);
			}
		}

	}
}
