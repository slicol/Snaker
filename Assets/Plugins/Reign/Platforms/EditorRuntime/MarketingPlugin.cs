#if UNITY_EDITOR
using UnityEngine;

namespace Reign.Plugin
{
	public class MarketingPlugin : IIMarketingPlugin
	{
		public void OpenStore(MarketingDesc desc)
		{
			Application.OpenURL(desc.Editor_URL);
		}

		public void OpenStoreForReview(MarketingDesc desc)
		{
			Application.OpenURL(desc.Editor_URL);
		}
	}
}
#endif