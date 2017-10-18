#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reign.Plugin
{
	public class InAppPurchasePlugin : IInAppPurchasePlugin
	{
		private bool testTrialMode;
		public bool IsTrial {get{return testTrialMode;}}
		public InAppPurchaseID[] InAppIDs {get; private set;}

		public InAppPurchasePlugin(InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod callback)
		{
			testTrialMode = desc.TestTrialMode;
			InAppIDs = desc.Editor_InAppIDs;
			if (callback != null) callback(true);
		}

		public void GetProductInfo(InAppPurchaseGetProductInfoCallbackMethod callback)
		{
			if (callback != null)
			{
				var infos = new InAppPurchaseInfo[InAppIDs.Length];
				for (int i = 0; i != infos.Length; ++i)
				{
					infos[i] = new InAppPurchaseInfo()
					{
						ID = InAppIDs[i].ID,
						FormattedPrice = InAppIDs[i].CurrencySymbol + InAppIDs[i].Price
					};
				}

				callback(infos, true);
			}
		}

		public void Restore(InAppPurchaseRestoreCallbackMethod restoreCallback)
		{
			if (restoreCallback == null) return;
			
			foreach (var inAppID in InAppIDs)
			{
				restoreCallback(inAppID.ID, true);
			}
		}

		public void BuyInApp(string inAppID, InAppPurchaseBuyCallbackMethod purchasedCallback)
		{
			PlayerPrefs.SetInt("ReignIAP_PurchasedAwarded_" + inAppID, 0);
			if (purchasedCallback != null) purchasedCallback(inAppID, null, true);
		}
		
		public void Update()
		{
			// do nothing...
		}
	}
}
#endif