using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EasyButton))]
public class GUIEasyButtonInspector : Editor {

	GUIStyle paddingStyle1;

	public GUIEasyButtonInspector(){


	}

	void OnEnable(){

        paddingStyle1 = new GUIStyle();
        paddingStyle1.padding = new RectOffset(15, 0, 0, 0);

		EasyButton t = (EasyButton)target;
		if (t.NormalTexture==null){
			t.NormalTexture = (Texture2D)Resources.Load("Button_normal");
			EditorUtility.SetDirty(t);
		}
		if (t.ActiveTexture==null){
			t.ActiveTexture = (Texture2D)Resources.Load("Button_active");
			EditorUtility.SetDirty(t);
		}
		
		t.showDebugArea = true;
		EditorUtility.SetDirty(t);

	}
	
	void OnDisable(){
		EasyButton t = (EasyButton)target;
		t.showDebugArea = false;
		
	}
	
	public override void OnInspectorGUI(){
		
		EasyButton t = (EasyButton)target;
	
		// Button Properties
		 HTGUILayout.FoldOut( ref t.showInspectorProperties,"Button properties",false);
		if (t.showInspectorProperties){
			
			EditorGUILayout.BeginVertical(paddingStyle1);		
			
			t.name = EditorGUILayout.TextField("Button name",t.name);
			
			if (t.enable) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.enable = EditorGUILayout.Toggle("Enable button",t.enable);
			if (t.isActivated) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.isActivated = EditorGUILayout.Toggle("Activated",t.isActivated);
			if (t.showDebugArea) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.showDebugArea = EditorGUILayout.Toggle("Show debug area",t.showDebugArea);
			GUI.backgroundColor = Color.white;
			
			HTGUILayout.DrawSeparatorLine(paddingStyle1.padding.left);
			EditorGUILayout.Separator();
			
			if (t.isUseGuiLayout) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.isUseGuiLayout = EditorGUILayout.Toggle("Use GUI Layout",t.isUseGuiLayout);
			GUI.backgroundColor = Color.white;
			if (!t.isUseGuiLayout){
				EditorGUILayout.HelpBox("This lets you skip the GUI layout phase (Increase GUI performance). It can only be used if you do not use GUI.Window and GUILayout inside of this OnGUI call.",MessageType.Warning);	
			}						
			EditorGUILayout.EndVertical();
		}
		
		// Button position and size
		HTGUILayout.FoldOut( ref t.showInspectorPosition,"Button position & size",false);
		if (t.showInspectorPosition){
			GUI.backgroundColor = Color.cyan;
			t.Anchor = (EasyButton.ButtonAnchor)EditorGUILayout.EnumPopup("Anchor",t.Anchor);
			GUI.backgroundColor = Color.white;
			t.Offset = EditorGUILayout.Vector2Field("Offset",t.Offset);	
			t.Scale =  EditorGUILayout.Vector2Field("Scale",t.Scale);	
			
			HTGUILayout.DrawSeparatorLine(paddingStyle1.padding.left);
			EditorGUILayout.Separator();
			
			if (t.isSwipeIn) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.isSwipeIn = EditorGUILayout.Toggle("Swipe in",t.isSwipeIn);
			if (t.isSwipeOut) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
			t.isSwipeOut = EditorGUILayout.Toggle("Swipe out",t.isSwipeOut);
			GUI.backgroundColor = Color.white;
		}
		
		// Event
		 HTGUILayout.FoldOut( ref t.showInspectorEvent,"Button Interaction & Events",false);
		if (t.showInspectorEvent){
			EditorGUILayout.BeginVertical(paddingStyle1);
			
			GUI.backgroundColor = Color.cyan;
			t.interaction = (EasyButton.InteractionType)EditorGUILayout.EnumPopup("Interaction type",t.interaction);
			GUI.backgroundColor = Color.white;
			if (t.interaction == EasyButton.InteractionType.Event){
				if (t.useBroadcast) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
				t.useBroadcast = EditorGUILayout.Toggle("Broadcast messages",t.useBroadcast); 
				GUI.backgroundColor = Color.white;
				if (t.useBroadcast){
					EditorGUILayout.BeginVertical(paddingStyle1);
					t.receiverGameObject =(GameObject) EditorGUILayout.ObjectField("Receiver object",t.receiverGameObject,typeof(GameObject),true);
					GUI.backgroundColor = Color.cyan;
					t.messageMode =(EasyButton.Broadcast) EditorGUILayout.EnumPopup("Sending mode",t.messageMode);
					GUI.backgroundColor = Color.white;
					EditorGUILayout.Separator();
					
					if (t.useSpecificalMethod) GUI.backgroundColor = Color.green; else GUI.backgroundColor = Color.red;
					t.useSpecificalMethod = EditorGUILayout.Toggle("Use specific method",t.useSpecificalMethod);
					GUI.backgroundColor = Color.white;
					if (t.useSpecificalMethod){
						t.downMethodName = EditorGUILayout.TextField("   Down method name",t.downMethodName);
						t.pressMethodName = EditorGUILayout.TextField("   Press method name",t.pressMethodName);
						t.upMethodName = EditorGUILayout.TextField("   Up method name",t.upMethodName);
						
					}
					EditorGUILayout.EndVertical();
				}
			}
			EditorGUILayout.EndVertical();
			
		}
		
		// Button texture
		HTGUILayout.FoldOut(ref t.showInspectorTexture,"Button textures",false);
		if (t.showInspectorTexture){
			EditorGUILayout.BeginVertical(paddingStyle1);
			t.guiDepth = EditorGUILayout.IntField("Gui depth",t.guiDepth);
			EditorGUILayout.Separator();
			t.buttonNormalColor = EditorGUILayout.ColorField("Color",t.buttonNormalColor);
			t.NormalTexture = (Texture2D)EditorGUILayout.ObjectField("Normal texture",t.NormalTexture,typeof(Texture),true);
			EditorGUILayout.Separator();
			t.buttonActiveColor = EditorGUILayout.ColorField("Color",t.buttonActiveColor);
			t.ActiveTexture = (Texture2D)EditorGUILayout.ObjectField("Active texture",t.ActiveTexture,typeof(Texture),true);
			EditorGUILayout.EndVertical();
						
		}
		
		
		
		// Refresh
		if (GUI.changed){
		 	EditorUtility.SetDirty(t);
		}
	}
}


