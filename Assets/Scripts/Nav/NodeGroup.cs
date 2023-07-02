using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Functionality for this script needed later when we only want enemies to know about certian node grids    -lucas
public class NodeGroup : MonoBehaviour
{
    [SerializeField] List<NodeGrid> grids;
    List<GameObject> nodes;

}
