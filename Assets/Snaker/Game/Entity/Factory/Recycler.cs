using System;
using System.Collections.Generic;

namespace Snaker.Game.Entity.Factory
{
    public interface IRecyclableObject
    {
        string GetRecycleType();
        void Dispose();
    }
    
    public class Recycler
    {
        /// <summary>
        /// 被工厂回收的空闲对象列表
        /// </summary>
        private static DictionaryEx<string, Stack<IRecyclableObject>> m_poolIdleObject;

        public Recycler()
        {
            m_poolIdleObject = new DictionaryEx<string, Stack<IRecyclableObject>>();
        }

        public void Release()
        {
            foreach (var pair in m_poolIdleObject)
            {
                foreach (var obj in pair.Value)
                {
                    obj.Dispose();
                }
                pair.Value.Clear();
            }

        }

        public void Push(IRecyclableObject obj)
        {
            string type = obj.GetRecycleType();
            Stack<IRecyclableObject> stackIdleObject = m_poolIdleObject[type];
            if (stackIdleObject == null)
            {
                stackIdleObject = new Stack<IRecyclableObject>();
                m_poolIdleObject.Add(type, stackIdleObject);
            }
            stackIdleObject.Push(obj);
        }

        public IRecyclableObject Pop(string type)
        {
            Stack<IRecyclableObject> stackIdleObject = m_poolIdleObject[type];
            if (stackIdleObject != null && stackIdleObject.Count > 0)
            {
                return stackIdleObject.Pop();
            }
            return null;
        }
    }
}
