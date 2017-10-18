using UnityEngine;
using System.Collections;

namespace Reign.Plugin
{
	/// <summary>
	/// Internal use only!
	/// </summary>
	enum ReignScores_ClassicGuiModes
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

	public class ReignScores_ClassicGUI : MonoBehaviour, IScores_UI
	{
		/// <summary>
		/// Login UI Title
		/// </summary>
		public string LoginTitle = "Login";
		
		/// <summary>
		/// Create User UI Title
		/// </summary>
		public string CreateUserTitle = "Create Account";

		/// <summary>
		/// Background UI texture. (Or you can set to NULL and use your own backgrounds)
		/// </summary>
		public Texture BackgroudTexture;
		
		/// <summary>
		/// Leaderboard background texture.
		/// </summary>
		public Texture TopScoreBoardTexture;
		
		/// <summary>
		/// Achievement background texture.
		/// </summary>
		public Texture AchievementBoardTexture;

		/// <summary>
		/// CloseBox texture
		/// </summary>
		public Texture TopScoreBoardButton_CloseNormal, TopScoreBoardButton_CloseHover;

		/// <summary>
		/// CloseBox button texture
		/// </summary>
		public Texture AchievementBoardButton_CloseNormal, AchievementBoardButton_CloseHover;

		/// <summary>
		/// Navigation button texture
		/// </summary>
		public Texture TopScoreBoardButton_PrevNormal, TopScoreBoardButton_PrevHover, TopScoreBoardButton_NextNormal, TopScoreBoardButton_NextHover;
		
		/// <summary>
		/// Navigation button texture
		/// </summary>
		public Texture AchievementBoardButton_PrevNormal, AchievementBoardButton_PrevHover, AchievementBoardButton_NextNormal, AchievementBoardButton_NextHover;
		
		/// <summary>
		/// All usernames will fit in this rect. (Auto scales to fit in ReignScores_TopScoreBoardTexture)
		/// </summary>
		public Rect TopScoreBoardFrame_Usernames = new Rect(225, 250, 340, 560);
		
		/// <summary>
		/// All score values will fit in this rect. (Auto scales to fit in ReignScores_TopScoreBoardTexture)
		/// </summary>
		public Rect TopScoreBoardFrame_Scores = new Rect(585, 250, 240, 560);
		
		/// <summary>
		/// Button rect. (Auto scales to fit in ReignScores_TopScoreBoardTexture)
		/// </summary>
		public Rect TopScoreBoardFrame_PrevButton = new Rect(150, 750, 256, 256),
			TopScoreBoardFrame_NextButton = new Rect(650, 750, 256, 256),
			TopScoreBoardFrame_CloseBox = new Rect(750, 50, 128, 128);

		/// <summary>
		/// All achievement names will fit in this rect. (Auto scales to fit in ReignScores_AchievementBoardTexture)
		/// </summary>
		public Rect AchievementBoardFrame_Names = new Rect(225, 250, 270, 560);
		
		/// <summary>
		/// All achievement descs will fit in this rect. (Auto scales to fit in ReignScores_AchievementBoardTexture)
		/// </summary>
		public Rect AchievementBoardFrame_Descs = new Rect(520, 250, 300, 560);
		
		/// <summary>
		/// Button rect. (Auto scales to fit in ReignScores_AchievementBoardTexture)
		/// </summary>
		public Rect AchievementBoardFrame_PrevButton = new Rect(150, 750, 256, 256),
			AchievementBoardFrame_NextButton = new Rect(650, 750, 256, 256),
			AchievementBoardFrame_CloseBox = new Rect(750, 50, 128, 128);

		/// <summary>
		/// Board font size (Defaults to 12)
		/// </summary>
		public int TopScoreBoardFont_Size = 12, AchievementBoardFont_Size = 12;

		/// <summary>
		/// Board font color (Defaults to white)
		/// </summary>
		public Color TopScoreBoardFont_Color = Color.white, AchievementBoardFont_Color = Color.white;

		/// <summary>
		/// Amount to show on board (Defaults to 10)
		/// </summary>
		public int TopScoresToListPerPage = 10, AchievementsToListPerPage = 10;

		/// <summary>
		/// Set to true to visual see where your board rects are placed.
		/// </summary>
		public bool EnableTestRects;

		/// <summary>
		/// Set to your UI audio source
		/// </summary>
		public AudioSource AudioSource;

		/// <summary>
		/// Button click sound
		/// </summary>
		public AudioClip ButtonClick;

		// =========================================================================================================
		// IScores_UI interface methods
		// =========================================================================================================
		public event ScoreFormatCallbackMethod ScoreFormatCallback;
		private IScorePlugin plugin;
		private AuthenticateCallbackMethod authenticateCallback;

