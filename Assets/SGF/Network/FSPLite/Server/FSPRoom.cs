////////////////////////////////////////////////////////////////////
//                            _ooOoo_                             //
//                           o8888888o                            //
//                           88" . "88                            //
//                           (| ^_^ |)                            //
//                           O\  =  /O                            //
//                        ____/`---'\____                         //
//                      .'  \\|     |//  `.                       //
//                     /  \\|||  :  |||//  \                      //
//                    /  _||||| -:- |||||-  \                     //
//                    |   | \\\  -  /// |   |                     //
//                    | \_|  ''\---/''  |   |                     //
//                    \  .-\__  `-`  ___/-. /                     //
//                  ___`. .'  /--.--\  `. . ___                   //
//                ."" '<  `.___\_<|>_/___.'  >'"".                //
//              | | :  `- \`.;`\ _ /`;.`/ - ` : | |               //
//              \  \ `-.   \_ __\ /__ _/   .-` /  /               //
//        ========`-.____`-.___\_____/___.-`____.-'========       //
//                             `=---='                            //
//        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^      //
//            佛祖保佑       无BUG        不修改                   //
////////////////////////////////////////////////////////////////////
/*
* 描述：
* 作者：slicol
*/
using System;
using System.Collections.Generic;
using System.Net;
using SGF.Network.FSPLite.Server.Data;
using SGF.Network.RPCLite;


namespace SGF.Network.FSPLite.Server
{
    public class FSPRoom : RPCService
    {
        public const string RPC_UpdateRoomInfo = "_RPC_UpdateRoomInfo";
        public const string RPC_NotifyGameStart = "_RPC_NotifyGameStart";
        public const string RPC_NotifyGameResult = "_RPC_NotifyGameResult";

        public const string RPC_JoinRoom = "_RPC_JoinRoom";
        public const string RPC_OnJoinRoom = "_RPC_OnJoinRoom";

        public const string RPC_ExitRoom = "_RPC_ExitRoom";
        public const string RPC_OnExitRoom = "_RPC_OnExitRoom";

        public const string RPC_RoomReady = "_RPC_RoomReady";
        public const string RPC_CancelReady = "_RPC_CancelReady";

        public const string RPC_Ping = "_RPC_Ping";
        public const string RPC_OnPing = "_RPC_OnPing";


        private FSPRoomData m_data = new FSPRoomData();
        private byte[] m_customGameParam;
        private DictionaryEx<uint, IPEndPoint> m_mapUserId2Address = new DictionaryEx<uint, IPEndPoint>();

        public FSPRoom():base(0)
        {
            
        }

        public uint Id { get { return m_data.id; } }


        public void Create()
        {
            m_data.id = 1;
        }

        public override void Dispose()
        {
            base.Dispose();

        }


        public void SetCustomGameParam(byte[] custom)
        {
            m_customGameParam = custom;
        }


        private void AddPlayer(uint userId, string name, byte[] customPlayerData, IPEndPoint address)
        {
            FSPPlayerData data = GetPlayerInfoByUserId(userId);
            if (data == null)
            {
                data = new FSPPlayerData();
                m_data.players.Add(data);
                data.id = (uint)m_data.players.Count;
                data.sid = data.id;
            }
            data.customPlayerData = customPlayerData;
            data.isReady = false;
            data.userId = userId;
            data.name = name;
            m_mapUserId2Address[userId] = address;
        }

        private void RemovePlayerByUserId(uint userId)
        {
            int i = GetPlayerIndexByUserId(userId);
            if (i >= 0)
            {
                m_data.players.RemoveAt(i);
            }

            if (m_mapUserId2Address.ContainsKey(userId))
            {
                m_mapUserId2Address.Remove(userId);
            }
        }



		private void RemovePlayerById(uint playerId)
		{
			for (int i = 0; i < m_data.players.Count; i++)
			{
				if (m_data.players[i].id == playerId)
				{
					m_data.players.RemoveAt(i);
					if (m_mapUserId2Address.ContainsKey(m_data.players[i].userId))
					{
						m_mapUserId2Address.Remove(m_data.players[i].userId);
					}
				}
			}
		}


