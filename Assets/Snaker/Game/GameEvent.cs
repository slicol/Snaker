/// <summary>
/// 定义游戏单局中会向外部模块抛出的事件
/// </summary>


namespace Snaker.Game
{
    /// <summary>
    /// 玩家死亡事件
    /// </summary>
	public delegate void PlayerDieEvent(uint playerId);
}