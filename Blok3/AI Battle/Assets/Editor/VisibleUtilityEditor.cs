using UnityEngine;
using UnityEditor;
using AI_UtilitySystem;

//this script is purely for visible feedback
//it displays all of the states that the agent has, the utility-score, and the chosen state
[CustomEditor(typeof(UtilityStateMachine))]
public class VisibleUtilityEditor : Editor {

	Rect windowRect = new Rect(10, 30, 100, 10);
	Texture2D defaultStateImg;
	Texture2D activeStateImg;
	const int sideOffset = 5;
	Vector2 stateSize = new Vector2 (200, 20);

	void OnEnable(){
		defaultStateImg = Resources.Load<Texture2D> ("defaultStateImg") as Texture2D;
		activeStateImg = Resources.Load<Texture2D> ("activeStateImg") as Texture2D;
	}

	void OnSceneGUI(){
		string windowTitle = ((UtilityStateMachine)target).name + " states:";
		GUI.skin.window.alignment = TextAnchor.UpperLeft;
		GUI.skin.box.alignment = TextAnchor.MiddleLeft;

		windowRect = GUILayout.Window (0, windowRect, DoMyWindow, windowTitle, GUILayout.Width(370));
		ClampWindowRect ();
	}

	void DoMyWindow(int windowID){ 
		UtilityStateMachine t = (UtilityStateMachine)target;
		GUI.skin.box.alignment = TextAnchor.MiddleLeft;
		for (int i = 0; i < t.allStates.Count; i++) {
			State state = t.allStates [i];
			if (state == null)
				continue;

			string roundedUtilityValue = state.utilityValue.ToString ("F3");
			Texture2D stateTexture = state.isActive ? activeStateImg : defaultStateImg;
			GUI.skin.box.normal.background = stateTexture; 
			GUI.skin.box.clipping = TextClipping.Overflow;
			GUI.skin.box.wordWrap = false;

			GUILayout.BeginHorizontal ();
			GUILayout.Box(((int)state.utility).ToString() + "|" + state.name, GUILayout.Height(stateSize.y), GUILayout.Width(100));
			GUILayout.Box(roundedUtilityValue, GUILayout.Height(stateSize.y), GUILayout.Width(stateSize.x * state.utilityValue));
			GUILayout.EndHorizontal ();
		}

		GUI.DragWindow ();
	}

	void ClampWindowRect(){
		Rect sceneSize = Camera.current.pixelRect;
		windowRect.x = Mathf.Clamp (windowRect.x, sideOffset, sceneSize.width - windowRect.width - sideOffset);
		windowRect.y = Mathf.Clamp (windowRect.y, sideOffset + 15, sceneSize.height - windowRect.height + 20 - sideOffset);
	}
}
