using System;
using UnityEngine;

namespace Reign
{
	/// <summary>
	/// Score API types
	/// </summary>
	public enum ScoreAPIs
	{
		/// <summary>
		/// None
		/// </summary>
		None,

		/// <summary>
		/// ReignScores
		/// </summary>
		ReignScores,

		/// <summary>
		/// GooglePlay
		/// </summary>
		GooglePlay,

		/// <summary>
		/// GameCircle
		/// </summary>
		GameCircle,

		/// <summary>
		/// GameCenter
		/// </summary>
		GameCenter
	}

	/// <summary>
	/// Use for Leaderboard sort orders
	/// </summary>
	public enum LeaderboardSortOrders
	{
		/// <summary>
		/// Ascending
		/// </summary>
		Ascending,

		/// <summary>
		/// Descending
		/// </summary>
		Descending
	}

	/// <summary>
	/// Use for formating score data
	/// </summary>
	public enum LeaderbaordScoreFormats
	{
		/// <summary>
		/// Numerical
		/// </summary>
		Numerical,

		/// <summary>
		/// Time (make sure to set 
		/// </summary>
		Time,

		/// <summary>
		/// Currency
		/// </summary>
		Currency
	}

	public enum LeaderboardScoreTimeFormats
	{
		/// <summary>
		/// Time in minutes
		/// </summary>
		Minutes,

		/// <summary>
		/// Time in seconds
		/// </summary>
		Seconds,

		/// <summary>
		/// Time in centiseconds
		/// </summary>
		Centiseconds,

		/// <summary>
		/// Time in milliseconds
		/// </summary>
		Milliseconds,
	}

	/// <summary>
	/// Leaderboard desc object
	/// </summary>
	public class LeaderboardDesc
	{
		/// <summary>
		/// ID value (Can be any unique string)
		/// </summary>
		public string ID;

		/// <summary>
		/// Name
		/// </summary>
		public string Name;
		
		/// <summary>
		/// Desc
		/// </summary>
		public string Desc;

		/// <summary>
		/// Score format leaderboard shows and receives data in (Default Numerical)
		/// </summary>
		public LeaderbaordScoreFormats ScoreFormat = LeaderbaordScoreFormats.Numerical;

		/// <summary>
		/// Floating point decimal places for numerical or currency scores (Default 0)
		/// </summary>
		public int ScoreFormat_DecimalPlaces;

		/// <summary>
		/// Time format for time based scores (Default Milliseconds)
		/// </summary>
		public LeaderboardScoreTimeFormats ScoreTimeFormat = LeaderboardScoreTimeFormats.Milliseconds;

		/// <summary>
		/// Tells what order to pull scores in (default is Assending)
		/// </summary>
		public LeaderboardSortOrders SortOrder = LeaderboardSortOrders.Ascending;

		/// <summary>
		/// ReignScores ID (Unique value)
		/// </summary>
		public Guid Editor_ReignScores_ID, Win32_ReignScores_ID, OSX_ReignScores_ID, Linux_ReignScores_ID, Web_ReignScores_ID;

		/// <summary>
		/// ReignScores ID (Unique value)
		/// </summary>
		public Guid WinRT_ReignScores_ID, WP8_ReignScores_ID, BB10_ReignScores_ID, iOS_ReignScores_ID, Android_ReignScores_ID;

		/// <summary>
		/// GooglePlay ID (NOTE: Not name)
		/// </summary>
		public string Android_GooglePlay_ID;

		/// <summary>
		/// GameCircle ID (Unique Leaderboard ID)
		/// </summary>
		public string Android_GameCircle_ID;

		/// <summary>
		/// GameCenter ID (NOTE: Not name)
		/// </summary>
		public string iOS_GameCenter_ID;
	}
	
	/// <summary>
	/// Achievement desc object
	/// </summary>
	public class AchievementDesc
	{
		/// <summary>
		/// ID Value (Can be any unique string)
		/// </summary>
		public string ID;

		/// <summary>
		/// Name
		/// </summary>
		public string Name;

		/// <summary>
		/// Desc
		/// </summary>
		public string Desc;

		/// <summary>
		/// The number completed an achievement must until its unlocked. (Default = 100)
		/// </summary>
		public int PercentCompletedAtValue = 100;

		/// <summary>
		/// Mark true if you want to use Incremental/PercentCompleted achievement. (Default = false)
		/// </summary>
		public bool IsIncremental;

		// ReignScores
		/// <summary>
		/// ID value (Unique value)
		/// </summary>
		public Guid Editor_ReignScores_ID, Win32_ReignScores_ID, OSX_ReignScores_ID, Linux_ReignScores_ID, Web_ReignScores_ID;

		/// <summary>
		/// ID value (Unique value)
		/// </summary>
		public Guid WinRT_ReignScores_ID, WP8_ReignScores_ID, BB10_ReignScores_ID, iOS_ReignScores_ID, Android_ReignScores_ID;

		/// <summary>
		/// GooglePlay ID (NOTE: Not name)
		/// </summary>
		public string Android_GooglePlay_ID;

