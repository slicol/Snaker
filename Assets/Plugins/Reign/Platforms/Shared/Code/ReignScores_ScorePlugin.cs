using System;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Reign.Plugin
{
	namespace XML
	{
		public enum ResponseTypes
		{
			Error,
			Succeeded
		}

		public class WebResponse_Score
		{
			[XmlElement("ID")] public string ID;
			[XmlElement("UserID")] public string UserID;
			[XmlElement("Username")] public string Username;
			[XmlElement("Score")] public long Score;
		}

		public class WebResponse_Achievement
		{
			[XmlElement("ID")] public string ID;
			[XmlElement("AchievementID")] public string AchievementID;
			[XmlElement("PercentComplete")] public float PercentComplete;
		}

		public class WebResponse_Game
		{
			[XmlElement("ID")] public string ID;
			[XmlElement("Name")] public string Name;
		}

		[XmlRoot("ClientResponse")]
		public class WebResponse
		{
			[XmlAttribute("Type")] public ResponseTypes Type;
			[XmlElement("ErrorMessage")] public string ErrorMessage;
			[XmlElement("ClientID")] public string ClientID;
			[XmlElement("UserID")] public string UserID;
			[XmlElement("Username")] public string Username;
			[XmlElement("Score")] public List<WebResponse_Score> Scores;
			[XmlElement("Achievement")] public List<WebResponse_Achievement> Achievements;
			[XmlElement("Games")] public List<WebResponse_Game> Games;

			public WebResponse() {}
			public WebResponse(ResponseTypes type)
			{
				this.Type = type;
			}
		}
	}

	public enum ReignScores_ServiceTypes
	{
		Games,
		Users
	}

	class ReignScores_ServicesHelper
	{
		public delegate void CallbackMethod(bool succeeded, XML.WebResponse response);
		private string reignScoresURL, userAPIKey, gameAPIKey;

		public ReignScores_ServicesHelper(ScoreDesc desc)
		{
			reignScoresURL = desc.ReignScores_ServicesURL;
			gameAPIKey = desc.ReignScores_GameKey;
			userAPIKey = desc.ReignScores_UserKey;
		}

		private string convertType(ReignScores_ServiceTypes type)
		{
			switch (type)
			{
				case ReignScores_ServiceTypes.Games: return "Games";
				case ReignScores_ServiceTypes.Users: return "Users";
				default: throw new Exception("Invalid type: " + type);
			}
		}

		private string convertAPI_Key(ReignScores_ServiceTypes type)
		{
			switch (type)
			{
				case ReignScores_ServiceTypes.Games: return gameAPIKey;
				case ReignScores_ServiceTypes.Users: return userAPIKey;
				default: throw new Exception("Invalid type: " + type);
			}
		}

		private XML.WebResponse generateResponse(string response)
		{
			try
			{
				var xml = new XmlSerializer(typeof(XML.WebResponse));
				using (var reader = new StringReader(response))
				{
					return xml.Deserialize(reader) as XML.WebResponse;
				}
			}
			catch (Exception e)
			{
				Debug.LogError("generateResponse Failed: " + e.Message);
			}

			return null;
		}

		public void InvokeServiceMethod(ReignScores_ServiceTypes type, string method, CallbackMethod callback, MonoBehaviour services, params string[] args)
		{
			if (callback != null)
			{
				string url = string.Format("{0}{1}/{2}.cshtml", reignScoresURL, convertType(type), method);
				Debug.Log("Invoking URL: " + url);
				services.StartCoroutine(invokeWebMethod(convertAPI_Key(type), url, callback, args));
			}
		}

		private IEnumerator invokeWebMethod(string api_key, string url, CallbackMethod callback, params string[] args)
		{
			// add form data
			var form = new WWWForm();
			form.AddField("api_key", api_key);
			foreach (var arg in args)
			{
				var values = arg.Split('=');
				form.AddField(values[0], values[1]);
			}

			// invoke http method
			var www = new WWW(url, form);
			yield return www;

			// check for server errors
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				callback(false, null);
				yield break;
			}

			// process response
			var response = generateResponse(www.text);
			if (response != null)
			{
				if (response.Type == XML.ResponseTypes.Error)
				{
					Debug.LogError("Failed: " + response.ErrorMessage);
					callback(false, response);
					yield break;
				}

				callback(true, response);
			}
			else
			{
				Debug.LogError("response is null");
				callback(false, null);
				yield break;
			}
		}
	}

	public class ReignScores_ScorePlugin : IScorePlugin
	{
		private ReignScores_ServicesHelper helper;
		private IScores_UI ui;

		public bool IsAuthenticated {get; private set;}
		public string Username {get; private set;}
		public string UserID {get; private set;}

		private ScoreDesc desc;
		private string gameID;
		private Achievement[] achievements;
	
		public ReignScores_ScorePlugin(ScoreDesc desc, CreatedScoreAPICallbackMethod callback)
		{
			this.desc = desc;
			#if UNITY_EDITOR
			gameID = desc.Editor_ReignScores_GameID;
			#elif UNITY_STANDALONE_WIN
			gameID = desc.Win32_ReignScores_GameID;
			#elif UNITY_STANDALONE_OSX
			gameID = desc.OSX_ReignScores_GameID;
			#elif UNITY_STANDALONE_LINUX
			gameID = desc.Linux_ReignScores_GameID;
			#elif UNITY_WEBPLAYER
			gameID = desc.Web_ReignScores_GameID;
			#elif UNITY_WP8
			gameID = desc.WP8_ReignScores_GameID;
			#elif UNITY_METRO
			gameID = desc.WinRT_ReignScores_GameID;
			#elif UNITY_BLACKBERRY
			gameID = desc.BB10_ReignScores_GameID;
			#elif UNITY_IPHONE
			gameID = desc.iOS_ReignScores_GameID;
			#elif UNITY_ANDROID
			gameID = desc.Android_ReignScores_GameID;
			#endif

			ui = desc.ReignScores_UI;
			ui.Init(this);
			helper = new ReignScores_ServicesHelper(desc);

			if (callback != null) callback(true, null);
		}

		public void Authenticate(AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			try
			{
				// check if player exists
				if (PlayerPrefs.HasKey("ReignScores_Username"))
				{
					ui.AutoLogin(callback);
					string username = PlayerPrefs.GetString("ReignScores_Username");
					string password = PlayerPrefs.GetString("ReignScores_Pass");
					helper.InvokeServiceMethod(ReignScores_ServiceTypes.Users, "Login", loginCallback, services, "game_id="+gameID, "username="+username, "password="+password);
				}
				else
				{
					ui.RequestLogin(callback);
				}
			}
			catch (Exception e)
			{
				string error = "ReignScores Authenticate error: " + e.Message;
				Debug.LogError(error);
				IsAuthenticated = false;
				if (callback != null) callback(false, error);
			}
		}

		public void Logout()
		{
			IsAuthenticated = false;
			UserID = "???";
			Username = "???";
			if (PlayerPrefs.HasKey("ReignScores_Username")) PlayerPrefs.DeleteKey("ReignScores_Username");
			if (PlayerPrefs.HasKey("ReignScores_Pass")) PlayerPrefs.DeleteKey("ReignScores_Pass");
		}

		public void ManualLogin(string username, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			PlayerPrefs.SetString("ReignScores_Username", username);
			PlayerPrefs.SetString("ReignScores_Pass", password);
			helper.InvokeServiceMethod(ReignScores_ServiceTypes.Users, "Login", loginCallback, services, "game_id="+gameID, "username="+username, "password="+password);
		}

		private void loginCallback(bool succeeded, XML.WebResponse response)
		{
			IsAuthenticated = succeeded;
			if (succeeded)
			{
				UserID = response.UserID;
				Username = response.Username;
			}
			else
			{
				Debug.LogError(response != null ? response.ErrorMessage : "Unkown");
				Logout();
			}

			ui.LoginCallback(succeeded, response != null ? response.ErrorMessage : "Unkown");
		}

		public void ManualCreateUser(string username, string password, AuthenticateCallbackMethod callback, MonoBehaviour services)
		{
			PlayerPrefs.SetString("ReignScores_Username", username);
			PlayerPrefs.SetString("ReignScores_Pass", password);
			helper.InvokeServiceMethod(ReignScores_ServiceTypes.Games, "CreateUser", createUserCallback, services, "game_id="+gameID, "username="+username, "password="+password);
		}

		private void createUserCallback(bool succeeded, XML.WebResponse response)
		{
			IsAuthenticated = succeeded;
			if (succeeded)
			{
				UserID = response.UserID;
				Username = response.Username;
			}
			else
			{
				Debug.LogError(response != null ? response.ErrorMessage : "Unkown");
				Logout();
			}

			ui.LoginCallback(succeeded, response != null ? response.ErrorMessage : "Unkown");
		}

		private LeaderboardDesc findLeaderboard(string leaderboardID)
		{
			LeaderboardDesc leaderboardDesc = null;
			foreach (var id in desc.LeaderboardDescs)
			{
				if (id.ID == leaderboardID)
				{
					leaderboardDesc = id;
					break;
				}
			}

			if (leaderboardDesc == null)
			{
				Debug.LogError("Failed to find leaderboardID: " + leaderboardID);
				return null;
			}

			return leaderboardDesc;
		}

		private AchievementDesc findAchievement(string achievementID)
		{
			AchievementDesc achievementDesc = null;
			foreach (var id in desc.AchievementDescs)
			{
				if (id.ID == achievementID)
				{
					achievementDesc = id;
					break;
				}
			}

			if (achievementDesc == null)
			{
				Debug.LogError("Failed: achievementID not found.");
				return null;
			}

			return achievementDesc;
		}
	
		private ReportScoreCallbackMethod ReportScore_callback;
		public void ReportScore(string leaderboardID, long score, ReportScoreCallbackMethod callback, MonoBehaviour services)
		{
			// find leaderboard
			var leaderboardDesc = findLeaderboard(leaderboardID);
			if (leaderboardDesc == null)
			{
				if (callback != null) callback(false, "Failed to find leaderboardID: " + leaderboardID);
				return;
			}

			// get server id
			#if UNITY_EDITOR
			var serverLeaderboardID = leaderboardDesc.Editor_ReignScores_ID;
			#elif UNITY_STANDALONE_WIN
			var serverLeaderboardID = leaderboardDesc.Win32_ReignScores_ID;
			#elif UNITY_STANDALONE_OSX
			var serverLeaderboardID = leaderboardDesc.OSX_ReignScores_ID;
			#elif UNITY_STANDALONE_LINUX
			var serverLeaderboardID = leaderboardDesc.Linux_ReignScores_ID;
			#elif UNITY_WEBPLAYER
			var serverLeaderboardID = leaderboardDesc.Web_ReignScores_ID;
			#elif UNITY_WP8
			var serverLeaderboardID = leaderboardDesc.WP8_ReignScores_ID;
			#elif UNITY_METRO
			var serverLeaderboardID = leaderboardDesc.WinRT_ReignScores_ID;
			#elif UNITY_BLACKBERRY
			var serverLeaderboardID = leaderboardDesc.BB10_ReignScores_ID;
			#elif UNITY_IPHONE
			var serverLeaderboardID = leaderboardDesc.iOS_ReignScores_ID;
			#elif UNITY_ANDROID
			var serverLeaderboardID = leaderboardDesc.Android_ReignScores_ID;
			#endif

			// submit score
			ReportScore_callback = callback;
			helper.InvokeServiceMethod(ReignScores_ServiceTypes.Users, "ReportScore", reportScoreCallback, services, "user_id="+UserID, "leaderboard_id="+serverLeaderboardID, "score="+score);
		}

		private void reportScoreCallback(bool succeeded, XML.WebResponse response)
		{
			if (ReportScore_callback != null) ReportScore_callback(succeeded, response != null ? response.ErrorMessage : "Unknown");
		}

		private RequestScoresCallbackMethod RequestScores_callback;
		public void RequestScores(string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services)
		{
			// find leaderboard
			var leaderboardDesc = findLeaderboard(leaderboardID);
			if (leaderboardDesc == null)
			{
				if (callback != null) callback(null, false, "Failed to find leaderboardID: " + leaderboardID);
				return;
			}

			// get server id
			#if UNITY_EDITOR
			var serverLeaderboardID = leaderboardDesc.Editor_ReignScores_ID;
			#elif UNITY_STANDALONE_WIN
			var serverLeaderboardID = leaderboardDesc.Win32_ReignScores_ID;
			#elif UNITY_STANDALONE_OSX
			var serverLeaderboardID = leaderboardDesc.OSX_ReignScores_ID;
			#elif UNITY_STANDALONE_LINUX
			var serverLeaderboardID = leaderboardDesc.Linux_ReignScores_ID;
			#elif UNITY_WEBPLAYER
			var serverLeaderboardID = leaderboardDesc.Web_ReignScores_ID;
			#elif UNITY_WP8
			var serverLeaderboardID = leaderboardDesc.WP8_ReignScores_ID;
			#elif UNITY_METRO
			var serverLeaderboardID = leaderboardDesc.WinRT_ReignScores_ID;
			#elif UNITY_BLACKBERRY
			var serverLeaderboardID = leaderboardDesc.BB10_ReignScores_ID;
			#elif UNITY_IPHONE
			var serverLeaderboardID = leaderboardDesc.iOS_ReignScores_ID;
			#elif UNITY_ANDROID
			var serverLeaderboardID = leaderboardDesc.Android_ReignScores_ID;
			#endif

			// get scores
			RequestScores_callback = callback;
			helper.InvokeServiceMethod(ReignScores_ServiceTypes.Games, "RequestScores", requestScoresCallback, services, "user_id="+UserID, "leaderboard_id="+serverLeaderboardID, "offset="+offset, "range="+range, "sort_order="+leaderboardDesc.SortOrder);
		}

		private void requestScoresCallback(bool succeeded, XML.WebResponse response)
		{
			if (succeeded)
			{
				var scores = new LeaderboardScore[response.Scores.Count];
				for (int i = 0; i != response.Scores.Count; ++i)
				{
					var score = response.Scores[i];
					scores[i] = new LeaderboardScore(score.Username, score.Score);
				}

				if (RequestScores_callback != null) RequestScores_callback(scores, true, null);
			}
			else
			{
				if (RequestScores_callback != null) RequestScores_callback(null, false, response != null ? response.ErrorMessage : "Unkown");
			}
		}

		private ReportAchievementCallbackMethod ReportAchievement_callback;
		public void ReportAchievement(string achievementID, float percentComplete, ReportAchievementCallbackMethod callback, MonoBehaviour services)
		{
			// find achievement
			var achievementDesc = findAchievement(achievementID);
			if (achievementDesc == null)
			{
				if (callback != null) callback(false, "Failed to find achievementID: " + achievementID);
				return;
			}

			#if UNITY_EDITOR
			var serverAchievementID = achievementDesc.Editor_ReignScores_ID;
			#elif UNITY_STANDALONE_WIN
			var serverAchievementID = achievementDesc.Win32_ReignScores_ID;
			#elif UNITY_STANDALONE_OSX
			var serverAchievementID = achievementDesc.OSX_ReignScores_ID;
			#elif UNITY_STANDALONE_LINUX
			var serverAchievementID = achievementDesc.Linux_ReignScores_ID;
			#elif UNITY_WEBPLAYER
			var serverAchievementID = achievementDesc.Web_ReignScores_ID;
			#elif UNITY_WP8
			var serverAchievementID = achievementDesc.WP8_ReignScores_ID;
			#elif UNITY_METRO
			var serverAchievementID = achievementDesc.WinRT_ReignScores_ID;
			#elif UNITY_BLACKBERRY
			var serverAchievementID = achievementDesc.BB10_ReignScores_ID;
			#elif UNITY_IPHONE
			var serverAchievementID = achievementDesc.iOS_ReignScores_ID;
			#elif UNITY_ANDROID
			var serverAchievementID = achievementDesc.Android_ReignScores_ID;
			#endif

			// submit achievement
			ReportAchievement_callback = callback;
			helper.InvokeServiceMethod(ReignScores_ServiceTypes.Users, "ReportAchievement", reportAchievementCallback, services, "user_id="+UserID, "achievement_id="+serverAchievementID, "percent_complete="+((percentComplete / achievementDesc.PercentCompletedAtValue) * 100f));
		}

		private void reportAchievementCallback(bool succeeded, XML.WebResponse response)
		{
			if (ReportAchievement_callback != null) ReportAchievement_callback(succeeded, response != null ? response.ErrorMessage : "Unknown");
		}

		private RequestAchievementsCallbackMethod RequestAchievements_callback;
		public void RequestAchievements(RequestAchievementsCallbackMethod callback, MonoBehaviour services)
		{
			RequestAchievements_callback = callback;
			helper.InvokeServiceMethod(ReignScores_ServiceTypes.Users, "RequestAchievements", requestAchievementsCallback, services, "user_id="+UserID);
		}

		private void requestAchievementsCallback(bool succeeded, XML.WebResponse response)
		{
			if (succeeded)
			{
				if (achievements == null || achievements.Length != response.Achievements.Count) achievements = new Achievement[desc.AchievementDescs.Length];
				for (int i = 0; i != desc.AchievementDescs.Length; ++i)
				{
					var achievementDesc = desc.AchievementDescs[i];

					#if UNITY_EDITOR
					var id = achievementDesc.Editor_ReignScores_ID;
					#elif UNITY_STANDALONE_WIN
					var id = achievementDesc.Win32_ReignScores_ID;
					#elif UNITY_STANDALONE_OSX
					var id = achievementDesc.OSX_ReignScores_ID;
					#elif UNITY_STANDALONE_LINUX
					var id = achievementDesc.Linux_ReignScores_ID;
					#elif UNITY_WEBPLAYER
					var id = achievementDesc.Web_ReignScores_ID;
					#elif UNITY_WP8
					var id = achievementDesc.WP8_ReignScores_ID;
					#elif UNITY_METRO
					var id = achievementDesc.WinRT_ReignScores_ID;
					#elif UNITY_BLACKBERRY
					var id = achievementDesc.BB10_ReignScores_ID;
					#elif UNITY_IPHONE
					var id = achievementDesc.iOS_ReignScores_ID;
					#elif UNITY_ANDROID
					var id = achievementDesc.Android_ReignScores_ID;
					#endif
					
					// find achievement
					XML.WebResponse_Achievement found = null;
					foreach (var a in response.Achievements)
					{
						if (id == new Guid(a.AchievementID))
						{
							found = a;
							break;
						}
					}

					if (achievements[i] == null)
					{
						// load textures
						string fileName = "Reign/Achievements/" + achievementDesc.ID + "_achieved";
						var achievedTexture = (Texture2D)Resources.Load(fileName);
						if (achievedTexture == null)
						{
							string error = "RequestAchievements Failed to load texture: " + fileName;
							Debug.LogError(error);
							if (RequestAchievements_callback != null) RequestAchievements_callback(null, false, error);
							return;
						}

						fileName = "Reign/Achievements/" + achievementDesc.ID + "_unachieved";
						var unachievedTexture = (Texture2D)Resources.Load(fileName);
						if (unachievedTexture == null)
						{
							string error = "RequestAchievements Failed to load texture: " + fileName;
							Debug.LogError(error);
							if (RequestAchievements_callback != null) RequestAchievements_callback(null, false, error);
							return;
						}

						// add achievment object
						if (found != null) achievements[i] = new Achievement(found.PercentComplete >= achievementDesc.PercentCompletedAtValue, (found.PercentComplete/100f)*achievementDesc.PercentCompletedAtValue, achievementDesc.ID, achievementDesc.Name, achievementDesc.Desc, achievedTexture, unachievedTexture);
						else achievements[i] = new Achievement(false, 0, achievementDesc.ID, achievementDesc.Name, achievementDesc.Desc, achievedTexture, unachievedTexture);
					}
					else
					{
						// update achievment object
						if (found != null)
						{
							achievements[i].IsAchieved = found.PercentComplete >= 100;
							achievements[i].PercentComplete = found.PercentComplete;
						}
						else
						{
							achievements[i].IsAchieved = false;
							achievements[i].PercentComplete = 0;
						}
					}
				}

				if (RequestAchievements_callback != null) RequestAchievements_callback(achievements, true, null);
			}
			else
			{
				if (RequestAchievements_callback != null) RequestAchievements_callback(null, false, response != null ? response.ErrorMessage : "Unkown");
			}
		}

		public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			ui.ShowNativeScoresPage(leaderboardID, callback);
		}

		public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services)
		{
			ui.ShowNativeAchievementsPage(callback);
		}

		public void ResetUserAchievementsProgress(ResetUserAchievementsCallbackMethod callback, MonoBehaviour services)
		{
			if (callback != null) callback(false, "Unsupported on this platform!");
		}

		public void Update()
		{
			// do nothing...
		}
	}
}

