using UnityEngine;
using System.Collections;

namespace Reign
{
	enum PlayerPrefsEx_OperationTypes
	{
		SetInt,
		SetFloat,
		SetString,
		GetInt,
		GetFloat,
		GetString
	}

	public static class PlayerPrefsEx
	{
		public delegate void AsyncGetIntCallbackMethod(int value);
		public delegate void AsyncGetFloatCallbackMethod(float value);
		public delegate void AsyncGetStringCallbackMethod(string value);

		private static bool performingOperation;
		private static PlayerPrefsEx_OperationTypes operationType;
		private static string asyncKey;
		private static object asyncValue;
		private static bool asyncSaveWhenDone;
		private static AsyncGetIntCallbackMethod asyncGetIntCallback;
		private static AsyncGetFloatCallbackMethod asyncGetFloatCallback;
		private static AsyncGetStringCallbackMethod asyncGetStringCallback;

		static PlayerPrefsEx()
		{
			ReignServices.AddService(update, null, null);
		}

		private static void update()
		{
			if (!performingOperation) return;
			
			switch (operationType)
			{
				case PlayerPrefsEx_OperationTypes.SetInt:
					PlayerPrefs.SetInt(asyncKey, (int)asyncValue);
					if (asyncSaveWhenDone) PlayerPrefs.Save();
					break;

				case PlayerPrefsEx_OperationTypes.SetFloat:
					PlayerPrefs.SetFloat(asyncKey, (float)asyncValue);
					if (asyncSaveWhenDone) PlayerPrefs.Save();
					break;

				case PlayerPrefsEx_OperationTypes.SetString:
					PlayerPrefs.SetString(asyncKey, (string)asyncValue);
					if (asyncSaveWhenDone) PlayerPrefs.Save();
					break;

				case PlayerPrefsEx_OperationTypes.GetInt:
					asyncGetIntCallback(PlayerPrefs.GetInt(asyncKey));
					break;

				case PlayerPrefsEx_OperationTypes.GetFloat:
					asyncGetFloatCallback(PlayerPrefs.GetFloat(asyncKey));
					break;

				case PlayerPrefsEx_OperationTypes.GetString:
					asyncGetStringCallback(PlayerPrefs.GetString(asyncKey));
					break;

				default:
					Debug.LogError("Unknown Operation Type: " + operationType);
					break;
			}

			performingOperation = false;
		}

		private static void waitForLastOperation()
		{
			while (performingOperation)
			{
				if (!performingOperation) break;
			}
		}

		public static void SetIntAsync(string key, int value, bool saveWhenDone)
		{
			waitForLastOperation();
			asyncKey = key;
			asyncValue = value;
			asyncSaveWhenDone = saveWhenDone;

			operationType = PlayerPrefsEx_OperationTypes.SetInt;
			performingOperation = true;
		}

		public static void SetFloatAsync(string key, float value, bool saveWhenDone)
		{
			waitForLastOperation();
			asyncKey = key;
			asyncValue = value;
			asyncSaveWhenDone = saveWhenDone;

			operationType = PlayerPrefsEx_OperationTypes.SetFloat;
			performingOperation = true;
		}

		public static void SetStringAsync(string key, string value, bool saveWhenDone)
		{
			waitForLastOperation();
			asyncKey = key;
			asyncValue = value;
			asyncSaveWhenDone = saveWhenDone;

			operationType = PlayerPrefsEx_OperationTypes.SetString;
			performingOperation = true;
		}

		public static void GetIntAsync(string key, AsyncGetIntCallbackMethod callback)
		{
			if (callback == null) return;

			waitForLastOperation();
			asyncKey = key;
			asyncGetIntCallback = callback;

			operationType = PlayerPrefsEx_OperationTypes.GetInt;
			performingOperation = true;
		}

		public static void GetFloatAsync(string key, AsyncGetFloatCallbackMethod callback)
		{
			if (callback == null) return;

			waitForLastOperation();
			asyncKey = key;
			asyncGetFloatCallback = callback;

			operationType = PlayerPrefsEx_OperationTypes.GetFloat;
			performingOperation = true;
		}

		public static void GetIntAsync(string key, AsyncGetStringCallbackMethod callback)
		{
			if (callback == null) return;

			waitForLastOperation();
			asyncKey = key;
			asyncGetStringCallback = callback;

			operationType = PlayerPrefsEx_OperationTypes.GetString;
			performingOperation = true;
		}
	}
}