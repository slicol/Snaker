using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Snaker.Game.Data
{
    /// <summary>
    /// 单局中蛇的数据，这里要与蛇的配置数据区分
    /// 如果我们要做蛇的成长系统，则需要区分蛇的配置数据和强化后的数据
    /// </summary>
    [ProtoContract]
    public class SnakeData
    {
        /// <summary>
        /// 蛇的ID，表示这是一条什么样的蛇，可以通过这个ID 找到蛇的资源和配置
        /// 为什么不直接使用蛇的配置数据？
        /// 因为，有可能不同的玩家使用了同一条蛇，通过不同的强化后，这条蛇的实际参数会不同
        /// 比如初始长度不同等
        /// </summary>
        [ProtoMember(1)] public int id;

        /// <summary>
        /// 蛇的名号
        /// </summary>
        [ProtoMember(2)] public string name = "";


		/// <summary>
		/// 蛇的大小，即蛇的每一块的肌肉大小。
		/// 这是一个配置值，其用来指导美术资源的制作
		/// </summary>
		[ProtoMember(3)] public int size = 32;

        /// <summary>
        /// 关键结点的步长，即每多少个骨骼，会有一块肌肉。
		/// 根据平滑程度，观察得出取5。
        /// </summary>
		[ProtoMember(4)] public int keyStep = 5;


		/// <summary>
		/// 蛇的长度，即骨骼数量。
		/// 有初始值。初始值对于不同的蛇可能不同，对于相同的蛇在不同玩家手里也可能不同
		/// 随着游戏的进行，该值会增加
		/// </summary>
		[ProtoMember(5)] public int length = 50;



		/// <summary>
		/// 玩家在游戏中因为蛇的长度增加再使视图变大
		/// </summary>
		[ProtoMember(6)] public float viewScale = 1;

		/// <summary>
		/// 蛇的身体是否可见，有些特效需要隐藏身体，而只显示特效 
		/// </summary>
		[ProtoMember(7)] public bool bodyVisible = true;
    }
}
