using System;
using UnityEngine;


namespace Snaker.Game.Entity.Factory
{
    public abstract class ViewObject : MonoBehaviour, IRecyclableObject
    {
        //----------------------------------------------------------------------
        private string m_recycleType;


        //----------------------------------------------------------------------
        /// <summary>
        /// Creates the in factory.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="recycleType">有些View的类型是资源名，有些是类名.</param>
        internal void CreateInFactory(EntityObject entity, string recycleType)
        {
            m_recycleType = recycleType;

            Create(entity);
        }


		protected abstract void Create(EntityObject entity);


        //----------------------------------------------------------------------
        internal void ReleaseInFactory()
		{
			Release();
		}

		protected abstract void Release();


        //----------------------------------------------------------------------

        public string GetRecycleType()
        {
            return m_recycleType;
        }

        public void Dispose()
        {
            try
            {
                GameObject.Destroy(this.gameObject);
            }
            catch (Exception e)
            {
                
            }
        }
    }

}
