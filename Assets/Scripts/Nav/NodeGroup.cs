using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//groups all grids and takes their nodes to create one collective set of nodes   -lucas
public class NodeGroup : MonoBehaviour
{
    [SerializeField] List<NodeGrid> grids;
    [HideInInspector] public List<Node> nodesInGroup;

    void Start()
    {
        //yield return new WaitForSeconds(0.0001f);
        for(int i = 0; i < grids.Count; i++)
        {
            nodesInGroup.AddRange (grids[i].nodeScript);
            grids[i].nodeScript.Clear();
        }
    }

}
