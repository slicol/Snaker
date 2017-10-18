
using System;
using Snaker.Game.Entity.Snake;
using Snaker.Game.Entity.Factory;
using Snaker.Game.Player;
using UnityEngine;

namespace Snaker.Game.Entity.View.Snake
{

	public class VSnakeHeadInk : VSnakeNode
	{
		protected ParticleSystem m_ps;


		protected override void Create (EntityObject entity)
		{
			base.Create (entity);

			m_ps = this.GetComponentInChildren<ParticleSystem> ();

			UnityEngine.ParticleSystem.MainModule main = m_ps.main;
			main.startLifetimeMultiplier = m_entity.Data.length * 0.033f;

			m_entity.Data.bodyVisible = false;
		}

		protected override void Release ()
		{
			m_ps = null;
			base.Release ();
		}

		protected override void Update ()
		{
			base.Update ();

			if (m_ps != null && m_entity != null)
			{
				if (m_ps.main.startLifetimeMultiplier != m_entity.Data.length * 0.033f)
				{
					UnityEngine.ParticleSystem.MainModule main = m_ps.main;
					main.startLifetimeMultiplier = m_entity.Data.length * 0.033f;
				}
			}
		}
	}

}