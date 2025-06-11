using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnityAssetProcessor : AssetPostprocessor
{

    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.mipmapEnabled = false;
        textureImporter.filterMode = FilterMode.Bilinear;
        textureImporter.maxTextureSize = 1024;
        textureImporter.textureCompression = TextureImporterCompression.Compressed;
    }
}
