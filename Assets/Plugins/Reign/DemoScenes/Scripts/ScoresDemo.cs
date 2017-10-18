// -----------------------------------------------
// Documentation: http://www.reign-studios.net/docs/unity-plugin/
// -----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Reign;

public class ScoresDemo : MonoBehaviour
{
	private static bool created;

	public bool UseUnityUI = true;
	public GameObject ReignScores_ModernRenderer, ReignScores_ClassicRenderer;
	public Button BackButton, LogoutButton, ReportScoreButton, ReportAchievementButton, ShowLeaderboardsButton, ShowAchievementsButton;
	public Text StatusText;

	private bool disableUI;
	GUIStyle uiStyle;

	// ======================================================
	// NOTE about users confused over Reign-Scores
	// Reign-Scores is an API target option, just as GooglePlay version GamceCircle is.
	// Reign-Scores is NOT required to use native services like GooglePlay, GameCenter ect.
	// Its a self-hosted option you can put on any ASP.NET server you own for platforms like WP8, BB10 ect.
	// ======================================================
	void Start()
	{
		// bind button events
		BackButton.Select();
		LogoutButton.onClick.AddListener(LogoutButton_Clicked);
		BackButton.onClick.AddListener(BackButton_Clicked);
		ReportScoreButton.onClick.AddListener(ReportScoreButton_Clicked);
		ReportAchievementButton.onClick.AddListener(ReportAchievementButton_Clicked);
		ShowLeaderboardsButton.onClick.AddListener(ShowLeaderboardsButton_Clicked);
		ShowAchievementsButton.onClick.AddListener(ShowAchievementsButton_Clicked);

		// make sure we don't init the same Score data twice
		if (created) return;
		created = true;

		// classic GUI stuff
		if (!UseUnityUI)
		{
			uiStyle = new GUIStyle()
			{
				alignment = TextAnchor.MiddleCenter,
				fontSize = 32,
				normal = new GUIStyleState() {textColor = Color.white},
			};
		}

		// Leaderboards ---------------------------
		var leaderboards = new LeaderboardDesc[1];
		var leaderboard = new LeaderboardDesc();
		leaderboards[0] = leaderboard;
		var reignScores_LeaderboardID = new System.Guid("f55e3800-eacd-4728-ae4f-31b00aaa63bf");
		leaderboard.SortOrder = LeaderboardSortOrders.Ascending;
		leaderboard.ScoreFormat = LeaderbaordScoreFormats.Numerical;
		leaderboard.ScoreFormat_DecimalPlaces = 0;
		#if UNITY_IOS
		leaderboard.ScoreTimeFormat = LeaderboardScoreTimeFormats.Centiseconds;
		#else
		leaderboard.ScoreTimeFormat = LeaderboardScoreTimeFormats.Milliseconds;
		#endif
		
		// Global
		leaderboard.ID = "Level1";// Any unique ID value you want
		leaderboard.Desc = "Level1 Desc...";// Any desc you want

		// Editor
		leaderboard.Editor_ReignScores_ID = reignScores_LeaderboardID;// Any unique value

		// WinRT
		leaderboard.WinRT_ReignScores_ID = reignScores_LeaderboardID;// Any unique value

		// WP8
		leaderboard.WP8_ReignScores_ID = reignScores_LeaderboardID;// Any unique value

		// BB10
		leaderboard.BB10_ReignScores_ID = reignScores_LeaderboardID;// Any unique value

		// iOS
		leaderboard.iOS_ReignScores_ID = reignScores_LeaderboardID;// Any unique value
		leaderboard.iOS_GameCenter_ID = "";// Set to your GameCenter leaderboard ID

		// Android
		leaderboard.Android_ReignScores_ID = reignScores_LeaderboardID;// Any unique value
		leaderboard.Android_GooglePlay_ID = "";// Set to your GooglePlay leaderboard ID (Not Name)
		leaderboard.Android_GameCircle_ID = "";// Set to your GameCircle leaderboard ID (Not Name)

		// Win32
		leaderboard.Win32_ReignScores_ID = reignScores_LeaderboardID;// Any unique value

		// OSX
		leaderboard.OSX_ReignScores_ID = reignScores_LeaderboardID;// Any unique value

		// Linux
		leaderboard.Linux_ReignScores_ID = reignScores_LeaderboardID;// Any unique value


		// Achievements ---------------------------
		var achievements = new AchievementDesc[1];
		var achievement = new AchievementDesc();
		achievements[0] = achievement;
		var reignScores_AchievementID = new System.Guid("352ce53d-142f-4a10-a4fb-804ad38be879");

		// Global
		achievement.ID = "Achievement1";// Any unique ID value you want
		achievement.Name = "Achievement1";// Any name you want
		achievement.Desc = "Achievement1 Desc...";// Any desc you want

		// When you report an achievement you pass a PercentComplete value.
		// Example: This allows you to change that ratio to something like (0-1000) before the achievement is unlocked.
		achievement.PercentCompletedAtValue = 100;// NOTE: For GooglePlay you must match this value in the developer dashboard under "How many steps are needed?" option.

		// Mark if you want Achievement to use PercentCompleted value or not.
		// Marking this true will make the "PercentComplete" value irrelevant.
		achievement.IsIncremental = true;

		// Editor
		achievement.Editor_ReignScores_ID = reignScores_AchievementID;// Any unique value

		// WinRT
		achievement.WinRT_ReignScores_ID = reignScores_AchievementID;// Any unique value

		// WP8
		achievement.WP8_ReignScores_ID = reignScores_AchievementID;// Any unique value

		// BB10
		achievement.BB10_ReignScores_ID = reignScores_AchievementID;// Any unique value

		// iOS
		achievement.iOS_ReignScores_ID = reignScores_AchievementID;// Any unique index value
		achievement.iOS_GameCenter_ID = "";// Set to your GameCenter achievement ID

		// Android
		achievement.Android_ReignScores_ID = reignScores_AchievementID;// Any unique value
		achievement.Android_GooglePlay_ID = "";// Set to your GooglePlay achievement ID (Not Name)
		achievement.Android_GameCircle_ID = "";// Set to your GameCircle achievement ID (Not Name)

		// Win32
		achievement.Win32_ReignScores_ID = reignScores_AchievementID;// Any unique value

		// OSX
		achievement.OSX_ReignScores_ID = reignScores_AchievementID;// Any unique value

		// Linux
		achievement.Linux_ReignScores_ID = reignScores_AchievementID;// Any unique value


		// Desc ---------------------------
		const string reignScores_gameID = "B2A24047-0487-41C4-B151-0F175BB54D0E";// Get this ID from the Reign-Scores Console.
		var desc = new ScoreDesc();
		if (UseUnityUI) desc.ReignScores_UI = ReignScores_ModernRenderer.GetComponent<Reign.Plugin.ReignScores_UnityUI>() as IScores_UI;
		else desc.ReignScores_UI = ReignScores_ClassicRenderer.GetComponent<MonoBehaviour>() as IScores_UI;
		desc.ReignScores_UI.ScoreFormatCallback += scoreFormatCallback;
		desc.ReignScores_ServicesURL = "http://localhost:5537/Services/";// Set to your server!
		desc.ReignScores_GameKey = "04E0676D-AAF8-4836-A584-DE0C1D618D84";// Set to your servers game_api_key!
		desc.ReignScores_UserKey = "CE8E55E1-F383-4F05-9388-5C89F27B7FF2";// Set to your servers user_api_key!
		desc.LeaderboardDescs = leaderboards;
		desc.AchievementDescs = achievements;

		// Editor
		desc.Editor_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Editor_ReignScores_GameID = reignScores_gameID;

		// WinRT
		desc.WinRT_ScoreAPI = ScoreAPIs.ReignScores;
		desc.WinRT_ReignScores_GameID = reignScores_gameID;

		// WP8
		desc.WP8_ScoreAPI = ScoreAPIs.ReignScores;
		desc.WP8_ReignScores_GameID = reignScores_gameID;

		// BB10
		desc.BB10_ScoreAPI = ScoreAPIs.ReignScores;
		desc.BB10_ReignScores_GameID = reignScores_gameID;

		// iOS
		desc.iOS_ScoreAPI = ScoreAPIs.GameCenter;
		desc.iOS_ReignScores_GameID = reignScores_gameID;

		// Android
		#if GOOGLEPLAY
		desc.Android_ScoreAPI = ScoreAPIs.GooglePlay;
		desc.Android_GooglePlay_DisableUsernameRetrieval = false;// This lets you remove the android.permission.GET_ACCOUNTS requirement if enabled
		#elif AMAZON
		desc.Android_ScoreAPI = ScoreAPIs.GameCircle;
		#else
		desc.Android_ScoreAPI = ScoreAPIs.ReignScores;
		#endif
		desc.Android_ReignScores_GameID = reignScores_gameID;

		// Win32
		desc.Win32_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Win32_ReignScores_GameID = reignScores_gameID;

		// OSX
		desc.OSX_ScoreAPI = ScoreAPIs.ReignScores;
		desc.OSX_ReignScores_GameID = reignScores_gameID;

		// Linux
		desc.Linux_ScoreAPI = ScoreAPIs.ReignScores;
		desc.Linux_ReignScores_GameID = reignScores_gameID;

		// init
		ScoreManager.Init(desc, createdCallback);

		// <<< Reign-Scores manual methods >>>
		//ScoreManager.RequestScores(...);
		//ScoreManager.RequestAchievements(...);
		//ScoreManager.ManualLogin(...);
		//ScoreManager.ManualCreateUser(...);
	}