		/// <summary>
		/// GameCircle ID (Unique Achievement ID)
		/// </summary>
		public string Android_GameCircle_ID;

		/// <summary>
		/// GameCenter ID (NOTE: Not name)
		/// </summary>
		public string iOS_GameCenter_ID;
	}

	/// <summary>
	/// Use to implement your own Score UI
	/// </summary>
	public interface IScores_UI
	{
		/// <summary>
		/// Call to invoke user score formating
		/// </summary>
		event ScoreFormatCallbackMethod ScoreFormatCallback;

		/// <summary>
		/// Called after scores API is ready and need to be bound
		/// </summary>
		void Init(Plugin.IScorePlugin plugin);

		/// <summary>
		/// Called when user needs to enter login info or create an account
		/// </summary>
		void RequestLogin(AuthenticateCallbackMethod callback);

		/// <summary>
		/// May be called after Authentication is called and Username/Password was saved
		/// </summary>
		void AutoLogin(AuthenticateCallbackMethod callback);

		/// <summary>
		/// Called after login attempt finishes.
		/// </summary>
		/// <param name="succeeded">If succeeded, user is logged in</param>
		/// <param name="errorMessage">If failed error</param>
		void LoginCallback(bool succeeded, string errorMessage);

		/// <summary>
		/// Called when UI needs to show leaderboard screen
		/// </summary>
		/// <param name="leaderboardID">Leaderboad to show</param>
		/// <param name="callback">Leaderboad callback</param>
		void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback);

		/// <summary>
		/// Called when UI needs to show achievement screen
		/// </summary>
		/// <param name="callback">Achievement callback</param>
		void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback);
	}

	/// <summary>
	/// Score desc object
	/// </summary>
	public class ScoreDesc
	{
		// api
		/// <summary>
		/// Score API type
		/// </summary>
		public ScoreAPIs Editor_ScoreAPI = ScoreAPIs.None, Win32_ScoreAPI = ScoreAPIs.None, OSX_ScoreAPI = ScoreAPIs.None, Linux_ScoreAPI = ScoreAPIs.None,
			Web_ScoreAPI = ScoreAPIs.None, WinRT_ScoreAPI = ScoreAPIs.None, WP8_ScoreAPI = ScoreAPIs.None, BB10_ScoreAPI = ScoreAPIs.None, iOS_ScoreAPI = ScoreAPIs.None,
			Android_ScoreAPI = ScoreAPIs.None;

		/// <summary>
		/// Set to your UI implementation
		/// </summary>
		public IScores_UI ReignScores_UI;

		/// <summary>
		/// Set to your servers url
		/// </summary>
		public string ReignScores_ServicesURL;

		/// <summary>
		/// Set to your servers game_api_key
		/// </summary>
		public string ReignScores_GameKey;

		/// <summary>
		/// Set to your servers user_api_key
		/// </summary>
		public string ReignScores_UserKey;

		/// <summary>
		/// Leaderboard descs
		/// </summary>
		public LeaderboardDesc[] LeaderboardDescs;

		/// <summary>
		/// Achievement descs
		/// </summary>
		public AchievementDesc[] AchievementDescs;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Editor_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Win32_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Linux_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string OSX_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Web_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string WinRT_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string WP8_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string BB10_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string iOS_ReignScores_GameID;
		
		/// <summary>
		/// ReignScores Game ID
		/// </summary>
		public string Android_ReignScores_GameID;

		/// <summary>
		/// Disables the retrieval of the GooglePlay username when you login. (This lets you remove the android.permission.GET_ACCOUNTS requirement) [Defaults to false]
		/// </summary>
		public bool Android_GooglePlay_DisableUsernameRetrieval;
	}
	
	/// <summary>
	/// Leaderboard object
	/// </summary>
	public class LeaderboardScore
	{
		/// <summary>
		/// Username of player
		/// </summary>
		public string Username {get; private set;}

		/// <summary>
		/// Score of player
		/// </summary>
		public long Score {get; private set;}
		
		/// <summary>
		/// Used to construct an leaderboard object
		/// </summary>
		/// <param name="username">Username of player</param>
		/// <param name="score">Score of player</param>
		public LeaderboardScore(string username, long score)
		{
			this.Username = username;
			this.Score = score;
		}
	}
	
	/// <summary>
	/// Achievement object
	/// </summary>
	public class Achievement
	{
		/// <summary>
		/// Use to check if item is achieved
		/// </summary>
		public bool IsAchieved {get; internal set;}

		/// <summary>
		/// Use to check percent complete of achievement
		/// </summary>
		public float PercentComplete {get; internal set;}

		/// <summary>
		/// Use to get achievement ID
		/// </summary>
		public string ID {get; private set;}

		/// <summary>
		/// Use to get achievement name
		/// </summary>
		public string Name {get; private set;}

		/// <summary>
		/// Use to get achievement desc
		/// </summary>
		public string Desc {get; private set;}

		/// <summary>
		/// Use to get achievement achieved image
		/// </summary>
		public Texture2D AchievedImage {get; private set;}

		/// <summary>
		/// Use to get achievement unachieved name
		/// </summary>
		public Texture2D UnachievedImage {get; private set;}

		/// <summary>
		/// Use to get achievement achieved image
		/// </summary>
		public Sprite AchievedSprite {get; private set;}

		/// <summary>
		/// Use to get achievement unachieved name
		/// </summary>
		public Sprite UnachievedSprite {get; private set;}
		
		/// <summary>
		/// Used to construct an achievement object
		/// </summary>
		/// <param name="isAchieved">Is achieved</param>
		/// <param name="id">ID</param>
		/// <param name="name">Name</param>
		/// <param name="desc">Desc</param>
		/// <param name="achievedImage">Achieved Image</param>
		/// <param name="unachievedImage">Unachieved Image</param>
		public Achievement(bool isAchieved, float percentComplete, string id, string name, string desc, Texture2D achievedImage, Texture2D unachievedImage)
		{
			this.IsAchieved = isAchieved;
			this.PercentComplete = percentComplete;
			this.ID = id;
			this.Name = name;
			this.Desc = desc;
			this.AchievedImage = achievedImage;
			this.UnachievedImage = unachievedImage;
			if (achievedImage != null) this.AchievedSprite = Sprite.Create(achievedImage, new Rect(0, 0, achievedImage.width, achievedImage.height), Vector2.zero);
			if (unachievedImage != null) this.UnachievedSprite = Sprite.Create(unachievedImage, new Rect(0, 0, unachievedImage.width, unachievedImage.height), Vector2.zero);
		}
	}

	/// <summary>
	/// Used for creating api
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void CreatedScoreAPICallbackMethod(bool succeeded, string errorMessage);
	
	/// <summary>
	/// Used for authenticating user
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void AuthenticateCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for reporting a score
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void ReportScoreCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for reporting a achievement
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void ReportAchievementCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for requesting scores
	/// </summary>
	/// <param name="scores">Score objects</param>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void RequestScoresCallbackMethod(LeaderboardScore[] scores, bool succeeded, string errorMessage);

	/// <summary>
	/// Used for requesting achievements
	/// </summary>
	/// <param name="achievements">Achievement objects</param>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void RequestAchievementsCallbackMethod(Achievement[] achievements, bool succeeded, string errorMessage);

	/// <summary>
	/// Used for showing native views
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void ShowNativeViewDoneCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Called after reset completed
	/// </summary>
	/// <param name="succeeded">Tells if the API was successful or not.</param>
	/// <param name="errorMessage">Error message or null.</param>
	public delegate void ResetUserAchievementsCallbackMethod(bool succeeded, string errorMessage);

	/// <summary>
	/// Used for formating score values
	/// </summary>
	/// <param name="score">Score value to format</param>
	/// <param name="scoreValue">Output formated score value</param>
	public delegate void ScoreFormatCallbackMethod(long score, out string scoreValue);
}

