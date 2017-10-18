using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SGF;
using UnityEngine;
using UnityEngine.UI;

namespace SGF.UI.Framework
{
    public abstract class UIPanel:MonoBehaviour
    {
        public virtual void Open(object arg = null)
        {
            this.Log("Open() arg:{0}", arg);
        }

        public virtual void Close(object arg = null)
        {
            this.Log("Close() arg:{0}", arg);
        }

        /// <summary>
        /// 当前UI是否打开
        /// </summary>
        public bool IsOpen { get { return this.gameObject.activeSelf; } }


        /// <summary>
        /// 当UI关闭时，会响应这个函数
        /// 该函数在重写时，需要支持可重复调用
        /// </summary>
        protected virtual void OnClose(object arg = null)
        {
            this.Log("OnClose()");
        }

        /// <summary>
        /// 当UI打开时，会响应这个函数
        /// </summary>
        /// <param name="arg"></param>
        protected virtual void OnOpen(object arg = null)
        {
            this.Log("OnOpen() ");
        }
    }
}
