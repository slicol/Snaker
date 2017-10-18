using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EasyTouch))]
public class GUIEasyTouchInspector : Editor {

	GUIStyle paddingStyle1;

	public GUIEasyTouchInspector(){


	}

    void OnEnable()
    {
        paddingStyle1 = new GUIStyle();
        paddingStyle1.padding = new RectOffset(15, 0, 0, 0);
    }

	public override void OnInspectorGUI(){
			
		EasyTouch t = (EasyTouch)target;
		
		HTGUILayout.FoldOut( ref t.showGeneral,"General properties", false);
		if (t.showGeneral){
			if (t.enable) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.enable = EditorGUILayout.Toggle("Enable EasyTouch",t.enable);
			if (t.enableRemote) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.enableRemote = EditorGUILayout.Toggle("Enable unity remote",t.enableRemote);
			
			
			if (t.useBroadcastMessage) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.useBroadcastMessage = EditorGUILayout.BeginToggleGroup("Broadcast messages",t.useBroadcastMessage);
			GUI.backgroundColor = Color.white;
			if (t.useBroadcastMessage){
				EditorGUILayout.BeginVertical(paddingStyle1);
				t.receiverObject = (GameObject)EditorGUILayout.ObjectField("Other receiver",t.receiverObject,typeof(GameObject),true);
				if (t.isExtension) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
				t.isExtension = EditorGUILayout.Toggle("Joysticks & buttons",t.isExtension);
				GUI.backgroundColor = Color.white;
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.EndToggleGroup();
			EditorGUILayout.Space();
			
			if (t.enableReservedArea) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.enableReservedArea = EditorGUILayout.Toggle("Enable reserved area",t.enableReservedArea);
			EditorGUILayout.Space();
			if (t.enabledNGuiMode) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.enabledNGuiMode = EditorGUILayout.Toggle("Enable NGUI compatibilty",t.enabledNGuiMode);
			GUI.backgroundColor = Color.white;
			if (t.enabledNGuiMode){
					EditorGUILayout.BeginVertical(paddingStyle1);
				
					// Camera
					serializedObject.Update();
			   		EditorGUIUtility.LookLikeInspector();
			    	SerializedProperty cameras = serializedObject.FindProperty("nGUICameras");
					EditorGUILayout.PropertyField( cameras,true);
			   		serializedObject.ApplyModifiedProperties();
					EditorGUIUtility.LookLikeControls();
					
					EditorGUILayout.Space();
				
					// layers
					serializedObject.Update();
			   		EditorGUIUtility.LookLikeInspector();
			    	SerializedProperty layers = serializedObject.FindProperty("nGUILayers");
					EditorGUILayout.PropertyField( layers,false);
			   		serializedObject.ApplyModifiedProperties();
					EditorGUIUtility.LookLikeControls();
				
					EditorGUILayout.EndVertical();
			}			
		}
			
		if (t.enable){
			
			// Auto select porperties
			 HTGUILayout.FoldOut( ref t.showSelect,  "Auto-select properties",false);
			if (t.showSelect){
				t.easyTouchCamera = (Camera)EditorGUILayout.ObjectField("Camera",t.easyTouchCamera,typeof(Camera),true);
				if (t.autoSelect) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
				t.autoSelect = EditorGUILayout.Toggle("Enable auto-select",t.autoSelect);
				GUI.backgroundColor = Color.white;
				if (t.autoSelect){
					
					serializedObject.Update();
			   		EditorGUIUtility.LookLikeInspector();
			    	SerializedProperty layers = serializedObject.FindProperty("pickableLayers");
					EditorGUILayout.PropertyField( layers,true);
			   		serializedObject.ApplyModifiedProperties();
					EditorGUIUtility.LookLikeControls();
				}
			}
				
			// General gesture properties
			HTGUILayout.FoldOut(ref t.showGesture, "General gesture properties",false);
				if (t.showGesture){
				t.StationnaryTolerance = EditorGUILayout.FloatField("Stationary tolerance",t.StationnaryTolerance);
				t.longTapTime = EditorGUILayout.FloatField("Long tap time",t.longTapTime);
				t.swipeTolerance = EditorGUILayout.FloatField("Swipe tolerance",t.swipeTolerance);
			}
			
			// Two fingers gesture
			HTGUILayout.FoldOut(ref t.showTwoFinger, "Two fingers gesture properties",false);
			if (t.showTwoFinger){
				if (t.enable2FingersGesture) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
				t.enable2FingersGesture = EditorGUILayout.Toggle("2 fingers gesture",t.enable2FingersGesture);
				GUI.backgroundColor = Color.white;
				if (t.enable2FingersGesture){
					EditorGUILayout.Separator();
					if (t.enablePinch) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
					t.enablePinch = EditorGUILayout.Toggle("Enable Pinch",t.enablePinch);
					GUI.backgroundColor = Color.white;
					if (t.enablePinch){
						t.minPinchLength = EditorGUILayout.FloatField("Min pinch length",t.minPinchLength);
					}
					EditorGUILayout.Separator();
					if (t.enableTwist) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
					t.enableTwist = EditorGUILayout.Toggle("Enable twist",t.enableTwist);
					GUI.backgroundColor = Color.white;
					if (t.enableTwist){
						t.minTwistAngle = EditorGUILayout.FloatField("Min twist angle",t.minTwistAngle);
					}
					
					EditorGUILayout.Separator();
					
				}
			}
			
			// Second Finger simulation
			HTGUILayout.FoldOut(ref t.showSecondFinger, "Second finger simulation",false);
			if (t.showSecondFinger){
				if (t.secondFingerTexture==null){
					t.secondFingerTexture =Resources.Load("secondFinger") as Texture;
				}
				
				t.secondFingerTexture = (Texture)EditorGUILayout.ObjectField("Texture",t.secondFingerTexture,typeof(Texture),true);
				EditorGUILayout.HelpBox("Change the keys settings for a fash compilation, or if you want to change the keys",MessageType.Info);
				t.twistKey = (KeyCode)EditorGUILayout.EnumPopup( "Twist & pinch key", t.twistKey);	
				t.swipeKey = (KeyCode)EditorGUILayout.EnumPopup( "Swipe key", t.swipeKey);
			}
		}		
		
	}
}
