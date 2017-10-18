#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reign.Plugin
{
	public class GooglePlay_ScorePlugin_Android : IScorePlugin
	{
		public bool IsAuthenticated {get; private set;}
		public bool PerformingGUIOperation {get; private set;}
		public string Username {get; private set;}

		private AndroidJavaClass native;
		private AuthenticateCallbackMethod authenticateCallback;
		private ReportAchievementCallbackMethod reportAchievementCallback;
		private ReportScoreCallbackMethod reportScoreCallback;
		private ShowNativeViewDoneCallbackMethod showNativeViewDoneCallback;
		private CreatedScoreAPICallbackMethod createdCallback;
		private RequestAchievementsCallbackMethod requestAchievementsCallback;

		private ScoreDesc desc;

		public GooglePlay_ScorePlugin_Android (ScoreDesc desc, CreatedScoreAPICallbackMethod createdCallback)
		{
			this.desc = desc;
			this.createdCallback = createdCallback;
			try
			{
				native = new AndroidJavaClass("com.reignstudios.reignnative.GooglePlay_LeaderboardsAchievements");
				native.CallStatic("Init", desc.Android_GooglePlay_DisableUsernameRetrieval);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (createdCallback != null) createdCallback(false, e.Message);
			}
		}
			
		public void Authenticate (AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			authenticateCallback = callback;
			native.CallStatic("Authenticate");
		}

		public void Logout()
		{
			IsAuthenticated = false;
			Username = "???";
			native.CallStatic("Logout");
		}

		public void ManualCreateUser (string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "ManualCreateUser: Not supported with this API");
		}

		public void ManualLogin (string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "ManualLogin: Not supported with this API");
		}

		public void ReportAchievement (string achievementID, float percentComplete, ReportAchievementCallbackMethod callback, MonoBehaviour services)
		{
			// find achievement desc
			AchievementDesc found = null;
			foreach (var a in desc.AchievementDescs)
			{
				if (a.ID == achievementID) found =  a;
			}
			if (found == null) throw new Exception("Failed to find AchievementID: " + achievementID);

			// report
			reportAchievementCallback = callback;
			native.CallStatic("ReportAchievement", findAchievementID(achievementID), percentComplete, found.IsIncremental);
		}

		public void ReportScore (string leaderboardID, long score, ReportScoreCallbackMethod callback, MonoBehaviour services)
		{
			reportScoreCallback = callback;
			native.CallStatic("ReportScore", findLeaderboardID(leaderboardID), score);
		}

		public void RequestAchievements (RequestAchievementsCallbackMethod callback, MonoBehaviour services)
		{
			requestAchievementsCallback = callback;
			native.CallStatic("RequestAchievements");
		}

		public void RequestScores (string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(null, false, "RequestScores: Not supported with this API (Use ShowNativeScoresPage instead)");
		}

		public void ShowNativeAchievementsPage (ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			showNativeViewDoneCallback = callback;
			native.CallStatic("ShowNativeAchievementsPage");
		}

		public void ShowNativeScoresPage (string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			showNativeViewDoneCallback = callback;
			native.CallStatic("ShowNativeScoresPage", findLeaderboardID(leaderboardID));
		}

		private string findAchievementID(string id)
		{
			foreach (var descObj in desc.AchievementDescs)
			{
				if (descObj.ID == id) return descObj.Android_GooglePlay_ID;
			}

			throw new Exception("Failed to find GooglePlay AchievementID for: " + id);
		}

		private string findLeaderboardID(string id)
		{
			foreach (var descObj in desc.LeaderboardDescs)
			{
				if (descObj.ID == id) return descObj.Android_GooglePlay_ID;
			}

			throw new Exception("Failed to find GooglePlay LeaderboardID for: " + id);
		}

		public void ResetUserAchievementsProgress(ResetUserAchievementsCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Unsupported on this platform!");
		}

		public void Update ()
		{
			// check if init done
			int status = native.CallStatic<int>("CheckInitStatus");
			if (status != 0 && createdCallback != null) createdCallback(status == 1, status == 1 ? "" : "Failed to Init");

			// get events
			if (native.CallStatic<bool>("HasEvents"))
			{
				string _event = native.CallStatic<string>("GetNextEvent");
				string[] eventValues = _event.Split(':');
				if (eventValues.Length >= 2) _event = eventValues[0];
				bool success;
				switch (_event)
				{
					case "Connected":
						IsAuthenticated = true;
						Username = eventValues[1];
						if (authenticateCallback != null) authenticateCallback(true, null);
						break;

					case "Error":
						IsAuthenticated = native.CallStatic<bool>("CheckIsAuthenticated");
						Username = IsAuthenticated ? Username : "???";
						if (authenticateCallback != null) authenticateCallback(false, eventValues[1]);
						break;

					case "ReportScore":
						success = eventValues[1] == "Success";
						if (reportScoreCallback != null) reportScoreCallback(success, eventValues[1]);
						break;

					case "ReportAchievement":
						success = eventValues[1] == "Success";
						if (reportAchievementCallback != null) reportAchievementCallback(success, eventValues[1]);
						break;

					case "RequestAchievements":
						success = eventValues[1] == "Success";
						if (requestAchievementsCallback != null)
						{
							var achievementValues = native.CallStatic<string>("GetRequestAchievementsResult").Split(':');
							var achievements = new List<Achievement>();
							for (int i = 0; i < achievementValues.Length-1; i += 2)
							{
								// find achievement desc
								AchievementDesc found = null;
								foreach (var descObj in desc.AchievementDescs)
								{
									if (descObj.Android_GooglePlay_ID == achievementValues[i]) found = descObj;
								}

								if (found == null)
								{
									Debug.LogError("Failed to find achievement: " + achievementValues[i]);
									continue;
								}

								// add achievement
								float percentComplete = 0;
								if (float.TryParse(achievementValues[i+1], out percentComplete))
								{
									achievements.Add(new Achievement(percentComplete >= found.PercentCompletedAtValue, percentComplete, found.ID, found.Name, found.Desc, null, null));
								}
								else
								{
									bool isComplete = achievementValues[i+1] == "Unlocked";
									achievements.Add(new Achievement(isComplete, isComplete ? found.PercentCompletedAtValue : 0, found.ID, found.Name, found.Desc, null, null));
								}
							}

							requestAchievementsCallback(achievements.ToArray(), success, eventValues[1]);
						}
						break;

					case "ShowNativePage":
						success = eventValues[1] == "Success";
						if (showNativeViewDoneCallback != null) showNativeViewDoneCallback(success, eventValues[1]);
						break;
				}
			}

			IsAuthenticated = native.CallStatic<bool>("CheckIsAuthenticated");
		}
	}
}
#endif