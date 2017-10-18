using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snaker.Game.Data
{
    /// <summary>
    /// 定义游戏单局中可能的玩家操作
    /// </summary>
    public static class GameVKey
    {
        /// <summary>
        /// X方向移动
        /// </summary>
        public const int MoveX = 11;

        /// <summary>
        /// Y方向移动
        /// </summary>
        public const int MoveY = 12;

        /// <summary>
        /// 加速移动
        /// </summary>
        public const int SpeedUp = 1;

        /// <summary>
        /// 在单局中创建一个玩家
        /// </summary>
        public const int CreatePlayer = 20;
    }


}
