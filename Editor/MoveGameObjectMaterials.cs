using UnityEditor;
using UnityEngine;
using System.IO;
public static class MoveGameObjectMaterials
{
    [MenuItem("Fluffums/Move Selected GameObject Materials")]
    public static void MoveMaterialsForSelectedGameObject()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            Debug.LogError("No GameObject selected. Please select a GameObject.");
            return;
        }
        string fluffumsFolder = "Assets/_Fluffums";
        string matsFolder = Path.Combine(fluffumsFolder, "Mats");
        string targetFolder = Path.Combine(matsFolder, selectedObject.name);
        if (!AssetDatabase.IsValidFolder(fluffumsFolder))
        {
            AssetDatabase.CreateFolder("Assets", "_Fluffums");
        }
        if (!AssetDatabase.IsValidFolder(matsFolder))
        {
            AssetDatabase.CreateFolder(fluffumsFolder, "Mats");
        }
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            AssetDatabase.CreateFolder(matsFolder, selectedObject.name);
        }
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.sharedMaterials)
            {
                if (mat == null) continue;
                string assetPath = AssetDatabase.GetAssetPath(mat);
                if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets"))
                {
                    string newPath = Path.Combine(targetFolder, Path.GetFileName(assetPath));
                    newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);
                    AssetDatabase.MoveAsset(assetPath, newPath);
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Moved materials of {selectedObject.name} and its children to {targetFolder}");
    }
}
