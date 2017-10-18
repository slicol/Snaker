using System;
using SGF;
using Snaker.Game.Data;
using Snaker.Game.Entity.Factory;
using Snaker.Game.Player;
using UnityEngine;

namespace Snaker.Game.Entity.Snake
{
    public class SnakeNode : EntityObject
    {
        //==================================================================
        protected SnakeData m_data;
        protected PlayerData m_playerData;
        protected SnakeNode m_next;
        protected Vector3 m_pos;
        protected Vector3 m_angles;
        protected int m_index;

        public Action OnBlast;

        public void Create(int index, PlayerData playerData, Transform container)
        {
            m_playerData = playerData;
            m_data = playerData.snakeData;
            m_index = index;

            if (IsKeyNode())
            {
                if (this is SnakeHead)
                {
                    ViewFactory.CreateView("snake/" + m_data.id + "/head", "snake/0/head", this, container);
                }
                else if(this is SnakeTail)
                {
                    ViewFactory.CreateView("snake/" + m_data.id + "/tail", "snake/0/tail", this, container);
                }
                else
                {
                    ViewFactory.CreateView("snake/" + m_data.id + "/node", "snake/0/node", this, container);
                }
            }
        }

        protected override void Release()
        {
			if (IsKeyNode())
            {
                ViewFactory.ReleaseView(this);
            }

            m_next = null;
            m_data = null;
            m_playerData = null;
        }

		public bool IsKeyNode()
        {
			return m_index% (m_data.keyStep) == 0;
        }

        //==================================================================

        internal void MoveTo(Vector3 pos)
        {
            Vector3 oldPos = m_pos;
            m_pos = pos;

            Vector3 dir = m_pos - oldPos;
            m_angles.z = (float)(Math.Atan2(dir.y, dir.x) * 180 / Math.PI) - 90;

            if (m_next != null)
            {
                m_next.MoveTo(oldPos);
            }
        }

        public override Vector3 Position() 
        { 
            return m_pos; 
        }

        internal void SetNext(SnakeNode node)
        {
            m_next = node;
            m_next.MoveTo(m_pos);
        }

        internal SnakeNode Next{get { return m_next; }}
        public int Index { get { return m_index; } }
		public SnakeData Data{get{ return m_data;}}
        public Vector3 EulerAngles { get { return m_angles; } }
        public int TeamId{ get { return m_playerData.teamId; } }


        //==================================================================

        internal void Blast()
        {
            if (OnBlast != null)
            {
                OnBlast();
            }
        }
        //==================================================================


    }




}
