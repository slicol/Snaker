using Snaker.Game.Data;
using SGF.Module.Framework;
using SGF.Network.FSPLite;
using SGF.UI.Framework;
using Snaker.Module.PVP;
using Snaker.Module.PVP.Data;
using Snaker.Service.User;

namespace Snaker.Module
{
	public class PVPModule : BusinessModule
	{
		private PVPGame m_game;
        private PVPRoom m_room;


        /// <summary>
        /// 显示PVP模块的主UI
        /// </summary>
        /// <param name="arg"></param>
	    protected override void Show(object arg)
	    {
	        base.Show(arg);

            //直接打开房间
            OpenRoom();
	    }


        /// <summary>
        /// 打开房间
        /// </summary>
	    private void OpenRoom()
		{
            //创建房间逻辑
            m_room = new PVPRoom();
            m_room.Create();

            //房间通知游戏开始
            m_room.onNotifyGameStart += (param) =>
		    {
		        StartGame(param);
		    };

            //显示房间UI
            UIManager.Instance.OpenPage(UIDef.UIPVPRoomPage);
		}

        /// <summary>
        /// 关闭房间
        /// </summary>
	    public void CloseRoom()
	    {
	        if (m_room != null)
	        {
                m_room.Release();
	            m_room = null;
	        }

            //返回上一个UI
            UIManager.Instance.GoBackPage();
        }

        public PVPRoom GetRoom()
        {
            return m_room;
        }


        

        //----------------------------------------------------------------------
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="param"></param>
        private void StartGame(PVPStartParam param)
        {
            //创建游戏逻辑
			m_game = new PVPGame();
            m_game.Start(param);

            //当游戏结束时
            m_game.onGameEnd += () =>
            {
                StopGame();
            };

            //显示战斗UI
			UIManager.Instance.OpenPage(UIDef.UIPVPGamePage);
        }

        /// <summary>
        /// 停止游戏
        /// </summary>
        private void StopGame()
        {
			if (m_game != null)
			{
				m_game.Stop();
				m_game = null;
			}

			if (m_room != null) 
			{
				m_room.CancelReady ();
			}

			ModuleManager.Instance.SendMessage (ModuleDef.HostModule, "ReStart");
        }

        public PVPGame GetGame()
        {
            return m_game;
        }


        /// <summary>
        /// 用于本地测试
        /// </summary>
        public void StartLocalTest()
        {
			//战斗参数
			GameParam gameParam = new GameParam();
			gameParam.mapData.id = 0;
			gameParam.mode = GameMode.EndlessPVP;

			//帧同步参数
			FSPParam fspParam = new FSPParam();
			fspParam.useLocal = true;
			fspParam.sid = 1;


			//玩家参数
			PlayerData playerData = new PlayerData();
			playerData.userId = UserManager.Instance.MainUserData.id;
			playerData.id = 1;

			PVPStartParam param = new PVPStartParam();
			param.fspParam = fspParam;
			param.gameParam = gameParam;
			param.players.Add(playerData);

            StartGame(param);
        }


	}
}
