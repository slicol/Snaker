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


namespace SGF.Network
{
    public delegate void ProtocolListener(uint pid, object rsp);
    public delegate void ErrorListener(uint pid, NetErrorCode errcode);

    public class ProtocolManager : SGF.Unity.MonoSingleton<ProtocolManager>
    {
        class PTLListenerHelper
        {
            public uint pid = 0;
            public uint index = 0;
            public ProtocolListener listener;
            public ErrorListener errorListener;
            public float timestamp = 0;

            public void Dispatch(object ptlObj)
            {
                if (listener != null)
                {
                    listener(pid, ptlObj);
                }
            }

            public void DispatchError(NetErrorCode errcode)
            {
                if (errorListener != null)
                {
                    errorListener(pid, errcode);
                }
            }

        }

        private const string TAG = "ProtocolManager";
        private const int PTLStreamMaxLen = 102400;
        private const float PTLTimeoutValue = 10;

        private DictionaryEx<uint, Type> m_mapPTLType;
        private DictionaryEx<uint, PTLListenerHelper> m_mapPID2Listener;
        private List<PTLListenerHelper> m_lstRspListener;
        private DictionaryEx<uint, NetBuffer> m_mapPTLStream;
        private NetBuffer m_recvBufferTemp;
        private NetBuffer m_sendBufferTemp;

        private uint m_index = 0;

        public void Init()
        {
            m_mapPTLType = new DictionaryEx<uint, Type>();
            m_mapPID2Listener = new DictionaryEx<uint, PTLListenerHelper>();
            m_lstRspListener = new List<PTLListenerHelper>();
            m_recvBufferTemp = new NetBuffer(PTLStreamMaxLen);
            m_sendBufferTemp = new NetBuffer(PTLStreamMaxLen);
        }

        //==================================================================
        public void Register(uint pid, Type type)
        {
            m_mapPTLType[pid] = type;
        }

        //==================================================================

        public void AddListener(uint pid, ProtocolListener listener)
        {
            PTLListenerHelper helper = m_mapPID2Listener[pid];
            if (helper == null)
            {
                helper = new PTLListenerHelper();
                helper.pid = pid;
                m_mapPID2Listener[pid] = helper;
            }
            helper.listener -= listener;
            helper.listener += listener;
        }

        public void RemoveListener(uint pid, ProtocolListener listener)
        {
            PTLListenerHelper helper = m_mapPID2Listener[pid];
            if (helper != null)
            {
                helper.listener -= listener;
            }
        }

        //==================================================================
        private void DispatchProtocol(uint pid, uint index, object ptlObj)
        {
            PTLListenerHelper helper = null;
            if (index > 0)
            {
                int i = 0;
                for (i = 0; i < m_lstRspListener.Count; i++)
                {
                    helper = m_lstRspListener[i];
                    if (helper.index == index)
                    {
                        m_lstRspListener.RemoveAt(i);
                        helper.Dispatch(ptlObj);
                        break;
                    }
                }

                if (i == m_lstRspListener.Count)
                {
                    Debuger.LogWarning(TAG, "DispatchProtocol() pid:{0}, index:{1} 重复收到！",pid,index);
                }
            }

            helper = m_mapPID2Listener[pid];
            if (helper != null)
            {
                helper.Dispatch(ptlObj);
            }
            else
            {
                Debuger.LogWarning(TAG, "DispatchProtocol() pid:{0} 没有监听者！", pid);
            }
        }

        //==================================================================
        void Update()
        {
            float currTime = UnityEngine.Time.realtimeSinceStartup;
            for (int i = 0; i < m_lstRspListener.Count; i++)
            {
                PTLListenerHelper helper = m_lstRspListener[i];
                if (currTime - helper.timestamp >= PTLTimeoutValue)
                {
                    m_lstRspListener.RemoveAt(i);
                    --i;
                    helper.DispatchError(NetErrorCode.Timeout);
                }
            }
        }

        //===================================================================

        public void Send<TRsp>(uint pid, object req, uint connId, ProtocolListener listener,
            ErrorListener errorListener = null)
        {
            Send(pid, req, connId, typeof(TRsp), listener, errorListener);
        }


        public void Send(uint pid, object req, uint connId)
        {
            Send(pid, req, connId, null, null, null);
        }

