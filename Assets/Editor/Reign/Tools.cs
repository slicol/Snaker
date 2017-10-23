// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Reign;

using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

#if UNITY_IOS && UNITY_5
using UnityEditor.iOS.Xcode;
#endif

namespace Reign.EditorTools
{
	public static class Tools
	{
		#region GeneralTools
		[MenuItem("Edit/Reign/Tools/Print New Guid")]
		static void PrintNewGuid()
		{
			Debug.Log(Guid.NewGuid());
		}

		[MenuItem("Edit/Reign/Tools/Merge Folders")]
		static void MergeFolders()
		{
			string src = EditorUtility.OpenFolderPanel("Src Folder", "", "");
			if (string.IsNullOrEmpty(src)) return;
			string dst = EditorUtility.OpenFolderPanel("Dst Folder", "", "");
			if (string.IsNullOrEmpty(dst)) return;

			Debug.Log("Src Folder: " + src);
			Debug.Log("Dst Folder: " + dst);
			var files = new List<string>();
			gatherFilePaths(src, files);
			foreach (var file in files)
			{
				string newDst = dst + file.Substring(src.Length);
				Directory.CreateDirectory(Path.GetDirectoryName(newDst));
				File.Copy(file, newDst, true);
			}

			AssetDatabase.Refresh();
			Debug.Log("Merge Folders Done!");
		}

		static void gatherFilePaths(string path, List<string> files)
		{
			// add files in path
			var dir = new DirectoryInfo(path);
			foreach (var file in dir.GetFiles())
			{
				if ((file.Attributes & FileAttributes.Hidden) == 0 && (file.Attributes & FileAttributes.Directory) == 0) files.Add(file.FullName);
			}

			// add sub paths
			foreach (var subPath in Directory.GetDirectories(path))
			{
				gatherFilePaths(subPath, files);
			}
		}

		[MenuItem("Edit/Reign/Tools/Clear All PlayerPrefs")]
		static void InitClearAll()
		{
			PlayerPrefs.DeleteAll();
			Debug.Log("PlayerPrefs Cleared!");
		}
	
		[MenuItem("Edit/Reign/Tools/Reset Editor InApps Prefs (While game is running)")]
		static void InitClearInApps()
		{
			if (InAppPurchaseManager.InAppAPIs == null)
			{
				Debug.LogError("The app must be running with the IAP system initialized!");
				return;
			}
	
			foreach (var api in InAppPurchaseManager.InAppAPIs)
			{
				api.ClearPlayerPrefData();
			}

			Debug.Log("PlayerPrefs for IAP Only Cleared!");
		}

		[MenuItem("Edit/Reign/Clean (For troubleshooting only!)")]
		static void Clean()
		{
			if (!EditorUtility.DisplayDialog("Warning!", "This will remove all Reign plugin files.", "OK", "Cancel")) return;

			using (var stream = new FileStream(Application.dataPath+"/Editor/Reign/CleanSettings", FileMode.Open, FileAccess.Read, FileShare.None))
			using (var reader = new StreamReader(stream))
			{
				string file = reader.ReadLine();
				while (!string.IsNullOrEmpty(file))
				{
					file = Application.dataPath + file;
					try
					{
						if (File.Exists(file)) File.Delete(file);
					}
					catch
					{
						Debug.LogError("Failed to delete file: " + file);
					}

					file = reader.ReadLine();
				}
			}

			AssetDatabase.Refresh();
			Debug.Log("Clean Done!");
		}
		#endregion

		#region PostBuildTools
		static void addPostProjectCompilerDirectives(XDocument doc)
		{
			foreach (var element in doc.Root.Elements())
			{
				if (element.Name.LocalName != "PropertyGroup") continue;
				foreach (var subElement in element.Elements())
				{
					if (subElement.Name.LocalName == "DefineConstants")
					{
						// make sure we need to add compiler directive
						bool needToAdd = true;
						foreach (var name in subElement.Value.Split(';', ' '))
						{
							if (name == "REIGN_POSTBUILD")
							{
								needToAdd = false;
								break;
							}
						}

						// add compiler directive
						if (needToAdd) subElement.Value += ";REIGN_POSTBUILD";
					}
				}
			}
		}

