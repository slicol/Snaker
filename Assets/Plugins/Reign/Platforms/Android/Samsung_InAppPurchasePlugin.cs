#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reign.Plugin
{
	public class Samsung_InAppPurchasePlugin_Android : IInAppPurchasePlugin
	{
		private bool testTrialMode;
		public bool IsTrial {get{return testTrialMode;}}
		public InAppPurchaseID[] InAppIDs {get; private set;}

		private AndroidJavaClass native;
		private string buyInAppID;
		private InAppPurchaseGetProductInfoCallbackMethod productInfoCallback;
		private InAppPurchaseBuyCallbackMethod purchasedCallback;
		private InAppPurchaseRestoreCallbackMethod restoreCallback;
		private InAppPurchaseCreatedCallbackMethod createdCallback;

		public Samsung_InAppPurchasePlugin_Android(InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod createdCallback)
		{
			this.createdCallback = createdCallback;
			try
			{
				testTrialMode = desc.TestTrialMode;
				InAppIDs = desc.Android_Samsung_InAppIDs;

				native = new AndroidJavaClass("com.reignstudios.reignnative.Samsung_InAppPurchaseNative");
				string skus = "", types = "";
				foreach (var app in desc.Android_Samsung_InAppIDs)
				{
					if (app != desc.Android_Samsung_InAppIDs[0])
					{
						skus += ":";
						types += ":";
					}

					skus += app.ID;
					types += app.Type == InAppPurchaseTypes.NonConsumable ? "NonConsumable" : "Consumable";
				}
				native.CallStatic("Init", desc.Android_Samsung_ItemGroupID, skus, types, desc.Testing);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (createdCallback != null) createdCallback(false);
			}
		}

		~Samsung_InAppPurchasePlugin_Android()
		{
			if (native != null)
			{
				native.Dispose();
				native = null;
			}	
		}

		public void Dispose()
		{
			if (native != null)
			{
				native.CallStatic("Dispose");
				native.Dispose();
				native = null;
			}
		}

		public void GetProductInfo (InAppPurchaseGetProductInfoCallbackMethod productInfoCallback)
		{
			if (productInfoCallback == null) return;

			this.productInfoCallback = productInfoCallback;
			native.CallStatic("GetProductInfo");
		}

		public void Restore(InAppPurchaseRestoreCallbackMethod restoreCallback)
		{
			if (restoreCallback == null) return;

			this.restoreCallback = restoreCallback;
			native.CallStatic("Restore");
		}

		public void BuyInApp(string inAppID, InAppPurchaseBuyCallbackMethod purchasedCallback)
		{
			if (purchasedCallback == null) return;

			buyInAppID = inAppID;
			this.purchasedCallback = purchasedCallback;
			native.CallStatic("BuyApp", inAppID);
		}

		public void Update()
		{
			// check if init done
			int status = native.CallStatic<int>("CheckInitStatus");
			if (status != 0 && createdCallback != null) createdCallback(status == 1);

			// check product callbacks
			if (native.CallStatic<bool>("CheckProductInfoDone"))
			{
				var skus = native.CallStatic<string[]>("GetProductInfoItems");
				if (skus == null || skus.Length == 0)
				{
					productInfoCallback(null, false);
				}
				else
				{
					var products = new List<InAppPurchaseInfo>();
					for (int i = 0; i != skus.Length; i += 2)
					{
						var product = new InAppPurchaseInfo()
						{
							ID = skus[i],
							FormattedPrice = skus[i+1]
						};
						products.Add(product);
					}

					productInfoCallback(products.ToArray(), true);
				}
			}

			// check restore callbacks
			if (native.CallStatic<bool>("CheckRestoreDone"))
			{
				var skus = native.CallStatic<string[]>("GetRestoreItems");
				foreach (var inAppID in InAppIDs)
				{
					bool success = false;
					foreach (var sku in skus)
					{
						if (sku == inAppID.ID)
						{
							success = true;
							break;
						}
					}

					restoreCallback(inAppID.ID, success);
				}
			}

			// check buy callbacks
			if (native.CallStatic<bool>("CheckBuyDone"))
			{
				if (purchasedCallback != null) purchasedCallback(buyInAppID, native.CallStatic<string>("GetBuyReceipt"), native.CallStatic<bool>("CheckBuySuccess"));
			}
		}
	}
}
#endif