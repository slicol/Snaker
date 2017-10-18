
using System.Collections.Generic;
using SGF;
using Snaker.Game.Data;
using Snaker.Game.Entity.Food;
using Snaker.Game.Entity.Factory;
using Snaker.Game.Player;
using Snaker.Game.Map;
using SGF.Module.Framework;
using SGF.Network.FSPLite;
using UnityEngine;

namespace Snaker.Game
{
    public class GameManager:ServiceModule<GameManager>
    {
        private string LOG_TAG = "GameManager";
        
        private bool m_isRunning;


		/// <summary>
		/// 游戏的上下文
		/// </summary>
		private GameContext m_context;

		/// <summary>
		/// 地图
		/// </summary>
		private GameMap m_map;

        //玩家管理,食物管理
        private List<EntityObject> m_listFood = new List<EntityObject>();
        private List<SnakePlayer> m_listPlayer = new List<SnakePlayer>();
        private DictionaryEx<uint, PlayerData> m_mapPlayerData = new DictionaryEx<uint, PlayerData>();

		public event PlayerDieEvent onPlayerDie;

        //======================================================================
        public bool IsRunning { get { return m_isRunning; } }
        public GameContext Context { get { return m_context; } }
		public GameMode GameMode{ get { return m_context.param.mode; } }
        //======================================================================

        public void Init()
        {
            
        }

        //======================================================================


        public void CreateGame(GameParam param)
        {
            if (m_isRunning)
            {
                Debuger.LogError(LOG_TAG, "Create() Game Is Runing Already!");
                return;
            }
            
            Debuger.Log(LOG_TAG, "Create() param:{0}", param);

			//创建上下文，保存战斗全局参数
			m_context = new GameContext();
			m_context.param = param;
			m_context.random.Seed = param.randSeed;
			m_context.currentFrameIndex = 0;


			//创建地图
            m_map = new GameMap();
            m_map.Load(param.mapData);
			m_context.mapSize = m_map.Size;


            //初始化工厂
            EntityFactory.Init();
            ViewFactory.Init(m_map.View.transform);

			//初始化摄像机
			GameCamera.Create();

            m_isRunning = true;
        }

        public void ReleaseGame()
        {
            if (!m_isRunning)
            {
                return;
            }

            m_isRunning = false;

			GameCamera.Release();

            for (int i = 0; i < m_listPlayer.Count; ++i)
            {
                m_listPlayer[i].Release();
            }
            m_listPlayer.Clear();

            m_listFood.Clear();

            ViewFactory.Release();
            EntityFactory.Release();

            if (m_map != null)
            {
                m_map.Unload();
                m_map = null;
            }

			onPlayerDie = null;
        }


		//======================================================================

		public void InputVKey(int vkey, float arg, uint playerId)
        {
            if (playerId == 0)
            {
                //处理其它VKey，全局性的VKey
                HandleOtherVKey(vkey, arg, playerId);
            }
            else
            {
                SnakePlayer player = GetPlayer(playerId);
                if (player != null)
                {
                    player.InputVKey(vkey, arg);
                }
                else
                {
                    //处理其它Vkey
                    HandleOtherVKey(vkey, arg, playerId);
                    
                }
            }
        }

        private void HandleOtherVKey(int vkey, float arg, uint playerId)
        {
            //全局的VKey处理
            bool hasHandled = false;
            hasHandled = hasHandled || DoVKey_CreatePlayer(vkey, arg, playerId);
            hasHandled = hasHandled || DoVKey_ReleasePlayer(vkey, arg, playerId);
        }


        //===========================================================================
        private bool DoVKey_CreatePlayer(int vkey, float arg, uint playerId)
        {
            if (vkey == GameVKey.CreatePlayer)
            {
                CreatePlayer(playerId);
                return true;
            }

            return false;
        }

        private bool DoVKey_ReleasePlayer(int vkey, float arg, uint playerId)
        {
            if (vkey == FSPVKeyBase.GAME_EXIT)
            {
                ReleasePlayer(playerId);
                return true;
            }

            return false;
        }

        //===========================================================================

