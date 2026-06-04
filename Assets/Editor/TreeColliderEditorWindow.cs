using UnityEngine;
using UnityEditor;

//Script made using Google Gemini - not mine
public class TreeColliderEditorWindow : EditorWindow
{    
    private Terrain targetTerrain;

    [MenuItem("Tools/Terrain Tree Collider Generator")]
    public static void ShowWindow()
    {
        GetWindow<TreeColliderEditorWindow>("Tree Collider Gen");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generate Colliders from Terrain Trees", EditorStyles.boldLabel);

        targetTerrain = (Terrain)EditorGUILayout.ObjectField(
            "Target Terrain",
            targetTerrain,
            typeof(Terrain),
            true
        );

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Colliders", GUILayout.Height(30)))
        {
            GenerateColliders();
        }
    }

    private void GenerateColliders()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("Target Terrain is not assigned!");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;
        TreeInstance[] treeInstances = terrainData.treeInstances;
        TreePrototype[] prototypes = terrainData.treePrototypes;
        Transform terrainTransform = targetTerrain.transform;

        // Create a root holder object in the scene to keep things clean
        GameObject holder = new GameObject($"{targetTerrain.name}_TreeColliders");
        holder.transform.position = terrainTransform.position;

        // Register the root creation for Ctrl+Z Undo
        Undo.RegisterCreatedObjectUndo(holder, "Generate Tree Colliders");

        int colliderCount = 0;

        foreach (TreeInstance instance in treeInstances)
        {
            if (instance.prototypeIndex >= prototypes.Length) continue;

            GameObject originalPrefab = prototypes[instance.prototypeIndex].prefab;
            if (originalPrefab == null) continue;

            // 1. Use PrefabUtility to safely instantiate in the Editor
            GameObject prefabClone = (GameObject)PrefabUtility.InstantiatePrefab(originalPrefab);

            // 2. Unpack completely so it becomes a standard group of GameObjects
            PrefabUtility.UnpackPrefabInstance(
               prefabClone,
               PrefabUnpackMode.Completely,
               InteractionMode.AutomatedAction
           );
            prefabClone.name = $"TreeCollider_{colliderCount}";

            // 3. Set positions and hierarchy
            prefabClone.transform.position = Vector3.Scale(instance.position, terrainData.size) + terrainTransform.position;
            prefabClone.transform.localScale = Vector3.one * instance.widthScale;
            prefabClone.transform.parent = holder.transform;

            // 4. Strip everything except Colliders/Transforms/MeshFilters
            StripPrefabAndChildren(prefabClone);

            // Register the clone into the Undo stack
            Undo.RegisterCreatedObjectUndo(prefabClone, "Generate Tree Colliders");

            colliderCount++;
        }

        // Mark scene as dirty so Unity knows it needs saving
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(targetTerrain.gameObject.scene);
        Debug.Log($"Successfully generated {colliderCount} tree colliders under: {holder.name}");
    }

    private void StripPrefabAndChildren(GameObject targetObject)
    {
        Component[] components = targetObject.GetComponentsInChildren<Component>(true);

        for (int i = components.Length - 1; i >= 0; i--)
        {
            Component comp = components[i];
            if (comp == null) continue;

            // Keep critical components
            if (comp is Transform || comp is Collider || comp is MeshFilter) continue;

            // Use DestroyImmediate for immediate cleanup in Editor environments
            DestroyImmediate(comp);
        }
    }
}
