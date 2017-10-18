#if UNITY_IPHONE
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Reign.Plugin
{
	public class GameCenter_ScorePlugin_iOS : IScorePlugin
	{
		private bool waitingToAuthenticate;
		public bool IsAuthenticated {get; private set;}
		public bool PerformingGUIOperation {get {return false;}}
		public string Username {get; private set;}

		private ScoreDesc desc;
		private AuthenticateCallbackMethod authenticateCallback;
		private ReportScoreCallbackMethod reportScoreCallback;
		private ReportAchievementCallbackMethod reportAchievementCallback;
		private RequestAchievementsCallbackMethod requestAchievementsCallback;
		private ResetUserAchievementsCallbackMethod resetUserAchievementsCallback;

		[DllImport("__Internal", EntryPoint="InitGameCenter")]
		private static extern void InitGameCenter();

		[DllImport("__Internal", EntryPoint="AuthenticateGameCenter")]
		private static extern void AuthenticateGameCenter();

		[DllImport("__Internal", EntryPoint="GameCenterCheckAuthenticateDone")]
		private static extern bool GameCenterCheckAuthenticateDone();

		[DllImport("__Internal", EntryPoint="GameCenterCheckIsAuthenticated")]
		private static extern bool GameCenterCheckIsAuthenticated();

		[DllImport("__Internal", EntryPoint="GameCenterGetAuthenticatedError")]
		private static extern IntPtr GameCenterGetAuthenticatedError();

		[DllImport("__Internal", EntryPoint="GameCenterGetUserID")]
		private static extern IntPtr GameCenterGetUserID();

		[DllImport("__Internal", EntryPoint="GameCenterReportScore")]
		private static extern void GameCenterReportScore(long score, string leaderboardID);

		[DllImport("__Internal", EntryPoint="GameCenterReportScoreDone")]
		private static extern bool GameCenterReportScoreDone();

		[DllImport("__Internal", EntryPoint="GameCenterReportScoreSucceeded")]
		private static extern bool GameCenterReportScoreSucceeded();

		[DllImport("__Internal", EntryPoint="GameCenterReportScoreError")]
		private static extern IntPtr GameCenterReportScoreError();

		[DllImport("__Internal", EntryPoint="GameCenterReportAchievement")]
		private static extern void GameCenterReportAchievement(string achievementID, float percentComplete);

		[DllImport("__Internal", EntryPoint="GameCenterReportAchievementDone")]
		private static extern bool GameCenterReportAchievementDone();

		[DllImport("__Internal", EntryPoint="GameCenterReportAchievementSucceeded")]
		private static extern bool GameCenterReportAchievementSucceeded();

		[DllImport("__Internal", EntryPoint="GameCenterReportAchievementError")]
		private static extern IntPtr GameCenterReportAchievementError();

		[DllImport("__Internal", EntryPoint="GameCenterRequestAchievements")]
		private static extern void GameCenterRequestAchievements();

		[DllImport("__Internal", EntryPoint="GameCenterRequestAchievementDone")]
		private static extern bool GameCenterRequestAchievementDone();

		[DllImport("__Internal", EntryPoint="GameCenterRequestAchievementSucceeded")]
		private static extern bool GameCenterRequestAchievementSucceeded();

		[DllImport("__Internal", EntryPoint="GameCenterRequestAchievementError")]
		private static extern IntPtr GameCenterRequestAchievementError();

		[DllImport("__Internal", EntryPoint="GameCenterRequestAchievementResponse")]
		private static extern IntPtr GameCenterRequestAchievementResponse();

		[DllImport("__Internal", EntryPoint="GameCenterShowScoresPage")]
		private static extern void GameCenterShowScoresPage(string leaderboardID);

		[DllImport("__Internal", EntryPoint="GameCeneterShowAchievementsPage")]
		private static extern void GameCeneterShowAchievementsPage();

		[DllImport("__Internal", EntryPoint="GameCenterResetUserAchievements")]
		private static extern void GameCenterResetUserAchievements();

		[DllImport("__Internal", EntryPoint="GameCenterResetUserAchievementsDone")]
		private static extern bool GameCenterResetUserAchievementsDone();

		[DllImport("__Internal", EntryPoint="GameCenterResetUserAchievementsSucceeded")]
		private static extern bool GameCenterResetUserAchievementsSucceeded();

		public GameCenter_ScorePlugin_iOS (ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			this.desc = desc;

			try
			{
				InitGameCenter();
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				if (callback != null) callback(false, e.Message);
				return;
			}

			if (callback != null) callback(true, null);
		}

		public void Authenticate (AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			authenticateCallback = callback;
			waitingToAuthenticate = true;
			AuthenticateGameCenter();
		}

		public void Logout ()
		{
			Debug.Log("Logout not supported in GameCenter!");
		}

		public void ManualCreateUser (string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Not supported on iOS");
		}

		public void ManualLogin (string userID, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Not supported on iOS");
		}

		private string findNativeAchievementID(string acheivementID)
		{
			foreach (var id in desc.AchievementDescs)
			{
				if (id.ID == acheivementID) return id.iOS_GameCenter_ID;
			}

			throw new Exception("Failed to find AchievementID: " + acheivementID);
		}

		private string findNativeLoaderboardID(string leaderboardID)
		{
			foreach (var id in desc.LeaderboardDescs)
			{
				if (id.ID == leaderboardID) return id.iOS_GameCenter_ID;
			}

			throw new Exception("Failed to find LeaderboardID: " + leaderboardID);
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

			// make sure value within range
			if (percentComplete > found.PercentCompletedAtValue) percentComplete = found.PercentCompletedAtValue;

			// if non-incremental achievement, then force percent complete to 100%
			if (!found.IsIncremental) percentComplete = found.PercentCompletedAtValue;

			// request
			reportAchievementCallback = callback;
			GameCenterReportAchievement(findNativeAchievementID(achievementID), (percentComplete / found.PercentCompletedAtValue) * 100f);
		}

		public void ReportScore (string leaderboardID, long score, ReportScoreCallbackMethod callback, MonoBehaviour services)
		{
			reportScoreCallback = callback;
			GameCenterReportScore(score, findNativeLoaderboardID(leaderboardID));
		}

		public void RequestAchievements (RequestAchievementsCallbackMethod callback, MonoBehaviour services)
		{
			requestAchievementsCallback = callback;
			GameCenterRequestAchievements();
		}

		public void RequestScores (string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(null, false, "Not supported on iOS");
		}

		public void ShowNativeAchievementsPage (ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			if (IsAuthenticated)
			{
				GameCeneterShowAchievementsPage();
				if (callback != null) callback(true, null);
			}
			else
			{
				if (callback != null) callback(false, "Not Authenticated!");
			}
		}

		public void ShowNativeScoresPage (string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			if (IsAuthenticated)
			{
				GameCenterShowScoresPage(findNativeLoaderboardID(leaderboardID));
				if (callback != null) callback(true, null);
			}
			else
			{
				if (callback != null) callback(false, "Not Authenticated!");
			}
		}

		public void ResetUserAchievementsProgress(ResetUserAchievementsCallbackMethod callback, MonoBehaviour services)
		{
			if (IsAuthenticated)
			{
				resetUserAchievementsCallback = callback;
				GameCenterResetUserAchievements();
			}
			else
			{
				if (callback != null) callback(false, "Not Authenticated!");
			}
		}

		public void Update ()
		{
			// authenticate
			if (waitingToAuthenticate && GameCenterCheckAuthenticateDone())
			{
				waitingToAuthenticate = false;
				IsAuthenticated = GameCenterCheckIsAuthenticated();
				if (authenticateCallback != null)
				{
					// get userid
					IntPtr userPtr = GameCenterGetUserID();
					Username = userPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(userPtr) : null;

					// callback
					IntPtr errorPtr = GameCenterGetAuthenticatedError();
					string error = errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : null;
					authenticateCallback(IsAuthenticated, error);
				}
			}
			else
			{
				IsAuthenticated = GameCenterCheckIsAuthenticated();
			}

			// report score
			if (GameCenterReportScoreDone() && reportScoreCallback != null)
			{
				bool succeeded = GameCenterReportScoreSucceeded();
				string error = null;
				if (!succeeded)
				{
					IntPtr errorPtr = GameCenterReportScoreError();
					error = errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : null;
				}
				reportScoreCallback(succeeded, error);
			}

			// report achievement
			if (GameCenterReportAchievementDone() && reportAchievementCallback != null)
			{
				bool succeeded = GameCenterReportAchievementSucceeded();
				string error = null;
				if (!succeeded)
				{
					IntPtr errorPtr = GameCenterReportAchievementError();
					error = errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : null;
				}
				reportAchievementCallback(succeeded, error);
			}

			// request achievements
			if (GameCenterRequestAchievementDone() && requestAchievementsCallback != null)
			{
				bool succeeded = GameCenterRequestAchievementSucceeded();
				string error = null;
				if (!succeeded)
				{
					IntPtr errorPtr = GameCenterRequestAchievementError();
					error = errorPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(errorPtr) : null;
				}

				IntPtr achievementPtr = GameCenterRequestAchievementResponse();
				var achievementList = achievementPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(achievementPtr).Split(':') : null;
				var achievements = new List<Achievement>();
				foreach (var achievementsDesc in desc.AchievementDescs)
				{
					// find achievements
					bool found = false;
					float percentComplete = 0;
					for (int i = 0; i < achievementList.Length-1; i += 2)
					{
						if (achievementList[i] == achievementsDesc.iOS_GameCenter_ID)
						{
							found = true;
							percentComplete = (float)double.Parse(achievementList[i+1]);
							break;
						}
					}

					// add achievement
					if (found) achievements.Add(new Achievement(percentComplete >= achievementsDesc.PercentCompletedAtValue, (percentComplete/100f)*achievementsDesc.PercentCompletedAtValue, achievementsDesc.ID, achievementsDesc.Name, achievementsDesc.Desc, null, null));
					else achievements.Add(new Achievement(false, 0, achievementsDesc.ID, achievementsDesc.Name, achievementsDesc.Desc, null, null));
				}

				requestAchievementsCallback(achievements.ToArray(), succeeded, error);
			}

			// reset user achievements
			if (GameCenterResetUserAchievementsDone() && resetUserAchievementsCallback != null)
			{
				resetUserAchievementsCallback(GameCenterResetUserAchievementsSucceeded(), "Unknown");
			}
		}
	}
}
#endif