using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SGF.UI.Component
{
	public class UIComponent:MonoBehaviour
	{
		protected virtual void Awake()
		{ 
		}

		public virtual void SetData(object data)
		{ 
		}

		 

		public void SetVisible(bool value)
		{
			this.gameObject.SetActive(value);
		}

		public static void SetVisible(UIBehaviour ui, bool value)
		{
			if (ui != null)
			{
				ui.gameObject.SetActive(value);
			}
		}

		public bool IsVisible { get { return this.gameObject.activeSelf;} }

		public static bool GetVisible(UIBehaviour ui)
		{
			return ui.gameObject.activeSelf;
		}

	}
}
