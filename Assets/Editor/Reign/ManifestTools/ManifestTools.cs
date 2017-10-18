// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Linq;

namespace Reign.EditorTools
{
	public static class ManifestTools
	{
		[MenuItem("Edit/Reign/Manifests/Merge Reign Android Manifest")]
		static void MergeAndroidManifest()
		{
			if (!EditorUtility.DisplayDialog("Android Manifest Merge Tool", "Are you sure? You can also reference ReignAndroidManifest.xml and manually merge.\n\nThis tool will merge ReignAndroidManifest.xml with AndroidManifest.xml", "Ok", "Cancel")) return;
	
			XDocument mainDoc = XDocument.Load(Application.dataPath+"/Plugins/Android/AndroidManifest.xml");
			XDocument reignDoc = XDocument.Load(Application.dataPath+"/Plugins/Android/ReignAndroidManifest.xml");
			var context = reignDoc.Descendants("Types").Descendants("Type").Except(mainDoc.Descendants("Types").Descendants("Type")).ToArray();
			mainDoc.Root.Add(context);
		
			var settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.IndentChars = "\t";
			using (var writer = XmlWriter.Create(Application.dataPath+"/Plugins/Android/AndroidManifest.xml", settings))
			{
				mainDoc.WriteTo(writer);
			}
		
			EditorUtility.DisplayDialog("Android Manifest Merge Tool", "Successful!", "Ok");
		}

		[MenuItem("Edit/Reign/Manifests/Save current AndroidManifest")]
		static void SaveCustomManifiest()
		{
			string filename = EditorUtility.SaveFilePanel("Android Manifest", Application.dataPath+"/Editor/Reign/ManifestTools/DefaultAndroidManifests", "AndroidManifest_Custom", "xml");
			if (string.IsNullOrEmpty(filename)) return;
			
			AssetDatabase.SaveAssets();
			File.Copy(Application.dataPath + "/Plugins/Android/AndroidManifest.xml", filename, true);
			AssetDatabase.Refresh();
		}

		[MenuItem("Edit/Reign/Manifests/Load custom AndroidManifest")]
		static void LoadCustomManifiest()
		{
			string filename = EditorUtility.OpenFilePanel("Android Manifest", Application.dataPath+"/Editor/Reign/ManifestTools/DefaultAndroidManifests", "xml");
			if (string.IsNullOrEmpty(filename)) return;
			
			loadAndroidManifiest(filename);
		}

		internal static void loadAndroidManifiest(string filename)
		{
			AssetDatabase.SaveAssets();
			File.Copy(filename, Application.dataPath + "/Plugins/Android/AndroidManifest.xml", true);
			AssetDatabase.Refresh();
		}
	}
}