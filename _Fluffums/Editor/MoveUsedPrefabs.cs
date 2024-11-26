using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
public static class MoveUsedPrefabs
{
    [MenuItem("Fluffums/Move Used Prefabs")]
    public static void MoveAllUsedPrefabs()
    {
        string fluffumsFolder = "Assets/_Fluffums";
        string usedPrefabsFolder = Path.Combine(fluffumsFolder, "Prefabs");
        if (!AssetDatabase.IsValidFolder(fluffumsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "_Fluffums");
        }
        if (!AssetDatabase.IsValidFolder(usedPrefabsFolder))
        {
            AssetDatabase.CreateFolder(fluffumsFolder, "Prefabs");
        }
        HashSet<string> movedPrefabs = new HashSet<string>();
        GameObject[] sceneObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in sceneObjects)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
                if (prefab != null)
                {
                    string assetPath = AssetDatabase.GetAssetPath(prefab);
                    if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets") && !movedPrefabs.Contains(assetPath))
                    {
                        string newPath = Path.Combine(usedPrefabsFolder, Path.GetFileName(assetPath));
                        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
                        AssetDatabase.MoveAsset(assetPath, newPath);
                        movedPrefabs.Add(assetPath);
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Moved all used prefabs to {usedPrefabsFolder}");
    }
}
