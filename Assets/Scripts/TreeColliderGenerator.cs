using System.Linq;
using UnityEngine;

public class TreeColliderGenerator : MonoBehaviour
{
    [Tooltip("The Terrain object that holds the trees.")]
    public Terrain targetTerrain;

    public void Start()
    {
        GenerateColliders();
    }

    public void GenerateColliders()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("Target Terrain is not assigned!");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;
        TreeInstance[] treeInstances = terrainData.treeInstances;
        TreePrototype[] prototypes = terrainData.treePrototypes;
        int colliderCount = 0;

        Transform terrainTransform = targetTerrain.transform;

        // Loop through all painted trees
        foreach (TreeInstance instance in treeInstances)
        {
            // Ensure the index is valid
            if (instance.prototypeIndex >= prototypes.Length) continue;

            // Get the original prefab from the prototype list
            GameObject originalPrefab = prototypes[instance.prototypeIndex].prefab;

            //it would be faster to just copy the prefab >> strip it of anything other then the colliders and transform and then copy over the transform from the terrain
            //Create a copy of the prefab:
            GameObject prefabClone = Instantiate(originalPrefab);
            prefabClone.name = $"TreeCollider_{colliderCount}";
            //copy over the terrains transforms:
            prefabClone.transform.position = Vector3.Scale(instance.position, terrainData.size) + terrainTransform.position;
            prefabClone.transform.localScale = Vector3.one * instance.widthScale;
            prefabClone.transform.parent = this.transform;

            //strip components from children
            StripPrefabAndChildren(prefabClone);
            colliderCount++;
        }
    }
    void StripPrefabAndChildren(GameObject targetObject)
    {
        Component[] components = targetObject.GetComponentsInChildren<Component>(true);
        for (int i = components.Length - 1; i >= 0; i--)
        {
            Component comp = components[i];
            if (comp == null) continue;
            if (comp is Transform || comp is Collider || comp is MeshFilter) continue;

            Destroy(comp);
        }
    }
   
        
    
}