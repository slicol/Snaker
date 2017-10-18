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
    /// 游戏模式
    /// </summary>
    public enum GameMode
    {
        EndlessPVE,
        TimelimitPVE,
        EndlessPVP,
        TimelimitPVP
    }
}