        private int GetPlayerIndexByUserId(uint userId)
        {
            for (int i = 0; i < m_data.players.Count; i++)
            {
                if (m_data.players[i].userId == userId)
                {
                    return i;
                }
            }
            return -1;
        }


        private FSPPlayerData GetPlayerInfoByUserId(uint userId)
        {
            for (int i = 0; i < m_data.players.Count; i++)
            {
                if (m_data.players[i].userId == userId)
                {
                    return m_data.players[i];
                }
            }
            return null;
        }

        private List<IPEndPoint> GetAllAddress()
        {
            List<IPEndPoint> list = new List<IPEndPoint>();
            for (int i = 0; i < m_data.players.Count; i++)
            {
                uint userId = m_data.players[i].userId;
                IPEndPoint address = m_mapUserId2Address[userId];
                list.Add(address);
            }

            return list;
        }

        private bool CanStartGame()
        {
            if (m_data.players.Count > 1 && IsAllReady())
            {
                return true;
            }
            return false;
        }

        private bool IsAllReady()
        {
            bool isAllReady = true;
            for (int i = 0; i < m_data.players.Count; i++)
            {
                if (!m_data.players[i].isReady)
                {
                    isAllReady = false;
                    break;
                }
            }
            return isAllReady;
        }


        private void SetReady(uint userId, bool value)
        {
            var info = GetPlayerInfoByUserId(userId);
            if (info != null)
            {
                info.isReady = value;
            }

        }


        //-------------------------------------------------------------------------------------------------
        //RPC 绑定

        private void _RPC_JoinRoom(uint userId, string name, byte[] customData, IPEndPoint target)
        {
            AddPlayer(userId, name, customData, target);

            RPC(target, RPC_OnJoinRoom);
            _Call_UpdateRoomInfo();
        }

        private void _RPC_ExitRoom(uint userId, IPEndPoint target)
        {
            RemovePlayerByUserId(userId);

            RPC(target, RPC_OnExitRoom);
            _Call_UpdateRoomInfo();

            if (CanStartGame())
            {
                _Call_NotifyGameStart();
            }
        }

        private void _RPC_RoomReady(uint userId, IPEndPoint target)
        {
            SetReady(userId, true);
            _Call_UpdateRoomInfo();

            if (CanStartGame())
            {
                _Call_NotifyGameStart();
            }
        }

        private void _RPC_CancelReady(uint userId, IPEndPoint target)
        {
            SetReady(userId, false);
            _Call_UpdateRoomInfo();
        }

        private void _RPC_Ping(int pingArg, IPEndPoint target)
        {
            RPC(target, RPC_OnPing, pingArg);
        }



        private void _Call_NotifyGameStart()
        {
            FSPGameStartParam param = new FSPGameStartParam();
            param.fspParam = FSPServer.Instance.GetParam();
            param.players = m_data.players;
            param.customGameParam = m_customGameParam;

			FSPServer.Instance.StartGame ();
			FSPServer.Instance.Game.onGameExit += OnGameExit;
			FSPServer.Instance.Game.onGameEnd += OnGameEnd;

            for (int i = 0; i < m_data.players.Count; i++)
            {
                var player = m_data.players[i];
                IPEndPoint address = m_mapUserId2Address[player.userId];
                param.fspParam.sid = player.sid;

                //将玩家加入到FSPServer中
                FSPServer.Instance.Game.AddPlayer(player.id, player.sid);

                byte[] buff = PBSerializer.NSerialize(param);
                RPC(address, RPC_NotifyGameStart, buff);
            }
        }

		private void OnGameExit(uint playerId)
		{
			RemovePlayerById (playerId);
			_Call_UpdateRoomInfo();
		}

		private void OnGameEnd(int reason)
		{
			FSPServer.Instance.StopGame ();
			RPC(GetAllAddress(), RPC_NotifyGameResult, reason);
		}


        private void _Call_UpdateRoomInfo()
        {
			for (int i = 0; i < m_data.players.Count; ++i) 
			{
				this.Log ("_Call_UpdateRoomInfo() Player: {0}", m_data.players [i]);
			}

            byte[] param = PBSerializer.NSerialize(m_data);
            RPC(GetAllAddress(), RPC_UpdateRoomInfo, param);
        }


    }
}