		public void Init(IScorePlugin plugin)
		{
			this.plugin = plugin;
		}

		public void RequestLogin(AuthenticateCallbackMethod callback)
		{
			if (guiMode == ReignScores_ClassicGuiModes.None)
			{
				guiMode = ReignScores_ClassicGuiModes.Login;
				authenticateCallback = callback;
			}
		}

		public void AutoLogin(AuthenticateCallbackMethod callback)
		{
			guiMode = ReignScores_ClassicGuiModes.None;
			authenticateCallback = callback;
		}

		public void LoginCallback(bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				errorText = "";
				guiMode = ReignScores_ClassicGuiModes.None;
				if (authenticateCallback != null) authenticateCallback(true, null);
			}
			else
			{
				errorText = errorMessage != null ? errorMessage : "???";
				if (guiMode == ReignScores_ClassicGuiModes.LoggingIn) guiMode = ReignScores_ClassicGuiModes.Login;
				else if (guiMode == ReignScores_ClassicGuiModes.CreatingUser) guiMode = ReignScores_ClassicGuiModes.CreateUser;
			}
		}

		// =========================================================================================================
		// OnGUI Interaction
		// =========================================================================================================
		private ReignScores_ClassicGuiModes guiMode = ReignScores_ClassicGuiModes.None;

