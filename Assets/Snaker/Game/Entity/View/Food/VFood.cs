using Snaker.Game.Entity.Food;
using Snaker.Game.Entity.Factory;
using UnityEngine;

namespace Snaker.Game.Entity.View.Food
{
    public class VFood:ViewObject
    {
		[SerializeField]
		private Vector3 m_entityPosition;

        private NormalFood m_entity;

        protected override void Create(EntityObject entity)
        {
            m_entity = entity as NormalFood;
            
            SpriteRenderer r = this.GetComponent<SpriteRenderer>();
            if (r != null)
            {
                r.color = GameManager.Instance.Context.GetUniqueColor(m_entity.Color);
            }

			m_entityPosition = m_entity.Position ();
			Vector3 pos = GameManager.Instance.Context.EntityToViewPoint(m_entityPosition);
            this.transform.localPosition = pos;
        }

        protected override void Release()
        {
            m_entity = null;
        }


    }
}
