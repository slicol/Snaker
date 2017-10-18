using System;
using Snaker.Game.Entity.Factory;

namespace Snaker.Game.Player.Component
{
    public abstract class PlayerComponent
    {
        public PlayerComponent(SnakePlayer player)
        {
        }

        public abstract void Release();

        public abstract void EnterFrame(int frameIndex);
    }
}
