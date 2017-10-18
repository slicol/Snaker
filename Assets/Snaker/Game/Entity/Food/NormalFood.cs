using Snaker.Game.Entity.Factory;
using UnityEngine;

namespace Snaker.Game.Entity.Food
{
    public class NormalFood : EntityObject
    {
        //=======================================================================
        private int m_color = 0;
        private int m_type = 0;
        private Vector3 m_pos;
 

        public void Create(int type, int color, Vector3 pos)
        {
            m_type = type;
            m_color = color;
            m_pos = pos;

            ViewFactory.CreateView("food/food_" + m_type, "food/food_0", this);
        }

        protected override void Release()
        {
            ViewFactory.ReleaseView(this);
        }

        public int Color { get { return m_color; } }

		public override Vector3 Position()
		{
			return m_pos;
		}
    }
}
