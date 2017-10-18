using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF.Network.FSPLite.Client;
using SGF.Network.FSPLite.Server;
using SGF.Unity;
using UnityEngine;

namespace SGF.Network.FSPLite.Example
{
    public class Example_FSPLite:MonoBehaviour
    {
        private FSPGameClientImpl m_client1 = new FSPGameClientImpl(1);
        private FSPGameClientImpl m_client2 = new FSPGameClientImpl(2);
        private FSPParam m_fspParam;
        void Start()
        {
            Debuger.EnableLog = true;
        }

        void OnGUI()
        {
            if (GUILayout.Button("StartServer"))
            {
                FSPServer.Instance.Start(0);
                FSPServer.Instance.SetFrameInterval(66,2);
                FSPServer.Instance.SetServerTimeout(0);

                FSPServer.Instance.StartGame();
                FSPServer.Instance.Game.AddPlayer(1, 1);
                FSPServer.Instance.Game.AddPlayer(2, 2);
                m_fspParam = FSPServer.Instance.GetParam();
            }
            
            if (FSPServer.Instance.IsRunning)
            {
                GUILayout.Label("Server IP  :" + m_fspParam.host);
                GUILayout.Label("Server Port:" + m_fspParam.port);
            }

            if (GUILayout.Button("CloseServer"))
            {
                FSPServer.Instance.Close();
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    m_client1.OnGUI();
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    m_client2.OnGUI();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        void FixedUpdate()
        {
            m_client1.FixedUpdate();
            m_client2.FixedUpdate();
        }
    }


    public class FSPGameClientImpl
    {
        private FSPManager m_mgrFSP;
        public string LOG_TAG = "FSPGameClientImpl";
        private uint m_playerId;
        private string m_lastVKey;

        public FSPGameClientImpl(uint playerId)
        {
            m_playerId = playerId;
            LOG_TAG = LOG_TAG + "[" + m_playerId + "]";
        }

        public void OnGUI()
        {

            if (m_mgrFSP != null)
            {
                GUILayout.Label("Client[" + m_playerId + "]: " + m_mgrFSP.GameState);
            }
            else
            {
                GUILayout.Label("Client[" + m_playerId + "]");
            }

            GUILayout.Label("最近一次VKey：" + m_lastVKey);

            if (FSPServer.Instance.IsRunning)
            {
                if (GUILayout.Button("Start"))
                {
                    Start(FSPServer.Instance.GetParam());
                }
            }

            if (GUILayout.Button("SendRoundBegin"))
            {
                m_mgrFSP.SendRoundBegin();
            }

            if (GUILayout.Button("SendControlStart"))
            {
                m_mgrFSP.SendControlStart();
            }

            if (GUILayout.Button("SendPing"))
            {
                m_mgrFSP.SendFSP(FSPVKeyBase.PING, SGFRandom.Default.Range(1,1000));
            }

            if (GUILayout.Button("SendRoundEnd"))
            {
                m_mgrFSP.SendRoundEnd();
            }

            if (GUILayout.Button("SendGameEnd"))
            {
                m_mgrFSP.SendGameEnd();
            }

			if (GUILayout.Button("SendGameExit"))
            {
                m_mgrFSP.SendGameExit();
                
            }


        }


        public void Start(FSPParam param)
        {
            this.Log("Start()");
            param.sid = m_playerId;
            m_mgrFSP = new FSPManager();
            m_mgrFSP.Start(param, m_playerId);
            m_mgrFSP.SetFrameListener(OnEnterFrame);
            m_mgrFSP.onGameEnd += (arg) =>
            {
                this.Log("OnGameEnd() " + arg);
                DelayInvoker.DelayInvoke(0.01f, Close);
                //Close();
            };
            m_mgrFSP.onGameExit += (playerId) =>
            {
				this.Log("onGameExit() " + playerId);
                if (playerId == m_playerId)
                {
                    DelayInvoker.DelayInvoke(0.01f, Close);
                    //Close();
                }
            };

            m_mgrFSP.SendGameBegin();
        }

        public void Close(params object[] args)
        {
            this.Log("Close()");

            if (m_mgrFSP != null)
            {
                m_mgrFSP.Stop();
                m_mgrFSP = null;
            }
        }

        public void SendVKey(int vkey, int arg)
        {
            m_mgrFSP.SendFSP(vkey, arg);
        }

        public void FixedUpdate()
        {
            if (m_mgrFSP != null)
            {
                m_mgrFSP.EnterFrame();
            }
        }

        private void OnEnterFrame(int frameId, FSPFrame frame)
        {
            if (frame != null && frame.vkeys != null)
            {
                for (int i = 0; i < frame.vkeys.Count; i++)
                {
                    FSPVKey cmd = frame.vkeys[i];
                    m_lastVKey = cmd.ToString();
                    this.Log("OnEnterFrame() frameId:{0}, cmd:{1}", frameId, cmd.ToString());
                }
            }
        }




    }


}

