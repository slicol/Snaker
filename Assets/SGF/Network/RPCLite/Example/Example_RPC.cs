
using System;
using System.Net;
using System.Text;
using UnityEngine;

namespace SGF.Network.RPCLite.Example
{
    public class Example_RPC:MonoBehaviour
    {
        private HostA a;
        private HostB b;

        void Start()
        {
            Debuger.UseUnityEngine = true;
            Debuger.EnableLog = true;

            a = new HostA();
            b = new HostB();
            b.Bind<int,int,string,byte[],byte[]>("_RPC_Test_1", _RPC_Test_1);

            a.Test();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.playmodeStateChanged -= OnEditorPlayModeChanged;
            UnityEditor.EditorApplication.playmodeStateChanged += OnEditorPlayModeChanged;
#endif
        }

        private void _RPC_Test_1(int arg1, int arg2, string arg3, byte[] arg4, byte[] arg5, IPEndPoint target)
        {
            Debuger.Log("Example_RPC", "_RPC_Test_1() {0},{1},{2},{3},{4},{5}", arg1, arg2, arg3, UTF8Encoding.Default.GetString(arg4), UTF8Encoding.Default.GetString(arg5), target);
        }

#if UNITY_EDITOR
        private void OnEditorPlayModeChanged()
        {
            if (Application.isPlaying == false)
            {
                Debuger.Log("OnEditorPlayModeChanged()");
                UnityEditor.EditorApplication.playmodeStateChanged -= OnEditorPlayModeChanged;

                try
                {
                    
                    a.Dispose();
                }
                catch (Exception e)
                {
                    Debuger.LogError("OnEditorPlayModeChanged() " + e.Message);
                }

                try
                {
                    b.Dispose();
                }
                catch (Exception e)
                {
                    Debuger.LogError("OnEditorPlayModeChanged()" + e.Message);
                }
            }
        }
#endif


        void Update()
        {
            a.RPCTick();
            b.RPCTick();
        }

    }


    public class HostA : RPCService
    {
        public HostA():base(10001)
        {
            
        }

        public void Test()
        {
            string str1 = "str1";
            string str2 = "str2";

            byte[] buff1 = UTF8Encoding.Default.GetBytes(str1);
            byte[] buff2 = UTF8Encoding.Default.GetBytes(str2);

            IPEndPoint target = IPUtils.GetHostEndPoint("127.0.0.1", 10002);

            RPC(target, "_RPC_Test", 1, 2, "abc", buff1, buff2);
        }
    }


    public class HostB : RPCService
    {
        public HostB():base(10002)
        {
            
        }

        private void _RPC_Test(int arg1, int arg2, string arg3, byte[] arg4, byte[] arg5, IPEndPoint target)
        {
            Debuger.Log("HostB", "_RPC_Test() {0},{1},{2},{3},{4},{5}", arg1, arg2, arg3, UTF8Encoding.Default.GetString(arg4), UTF8Encoding.Default.GetString(arg5), target);
        }
    }
}
