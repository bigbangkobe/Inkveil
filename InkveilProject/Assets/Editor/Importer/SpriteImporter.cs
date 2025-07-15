using UnityEditor;
using UnityEngine;

/// <summary>
/// 精灵导入器
/// </summary>
public sealed class SpriteImporter : AssetPostprocessor
{
    public void OnPreprocessTexture()
    {
        //TextureImporter textureImporter = assetImporter as TextureImporter;
        //if (textureImporter.textureType != TextureImporterType.Sprite)
        //{
        //    return;
        //}
        //Debug.Log("OnPreprocessTexture处理图片格式");
        //textureImporter.mipmapEnabled = true;
        //textureImporter.wrapMode = TextureWrapMode.Clamp;
        //textureImporter.sRGBTexture = true;
        //textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
        //textureImporter.alphaIsTransparency = true;
        //textureImporter.filterMode = FilterMode.Trilinear;
        //textureImporter.anisoLevel = 1;

        //TextureImporterPlatformSettings textureSettings = new TextureImporterPlatformSettings();
        //textureSettings.name = "Android";
        //textureSettings.overridden = true;
        //textureSettings.maxTextureSize = 4096;
        //textureSettings.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
        //textureSettings.format = TextureImporterFormat.ETC2_RGBA8;
        //textureImporter.SetPlatformTextureSettings(textureSettings);

        //textureSettings = new TextureImporterPlatformSettings();
        //textureSettings.name = "iPhone";
        //textureSettings.overridden = true;
        //textureSettings.maxTextureSize = 4096;
        //textureSettings.format = TextureImporterFormat.Automatic;
        //textureSettings.textureCompression = TextureImporterCompression.Uncompressed;
        //textureImporter.SetPlatformTextureSettings(textureSettings);
    }
}