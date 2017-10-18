
using System;
using Snaker.Game.Entity.Snake;
using Snaker.Game.Entity.Factory;
using Snaker.Game.Player;
using UnityEngine;

namespace Snaker.Game.Entity.View.Snake
{
    public class VSnakeNode:ViewObject
    {
		[SerializeField]
		protected Vector3 m_entityPosition;

		protected SnakeNode m_entity;
		protected GameContext m_context;
		protected bool m_visible = true;
		protected SpriteRenderer m_renderer;

        protected override void Create(EntityObject entity)
        {
            m_entity = entity as SnakeNode;
			m_context = GameManager.Instance.Context;
			m_visible = true;

			m_renderer = this.GetComponent<SpriteRenderer>();
            if (m_renderer == null)
            {
                m_renderer = this.GetComponentInChildren<SpriteRenderer>();
            }

            if (m_renderer != null)
            {
				m_renderer.enabled = true;
				m_renderer.color = m_context.GetUniqueColor(m_entity.TeamId);
                if (m_entity.Index > 0)
                {
					m_renderer.sortingOrder = 10000 - m_entity.Index;
                }
                else
                {
                    if (m_entity is SnakeTail)
                    {
						m_renderer.sortingOrder = 0;
                    }
                    else if (m_entity is SnakeHead)
                    {
						m_renderer.sortingOrder = 10000;
                    }
                }
            }
            


        }



        protected override void Release()
        {
            m_entity = null;
			m_context = null;
        }

        protected virtual void Update()
        {
			if (m_entity != null && m_renderer != null)
            {
				m_renderer.enabled = m_entity.Data.bodyVisible;

				m_entityPosition = m_entity.Position ();
				Vector3 pos = m_context.EntityToViewPoint(m_entityPosition);
                this.transform.localPosition = pos;
                this.transform.localEulerAngles = m_entity.EulerAngles;
                float viewScale = m_entity.Data.viewScale;
				this.transform.localScale = new Vector3(viewScale, viewScale, 1f);
                //this.transform.rotation = m_Model.Rotation;
            }
        }
    }
}
