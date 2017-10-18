using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Snaker.Game.Data;
using UnityEngine;

namespace Snaker.Game.Data
{
    /// <summary>
    /// 游戏的启动参数
    /// </summary>
    [ProtoContract]
    public class GameParam
    {
		/// <summary>
		/// GameId,服务上可能同时开始多场游戏
		/// 每一局游戏都有一个编号，在PVP会用到
		/// </summary>
		[ProtoMember(1)] public int id = 0;

        /// <summary>
        /// 地图数据，决定这场游戏用哪个地图
        /// </summary>
        [ProtoMember(2)] public MapData mapData = new MapData();


        /// <summary>
        /// 随机数种子，用于在不同的客户端产生相同的随机数
        /// </summary>
        [ProtoMember(3)] public int randSeed = 0;


		/// <summary>
		/// 游戏的模式
		/// </summary>
		[ProtoMember(4)] public GameMode mode = GameMode.EndlessPVE;


		/// <summary>
		/// 限时模式的时间
		/// </summary>
		[ProtoMember(5)] public int limitedTime = 180;//Second
    }
}
