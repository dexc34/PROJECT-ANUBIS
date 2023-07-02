using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCircle : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] GameObject nodeTemplate;
    List<GameObject> nodes;
    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius, 3f);
    }

    private void Start()
    {
        //PlaceNodes();
    }

    /*void PlaceNodes()
    {
        //PLACE +X NODES 
        for (int i = 0; i <= radius; i++)
        {
            nodes.Add (Instantiate(nodeTemplate, transform.position + new Vector3 (i, 0, 0), transform.rotation));
            nodes[i].transform.SetParent(transform);
        }
        //PLACE -X NODES 
        for (int i = 0; i <= radius; i++)
        {
            nodes.Add(Instantiate(nodeTemplate, transform.position + new Vector3(-i, 0, 0), transform.rotation));
            nodes[i].transform.SetParent(transform);
        }
        //PLACE +Z NODES 
        for (int i = 0; i <= radius; i++)
        {
            nodes.Add(Instantiate(nodeTemplate, transform.position + new Vector3(0, 0, i), transform.rotation));
            nodes[i].transform.SetParent(transform);
        }
        //PLACE -Z NODES 
        for (int i = 0; i <= radius; i++)
        {
            nodes.Add(Instantiate(nodeTemplate, transform.position + new Vector3(0, 0, -i), transform.rotation));
            nodes[i].transform.SetParent(transform);
        }
    }*/
}
