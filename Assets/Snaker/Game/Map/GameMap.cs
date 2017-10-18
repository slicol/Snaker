using SGF;
using Snaker.Game.Data;
using UnityEngine;


namespace Snaker.Game.Map
{
	public class GameMap 
    {
		//===========================================================================

        private MapScript m_script;
        private GameObject m_view;

        private Vector3 m_size;

        public void Load(MapData data)
        {
			GameObject mapPrefab = Resources.Load<GameObject>("map/map_" + data.id);
			m_view = GameObject.Instantiate(mapPrefab);

			Vector3 size = m_view.GetComponent<SpriteRenderer>().sprite.bounds.size;
			size.x *= m_view.transform.localScale.x;
			size.y *= m_view.transform.localScale.y;
			size.z *= m_view.transform.localScale.z;
			m_size = size;

            m_script = m_view.GetComponent<MapScript>();
        }

        public void Unload()
        {
			m_script = null;

            if(m_view != null)
            {
                GameObject.Destroy(m_view);
                m_view = null;
            }

        }

        public void EnterFrame(int frameIndex)
        {
            if(m_script != null)
            {
                m_script.EnterFrame(frameIndex);
            }
        }

        public Vector3 Size{ get { return m_size; } }
        public GameObject View{ get { return m_view; } } 

    }
}
