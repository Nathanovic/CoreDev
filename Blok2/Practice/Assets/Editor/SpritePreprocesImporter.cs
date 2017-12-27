using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class SpritePreprocesImporter : AssetPostprocessor {

	void OnPreprocessTexture(){
		if (assetPath == "CustomSprites") {
			TextureImporter importer = (TextureImporter)assetImporter;
			Debug.Log ("importing new sprite! " + importer.name);
			importer.spriteImportMode = SpriteImportMode.Multiple;
		}
	}
}
