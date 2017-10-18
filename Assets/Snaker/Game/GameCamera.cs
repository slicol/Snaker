using SGF;
using SGF.Unity;
using Snaker.Game.Player;
using UnityEngine;


namespace Snaker.Game
{
    public class GameCamera:MonoBehaviour
    {
		public static GameCamera Current;
        public static Camera MainCamera;
        public static uint FocusPlayerId = 0;
	

		public static void Create()
		{
			Camera c = GameObject.FindObjectOfType<Camera> ();
			if (c != null) {
				GameObjectUtils.EnsureComponent<GameCamera> (c.gameObject);
			} else {
				Debuger.LogError ("GameCamera","Create() Cannot Find Camera In Scene!");
			}
		}

		public static void Release()
		{
			if (Current != null) {
				GameObject.Destroy (Current);
				Current = null;
			}
		}


		private GameContext m_context;

        void Start()
        {
			Current = this;
            MainCamera = this.GetComponent<Camera>();
			m_context = GameManager.Instance.Context;
        }

        void Update()
        {
            if (GameManager.Instance.IsRunning)
            {
                SnakePlayer player = GameManager.Instance.GetPlayer(FocusPlayerId);
                if (player != null)
                {
                    Vector3 targetPos = player.Head.Position();
					targetPos = m_context.EntityToViewPoint(targetPos);

                    Vector3 pos = this.transform.position;
                    pos.x = targetPos.x;
                    pos.y = targetPos.y;
                    this.transform.position = pos;

                }
            }
        }

		void OnDestroy()
		{
			m_context = null;
		}

    }
}