		static void addPostProjectReferences(XDocument doc, string pathToBuiltProject, string extraPath, string productName, string extraRefValue)
		{
			XElement sourceElementRoot = null;
			foreach (var element in doc.Root.Elements())
			{
				if (element.Name.LocalName != "ItemGroup") continue;
				foreach (var subElement in element.Elements())
				{
					if (subElement.Name.LocalName == "Compile")
					{
						sourceElementRoot = element;
						break;
					}
				}

				if (sourceElementRoot != null) break;
			}

			if (sourceElementRoot != null)
			{
				var csSources = new string[]
				{
					"Shared/WinRT/EmailPlugin.cs",
					"Shared/WinRT/MarketingPlugin.cs",
					"Shared/WinRT/MessageBoxPlugin.cs",
					"Shared/WinRT/MicrosoftAdvertising_AdPlugin.cs",
					"Shared/WinRT/MicrosoftStore_InAppPurchasePlugin.cs",
					"Shared/WinRT/StreamPlugin.cs",
					"Shared/WinRT/SocialPlugin.cs",
					"Shared/WinRT/WinRTPlugin.cs",

					#if UNITY_WP8
					"WP8/AdMob_AdPlugin.cs",
					"WP8/AdMob_InterstitialAdPlugin.cs",

					"WP8/CurrentAppSimulator/CurrentApp.cs",
					"WP8/CurrentAppSimulator/LicenseInformation.cs",
					"WP8/CurrentAppSimulator/ListingInformation.cs",
					"WP8/CurrentAppSimulator/MockIAP.cs",
					"WP8/CurrentAppSimulator/MockReceiptState.cs",
					"WP8/CurrentAppSimulator/MockReceiptStore.cs",
					"WP8/CurrentAppSimulator/ProductLicense.cs",
					"WP8/CurrentAppSimulator/ProductListing.cs",
					#endif
				};

				foreach (var source in csSources)
				{
					// copy cs file
					string sourcePath = string.Format("{0}/{1}/{2}", Application.dataPath, "Plugins/Reign/Platforms", source);
					string sourceFileName = Path.GetFileName(source);
					File.Copy(sourcePath, string.Format("{0}/{1}{2}/{3}", pathToBuiltProject, productName, extraPath, sourceFileName), true);

					// make sure we need to reference the file
					bool needToRefFile = true;
					foreach (var element in sourceElementRoot.Elements())
					{
						if (element.Name.LocalName == "Compile")
						{
							foreach (var a in element.Attributes())
							{
								if (a.Name.LocalName == "Include" && a.Value == (extraRefValue + sourceFileName))
								{
									needToRefFile = false;
									break;
								}
							}
						}

						if (!needToRefFile) break;
					}

					// add reference to cs proj
					if (needToRefFile)
					{
						var name = XName.Get("Compile", doc.Root.GetDefaultNamespace().NamespaceName);
						var newSource = new XElement(name);
						newSource.SetAttributeValue(XName.Get("Include"), extraRefValue + sourceFileName);
						sourceElementRoot.Add(newSource);
					}
				}
			}
			else
			{
				Debug.LogError("Reign Post Build Error: Failed to find CS source element in proj!");
			}
		}

		[PostProcessBuild]
		static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			#if UNITY_5
			if (target == BuildTarget.WSAPlayer || target == BuildTarget.WP8Player)
			#else
			if (target == BuildTarget.MetroPlayer || target == BuildTarget.WP8Player)
			#endif
			{
				#if UNITY_WP8
				var productName = PlayerSettings.productName.Replace(" ", "").Replace("_", "");
				#else
				var productName = PlayerSettings.productName;
				#endif
				
				#if UNITY_5
				if (EditorUserBuildSettings.wsaSDK == WSASDK.UniversalSDK81 && EditorUserBuildSettings.activeBuildTarget != BuildTarget.WP8Player)
				#else
				if (EditorUserBuildSettings.metroSDK == MetroSDK.UniversalSDK81 && EditorUserBuildSettings.activeBuildTarget != BuildTarget.WP8Player)
				#endif
				{
					var projPath = string.Format("{0}/{1}/{1}.Shared/{1}.Shared.projItems", pathToBuiltProject, productName);
					Debug.Log("Modifying Proj: " + projPath);
					var doc = XDocument.Load(projPath);
					addPostProjectReferences(doc, pathToBuiltProject, string.Format("/{0}.Shared", productName), productName, "$(MSBuildThisFileDirectory)");
					doc.Save(projPath);

					projPath = string.Format("{0}/{1}/{1}.Windows/{1}.Windows.csproj", pathToBuiltProject, productName);
					Debug.Log("Modifying Proj: " + projPath);
					doc = XDocument.Load(projPath);
					addPostProjectCompilerDirectives(doc);
					doc.Save(projPath);

					projPath = string.Format("{0}/{1}/{1}.WindowsPhone/{1}.WindowsPhone.csproj", pathToBuiltProject, productName);
					Debug.Log("Modifying Proj: " + projPath);
					doc = XDocument.Load(projPath);
					addPostProjectCompilerDirectives(doc);
					doc.Save(projPath);
				}
				else
				{
					var projPath = string.Format("{0}/{1}/{1}.csproj", pathToBuiltProject, productName);
					Debug.Log("Modifying Proj: " + projPath);

					var doc = XDocument.Load(projPath);
					addPostProjectCompilerDirectives(doc);
					addPostProjectReferences(doc, pathToBuiltProject, "", productName, "");
					doc.Save(projPath);
				}
			}
			#if UNITY_IOS && UNITY_5
			else if (target == BuildTarget.iOS)
			{
				string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

				var proj = new PBXProject();
				proj.ReadFromString(File.ReadAllText (projPath));

				string targetID = proj.TargetGuidByName ("Unity-iPhone");

				// set custom link flags
				proj.AddBuildProperty (targetID, "OTHER_LDFLAGS", "-all_load");
				proj.AddBuildProperty (targetID, "OTHER_LDFLAGS", "-ObjC");

				// add frameworks
				proj.AddFrameworkToProject(targetID, "AdSupport.framework", true);
				proj.AddFrameworkToProject(targetID, "CoreTelephony.framework", true);
				proj.AddFrameworkToProject(targetID, "EventKit.framework", true);
				proj.AddFrameworkToProject(targetID, "EventKitUI.framework", true);
				proj.AddFrameworkToProject(targetID, "iAd.framework", true);
				proj.AddFrameworkToProject(targetID, "MessageUI.framework", true);
				proj.AddFrameworkToProject(targetID, "StoreKit.framework", true);
				proj.AddFrameworkToProject(targetID, "Security.framework", true);
				proj.AddFrameworkToProject(targetID, "GameKit.framework", true);
				proj.AddFrameworkToProject(targetID, "GoogleMobileAds.framework", false);

				// change GoogleMobileAds to use reletive path
				string projData = proj.WriteToString();
				projData = projData.Replace
				(
					@"/* GoogleMobileAds.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = GoogleMobileAds.framework; path = System/Library/Frameworks/GoogleMobileAds.framework; sourceTree = SDKROOT; };",
					@"/* GoogleMobileAds.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = GoogleMobileAds.framework; path = Frameworks/GoogleMobileAds.framework; sourceTree = ""<group>""; };"
					//@"/* GoogleMobileAds.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = GoogleMobileAds.framework; path = """ + pathToBuiltProject+"/Frameworks/GoogleMobileAds.framework" + @"""; sourceTree = ""<absolute>""; };"
				);

				// change framework search path to include local framework directory
				projData = projData.Replace
				(
					@"FRAMEWORK_SEARCH_PATHS = ""$(inherited)"";",
					@"FRAMEWORK_SEARCH_PATHS = (""$(inherited)"", ""$(PROJECT_DIR)/Frameworks"",);"
				);

				// save proj data
				File.WriteAllText(projPath, projData);

				// extract GoogleMobileAds.framework.zip to xcode framework path
				if (!Directory.Exists(pathToBuiltProject+"/Frameworks/GoogleMobileAds.framework"))
				{
					var startInfo = new System.Diagnostics.ProcessStartInfo();
					startInfo.Arguments = Application.dataPath+"/Plugins/IOS/GoogleMobileAds.framework.zip" + " -d " + pathToBuiltProject+"/Frameworks";
					startInfo.FileName = "unzip";
					startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
					startInfo.CreateNoWindow = true;
					using (var process = System.Diagnostics.Process.Start(startInfo))
					{
						process.WaitForExit();
						int exitCode = process.ExitCode;
						if (exitCode != 0) Debug.LogError("Failed to unzip GoogleMobileAds.framework.zip with ErrorCode: " + exitCode);
					}
				}
			}
			#endif
    	}
		#endregion

