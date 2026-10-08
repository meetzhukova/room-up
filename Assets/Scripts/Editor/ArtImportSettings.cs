using UnityEditor;
using UnityEngine;

public class ArtImportSettings : AssetPostprocessor
{
    private const string ArtFolder = "Assets/Art";
    private const float PixelsPerUnit = 16f;

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ArtFolder))
        {
            return;
        }

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
    }

    private void OnPreprocessAsset()
    {
        if (!assetPath.StartsWith(ArtFolder) || !(assetImporter is TrueTypeFontImporter fontImporter))
        {
            return;
        }

        fontImporter.fontRenderingMode = FontRenderingMode.HintedRaster;
    }

    [MenuItem("Room Up/Reimport Art")]
    private static void ReimportArt()
    {
        AssetDatabase.ImportAsset(ArtFolder, ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        Debug.Log("Art reimported with pixel art settings.");
    }
}
