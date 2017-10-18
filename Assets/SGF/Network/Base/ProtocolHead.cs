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


namespace SGF.Network
{
    public class ProtocolHead
    {
        public const int Length = 14;
        public uint pid = 0;
        public uint index = 0;
        public int dataSize = 0;
        public ushort checksum = 0;
        public static ProtocolHead Deserialize(NetBuffer buffer)
        {
            ProtocolHead head = new ProtocolHead();
            head.pid = buffer.ReadUInt();
            head.index = buffer.ReadUInt();
            head.dataSize = buffer.ReadInt();
            head.checksum = buffer.ReadUShort();
            return head;
        }

        public NetBuffer Serialize(NetBuffer buffer)
        {
            buffer.WriteUInt(pid);
            buffer.WriteUInt(index);
            buffer.WriteInt(dataSize);
            buffer.WriteUShort(checksum);
            return buffer;
        }

    }
}
