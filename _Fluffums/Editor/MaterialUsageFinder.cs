using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class MaterialUsageFinder : EditorWindow
{
    private Material selectedMaterial;
    [MenuItem("Fluffums/Find Material Usage")]
    public static void ShowWindow()
    {
        GetWindow<MaterialUsageFinder>("Find Material Usage");
    }
    private void OnGUI()
    {
        selectedMaterial = (Material)EditorGUILayout.ObjectField("Material", selectedMaterial, typeof(Material), false);
        if (GUILayout.Button("Find and Select Usage"))
        {
            if (selectedMaterial != null)
            {
                FindAndSelectMaterialUsage();
            }
            else
            {
                Debug.LogWarning("No material selected.");
            }
        }
    }
    private void FindAndSelectMaterialUsage()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        List<GameObject> matchingObjects = new List<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.TryGetComponent<Renderer>(out Renderer renderer) && renderer.sharedMaterials != null)
            {
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat == selectedMaterial)
                    {
                        matchingObjects.Add(obj);
                        break;
                    }
                }
            }
        }
        if (matchingObjects.Count > 0)
        {
            Selection.objects = matchingObjects.ToArray();
            Debug.Log($"Found and selected {matchingObjects.Count} GameObjects using the material.");
        }
        else
        {
            Debug.LogWarning("No GameObjects found using the selected material.");
        }
    }
}
