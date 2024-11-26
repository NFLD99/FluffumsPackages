using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using System.Collections.Generic;
public class OcclusionCubeGeneratorWindow : EditorWindow
{
    private List<GameObject> selectedObjects = new List<GameObject>();
    [MenuItem("Fluffums/Occlusion Cube Generator")]
    public static void ShowWindow()
    {
        GetWindow<OcclusionCubeGeneratorWindow>("Occlusion Cube Generator");
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Occlusion Cube Generator", EditorStyles.boldLabel);
        if (GUILayout.Button("Select GameObjects"))
        {
            selectedObjects = new List<GameObject>(Selection.gameObjects);
        }
        EditorGUILayout.Space();
        if (selectedObjects.Count > 0)
        {
            EditorGUILayout.LabelField("Selected Objects:");
            foreach (var obj in selectedObjects)
            {
                EditorGUILayout.LabelField(obj.name);
            }
            if (GUILayout.Button("Generate Occlusion Cubes"))
            {
                foreach (var obj in selectedObjects)
                {
                    GenerateOcclusionCubes(obj);
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("No objects selected.");
        }
    }
    private void GenerateOcclusionCubes(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }
        Bounds bounds = CalculateBounds(targetObject);
        if (bounds.size == Vector3.zero)
        {
            Debug.LogWarning($"Object {targetObject.name} has no valid renderers.");
            return;
        }
        float thickness = 0.1f;
        float gap = 3.0f;
        GameObject parentObject = new GameObject("Occlusion - " + targetObject.name)
        {
            transform = { position = targetObject.transform.position }
        };
        ParentConstraint parentConstraint = parentObject.AddComponent<ParentConstraint>();
        ConstraintSource constraintSource = new ConstraintSource
        {
            sourceTransform = targetObject.transform,
            weight = 1.0f
        };
        parentConstraint.AddSource(constraintSource);
        parentConstraint.constraintActive = true;
        parentConstraint.locked = true;
        Vector3[] positions = {
            bounds.center + Vector3.right * (bounds.extents.x + gap + thickness / 2),
            bounds.center + Vector3.left * (bounds.extents.x + gap + thickness / 2),
            bounds.center + Vector3.up * (bounds.extents.y + gap + thickness / 2),
            bounds.center + Vector3.down * (bounds.extents.y + gap + thickness / 2),
            bounds.center + Vector3.forward * (bounds.extents.z + gap + thickness / 2),
            bounds.center + Vector3.back * (bounds.extents.z + gap + thickness / 2)
        };
        Vector3[] scales = {
            new Vector3(thickness, bounds.size.y + 2 * gap, bounds.size.z + 2 * gap),
            new Vector3(thickness, bounds.size.y + 2 * gap, bounds.size.z + 2 * gap),
            new Vector3(bounds.size.x + 2 * gap, thickness, bounds.size.z + 2 * gap),
            new Vector3(bounds.size.x + 2 * gap, thickness, bounds.size.z + 2 * gap),
            new Vector3(bounds.size.x + 2 * gap, bounds.size.y + 2 * gap, thickness),
            new Vector3(bounds.size.x + 2 * gap, bounds.size.y + 2 * gap, thickness)
        };
        string[] cubeNames = { "Right", "Left", "Top", "Bottom", "Front", "Back" };
        Material occlusionMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Fluffums/Mats/Occlusion.mat");
        if (occlusionMaterial == null)
        {
            Debug.LogError("Occlusion material not found.");
            return;
        }
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"Occlusion - Cube_{cubeNames[i]}";
            cube.transform.position = positions[i];
            cube.transform.localScale = scales[i];
            cube.transform.parent = parentObject.transform;
            cube.GetComponent<Renderer>().material = occlusionMaterial;
        }
        Debug.Log($"Generated occlusion cubes for {targetObject.name}");
    }
    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds();
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }
}
