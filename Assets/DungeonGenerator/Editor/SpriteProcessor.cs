using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class SpriteProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D textures)
    {
        string lowerCaseAssetPath = assetPath.ToLower();
        bool isInSpritesDirectory = lowerCaseAssetPath.IndexOf("/sprites/") != -1;

        if (isInSpritesDirectory)
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
}
