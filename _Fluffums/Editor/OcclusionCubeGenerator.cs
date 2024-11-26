using UnityEditor;
using UnityEngine;
public static class OcclusionCubeGenerator
{
    [MenuItem("Fluffums/Generate Occlusion Cubes")]
    public static void GenerateOcclusionCubes()
    {
        GameObject targetObject = Selection.activeGameObject;
        if (targetObject == null)
        {
            Debug.LogWarning("No GameObject selected. Please select a GameObject to generate occlusion cubes around.");
            return;
        }
        Bounds bounds = CalculateBounds(targetObject);
        if (bounds.size == Vector3.zero)
        {
            Debug.LogWarning("Selected object does not have valid renderers to calculate bounds. Cannot generate occlusion cubes.");
            return;
        }
        float thickness = 0.1f;
        float gap = 3.0f;
        GameObject parentObject = new GameObject("Occlusion - " + targetObject.name);
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
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = positions[i];
            cube.transform.localScale = scales[i];
            cube.transform.parent = parentObject.transform;
        }
        Debug.Log("Generated occlusion cubes with connected sides and a 3m gap for " + targetObject.name);
    }
    private static Bounds CalculateBounds(GameObject obj)
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
