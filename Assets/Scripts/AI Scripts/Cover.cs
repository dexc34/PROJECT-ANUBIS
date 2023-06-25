using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    //Creates gizmo for cover, making it easier to design where cover is
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
    }

}