		private ShowNativeViewDoneCallbackMethod guiShowNativeViewDoneCallback;
		private LeaderboardScore[] guiScores;
		private void guiRequestScoresCallback(LeaderboardScore[] scores, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				guiScores = scores;
				guiMode = ReignScores_ClassicGuiModes.ShowingScores;
			}
			else
			{
				guiMode = ReignScores_ClassicGuiModes.None;
				if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(false, errorMessage);
			}
		}

		private int guiScoreOffset;
		private string guiLeaderboardID;
		public void ShowNativeScoresPage(string leaderboardID, ShowNativeViewDoneCallbackMethod callback)
		{
			guiMode = ReignScores_ClassicGuiModes.LoadingScores;
			guiShowNativeViewDoneCallback = callback;
			guiLeaderboardID = leaderboardID;
			guiScoreOffset = 0;
			plugin.RequestScores(leaderboardID, guiScoreOffset, TopScoresToListPerPage, guiRequestScoresCallback, this);
		}

		private Achievement[] guiAchievements;
		private void guiRequestAchievementsCallback(Achievement[] achievements, bool succeeded, string errorMessage)
		{
			if (succeeded)
			{
				guiAchievements = achievements;
				guiMode = ReignScores_ClassicGuiModes.ShowingAchievements;
			}
			else
			{
				guiMode = ReignScores_ClassicGuiModes.None;
				if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(false, errorMessage);
			}
		}

		private int guiAchievementOffset;
		public void ShowNativeAchievementsPage(ShowNativeViewDoneCallbackMethod callback)
		{
			guiMode = ReignScores_ClassicGuiModes.LoadingAchievements;
			guiAchievementOffset = 0;
			guiShowNativeViewDoneCallback = callback;
			plugin.RequestAchievements(guiRequestAchievementsCallback, this);
		}

		private string userAccount_Name = "", userAccount_Pass = "", userAccount_ConfPass = "", errorText;
		void OnGUI()
		{
			if (guiMode == ReignScores_ClassicGuiModes.None) return;

			GUI.color = Color.white;
			GUI.matrix = Matrix4x4.identity;
			float scale = new Vector2(Screen.width, Screen.height).magnitude / new Vector2(1280, 720).magnitude;

			// draw background
			if (BackgroudTexture != null)
			{
				var size = MathUtilities.FillView(BackgroudTexture.width, BackgroudTexture.height, Screen.width, Screen.height);
				float offsetX = -Mathf.Max((size.x-Screen.width)*.5f, 0f);
				float offsetY = -Mathf.Max((size.y-Screen.height)*.5f, 0f);
				GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), BackgroudTexture);
			}

			float buttonWidth = 128 * scale;
			float buttonHeight = 64 * scale;
			float textWidth = 256 * scale;
			float textHeight = 32 * scale;
			float y = Screen.height / 2;

			// ======================================
			// Login Mode
			// ======================================
			if (guiMode == ReignScores_ClassicGuiModes.Login)
			{
				// title
				if (!string.IsNullOrEmpty(LoginTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), LoginTitle, style);
				}

				// labels
				GUI.Label(new Rect((Screen.width/2) - textWidth - (10*scale), y, textWidth, textHeight), "Username");
				GUI.Label(new Rect((Screen.width/2) + (10*scale), y, textWidth, textHeight), "Password");
				y += textHeight;

				// text fields
				userAccount_Name = GUI.TextField(new Rect((Screen.width/2) - textWidth - (10*scale), y, textWidth, textHeight), userAccount_Name);
				userAccount_Pass = GUI.PasswordField(new Rect((Screen.width/2) + (10*scale), y, textWidth, textHeight), userAccount_Pass, '*');
				y += textHeight * 2;

				// buttons
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, buttonWidth, buttonHeight), "Cancel"))
				{
					errorText = null;
					guiMode = ReignScores_ClassicGuiModes.None;
					if (authenticateCallback != null) authenticateCallback(false, "Canceled");
				}

				if (GUI.Button(new Rect((Screen.width/2) + (10*scale), y, buttonWidth, buttonHeight), "Login"))
				{
					errorText = null;
					bool validInfo = true;
					if (string.IsNullOrEmpty(userAccount_Name))
					{
						validInfo = false;
						errorText = "Invalid username.";
						Debug.LogError(errorText);
					}
					else if (string.IsNullOrEmpty(userAccount_Pass))
					{
						validInfo = false;
						errorText = "Invalid user password.";
						Debug.LogError(errorText);
					}

					if (validInfo)
					{
						guiMode = ReignScores_ClassicGuiModes.LoggingIn;
						plugin.ManualLogin(userAccount_Name, userAccount_Pass, null, this);
					}
				}

				y += buttonHeight * 2;
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, (buttonWidth*2)+(10*scale), buttonHeight), "Create New User"))
				{
					guiMode = ReignScores_ClassicGuiModes.CreateUser;
					errorText = null;
				}
			}
			// ======================================
			// CreateUser Mode
			// ======================================
			else if (guiMode == ReignScores_ClassicGuiModes.CreateUser)
			{
				// title
				if (!string.IsNullOrEmpty(CreateUserTitle))
				{
					var style = new GUIStyle();
					style.fontSize = (int)(128 * scale);
					style.alignment = TextAnchor.MiddleCenter;
					style.normal.textColor = Color.white;
					GUI.Label(new Rect(0, 0, Screen.width, Screen.height/4), CreateUserTitle, style);
				}

				// labels
				float offsetX = ((10*scale) + textWidth) * -.5f;
				GUI.Label(new Rect((Screen.width/2) - textWidth - (10*scale) + offsetX, y, textWidth, textHeight), "Username");
				GUI.Label(new Rect((Screen.width/2) + (10*scale) + offsetX, y, textWidth, textHeight), "Password");
				GUI.Label(new Rect((Screen.width/2) + (20*scale) + textWidth + offsetX, y, textWidth, textHeight), "Confirm Password");
				y += textHeight;

				// text fields
				userAccount_Name = GUI.TextField(new Rect((Screen.width/2) - textWidth - (10*scale) + offsetX, y, textWidth, textHeight), userAccount_Name);
				userAccount_Pass = GUI.PasswordField(new Rect((Screen.width/2) + (10*scale) + offsetX, y, textWidth, textHeight), userAccount_Pass, '*');
				userAccount_ConfPass = GUI.PasswordField(new Rect((Screen.width/2) + (20*scale) + textWidth + offsetX, y, textWidth, textHeight), userAccount_ConfPass, '*');
				y += textHeight * 2;

				// buttons
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, buttonWidth, buttonHeight), "Cancel"))
				{
					errorText = null;
					guiMode = ReignScores_ClassicGuiModes.None;
					if (authenticateCallback != null) authenticateCallback(false, "Canceled");
				}

				if (GUI.Button(new Rect((Screen.width/2) + (10*scale), y, buttonWidth, buttonHeight), "Create"))
				{
					errorText = null;
					bool validInfo = true;
					if (string.IsNullOrEmpty(userAccount_Name))
					{
						validInfo = false;
						errorText = "Invalid username.";
						Debug.LogError(errorText);
					}
					else if (string.IsNullOrEmpty(userAccount_Pass) || string.IsNullOrEmpty(userAccount_ConfPass))
					{
						validInfo = false;
						errorText = "Invalid user password.";
						Debug.LogError(errorText);
					}
					else if (userAccount_Pass != userAccount_ConfPass)
					{
						validInfo = false;
						errorText = "Passwords dont match.";
						Debug.LogError(errorText);
					}
					else if (userAccount_Pass.Length < 6)
					{
						validInfo = false;
						errorText = "Passwords to short.";
						Debug.LogError(errorText);
					}

					if (validInfo)
					{
						guiMode = ReignScores_ClassicGuiModes.CreatingUser;
						plugin.ManualCreateUser(userAccount_Name, userAccount_Pass, null, this);
					}
				}

				y += buttonHeight * 2;
				if (GUI.Button(new Rect((Screen.width/2) - buttonWidth - (10*scale), y, (buttonWidth*2)+(10*scale), buttonHeight), "Login Existing User"))
				{
					guiMode = ReignScores_ClassicGuiModes.Login;
					errorText = null;
				}
			}
			// ======================================
			// LoggingIn Mode
			// ======================================
			else if (guiMode == ReignScores_ClassicGuiModes.LoggingIn)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Logging In...", style);
			}
			// ======================================
			// Showing Scores Mode
			// ======================================
			else if (guiMode == ReignScores_ClassicGuiModes.ShowingScores)
			{
				if (TopScoreBoardTexture != null)
				{
					// draw board
					var size = MathUtilities.FitInView(TopScoreBoardTexture.width, TopScoreBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), TopScoreBoardTexture);

					// get main scale value
					var mainScale = MathUtilities.ScaleToFitInView(TopScoreBoardTexture.width, TopScoreBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(TopScoreBoardFrame_CloseBox, TopScoreBoardButton_CloseNormal, TopScoreBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						guiMode = ReignScores_ClassicGuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(TopScoreBoardFrame_PrevButton, TopScoreBoardButton_PrevNormal, TopScoreBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiScoreOffset != 0)
						{
							guiScoreOffset -= TopScoresToListPerPage;
							if (guiScoreOffset < 0) guiScoreOffset = 0;
							plugin.RequestScores(guiLeaderboardID, guiScoreOffset, TopScoresToListPerPage, guiRequestScoresCallback, this);
						}
					}

					if (processButton(TopScoreBoardFrame_NextButton, TopScoreBoardButton_NextNormal, TopScoreBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiScores.Length == TopScoresToListPerPage)
						{
							guiScoreOffset += TopScoresToListPerPage;
							plugin.RequestScores(guiLeaderboardID, guiScoreOffset, TopScoresToListPerPage, guiRequestScoresCallback, this);
						}
					}

					// draw names and scores
					var usernameRect = calculateFrame(TopScoreBoardFrame_Usernames, mainScale, offsetX, offsetY);
					var scoreRect = calculateFrame(TopScoreBoardFrame_Scores, mainScale, offsetX, offsetY);
					if (EnableTestRects)
					{
						GUI.Button(usernameRect, "TEST RECT");
						GUI.Button(scoreRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(TopScoreBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = TopScoreBoardFont_Color;
					int userI = 0, scoreI = 0;
					foreach (var score in guiScores)
					{
						// username
						float height = usernameRect.height / TopScoresToListPerPage;
						GUI.Label(new Rect(usernameRect.x, usernameRect.y + userI, usernameRect.width, height), score.Username, style);
						userI += (int)height;

						// score
						height = scoreRect.height / TopScoresToListPerPage;
						string scoreValue;
						if (ScoreFormatCallback != null) ScoreFormatCallback(score.Score, out scoreValue);
						else scoreValue = score.Score.ToString();
						GUI.Label(new Rect(scoreRect.x, scoreRect.y + scoreI, scoreRect.width, height), scoreValue, style);
						scoreI += (int)height;
					}
				}
				else
				{
					errorText = "ReignScores TopScoreBoardTexture MUST be set!";
					Debug.LogError(errorText);
				}
			}
			// ======================================
			// Showing Achievements Mode
			// ======================================
			else if (guiMode == ReignScores_ClassicGuiModes.ShowingAchievements)
			{
				if (AchievementBoardTexture != null)
				{
					// draw board
					var size = MathUtilities.FitInView(AchievementBoardTexture.width, AchievementBoardTexture.height, Screen.width, Screen.height);
					float offsetX = (Screen.width*.5f)-(size.x*.5f);
					float offsetY = (Screen.height*.5f)-(size.y*.5f);
					GUI.DrawTexture(new Rect(offsetX, offsetY, size.x, size.y), AchievementBoardTexture);

					// get main scale value
					var mainScale = MathUtilities.ScaleToFitInView(AchievementBoardTexture.width, AchievementBoardTexture.height, Screen.width, Screen.height);

					// handle buttons
					if (Input.GetKeyUp(KeyCode.Escape) || processButton(AchievementBoardFrame_CloseBox, AchievementBoardButton_CloseNormal, AchievementBoardButton_CloseHover, mainScale, offsetX, offsetY))
					{
						guiMode = ReignScores_ClassicGuiModes.None;
						if (guiShowNativeViewDoneCallback != null) guiShowNativeViewDoneCallback(true, null);
					}

					if (processButton(AchievementBoardFrame_PrevButton, AchievementBoardButton_PrevNormal, AchievementBoardButton_PrevHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset != 0)
						{
							guiAchievementOffset -= AchievementsToListPerPage;
							if (guiAchievementOffset < 0) guiAchievementOffset = 0;
						}
					}

					if (processButton(AchievementBoardFrame_NextButton, AchievementBoardButton_NextNormal, AchievementBoardButton_NextHover, mainScale, offsetX, offsetY))
					{
						if (guiAchievementOffset + AchievementsToListPerPage < guiAchievements.Length)
						{
							guiAchievementOffset += AchievementsToListPerPage;
						}
					}

					// draw names and scores
					var nameRect = calculateFrame(AchievementBoardFrame_Names, mainScale, offsetX, offsetY);
					var descRect = calculateFrame(AchievementBoardFrame_Descs, mainScale, offsetX, offsetY);
					if (EnableTestRects)
					{
						GUI.Button(nameRect, "TEST RECT");
						GUI.Button(descRect, "TEST RECT");
					}
					var style = new GUIStyle();
					style.fontSize = (int)(AchievementBoardFont_Size * scale);
					style.alignment = TextAnchor.LowerLeft;
					style.normal.textColor = AchievementBoardFont_Color;
					int nameI = 0, descI = 0;
					for (int i = guiAchievementOffset; i < guiAchievementOffset+AchievementsToListPerPage; ++i)
					{
						if (i == guiAchievements.Length) break;
						var ach = guiAchievements[i];

						// icon
						float height = nameRect.height / AchievementsToListPerPage;
						float iconSize = height * .8f;
						GUI.DrawTexture(new Rect(nameRect.x, nameRect.y + nameI + height - iconSize, iconSize, iconSize), ach.IsAchieved ? ach.AchievedImage : ach.UnachievedImage);

						// name
						GUI.Label(new Rect(height + nameRect.x, nameRect.y + nameI, nameRect.width, height), ach.Name, style);
						nameI += (int)height;

						// desc
						height = descRect.height / AchievementsToListPerPage;
						GUI.Label(new Rect(descRect.x, descRect.y + descI, descRect.width, height), ach.Desc, style);
						descI += (int)height;
					}
				}
				else
				{
					errorText = "ReignScores AchievementBoardTexture MUST be set!";
					Debug.LogError(errorText);
				}
			}
			// ======================================
			// CreatingUser Mode
			// ======================================
			else if (guiMode == ReignScores_ClassicGuiModes.CreatingUser)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Creating User...", style);
			}
			// ======================================
			// Loading Mode
			// ======================================
			else if (guiMode == ReignScores_ClassicGuiModes.LoadingScores || guiMode == ReignScores_ClassicGuiModes.LoadingAchievements)
			{
				var style = new GUIStyle();
				style.fontSize = (int)(128 * scale);
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;
				GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Loading...", style);
			}

			// error text
			if (!string.IsNullOrEmpty(errorText))
			{
				var errorStyle = new GUIStyle();
				errorStyle.fontSize = (int)(32 * scale);
				errorStyle.alignment = TextAnchor.MiddleCenter;
				errorStyle.normal.textColor = Color.red;
				GUI.Label(new Rect(0, Screen.height-(Screen.height/8), Screen.width, Screen.height/8), errorText, errorStyle);
			}
		}

		private Rect calculateFrame(Rect frame, Vector2 mainScale, float offsetX, float offsetY)
		{
			var rect = frame;
			rect.x = (rect.x / mainScale.x) + offsetX;
			rect.y = (rect.y / mainScale.y) + offsetY;
			rect.width /= mainScale.x;
			rect.height /= mainScale.y;

			return rect;
		}

		private bool processButton(Rect frame, Texture normal, Texture hover, Vector2 mainScale, float offsetX, float offsetY)
		{
			var rect = calculateFrame(frame, mainScale, offsetX, offsetY);

			bool pass;
			var style = new GUIStyle();
			if (normal != null) pass = GUI.Button(rect, normal, style);
			else pass = GUI.Button(rect, "???");

			if (hover != null)
			{
				var pos = Input.mousePosition;
				pos.y = Screen.height - pos.y;
				if (pos.x > rect.xMin && pos.x < rect.xMax && pos.y > rect.yMin && pos.y < rect.yMax) GUI.DrawTexture(rect, hover);
			}

			if (pass)
			{
				if (AudioSource != null && ButtonClick != null) AudioSource.PlayOneShot(ButtonClick);
				return true;
			}

			return false;
		}
	}
}