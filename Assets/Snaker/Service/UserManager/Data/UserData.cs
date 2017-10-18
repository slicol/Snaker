using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Snaker.Service.UserManager.Data
{
    /// <summary>
    /// 用户数据
    /// 使用ProtoBuf进行序列化
    /// </summary>
    [ProtoContract]
    public class UserData
    {
        [ProtoMember(1)]
        public uint id;//用户ID
        [ProtoMember(2)]
        public string name;//用户名字
        [ProtoMember(3)]
        public int level;//用户等级
        [ProtoMember(4)]
        public int defaultSnakeId;//用户的Snake的ID

    }
}