namespace Reign.Plugin
{
	/// <summary>
	/// Base Score interface object
	/// </summary>
	public interface IScorePlugin
	{
		/// <summary>
		/// Use to check if the user is authenticated
		/// </summary>
		bool IsAuthenticated {get;}

		/// <summary>
		/// Use to get the username or ID
		/// </summary>
		string Username {get;}
	
		/// <summary>
		/// Use to authenticate user
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void Authenticate(AuthenticateCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to logout a user
		/// </summary>
		void Logout();

		/// <summary>
		/// Use to manualy login a user
		/// </summary>
		/// <param name="userID">Username to login</param>
		/// <param name="password">User password</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ManualLogin(string username, string password, AuthenticateCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to manualy create a user
		/// </summary>
		/// <param name="userID">Username to create</param>
		/// <param name="password">User password</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ManualCreateUser(string username, string password, AuthenticateCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to report a score
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID</param>
		/// <param name="score">Score to report</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ReportScore(string leaderboardID, long score, ReportScoreCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to request scores
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID</param>
		/// <param name="offset">Score load offset</param>
		/// <param name="range">Score count to load</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void RequestScores(string leaderboardID, int offset, int range, RequestScoresCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to report a achievement
		/// </summary>
		/// <param name="achievementID">Achievement ID</param>
		/// <param name="percentComplete">Percent Complete</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ReportAchievement(string achievementID, float percentComplete, ReportAchievementCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to request achievements
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void RequestAchievements(RequestAchievementsCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to show native score page
		/// </summary>
		/// <param name="leaderboardID">Leaderboard ID</param>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Use to show native achievement page
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Resets the users achievement progress.
		/// </summary>
		/// <param name="callback">Callback fired when done</param>
		/// <param name="services">Takes in ReignServices object</param>
		void ResetUserAchievementsProgress(ResetUserAchievementsCallbackMethod callback, MonoBehaviour services);

		/// <summary>
		/// Used for update events
		/// </summary>
		void Update();
	}
}