	private void BackButton_Clicked()
	{
		Application.LoadLevel("MainDemo");
	}

	private void LogoutButton_Clicked()
	{
		ScoreManager.Logout();
		StatusText.text = "Logged out...";
	}

	private void ReportScoreButton_Clicked()
	{
		// Submit score as numerical value or currency
		ScoreManager.ReportScore("Level1", Random.Range(0, 500), reportScoreCallback);

		// Or submit score as floating-point or currency
		//ScoreManager.ReportScore("Level1", 3.14f, reportScoreCallback);

		// Or submit score as TimeSpan
		//ScoreManager.ReportScore("Level1", System.TimeSpan.FromSeconds(2.5f), reportScoreCallback);

		// Or submit score as numerical RAW value if you want to format manually
		//ScoreManager.ReportScoreRaw("Level1", 128, reportScoreCallback);
	}

	private void ReportAchievementButton_Clicked()
	{
		string value = "Achievement" + 1;
		ScoreManager.ReportAchievement(value, 100, reportAchievementCallback);
	}

	private void ShowLeaderboardsButton_Clicked()
	{
		ScoreManager.ShowNativeScoresPage("Level1", showNativePageCallback);
	}

	private void ShowAchievementsButton_Clicked()
	{
		ScoreManager.ShowNativeAchievementsPage(showNativePageCallback);
	}

