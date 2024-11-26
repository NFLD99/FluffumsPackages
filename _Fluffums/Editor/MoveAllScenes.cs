using UnityEditor;
using UnityEngine;
using System.IO;

public static class MoveAllScenes
{
    [MenuItem("Fluffums/Move All Scenes")]
    public static void MoveScenes()
    {
        string fluffumsFolder = "Assets/_Fluffums";
        string allScenesFolder = Path.Combine(fluffumsFolder, "Scenes");
        if (!AssetDatabase.IsValidFolder(fluffumsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "_Fluffums");
        }
        if (!AssetDatabase.IsValidFolder(allScenesFolder))
        {
            AssetDatabase.CreateFolder(fluffumsFolder, "Scenes");
        }
        string[] scenePaths = Directory.GetFiles(Application.dataPath, "*.unity", SearchOption.AllDirectories);
        foreach (string scenePath in scenePaths)
        {
            string assetPath = "Assets" + scenePath.Replace(Application.dataPath, "").Replace("\\", "/");
            string newPath = Path.Combine(allScenesFolder, Path.GetFileName(scenePath)).Replace("\\", "/");
            newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
            AssetDatabase.MoveAsset(assetPath, newPath);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Moved all scenes to {allScenesFolder}");
    }
}
