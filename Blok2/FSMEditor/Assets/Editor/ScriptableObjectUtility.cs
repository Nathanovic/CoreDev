using UnityEngine;
using UnityEditor;
using System.IO;

//this class makes it easy to create, name and place unique new ScriptableObject asset files
public static class ScriptableObjectUtility {

	public static void CreateAsset<T>() where T : ScriptableObject{
		T asset = ScriptableObject.CreateInstance<T> ();

		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") {
			path = "Assets";
		} else if (Path.GetExtension (path) != "") {
			path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
		}

		path += "/New " + typeof(T).ToString ();
		SaveAsset<T> (asset, path);

		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	public static void SaveAsset<T>(T asset, string path) where T : ScriptableObject {
		string uniquePath = AssetDatabase.GenerateUniqueAssetPath (path + ".asset");
		AssetDatabase.CreateAsset (asset, uniquePath);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
	}
}