	private void createdCallback(bool success, string errorMessage)
	{
		if (!success) Debug.LogError(errorMessage);
		else ScoreManager.Authenticate(authenticateCallback);
	}

	private void scoreFormatCallback(long score, out string scoreValue)
	{
		scoreValue = System.TimeSpan.FromSeconds(score).ToString();
	}

	private void authenticateCallback(bool succeeded, string errorMessage)
	{
		Debug.Log("Authenticated: " + succeeded);
		if (succeeded) StatusText.text = "Authenticated as: " + ScoreManager.Username;
		else StatusText.text = "Authenticated: " + succeeded;

		if (!succeeded && errorMessage != null) Debug.LogError(errorMessage);
		if (succeeded) ScoreManager.RequestAchievements(requestAchievementsCallback);
	}

	private void requestAchievementsCallback(Achievement[] achievements, bool succeeded, string errorMessage)
	{
		if (succeeded)
		{
			Debug.Log("Got Achievement count: " + achievements.Length);
			foreach (var achievement in achievements)
			{
				Debug.Log(string.Format("Achievement {0} PercentCompleted {1}", achievement.ID, achievement.PercentComplete));
			}
		}
		else
		{
			string error = "Request Achievements Error: " + errorMessage;
			Debug.LogError(error);
			StatusText.text = error;
		}
	}

