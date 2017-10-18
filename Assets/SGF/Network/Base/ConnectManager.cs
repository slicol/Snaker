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
using SGF.Unity;

namespace SGF.Network
{
    public delegate void ConnectErrorListener(uint connId, NetErrorCode errcode);

    public class ConnectManager: SGF.Unity.MonoSingleton<ConnectManager>
    {
        private const string TAG = "ConnectManager";

        private DictionaryEx<uint,IConnection> m_mapConnection;

        public void Init()
        {
            m_mapConnection = new DictionaryEx<uint, IConnection>();
        }

        void Update()
        {

        }


        public IConnection CreateConnection(uint connId, Type type, string ip, int port)
        {
            IConnection conn = type.Assembly.CreateInstance(type.FullName) as IConnection;
            m_mapConnection.Add(connId,conn);
            conn.Connect(ip,port);
            return conn;
        }

        public void DisposeConnection(uint connId)
        {
            IConnection conn = m_mapConnection[connId];
            if (conn != null)
            {
                m_mapConnection.Remove(connId);
                conn.Dispose();
            }
        }


        public void AddErrorListener(ConnectErrorListener listener)
        {
            
        }

        public void RemoveErrorListener(ConnectErrorListener listener)
        {
            
        }


        internal int Send(uint connId, byte[] buffer, int len)
        {
            IConnection conn = m_mapConnection[connId];
            if (conn != null)
            {
                return conn.Send(buffer, len);
            }
            else
            {
                Debuger.LogError(TAG,"Send() connId:{0} 不存在！", connId);
                return -1;
            }
        }

        internal int Recv(uint connId, byte[] buffer, int offset)
        {
            IConnection conn = m_mapConnection[connId];
            if (conn != null)
            {
                return conn.Recv(buffer, offset);
            }
            else
            {
                Debuger.LogError(TAG, "Recv() connId:{0} 不存在！", connId);
                return -1;
            }
        }

    }
}
