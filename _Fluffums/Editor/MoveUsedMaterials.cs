using UnityEditor;
using UnityEngine;
using System.IO;
public static class MoveUsedMaterials
{
    [MenuItem("Fluffums/Move Used Materials")]
    public static void MoveAllUsedMaterials()
    {
        string fluffumsFolder = "Assets/_Fluffums";
        string usedMatsFolder = Path.Combine(fluffumsFolder, "Mats");
        if (!AssetDatabase.IsValidFolder(fluffumsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "_Fluffums");
        }
        if (!AssetDatabase.IsValidFolder(usedMatsFolder))
        {
            AssetDatabase.CreateFolder(fluffumsFolder, "Mats");
        }
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        foreach (Material mat in materials)
        {
            string assetPath = AssetDatabase.GetAssetPath(mat);
            if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets"))
            {
                if (AssetDatabase.LoadMainAssetAtPath(assetPath) is Material)
                {
                    string newPath = Path.Combine(usedMatsFolder, Path.GetFileName(assetPath));
                    newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
                    AssetDatabase.MoveAsset(assetPath, newPath);
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Moved all used materials to {usedMatsFolder}");
    }
}
