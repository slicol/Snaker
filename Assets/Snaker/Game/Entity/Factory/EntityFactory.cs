

using System;
using System.Collections.Generic;
using SGF;

namespace Snaker.Game.Entity.Factory
{
	//==========================================================
	/// <summary>
	/// 实体工厂，用于创建实体对象
	/// 以及后续可能对其进行回收和重复利用
	/// </summary>
	public static class EntityFactory
	{
		public static bool EnableLog = false;
		private static string LOG_TAG = "EntityFactory";
		
        private static bool m_isInit = false;
        private static Recycler m_recycler;

		/// <summary>
		/// 工厂所实例化的对象列表
		/// </summary>
        private static List<EntityObject> m_listObject;

        public static void Init()
        {
            if (m_isInit)
            {
                return;
            }
            m_isInit = true;

            m_listObject = new List<EntityObject>();
            m_recycler = new Recycler();

        }

		/// <summary>
		/// 释放工厂所创建的所有对象，包括空闲的对象
		/// </summary>
		public static void Release()
		{
            m_isInit = false;

			for (int i = 0; i < m_listObject.Count; i++)
			{
				m_listObject[i].ReleaseInFactory();
                m_listObject[i].Dispose();
			}
			m_listObject.Clear();

            m_recycler.Release();
			
		}


        /// <summary>
        /// 实例化一个实体对象
        /// </summary>
        /// <returns>The entity.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T InstanceEntity<T>() where T : EntityObject, new()
		{
			EntityObject obj = null;
			bool useRecycler = true;

			//先从回收池中寻找
			Type type = typeof(T);
            obj = m_recycler.Pop(type.FullName) as EntityObject;
            if(obj == null)
            {
				useRecycler = false;
                obj = new T();
            }
			obj.InstanceInFactory ();

			m_listObject.Add(obj);

			if (EnableLog && Debuger.EnableLog) 
			{
				Debuger.Log (LOG_TAG, "InstanceEntity() {0}:{1}, UseRecycler:{2}", obj.GetType ().Name, obj.GetHashCode (), useRecycler);
			}

			return (T)obj;
		}

        /// <summary>
        /// 释放一个实例
        /// </summary>
        /// <param name="entity">EntityObject.</param>
        public static void ReleaseEntity(EntityObject obj)
		{
			if (obj != null)
			{
				if (EnableLog && Debuger.EnableLog) 
				{
					Debuger.Log (LOG_TAG, "ReleaseEntity() {0}:{1}", obj.GetType ().Name, obj.GetHashCode ());
				}

				obj.ReleaseInFactory();
				//这里不立即从listObject中删除，
				//而是在下一个逻辑循环统一进行删除
				//这样做可以提高效率
			}
		}



		/// <summary>
		/// Clears the released objects.
		/// 清理已经被释放的实例，并且对其进行回收
		/// </summary>
		public static void ClearReleasedObjects()
		{
			for (int i = m_listObject.Count - 1; i >= 0; i--)
			{
				if (m_listObject[i].IsReleased)
				{
					EntityObject obj = m_listObject[i];
					m_listObject.RemoveAt(i);

                    //将对象加入对象池
                    m_recycler.Push(obj);
				}
			}
		}
	}
}
