using System;
namespace Snaker.Game.Entity.Snake
{
    public class SnakeTail : SnakeNode
    {
        private SnakeNode m_prev;
        public SnakeNode Prev { get { return m_prev; } }

        public void SetPrev(SnakeNode node)
        {
            m_prev = node;
        }

        protected override void Release()
        {
            base.Release();
            m_prev = null;
        }
    }
}
