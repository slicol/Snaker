
using System;
using Snaker.Game.Entity.Snake;
using Snaker.Game.Entity.Factory;
using Snaker.Game.Player;
using UnityEngine;

namespace Snaker.Game.Entity.View.Snake
{

	public class VSnakeNodeGlare : VSnakeNode
	{
	    public enum GlareDirection
	    {
	        Forward,
            Backward,
	    }

	    public GlareDirection direction;

		protected override void Update ()
		{
			base.Update ();

			if (m_renderer != null && m_entity != null)
			{
				Color c = m_renderer.color;

			    if (direction == GlareDirection.Backward)
			    {
			        c.a =
			            Math.Abs(10 -
			                     (m_context.currentFrameIndex +
			                      (int) ((m_entity.Data.length - m_entity.Index)/m_entity.Data.keyStep))%20)/10f;
			    }
			    else
			    {
                    c.a = Math.Abs(10 - (m_context.currentFrameIndex + (int)((m_entity.Index) / m_entity.Data.keyStep)) % 20) / 10f;  
			    }
			    
				m_renderer.color = c;

			}
		}
	}

}