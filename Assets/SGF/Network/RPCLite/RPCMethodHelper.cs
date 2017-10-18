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
using System.Linq;
using System.Net;
using System.Text;

namespace SGF.Network.RPCLite
{
    public delegate void RPCMethod(IPEndPoint target);
    public delegate void RPCMethod<T0>(T0 arg0,IPEndPoint target);
    public delegate void RPCMethod<T0, T1>(T0 arg0, T1 arg1, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2, T3, T4, T5>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2, T3, T4, T5, T6>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, IPEndPoint target);
    public delegate void RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, IPEndPoint target);

    public class RPCMethodHelper
    {
        public string name;
        public RPCMethod method;

        public virtual void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke(target);
        }
    }

    public class RPCMethodHelper<T0> : RPCMethodHelper
    {
        public RPCMethod<T0> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method((T0)args[0], target);
        }
    }

    public class RPCMethodHelper<T0, T1> : RPCMethodHelper
    {
        public RPCMethod<T0, T1> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], target);
            
        }
    }

    public class RPCMethodHelper<T0, T1, T2> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], target);
        }
    }

    public class RPCMethodHelper<T0, T1, T2, T3> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2, T3> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3], target);
        }
    }

    public class RPCMethodHelper<T0, T1, T2, T3, T4> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2, T3, T4> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3], (T4)args[4], target);
        }
    }

    public class RPCMethodHelper<T0, T1, T2, T3, T4, T5> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2, T3, T4, T5> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3], (T4)args[4], (T5)args[5], target);
        }
    }

    public class RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2, T3, T4, T5, T6> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3], (T4)args[4], (T5)args[5], (T6)args[6], target);
        }
    }

    public class RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3], (T4)args[4], (T5)args[5], (T6)args[6], (T7)args[7], target);
        }
    }
    public class RPCMethodHelper<T0, T1, T2, T3, T4, T5, T6, T7, T8> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3], (T4)args[4], (T5)args[5], (T6)args[6], (T7)args[7], (T8)args[8], target);
        }
    }

    public class RPCMethodHelper<T0, T1, T2,T3,T4,T5,T6,T7,T8,T9> : RPCMethodHelper
    {
        public RPCMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8,T9> method;

        public override void Invoke(object[] args, IPEndPoint target)
        {
            method.Invoke((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3], (T4)args[4], (T5)args[5], (T6)args[6], (T7)args[7], (T8)args[8], (T9)args[9], target);
        }
    }
}
