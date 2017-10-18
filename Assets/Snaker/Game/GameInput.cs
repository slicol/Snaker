using System;
using SGF;
using SGF.Unity;
using Snaker.Game.Data;
using UnityEngine;

namespace Snaker.Game
{
    public class GameInput:MonoBehaviour
    {
        /// <summary>
        /// 当用键盘输入时，用来记录按键状态
        /// </summary>
        private static DictionaryEx<KeyCode, bool> m_MapKeyState = new DictionaryEx<KeyCode, bool>();

        /// <summary>
        /// 为调用者抛出虚拟按键事件！
        /// </summary>
        public static Action<int, float> OnVkey;

        private static GameInput m_Instance = null;

        //-------------------------------------------------------------------
        /// <summary>
        /// 用来控制移动的轮盘
        /// </summary>
        private EasyJoystick m_Joystick;

        /// <summary>
        /// 用来加速，或者使用技能的按钮
        /// </summary>
        private EasyButton m_Button;


        /// <summary>
        /// 初始化，用来在当前场景添加GameInput对象
        /// </summary>
        public static void Create()
        {
            if(m_Instance != null)
            {
                throw new Exception("GameInput 不能重复初始化！");
                return;
            }

            //实例化GameInput的Prefab，里面预置了EasyJoystick脚本！
            //因为EasyJoystick有一些参数，在Prefab里比较容易配置
            GameObject prefab = Resources.Load<GameObject>("GameInput");
            GameObject go = GameObject.Instantiate(prefab);
            m_Instance = GameObjectUtils.EnsureComponent<GameInput>(go);
        }

        /// <summary>
        /// 释放当前创建的GameInput对象
        /// </summary>
        public static void Release()
        {
            m_MapKeyState.Clear();
            if (m_Instance != null)
            {
                GameObject.Destroy(m_Instance.gameObject);
                m_Instance = null;
            }
            OnVkey = null;
        }


        void Start()
        {
            m_Joystick = this.GetComponentInChildren<EasyJoystick>();
            m_Button = this.GetComponentInChildren<EasyButton>();

            if (m_Joystick == null || m_Button == null)
            {
                this.LogError("Start() m_Joystick == null || m_Button == null!");
            }
        }


        void OnEnable()
        {
            EasyJoystick.On_JoystickMove += On_JoystickMove;
            EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;
            EasyButton.On_ButtonUp += On_ButtonUp;
			EasyButton.On_ButtonDown += On_ButtonDown;
        }

        void OnDisable()
        {
            EasyJoystick.On_JoystickMove -= On_JoystickMove;
            EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
			EasyButton.On_ButtonDown -= On_ButtonDown;
            EasyButton.On_ButtonUp -= On_ButtonUp;
        }

        void OnDestroy()
        {
            EasyJoystick.On_JoystickMove -= On_JoystickMove;
            EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
			EasyButton.On_ButtonDown -= On_ButtonDown;
            EasyButton.On_ButtonUp -= On_ButtonUp;
        }


        //-------------------------------------------------------------------


        void On_JoystickMove(MovingJoystick move)
        {
            if (move.joystick == m_Joystick)
            {
                HandleVKey(GameVKey.MoveX, move.joystickValue.x);
                HandleVKey(GameVKey.MoveY, move.joystickValue.y);
            }
        }

        void On_JoystickMoveEnd(MovingJoystick move)
        {
            if (move.joystick == m_Joystick)
            {
                HandleVKey(GameVKey.MoveX, 0);
                HandleVKey(GameVKey.MoveY, 0);
            }
        }


        void On_ButtonDown(string buttonName)
        {
            if (m_Button.name == buttonName)
            {
                HandleVKey(GameVKey.SpeedUp, 2);
            }
        }

        void On_ButtonUp(string buttonName)
        {
            if (m_Button.name == buttonName)
            {
                HandleVKey(GameVKey.SpeedUp, 1);
            }
        }

        //-------------------------------------------------------------------

        /// <summary>
        /// 对【虚拟按键】的处理，将其通过事件回调，抛给监听者
        /// </summary>
        /// <param name="vkey">Vkey.</param>
        /// <param name="arg">Argument.</param>
        private void HandleVKey(int vkey, float arg)
        {
            if (OnVkey != null)
            {
                OnVkey(vkey, arg);
            }
        }


        //-------------------------------------------------------------------
        //键盘控制
        #region 键盘控制

        void Update()
        {
            HandleKey(KeyCode.A, GameVKey.MoveX, -1, GameVKey.MoveX, 0);
            HandleKey(KeyCode.D, GameVKey.MoveX, 1, GameVKey.MoveX, 0);
            HandleKey(KeyCode.W, GameVKey.MoveY, 1, GameVKey.MoveY, 0);
            HandleKey(KeyCode.S, GameVKey.MoveY, -1, GameVKey.MoveY, 0);
            HandleKey(KeyCode.Space, GameVKey.SpeedUp, 2, GameVKey.SpeedUp, 1);
        }

        /// <summary>
        /// 对【物理按键】的轮询处理
        /// 将【物理按键】转化为【虚拟按键】
        /// </summary>
        /// <param name="key">所轮询的物理按键KeyCode</param>
        /// <param name="press_vkey">当物理按键按下时，对应的虚拟按键VKeyCode</param>
        /// <param name="press_arg">当物理按键按下时，对应的虚拟按键参数</param>
        /// <param name="release_vkey">当物理按键松开时，对应的虚拟按键VKeyCode</param>
        /// <param name="relase_arg">当物理按键松开时，对应的虚拟按键参数</param>
        private void HandleKey(KeyCode key, int press_vkey, float press_arg, int release_vkey, float relase_arg)
        {
            if (Input.GetKey(key))
            {
                if (!m_MapKeyState[key])
                {
                    m_MapKeyState[key] = true;
                    HandleVKey(press_vkey, press_arg);//转为虚拟按键
                }
            }
            else
            {
                if (m_MapKeyState[key])
                {
                    m_MapKeyState[key] = false;
                    HandleVKey(release_vkey, relase_arg);//转为虚拟按键
                }
            }
        }

        #endregion
        //-------------------------------------------------------------------
    }
}
