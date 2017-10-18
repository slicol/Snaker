using UnityEngine;
using System.Collections;
using Snaker.Game;
using Snaker.Game.Data;

namespace Snaker.Game.Map
{
	public class MapScript : MonoBehaviour
	{
		[SerializeField]
		private Vector3 m_size;

		private int m_lastActionFrame = 0;
		private GameContext m_context;

		void Start()
		{
			m_size = GameManager.Instance.Context.mapSize;
			m_context = GameManager.Instance.Context;
		}

		public void EnterFrame(int frameIndex)
		{
			//return;
			//每隔5秒钟
			float dt = frameIndex - m_lastActionFrame;
			if (dt > 150)
			{
				m_lastActionFrame = frameIndex;

				if (GameManager.Instance.GetFoodList().Count < 30)
				{
					GameManager.Instance.AddFoodRandom();
				}

				if (GameManager.Instance.GetPlayerList().Count < 5)
				{
	                PlayerData data = new PlayerData();
					data.id = (uint)m_context.random.Range(100,100000);

					data.snakeData.id = m_context.random.Range(0,5);
	
				    data.teamId = m_context.random.Range(1, 10);
	                data.ai = 1;

	                GameManager.Instance.RegPlayerData(data);

	                GameManager.Instance.CreatePlayer(data.id);
				}

			}
		}
	}



}