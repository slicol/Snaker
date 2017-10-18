using System;
using System.Collections.Generic;
using System.Net;
using SGF;
using SGF.Network;
using SGF.Network.FSPLite.Server;
using SGF.Network.FSPLite.Server.Data;
using SGF.Network.RPCLite;
using Snaker.Game.Data;
using Snaker.Module.PVP.Data;
using Snaker.Service.User;
using UnityEngine;
using SGF.Unity;

namespace Snaker.Module.PVP
{

    /// <summary>
    /// 房间逻辑
    /// 这里使用RPC与主机房间进行通讯
    /// </summary>
    public class PVPRoom:RPCService
    {
        
        private IPEndPoint m_roomAddress;

        private uint m_mainUserId;
		private string m_mainUserName = "";
        
        private List<FSPPlayerData> m_listPlayerInfo = new List<FSPPlayerData>();

        private bool m_isInRoom = false;
        private bool m_isReady = false;
        private int m_pingValue = 0;

        public Action onJoin; 
        public Action onExit;
		public Action<FSPRoomData> OnUpdateRoomInfo;
        public Action<PVPStartParam> onNotifyGameStart;
        public Action onNotifyGameResult;

        public PVPRoom() : base(0)
        {
        }

        public void Create()
        {
            m_mainUserId = UserManager.Instance.MainUserData.id;
			m_mainUserName = UserManager.Instance.MainUserData.name;
			MonoHelper.AddUpdateListener (OnUpdate);
        }

        public void Release()
        {
			MonoHelper.RemoveUpdateListener (OnUpdate);
            base.Dispose();
        }

		private void Reset()
		{
			m_isReady = false;
			m_isInRoom = false;
			m_listPlayerInfo.Clear ();
		}

		private void OnUpdate()
		{
			this.RPCTick ();
		}

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void JoinRoom(string ip, int port)
        {
            m_roomAddress = IPUtils.GetHostEndPoint(ip, port);
            
			PlayerData pd = new PlayerData ();
			pd.userId = m_mainUserId;
			pd.name = m_mainUserName;
            byte[] customPlayerData = PBSerializer.NSerialize(pd);

			RPC(m_roomAddress, FSPRoom.RPC_JoinRoom, m_mainUserId, m_mainUserName, customPlayerData);
        }

        private void _RPC_OnJoinRoom(IPEndPoint target)
        {
            if (onJoin != null)
            {
                onJoin();
            }
        }


        /// <summary>
        /// 退出房间
        /// </summary>
        public void ExitRoom()
        {
			RPC(m_roomAddress, FSPRoom.RPC_ExitRoom, m_mainUserId);

			Reset ();

			if (onExit != null)
			{
				onExit();
			}
        }
			


        /// <summary>
        /// 房间准备
        /// </summary>
		public void RoomReady()
		{
			RPC(m_roomAddress, FSPRoom.RPC_RoomReady, m_mainUserId);
        }


        /// <summary>
        /// 房间取消准备
        /// </summary>
        public void CancelReady()
        {
            RPC(m_roomAddress, FSPRoom.RPC_CancelReady, m_mainUserId);
        }



        /// <summary>
        /// PING计算延时
        /// </summary>
        public void Ping()
        {
            int t = (int)(Time.realtimeSinceStartup*1000);
            RPC(m_roomAddress, FSPRoom.RPC_Ping, t);
        }

        private void _RPC_OnPing(int pingArg, IPEndPoint target)
        {
            m_pingValue = (int)(Time.realtimeSinceStartup*1000) - pingArg;
        }



        public bool IsReady{ get { return m_isReady; } }
        public bool IsInRoom{ get { return m_isInRoom; } }
        public int PingValue { get { return m_pingValue;} }
		public List<FSPPlayerData> players{get{ return m_listPlayerInfo;}}

        
        /// <summary>
        /// 更新房间信息回调
        /// </summary>
        /// <param name="args"></param>
        /// <param name="targetAddress"></param>
        private void _RPC_UpdateRoomInfo(byte[] buff, IPEndPoint targetAddress)
        {
            var info = PBSerializer.NDeserialize<FSPRoomData>(buff);
            m_listPlayerInfo = info.players;

            m_isInRoom = false;
            m_isReady = false;

            for (int i = 0; i < m_listPlayerInfo.Count; ++i)
            {
                if(m_listPlayerInfo[i].userId == m_mainUserId)
                {
                    m_isInRoom = true;
                    m_isReady = m_listPlayerInfo[i].isReady;
                }
            }
				
            this.Log("RPC_UpdateRoomInfo() Player Count: {0}", m_listPlayerInfo.Count);

			if (OnUpdateRoomInfo != null) 
			{
				OnUpdateRoomInfo (info);
			}
        }


        /// <summary>
        /// 通知游戏开始
        /// </summary>
        /// <param name="args"></param>
        /// <param name="targetAddress"></param>
        private void _RPC_NotifyGameStart(byte[] buff, IPEndPoint targetAddress)
        {
            var info = PBSerializer.NDeserialize<FSPGameStartParam>(buff);

            PVPStartParam startParam = new PVPStartParam();

            for (int i = 0; i < info.players.Count; i++)
            {
                byte[] customPlayerData = info.players[i].customPlayerData;
                PlayerData pd = PBSerializer.NDeserialize<PlayerData>(customPlayerData);
				pd.id = info.players [i].id;
				pd.userId = info.players [i].userId;
				pd.name = info.players [i].name;
				pd.teamId = (int)info.players [i].id;
                startParam.players.Add(pd);
            }
            
            startParam.fspParam = info.fspParam;
            startParam.gameParam = PBSerializer.NDeserialize<GameParam>(info.customGameParam);

            this.Log("RPC_NotifyGameStart() param: \n{0}", startParam.ToString());

            if (onNotifyGameStart != null)
            {
                onNotifyGameStart(startParam);
            }
        }


        /// <summary>
        /// 通知游戏结算
        /// </summary>
        /// <param name="args"></param>
        /// <param name="targetAddress"></param>
		private void _RPC_NotifyGameResult(IPEndPoint targetAddress, int reason)
        {
            this.Log("_RPC_NotifyGameResult() ");
            if (onNotifyGameResult != null)
            {
                onNotifyGameResult();
            }
        }
    }
}
