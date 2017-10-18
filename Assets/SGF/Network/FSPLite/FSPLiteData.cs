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
using System.Collections.Generic;
using ProtoBuf;


namespace SGF.Network.FSPLite
{
    //==========================================================
    #region FSP启动参数定义

    [ProtoContract]
    public class FSPParam
    {
        [ProtoMember(1)]
        public string host;
        [ProtoMember(2)]
        public int port;
        [ProtoMember(3)]
        public uint sid;
        [ProtoMember(4)]
        public int serverFrameInterval = 66;
        [ProtoMember(5)]
        public int serverTimeout = 15000;//ms
        [ProtoMember(6)]
        public int clientFrameRateMultiple = 2;
        [ProtoMember(7)]
        public bool enableSpeedUp = true;
        [ProtoMember(8)]
        public int defaultSpeed = 1;
        [ProtoMember(9)]
        public int frameBufferSize = 0;
        [ProtoMember(10)]
        public bool enableAutoBuffer = true;
        [ProtoMember(11)]
        public int maxFrameId = 1800;
        [ProtoMember(12)]
        public bool useLocal = false;
        [ProtoMember(13)]
        public int authId = 0;


        public FSPParam Clone()
        {
            byte[] buffer = PBSerializer.NSerialize(this);
            return (FSPParam)PBSerializer.NDeserialize(buffer, typeof(FSPParam));
        }
    }
    #endregion

    //==========================================================

    //==========================================================
    #region 客户端上报的数据定义
    [ProtoContract]
    public class FSPDataC2S
    {
        [ProtoMember(1)]
        public ushort sid = 0;
        [ProtoMember(2)]
        public List<FSPVKey> vkeys = new List<FSPVKey>();
    }

    #endregion


    //==========================================================
    #region 服务器下发的数据定义
    [ProtoContract]
    public class FSPDataS2C
    {
        [ProtoMember(1)]
        public List<FSPFrame> frames = new List<FSPFrame>();
    }


    #endregion


    #region 公用数据结构定义

    /// <summary>
    /// 为了兼容键盘和轮盘操作，将玩家的操作抽象为【虚拟按键+参数】的【命令】形式：VKey+Arg
    /// </summary>
    [ProtoContract]
    public class FSPVKey
    {
        /// <summary>
        /// 键值
        /// </summary>
        [ProtoMember(1)] public int vkey;

        /// <summary>
        /// 参数列表
        /// </summary>
        [ProtoMember(2)] public int[] args;

        /// <summary>
        /// S2C  服务器下发PlayerId
        /// C2S  客户端上报ClientFrameId
        /// </summary>
        [ProtoMember(3)] public uint playerIdOrClientFrameId;

        public uint playerId
        {
            get { return playerIdOrClientFrameId; }
            set { playerIdOrClientFrameId = value; }
        }

        public uint clientFrameId
        {
            get { return playerIdOrClientFrameId; }
            set { playerIdOrClientFrameId = value; }
        }

        public override string ToString()
        {
            return "{vkey:" + vkey + ",arg:" + args[0] + ",playerIdOrClientFrameId:" + playerIdOrClientFrameId + "}";
        }

        
    }



    [ProtoContract]
    public class FSPFrame //服务器下发的
    {
        [ProtoMember(1)]
        public int frameId;
        [ProtoMember(2)]
        public List<FSPVKey> vkeys = new List<FSPVKey>();

        public bool IsEquals(FSPFrame obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.ToString() == this.ToString();
        }


        public bool IsEmpty()
        {
            if (vkeys == null || vkeys.Count == 0)
            {
                return true;
            }
            return false;
        }

        public bool ContainsVKey(int vkey)
        {
            if (!IsEmpty())
            {
                for (int i = 0; i < vkeys.Count; i++)
                {
                    if (vkeys[i].vkey == vkey)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            string tmp = "";

            if (vkeys != null && vkeys.Count > 0)
            {
                for (int i = 0; i < vkeys.Count - 1; i++)
                {
                    tmp += vkeys[i].ToString() + ",";
                }
                tmp += vkeys[vkeys.Count - 1].ToString();
            }

            return "{frameId:" + frameId + ", vkeys:[" + tmp + "]}";
        }
    }


    //==========================================================
    //公共数据定义
   
    #endregion

}

