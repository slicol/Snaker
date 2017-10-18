
using Snaker.Module.PVE;

using Snaker.UI;
using Snaker.Game.Data;
using SGF.Module.Framework;
using SGF.UI.Framework;

namespace Snaker.Module
{
    public class PVEModule:BusinessModule
    {
        private PVEGame m_game;

        //显示模块的主UI
        protected override void Show(object arg)
        {
            base.Show(arg);

            int model = (int)arg;

            //TODO 显示关卡选择UI

            //但是现在，我们的目的是为了核心玩法，所以直接启动游戏
            StartGame(model);
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="mode"></param>
        private void StartGame(int mode)
        {
			GameParam param = new GameParam ();
			param.mode = (GameMode)mode;
			param.limitedTime = 10;

            m_game = new PVEGame();
			m_game.Start(param);
			m_game.onGameEnd += () =>
			{
			    StopGame();
			};

            //打开战斗UI
            UIManager.Instance.OpenPage(UIDef.UIPVEGamePage);
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
        }

        public PVEGame GetCurrentGame()
        {
            return m_game;
        }

    }
}