		#region PlatformTools
		internal static void applyCompilerDirectives(bool append, params string[] directives)
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			string newValue = "";
			if (string.IsNullOrEmpty(valueBlock))
			{
				foreach (var directive in directives)
				{
					newValue += directive;
					if (directive != directives[directives.Length-1]) newValue += ';';
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}
			else
			{
				var values = valueBlock.Split(';', ' ');
				if (append)
				{
					foreach (var value in values)
					{
						newValue += value + ';';
					}
				}

				foreach (var directive in directives)
				{
					bool exists = false;
					foreach (var value in values)
					{
						if (value == directive)
						{
							exists = true;
							break;
						}
					}

					if (!exists)
					{
						newValue += directive;
						if (directive != directives[directives.Length-1]) newValue += ';';
					}
				}
				
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}

			Debug.Log("Compiler Directives set to: " + newValue);
		}

		internal static void removeCompilerDirectives(params string[] directives)
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			string valueBlock = PlayerSettings.GetScriptingDefineSymbolsForGroup(platform);
			string newValue = "";
			if (!string.IsNullOrEmpty(valueBlock))
			{
				var values = valueBlock.Split(';', ' ');
				foreach (var value in values)
				{
					bool exists = false;
					foreach (var directive in directives)
					{
						if (value == directive)
						{
							exists = true;
							break;
						}
					}

					if (!exists) newValue += value + ';';
				}
				
				if (newValue.Length != 0 && newValue[newValue.Length-1] == ';') newValue = newValue.Substring(0, newValue.Length-1);
				PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, newValue);
			}

			Debug.Log("Compiler Directives set to: " + newValue);
		}

		private static void clearCompilerDirectives()
		{
			var platform = EditorUserBuildSettings.selectedBuildTargetGroup;
			PlayerSettings.SetScriptingDefineSymbolsForGroup(platform, "");
			Debug.Log("Compiler Directives cleard");
		}

		[MenuItem("Edit/Reign/Platform Tools/Disable Reign")]
		private static void DisableReignForPlatform()
		{
			applyCompilerDirectives(true, "DISABLE_REIGN");
		}
		
		[MenuItem("Edit/Reign/Platform Tools/Enable Reign")]
		private static void EnableReignForPlatform()
		{
			removeCompilerDirectives("DISABLE_REIGN");
		}
		#endregion
	}
}