        public void Send(uint pid, object req, uint connId, Type rspType, 
            ProtocolListener listener, ErrorListener errorListener)
        {
            //注册Rsp类
            if (rspType != null)
            {
                Register(pid + 1, rspType);
            }

            //记录Listener
            uint index = 0;
            if (listener != null || errorListener != null)
            {
                index = ++m_index;

                PTLListenerHelper helper = new PTLListenerHelper();
                helper.listener = listener;
                helper.errorListener = errorListener;
                helper.timestamp = UnityEngine.Time.realtimeSinceStartup;
                helper.pid = pid + 1;//这是一个规则
                helper.index = index;

                m_lstRspListener.Add(helper);
            }

            //序列化协议数据
            byte[] dataBuffer = SerializeProtocol(req);

            //协议头
            ProtocolHead head = new ProtocolHead();
            head.pid = pid;
            head.index = index;
            head.dataSize = dataBuffer.Length;
            head.checksum = SGFEncoding.CheckSum(dataBuffer, dataBuffer.Length);

            //序列化协议头
            m_sendBufferTemp.Position = 0;
            head.Serialize(m_sendBufferTemp);

            //组合整个协议Buffer
            m_sendBufferTemp.AddBytes(dataBuffer);

            ConnectManager.Instance.Send(connId, m_sendBufferTemp.GetBytes(), m_sendBufferTemp.Length);
        }

        //===================================================================

        private void OnRecv(uint connId, byte[] buffer, int len)
        {
            NetBuffer stream = m_mapPTLStream[connId];
            if (stream == null)
            {
                stream = new NetBuffer(PTLStreamMaxLen);
                m_mapPTLStream[connId] = stream;
            }

            stream.Position = stream.Length;
            stream.AddBytes(buffer, 0, len);
            
            do
            {
                if (!TryHandleOneProtocol(stream, connId))
                {
                    break;
                }

                stream.Arrangement();
            
            } while (stream.BytesAvailable > 0);

        }


        private bool TryHandleOneProtocol(NetBuffer stream, uint connId)
        {
            ProtocolHead head = ProtocolHead.Deserialize(stream);
            if (head == null)
            {
                return false;
            }

            if (stream.Length < head.dataSize + ProtocolHead.Length)
            {
                return false;
            }

            if (m_recvBufferTemp.Capacity < head.dataSize)
            {
                Debuger.LogError(TAG, "OnRecv() 缓冲空间不够！ connId:{0}, dataSize:{1}, RecvBuffCapacity:{2}",
                    connId, head.dataSize, m_recvBufferTemp.Capacity);

                return false;
            }

            stream.ReadBytes(m_recvBufferTemp.GetBytes(), 0, head.dataSize);
            m_recvBufferTemp.AddLength(head.dataSize, 0);

            ushort sum = SGFEncoding.CheckSum(m_recvBufferTemp.GetBytes(), head.dataSize);
            if (sum != head.checksum)
            {
                Debuger.LogError(TAG, "OnRecv() CheckSum失败！ connId:{0}, dataSize:{1}, RecvBuffSize:{2}",
                    connId, head.dataSize, m_recvBufferTemp.Length);

                //这个时候，可能要重置连接
                stream.Clear();
                //ClearConnection();
                return false;
            }

            object ptlObj = DeserializeProtocol(head.pid, m_recvBufferTemp.GetBytes(), m_recvBufferTemp.Length);
            if (ptlObj != null)
            {
                DispatchProtocol(head.pid, head.index, ptlObj);
            }
            else
            {
                m_recvBufferTemp.Position = 0;
                byte[] buffer = m_recvBufferTemp.ReadBytes(head.dataSize);
                DispatchProtocol(head.pid, head.index, buffer);
            }

            return true;
        }

        //===================================================================


        private object DeserializeProtocol(uint pid, byte[] buffer, int len)
        {
            //采用流行的ProtoBuf做为协议的基础格式
            object ptlObj = null;
            Type type = m_mapPTLType[pid];
            if (type != null)
            {
                ptlObj = PBSerializer.NDeserialize(buffer, len, type);
            }
            return ptlObj;
        }

        private byte[] SerializeProtocol(object ptlObj)
        {
            //采用流行的ProtoBuf做为协议的基础格式
            return PBSerializer.NSerialize(ptlObj);
        }

    }
}