        public void EnterFrame(int frameIndex)
        {
            if (!m_isRunning)
            {
                return;
            }

            if (frameIndex < 0)
            {
                m_context.currentFrameIndex ++;
            }
            else
            {
                m_context.currentFrameIndex = frameIndex;
            }

            EntityFactory.ClearReleasedObjects();

            for (int i = 0; i < m_listPlayer.Count; ++i)
            {
                m_listPlayer[i].EnterFrame(frameIndex);
            }


            List<uint> listDiePlayerId = new List<uint>();

            for (int i = 0; i < m_listPlayer.Count; i++)
            {
                SnakePlayer player = m_listPlayer[i];

                if (player.TryHitBound(m_context))
                {
                    listDiePlayerId.Add(player.Id);
                    ReleasePlayerAt(i);
                    i--;

                    continue;
                }

                if (player.TryHitEnemies(m_listPlayer))
                {
					listDiePlayerId.Add(player.Id);
                    ReleasePlayerAt(i);
                    i--;
                    continue;
                }

                for (int j = 0; j < m_listFood.Count; j++)
                {
                    EntityObject food = m_listFood[j];
                    if (player.TryEatFood(food))
                    {
                        RemoveFoodAt(j);
                        j--;
                    }
                }
            }

            if (m_map != null)
            {
                m_map.EnterFrame(frameIndex);
            }

			if (onPlayerDie != null)
			{
				for (int i = 0; i < listDiePlayerId.Count; ++i)
				{
					onPlayerDie(listDiePlayerId[i]);
				}
			}

        }

		//==================================================================

        public void RegPlayerData(PlayerData data)
        {
			m_mapPlayerData [data.id] = data;
            //m_mapPlayerData.Add(data.id, data);
        }

		//==================================================================

        internal void CreatePlayer(uint playerId)
        {
            PlayerData data = m_mapPlayerData[playerId];

            SnakePlayer player = new SnakePlayer();

			Vector3 mapSize = m_context.mapSize;
			Vector3 posMax = mapSize * 0.7f;
			Vector3 posMin = mapSize - posMax;
            Vector3 pos = new Vector3();
            pos.x = m_context.random.Range(posMin.x, posMax.x);
            pos.y = m_context.random.Range(posMin.y, posMax.y);
            pos.z = m_context.random.Range(posMin.z, posMax.z);
            pos.z = 0;

            player.Create(data, pos);
            m_listPlayer.Add(player);
        }


        private void ReleasePlayer(uint playerId)
        {
            int index = GetPlayerIndex(playerId);
            ReleasePlayerAt(index);
        }

        private void ReleasePlayerAt(int index)
        {
            if (index >= 0)
            {
                SnakePlayer player = m_listPlayer[index];
                m_listPlayer.RemoveAt(index);

                player.Release();
            }
        }

        internal SnakePlayer GetPlayer(uint playerId)
        {
            int index = GetPlayerIndex(playerId);
            if (index >= 0)
            {
                return m_listPlayer[index];
            }
            return null;
        }

        private int GetPlayerIndex(uint playerId)
        {
            for (int i = 0; i < m_listPlayer.Count; i++)
            {
                if (m_listPlayer[i].Id == playerId)
                {
                    return i;
                }
            }
            return -1;
        }

        internal List<SnakePlayer> GetPlayerList()
        {
            return m_listPlayer;
        }

        //==================================================================

        internal void AddFoodRandom()
        {
            Vector3 pos;
            pos.x = m_context.random.Range(0, m_context.mapSize.x);
            pos.y = m_context.random.Range(0, m_context.mapSize.y);
            pos.z = m_context.random.Range(0, m_context.mapSize.z);

            pos.z = 0;
            int color = m_context.random.Range(1, m_mapPlayerData.Count);
            AddFood(pos, color);
        }

        internal void AddFood(Vector3 pos, int color)
        {
            NormalFood food = EntityFactory.InstanceEntity<NormalFood>();
            food.Create(0, color, pos);

            m_listFood.Add(food);
        }

        private void RemoveFoodAt(int i)
        {
            EntityObject food = m_listFood[i];
            m_listFood.RemoveAt(i);
            EntityFactory.ReleaseEntity(food);
        }

        public List<EntityObject> GetFoodList()
        {
            return m_listFood;
        }



    }
}
