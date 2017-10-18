using SGF;
using Snaker.Game.Data;
using UnityEngine;

namespace Snaker.Game
{
    /// <summary>
    /// 单局游戏的上下文
    /// 用来保存单局中所有逻辑都关心的数据
    /// </summary>
    public class GameContext
    {
        /// <summary>
        /// 游戏的启动参数肯定是大家都关心的
        /// </summary>
        public GameParam param = null;

        /// <summary>
        /// 随机数生成器
        /// </summary>
        public SGFRandom random = new SGFRandom();

        /// <summary>
        /// 当前是第几帧了
        /// </summary>
        public int currentFrameIndex = 0;


        /// <summary>
        /// 地图的大小
        /// </summary>
        public Vector3 mapSize = new Vector3();


        //======================================================================
        private DictionaryEx<int, Color> m_mapColor = new DictionaryEx<int, Color>();
        public Color GetUniqueColor(int colorId)
        {
            if (m_mapColor.ContainsKey(colorId))
            {
                return m_mapColor[colorId];
            }

            Color c = new Color(random.Rnd(), random.Rnd(), random.Rnd());
            m_mapColor.Add(colorId, c);
            return c;
        }

        //======================================================================
        public Vector3 EntityToViewPoint(Vector3 pos)
        {
            pos = pos - mapSize / 2;
            pos.z = 0;

            return pos;
        }
    }
}