	void showNativePageCallback(bool succeeded, string errorMessage)
	{
		disableUI = false;
		Debug.Log("Show Native Page: " + succeeded);
		if (!succeeded)
		{
			Debug.LogError(errorMessage);
			StatusText.text = errorMessage;
		}
	}

	void reportScoreCallback(bool succeeded, string errorMessage)
	{
		Debug.Log("Report Score Done: " + succeeded);
		if (!succeeded)
		{
			Debug.LogError(errorMessage);
			StatusText.text = errorMessage;
		}
	}

	void reportAchievementCallback(bool succeeded, string errorMessage)
	{
		Debug.Log("Report Achievement Done: " + succeeded);
		if (!succeeded)
		{
			Debug.LogError(errorMessage);
			StatusText.text = errorMessage;
		}
	}

	// NOTE: Ignore OnGUI stuff if you are using Unity UI
	#region ClassicGUI
	void OnGUI()
	{
		// don't use classic GUI if were using Unity UI
		if (UseUnityUI) return;

		// Classic GUI mode stuff only...
		if (ScoreManager.IsAuthenticated && !disableUI)
		{
			float offset = 0;
			GUI.Label(new Rect((Screen.width/2)-(256*.5f), offset, 256, 32), "<< Leaderboards & Achievements Demo >>", uiStyle);
			if (GUI.Button(new Rect(0, offset, 64, 32), "Back"))
			{
				gameObject.SetActive(false);
				Application.LoadLevel("MainDemo");
				return;
			}
			offset += 34;

			GUI.Label(new Rect(0, offset, Screen.width, Screen.height/8), "Authenticated Username: " + ScoreManager.Username);

			// Show Leaderboards
			if (GUI.Button(new Rect(0, Screen.height-64, 256, 64), "Show Leaderboard Scores") || Input.GetKeyUp(KeyCode.L))
			{
				disableUI = true;
				ShowLeaderboardsButton_Clicked();
			}

			// Show Achievements
			if (GUI.Button(new Rect(256, Screen.height-64, 256, 64), "Show Achievements") || Input.GetKeyUp(KeyCode.A))
			{
				disableUI = true;
				ShowAchievementsButton_Clicked();
			}

			// Report Score
			if (GUI.Button(new Rect(Screen.width-256, offset, 256, 64), "Report Random Score") || Input.GetKeyUp(KeyCode.S))
			{
				ReportScoreButton_Clicked();
			}

			// Report Achievements
			if (GUI.Button(new Rect(Screen.width-256, 64+offset, 256, 64), "Report Random Achievement") || Input.GetKeyUp(KeyCode.R))
			{
				ReportAchievementButton_Clicked();
			}

			// Logout (NOTE: Some platforms do not support this!)
			if (GUI.Button(new Rect((Screen.width/2)-(256/2), (Screen.height/2)-(64/2)+offset, 256, 64), "Logout") || Input.GetKeyUp(KeyCode.O))
			{
				LogoutButton_Clicked();
			}
		}
		else
		{
			GUI.Label(new Rect(0, 0, 256, 64), "Not Authenticated!");
		}
	}
	#endregion

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) Application.Quit();
	}
}
