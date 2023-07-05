using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//places nodes within the bounds of the grid
public class NodeGrid : MonoBehaviour
{
    public List<GameObject> nodesToSpawn;
    [SerializeField] int gridX;
    [SerializeField] int gridZ;
    [SerializeField] float gridSpacingOffset = 1f;

    [SerializeField] LayerMask blocker;

    GameObject clone;
    [HideInInspector] public List<Node> nodeScript;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f); 
        Vector3 pivotPosition = transform.position;
        pivotPosition.x += gridX / 2;
        pivotPosition.z += gridZ / 2;
        Gizmos.DrawCube(pivotPosition, new Vector3(gridX, 1, gridZ));
    }

    private void Awake()
    {
        SpawnGrid();
    }

    //spawns the grid + the nodes
    void SpawnGrid()
    {
        for (int x = 0; x < gridX; x++)
        {
            for (int z = 0; z < gridZ; z++)
            {
                Vector3 spawnPosition = new Vector3(x * gridSpacingOffset, 0, z * gridSpacingOffset);
                spawnPosition += transform.position;
                PickAndSpawn(spawnPosition, Quaternion.identity);
            }
        }
    }

    //spawns nodes. if node is in wall or no ground near the node, it WONT spawn.
    void PickAndSpawn (Vector3 positionToSpawn, Quaternion rotationToSpawn)
    {
        bool isNotDestroyed;
        int randomIndex = Random.Range(0, nodesToSpawn.Count);
        clone = Instantiate(nodesToSpawn[randomIndex], positionToSpawn, rotationToSpawn);
        clone.transform.SetParent(transform);

        isNotDestroyed = true;
        RaycastHit hit;
        if (Physics.CheckSphere(clone.transform.position, 1f, blocker))
        {
            Destroy(clone);
            isNotDestroyed = false;
        }
        if (Physics.Raycast(clone.transform.position, transform.TransformDirection(Vector3.down), out hit, 2f))
        {
            //Do nothing
        }
        else
        {
            Destroy(clone);
            isNotDestroyed = false;
        }
        if (isNotDestroyed)
        {
            nodeScript.Add(clone.GetComponent<Node>());
        }
    }
}
