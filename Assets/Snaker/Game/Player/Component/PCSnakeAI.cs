using System;
using System.Collections.Generic;
using Snaker.Game.Data;
using Snaker.Game.Entity.Snake;
using Snaker.Game.Entity.Factory;
using UnityEngine;

namespace Snaker.Game.Player.Component
{
    public class PCSnakeAI:PlayerComponent
    {
        private SnakePlayer m_player;
        private int m_lastActionFrame = 0;
        
        private GameContext m_context;

        public PCSnakeAI(SnakePlayer player) : base(player)
		{
            m_player = player as SnakePlayer;
			m_context = GameManager.Instance.Context;
            
			RandomDirection();
		}
			
        public override void Release()
        {
            m_player = null;
        }


        public override void EnterFrame(int frameIndex)
        {
            //先做一个比较傻的AI
            //每隔5秒钟改变一下方向
            float dt = frameIndex - m_lastActionFrame;
            if (dt > 150)
            {
                m_lastActionFrame = frameIndex;
                RandomDirection();
            }
            else if (dt > 30)
            {
                if (FindBound())
                {
                    m_lastActionFrame = frameIndex;
                    ReverseDirection();
                }
                else if (FindEnemies())
                {
                    m_lastActionFrame = frameIndex;
                    ReverseDirection();
                }
                else if (TryEatFood())
                {
                    m_lastActionFrame = frameIndex;
                }
            }
        }

        private bool FindBound()
        {
			int testDistance = m_player.SnakeData.size*4;

            Rect rect = new Rect(testDistance, testDistance, 
                m_context.mapSize.x - testDistance, 
                m_context.mapSize.y - testDistance);

            if (!rect.Contains(m_player.Head.Position()))
            {
                return true;
            }
            return false;
        }

        private bool FindEnemies()
        {
            List<SnakePlayer> listSnake = GameManager.Instance.GetPlayerList();
			int testDistance = m_player.SnakeData.size * 4;

            for (int j = 0; j < listSnake.Count; j++)
            {
                SnakePlayer other = listSnake[j];
                if (m_player != other)
                {
                    if (m_player.HitTest(other, testDistance))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryEatFood()
        {
            List<EntityObject> listFoods = GameManager.Instance.GetFoodList();
			int testDistance = m_player.SnakeData.size * 8;
            for (int j = 0; j < listFoods.Count; j++)
            {
                EntityObject food = listFoods[j];
                if (m_player.HitTest(food, testDistance))
                {
                    MoveTo(food.Position());
                    return true;
                }
            }
            return false;
        }


        private void RandomDirection()
        {
            m_player.InputVKey(GameVKey.MoveX, m_context.random.Range(-1f, 1f));
            m_player.InputVKey(GameVKey.MoveY, m_context.random.Range(-1f, 1f));
        }

        private void ReverseDirection()
        {
			var pos = -m_player.MoveDirection.normalized;
			m_player.InputVKey(GameVKey.MoveX,m_context.random.Range(0, pos.x));
			m_player.InputVKey(GameVKey.MoveY,m_context.random.Range(0, pos.y));
        }

        private void MoveTo(Vector3 pos)
        {
            Vector3 dir = pos - m_player.Head.Position();
            dir = dir.normalized;
            m_player.InputVKey(GameVKey.MoveX, dir.x);
            m_player.InputVKey(GameVKey.MoveY, dir.y);
        }
    }
}
