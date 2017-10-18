using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using UnityEngine;

namespace Snaker.Game.Data
{
    [ProtoContract]
    public class MapData
    {
        /// <summary>
        /// 地图的ID，通过ID 可以找到地图的资源
        /// </summary>
        [ProtoMember(1)] public int id = 0;
        /// <summary>
        /// 地图的名字，用于在UI中显示
        /// </summary>
		[ProtoMember(2)] public string name = "";

    }
}
