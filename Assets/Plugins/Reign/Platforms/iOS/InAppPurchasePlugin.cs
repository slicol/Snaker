#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace Reign.Plugin
{
	public class AppleStore_InAppPurchasePlugin_iOS : IInAppPurchasePlugin
	{
		public bool IsTrial {get{return false;}}
		public InAppPurchaseID[] InAppIDs {get; private set;}
		
		private IntPtr native;
		private InAppPurchaseRestoreCallbackMethod restoreCallback;
		private InAppPurchaseBuyCallbackMethod purchasedCallback;
		
		[DllImport("__Internal", EntryPoint="InitInAppPurchase")]
		private static extern IntPtr InitInAppPurchase(bool testing, string sharedSecretKey);
		
		[DllImport("__Internal", EntryPoint="DisposeInAppPurchase")]
		private static extern void DisposeInAppPurchase(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="CreateInAppPurchase")]
		private unsafe static extern void CreateInAppPurchase(IntPtr native, byte** productIDs, int productIDCount);
		
		[DllImport("__Internal", EntryPoint="RestoreInAppPurchase")]
		private static extern void RestoreInAppPurchase(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="BuyInAppPurchase")]
		private static extern void BuyInAppPurchase(IntPtr native, string inAppID);
		
		[DllImport("__Internal", EntryPoint="CheckRestoreInAppPurchaseStatus")]
		private static extern bool CheckRestoreInAppPurchaseStatus(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="GetRestoreInAppPurchaseIDs")]
		private unsafe static extern byte** GetRestoreInAppPurchaseIDs(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="CheckBuyInAppPurchaseStatus")]
		private static extern bool CheckBuyInAppPurchaseStatus(IntPtr native);
		
		[DllImport("__Internal", EntryPoint="GetBuyInAppPurchaseStatusID")]
		private unsafe static extern IntPtr GetBuyInAppPurchaseStatusID(IntPtr native, bool* succeeded);

		[DllImport("__Internal", EntryPoint="GetBuyInAppPurchaseReceipt")]
		private unsafe static extern IntPtr GetBuyInAppPurchaseReceipt(IntPtr native);

		[DllImport("__Internal", EntryPoint="GetInAppPurchaseProductInfo")]
		private unsafe static extern byte** GetInAppPurchaseProductInfo(IntPtr native);

		public AppleStore_InAppPurchasePlugin_iOS(InAppPurchaseDesc desc, InAppPurchaseCreatedCallbackMethod callback)
		{
			bool pass = true;
			try
			{
				InAppIDs = desc.iOS_AppleStore_InAppIDs;
				native = InitInAppPurchase(desc.Testing, desc.iOS_AppleStore_SharedSecretKey + '\0');
				
				unsafe
				{
					byte** dataPtr = (byte**)Marshal.AllocHGlobal(desc.iOS_AppleStore_InAppIDs.Length * IntPtr.Size).ToPointer();
					int i = 0;
					foreach (var id in desc.iOS_AppleStore_InAppIDs)
					{
						var data = Encoding.ASCII.GetBytes(id.ID);
						dataPtr[i] = (byte*)Marshal.AllocHGlobal(data.Length+1).ToPointer();
						Marshal.Copy(data, 0, new IntPtr(dataPtr[i]), data.Length);
						dataPtr[i][data.Length] = 0;
						
						++i;
					}
					
					CreateInAppPurchase(native, dataPtr, desc.iOS_AppleStore_InAppIDs.Length);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				pass = false;
			}
			
			if (callback != null) callback(pass);
		}
		
		~AppleStore_InAppPurchasePlugin_iOS()
		{
			DisposeInAppPurchase(native);
			native = IntPtr.Zero;
		}

		public unsafe void GetProductInfo (InAppPurchaseGetProductInfoCallbackMethod callback)
		{
			if (callback == null) return;

			byte** ptr = GetInAppPurchaseProductInfo(native);
			if (ptr == (byte**)0)
			{
				callback(null, false);
				return;
			}

			var products = new List<InAppPurchaseInfo>();
			while (ptr[0] != (byte*)0)
			{
				string id = Marshal.PtrToStringAnsi(new IntPtr(ptr[0]));
				ptr++;

				string price = Marshal.PtrToStringAnsi(new IntPtr(ptr[0]));
				ptr++;

				var product = new InAppPurchaseInfo()
				{
					ID = id,
					FormattedPrice = price
				};
				products.Add(product);
			}

			callback(products.ToArray(), true);
		}

		public void Restore(InAppPurchaseRestoreCallbackMethod restoreCallback)
		{
			if (restoreCallback == null) return;
			
			this.restoreCallback = restoreCallback;
			RestoreInAppPurchase(native);
		}

		public void BuyInApp(string inAppID, InAppPurchaseBuyCallbackMethod purchasedCallback)
		{
			this.purchasedCallback = purchasedCallback;
			BuyInAppPurchase(native, inAppID);
		}
		
		public void Update()
		{
			if (CheckRestoreInAppPurchaseStatus(native))
			{
				var ids = new List<string>();
				unsafe
				{
					byte** idPtr = GetRestoreInAppPurchaseIDs(native);
					byte** rootIDPtr = idPtr;
					while (idPtr[0] != (byte*)0)
					{
						ids.Add(Marshal.PtrToStringAnsi(new IntPtr(idPtr[0])));
						idPtr++;
					}
					
					Marshal.FreeHGlobal(new IntPtr(rootIDPtr));
				}
				
				foreach (var inAppID in InAppIDs)
				{
					bool succeeded = false;
					foreach (var id in ids)
					{
						if (inAppID.ID == id)
						{
							succeeded = true;
							break;
						}
					}
					
					if (restoreCallback != null) restoreCallback(inAppID.ID, succeeded);
				}
			}
		
			if (CheckBuyInAppPurchaseStatus(native))
			{
				if (purchasedCallback != null)
				{
					string id = null, receipt = null;
					bool succeeded = false;
					unsafe
					{
						IntPtr idPtr = GetBuyInAppPurchaseStatusID(native, &succeeded);
						if (idPtr != IntPtr.Zero) id = Marshal.PtrToStringAnsi(idPtr);

						IntPtr receiptPtr = GetBuyInAppPurchaseReceipt(native);
						if (receiptPtr != IntPtr.Zero)
						{
							receipt = Marshal.PtrToStringAnsi(receiptPtr);
							byte[] data = Convert.FromBase64String(receipt);
							receipt = Encoding.UTF8.GetString(data);
						}
					}
					
					purchasedCallback(id, receipt, succeeded);
				}
			}
		}
	}
}
#endif