namespace SGF.Network.FSPLite
{
    public class FSPVKeyBase
    {
        
        /// <summary>
        /// PVP战斗结束
        /// </summary>
        public const int GAME_BEGIN = 100;

        /// <summary>
        /// 对局开始
        /// </summary>
        public const int ROUND_BEGIN = 101;

        /// <summary>
        /// 开始加载
        /// </summary>
        public const int LOAD_START = 102;
        /// <summary>
        /// 加载进度条
        /// </summary>
        public const int LOAD_PROGRESS = 103;

        /// <summary>
        /// 可以开始控制...
        /// </summary>
        public const int CONTROL_START = 104;

        /// <summary>
        /// 发送中途退出
        /// </summary>
        public const int GAME_EXIT = 105;

        /// <summary>
        /// 对局结束
        /// </summary>
        public const int ROUND_END = 106;

        /// <summary>
        /// PVP战斗结束
        /// </summary>
        public const int GAME_END = 107;

        /// <summary>
        /// UDP身份字段
        /// </summary>
        public const int AUTH = 108;

        /// <summary>
        /// PING 响应回包...
        /// </summary>
        public const int PING = 109;


    }
}
