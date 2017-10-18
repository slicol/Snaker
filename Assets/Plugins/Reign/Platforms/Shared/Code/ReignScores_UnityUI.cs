using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Reign.Plugin
{
	/// <summary>
	/// Internal use only!
	/// </summary>
	enum ReignScores_UnityUIModes
	{
		None,
		Login,
		CreateUser,
		LoggingIn,
		CreatingUser,
		ShowingScores,
		ShowingAchievements,
		LoadingScores,
		LoadingAchievements
	}

	public class ReignScores_UnityUI : MonoBehaviour, IScores_UI
	{
		/// <summary>
		/// Panel
		/// </summary>
		public GameObject TitlePanel, LoginPanel, CreateUserPanel, LoaderPanel, ScoresPanel, AchievementsPanel;

		/// <summary>
		/// Error message text
		/// </summary>
		public Text LoginScreen_ErrorMessage, CreateUserScreen_ErrorMessage;

		/// <summary>
		/// Loading spinner
		/// </summary>
		public RectTransform Spinner;

		/// <summary>
		/// Login Screen button
		/// </summary>
		public Button LoginScreen_LoginButton, LoginScreen_CreateUserButton, LoginScreen_CancelButton;

		/// <summary>
		/// Login Screen field
		/// </summary>
		public InputField LoginScreen_UsernameInput, LoginScreen_PasswordInput;

		/// <summary>
		/// Create User Screen button
		/// </summary>
		public Button CreateUserScreen_CreateButton, CreateUserScreen_CancleButton;

		/// <summary>
		/// Create User Screen field
		/// </summary>
		public InputField CreateUserScreen_UsernameInput, CreateUserScreen_PasswordInput, CreateUserScreen_PasswordInput2;

		/// <summary>
		/// Native view images
		/// </summary>
		public Image ScoresImage, AchievementsImage;

		/// <summary>
		/// Native view buttons
		/// </summary>
		public Button Scores_NextButton, Scores_PrevButton, Scores_CloseButton, Achievements_NextButton, Achievements_PrevButton, Achievements_CloseButton;

		/// <summary>
		/// Label object
		/// </summary>
		public Text[] Scores_Usernames, Scores_ScoreValues, Achievements_Names, Achievements_Descs;

		/// <summary>
		/// Achievement images
		/// </summary>
		public Image[] AchievementImages;

		/// <summary>
		/// Amount to show on board (Defaults to 10)
		/// </summary>
		public int TopScoresToListPerPage = 10, AchievementsToListPerPage = 10;

		// =========================================================================================================
		// IScores_UI interface methods
		// =========================================================================================================
		public event ScoreFormatCallbackMethod ScoreFormatCallback;
		private IScorePlugin plugin;
		private AuthenticateCallbackMethod authenticateCallback;
		private ShowNativeViewDoneCallbackMethod showNativeViewDoneCallback;
		private int scoreOffset, achievementOffset;
		private string leaderboardID;
		private Achievement[] achievements;

		public void Init(IScorePlugin plugin)
		{
			// set defaults
			this.plugin = plugin;
			mode = ReignScores_UnityUIModes.None;
			fitImageInView(ScoresImage);
			fitImageInView(AchievementsImage);

			// bind buttons
			LoginScreen_LoginButton.onClick.AddListener(LoginScreen_LoginButton_Clicked);
			LoginScreen_CreateUserButton.onClick.AddListener(LoginScreen_CreateUserButton_Clicked);
			LoginScreen_CancelButton.onClick.AddListener(LoginScreen_CancelButton_Clicked);

			CreateUserScreen_CreateButton.onClick.AddListener(CreateUserScreen_CreateButton_Clicked);
			CreateUserScreen_CancleButton.onClick.AddListener(CreateUserScreen_CancleButton_Clicked);

			Scores_NextButton.onClick.AddListener(Scores_NextButton_Clicked);
			Scores_PrevButton.onClick.AddListener(Scores_PrevButton_Clicked);
			Scores_CloseButton.onClick.AddListener(Scores_CloseButton_Clicked);
			Achievements_NextButton.onClick.AddListener(Achievements_NextButton_Clicked);
			Achievements_PrevButton.onClick.AddListener(Achievements_PrevButton_Clicked);
			Achievements_CloseButton.onClick.AddListener(Achievements_CloseButton_Clicked);
		}

		private void fitImageInView(Image image)
		{
			var rect = image.GetComponent<RectTransform>();
			float width = image.mainTexture.width / (float)Screen.width, height = image.mainTexture.height / (float)Screen.height;
			var size = MathUtilities.FitInView(width, height, 1, 1);
			rect.anchorMin = new Vector2(-(size.x*.5f) + .5f, -(size.y*.5f) + .5f);
			rect.anchorMax = new Vector2(-(size.x*.5f) + .5f + size.x, -(size.y*.5f) + .5f + size.y);
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
		}

		public void RequestLogin(AuthenticateCallbackMethod callback)
		{
			if (mode == ReignScores_UnityUIModes.None)
			{
				mode = ReignScores_UnityUIModes.Login;
				authenticateCallback = callback;
			}
		}

		public void AutoLogin(AuthenticateCallbackMethod callback)
		{
			mode = ReignScores_UnityUIModes.None;
			authenticateCallback = callback;
		}

		public void LoginCallback(bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				LoginScreen_ErrorMessage.text = "";
				CreateUserScreen_ErrorMessage.text = "";
				mode = ReignScores_UnityUIModes.None;
				if (authenticateCallback != null) authenticateCallback(true, null);
			}
			else
			{
				string error = errorMessage != null ? errorMessage : "Unknown Error ???";
				LoginScreen_ErrorMessage.text = error;
				CreateUserScreen_ErrorMessage.text = error;
				if (mode == ReignScores_UnityUIModes.LoggingIn) mode = ReignScores_UnityUIModes.Login;
				else if (mode == ReignScores_UnityUIModes.CreatingUser) mode = ReignScores_UnityUIModes.CreateUser;
				else mode = ReignScores_UnityUIModes.Login;
			}
		}

		public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback)
		{
			mode = ReignScores_UnityUIModes.LoadingScores;
			showNativeViewDoneCallback = callback;
			this.leaderboardID = leaderboardID;
			scoreOffset = 0;
			plugin.RequestScores(leaderboardID, scoreOffset, TopScoresToListPerPage, requestScoresCallback, this);
		}

		private void requestScoresCallback(LeaderboardScore[] scores, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				mode = ReignScores_UnityUIModes.ShowingScores;

				// clear values
				foreach (var username in Scores_Usernames) username.text = "";
				foreach (var value in Scores_ScoreValues) value.text = "";

				// set new values
				for (int i = 0; i != TopScoresToListPerPage; ++i)
				{
					if (i >= Scores_Usernames.Length || i >= Scores_ScoreValues.Length || i >= scores.Length) break;

					var score = scores[i];
					Scores_Usernames[i].text = score.Username;

					string scoreValue;
					if (ScoreFormatCallback != null) ScoreFormatCallback(score.Score, out scoreValue);
					else scoreValue = score.Score.ToString();
					Scores_ScoreValues[i].text = scoreValue;
				}
			}
			else
			{
				mode = ReignScores_UnityUIModes.None;
				if (showNativeViewDoneCallback != null) showNativeViewDoneCallback(false, errorMessage);
			}
		}

		public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback)
		{
			mode = ReignScores_UnityUIModes.LoadingAchievements;
			achievementOffset = 0;
			showNativeViewDoneCallback = callback;
			plugin.RequestAchievements(requestAchievementsCallback, this);
		}

		private void requestAchievementsCallback(Achievement[] achievements, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				mode = ReignScores_UnityUIModes.ShowingAchievements;
				this.achievements = achievements;
				processAchievements();
			}
			else
			{
				mode = ReignScores_UnityUIModes.None;
				if (showNativeViewDoneCallback != null) showNativeViewDoneCallback(false, errorMessage);
			}
		}

		private void processAchievements()
		{
			// clear values
			foreach (var username in Achievements_Names) username.text = "";
			foreach (var value in Achievements_Descs) value.text = "";
			foreach (var value in AchievementImages) value.gameObject.SetActive(false);

			// set new values
			for (int i = achievementOffset; i != (achievementOffset+AchievementsToListPerPage); ++i)
			{
				if (i >= Achievements_Names.Length || i >= Achievements_Descs.Length || i >= AchievementImages.Length || i >= achievements.Length) break;

				var achievement = achievements[i];
				Achievements_Names[i].text = achievement.Name;
				Achievements_Descs[i].text = achievement.Desc;
				AchievementImages[i].sprite = achievement.IsAchieved ? achievement.AchievedSprite : achievement.UnachievedSprite;
				AchievementImages[i].gameObject.SetActive(true);
			}
		}

		// =========================================================================================================
		// UI Interaction
		// =========================================================================================================
		private ReignScores_UnityUIModes _mode;
		private ReignScores_UnityUIModes mode
		{
			get {return _mode;}
			set
			{
				switch (value)
				{
					case ReignScores_UnityUIModes.None:
						this.gameObject.SetActive(false);
						break;

					case ReignScores_UnityUIModes.Login:
						this.gameObject.SetActive(true);
						disableAll(_mode != ReignScores_UnityUIModes.LoggingIn);
						LoginPanel.gameObject.SetActive(true);
						LoginScreen_UsernameInput.text = "";
						LoginScreen_PasswordInput.text = "";
						break;

					case ReignScores_UnityUIModes.LoggingIn:
						LoaderPanel.gameObject.SetActive(true);
						LoginScreen_ErrorMessage.text = "";
						plugin.ManualLogin(LoginScreen_UsernameInput.text, LoginScreen_PasswordInput.text, null, this);
						break;

					case ReignScores_UnityUIModes.CreateUser:
						this.gameObject.SetActive(true);
						disableAll(_mode != ReignScores_UnityUIModes.CreatingUser);
						CreateUserPanel.gameObject.SetActive(true);
						CreateUserScreen_UsernameInput.text = "";
						CreateUserScreen_PasswordInput.text = "";
						CreateUserScreen_PasswordInput2.text = "";
						break;

					case ReignScores_UnityUIModes.CreatingUser:
						LoaderPanel.gameObject.SetActive(true);
						CreateUserScreen_ErrorMessage.text = "";
						plugin.ManualCreateUser(CreateUserScreen_UsernameInput.text, CreateUserScreen_PasswordInput.text, null, this);
						break;

					case ReignScores_UnityUIModes.LoadingScores:
					case ReignScores_UnityUIModes.LoadingAchievements:
						this.gameObject.SetActive(true);
						disableAll(true);
						TitlePanel.gameObject.SetActive(false);
						LoaderPanel.gameObject.SetActive(true);
						break;

					case ReignScores_UnityUIModes.ShowingScores:
						LoaderPanel.gameObject.SetActive(false);
						ScoresPanel.gameObject.SetActive(true);
						break;

					case ReignScores_UnityUIModes.ShowingAchievements:
						LoaderPanel.gameObject.SetActive(false);
						AchievementsPanel.gameObject.SetActive(true);
						break;

					default:
						Debug.LogError("Unimplemented type: " + value);
						break;
				}

				_mode = value;
			}
		}

		private void disableAll(bool clearErrors)
		{
			if (clearErrors)
			{
				LoginScreen_ErrorMessage.text = "";
				CreateUserScreen_ErrorMessage.text = "";
			}

			TitlePanel.gameObject.SetActive(true);// enable me though
			LoginPanel.gameObject.SetActive(false);
			CreateUserPanel.gameObject.SetActive(false);
			LoaderPanel.gameObject.SetActive(false);
			ScoresPanel.gameObject.SetActive(false);
			AchievementsPanel.gameObject.SetActive(false);
		}
		
		private void LoginScreen_LoginButton_Clicked()
		{
			if (string.IsNullOrEmpty(LoginScreen_UsernameInput.text) || LoginScreen_UsernameInput.text.Length < 3)
			{
				string error = "Username must be at least 3 characters long";
				Debug.LogError(error);
				LoginScreen_ErrorMessage.text = error;
				return;
			}

			if (string.IsNullOrEmpty(LoginScreen_PasswordInput.text) || LoginScreen_PasswordInput.text.Length < 4)
			{
				string error = "Password must be at least 4 characters long";
				Debug.LogError(error);
				LoginScreen_ErrorMessage.text = error;
				return;
			}

			mode = ReignScores_UnityUIModes.LoggingIn;
		}
		
		private void LoginScreen_CreateUserButton_Clicked()
		{
			mode = ReignScores_UnityUIModes.CreateUser;
		}

		private void LoginScreen_CancelButton_Clicked()
		{
			mode = ReignScores_UnityUIModes.None;
			if (authenticateCallback != null) authenticateCallback(false, "Canceled");
		}

		private void CreateUserScreen_CreateButton_Clicked()
		{
			if (string.IsNullOrEmpty(CreateUserScreen_UsernameInput.text) || CreateUserScreen_UsernameInput.text.Length < 3)
			{
				string error = "Username must be at least 3 characters long";
				Debug.LogError(error);
				CreateUserScreen_ErrorMessage.text = error;
				return;
			}

			if (string.IsNullOrEmpty(CreateUserScreen_PasswordInput.text) || CreateUserScreen_PasswordInput.text.Length < 4)
			{
				string error = "Password must be at least 4 characters long";
				Debug.LogError(error);
				CreateUserScreen_ErrorMessage.text = error;
				return;
			}

			if (string.IsNullOrEmpty(CreateUserScreen_PasswordInput2.text))
			{
				string error = "Please Re-Enter your password";
				Debug.LogError(error);
				CreateUserScreen_ErrorMessage.text = error;
				return;
			}

			if (CreateUserScreen_PasswordInput.text != CreateUserScreen_PasswordInput2.text)
			{
				string error = "Passwords do not match";
				Debug.LogError(error);
				CreateUserScreen_ErrorMessage.text = error;
				return;
			}

			mode = ReignScores_UnityUIModes.CreatingUser;
		}

		private void CreateUserScreen_CancleButton_Clicked()
		{
			mode = ReignScores_UnityUIModes.Login;
		}

		private void Scores_NextButton_Clicked()
		{
			if (Scores_Usernames != null && Scores_Usernames.Length != 0 && string.IsNullOrEmpty(Scores_Usernames[0].text)) return;

			scoreOffset += TopScoresToListPerPage;
			plugin.RequestScores(leaderboardID, scoreOffset, TopScoresToListPerPage, requestScoresCallback, this);
		}

		private void Scores_PrevButton_Clicked()
		{
			if ((scoreOffset - TopScoresToListPerPage) < 0) return;

			scoreOffset -= TopScoresToListPerPage;
			plugin.RequestScores(leaderboardID, scoreOffset, TopScoresToListPerPage, requestScoresCallback, this);
		}

		private void Scores_CloseButton_Clicked()
		{
			mode = ReignScores_UnityUIModes.None;
			if (showNativeViewDoneCallback != null) showNativeViewDoneCallback(true, null);
		}

		private void Achievements_NextButton_Clicked()
		{
			int nextOffset = achievementOffset + AchievementsToListPerPage;
			if (nextOffset >= achievements.Length) return;

			achievementOffset = nextOffset;
			processAchievements();
		}

		private void Achievements_PrevButton_Clicked()
		{
			int nextOffset = achievementOffset - AchievementsToListPerPage;
			if (nextOffset < 0) return;

			achievementOffset = nextOffset;
			processAchievements();
		}

		private void Achievements_CloseButton_Clicked()
		{
			mode = ReignScores_UnityUIModes.None;
			if (showNativeViewDoneCallback != null) showNativeViewDoneCallback(true, null);
		}

		void Update()
		{
			if (LoaderPanel.gameObject.activeSelf) Spinner.Rotate(Vector3.forward, 1);
		}
	}
}