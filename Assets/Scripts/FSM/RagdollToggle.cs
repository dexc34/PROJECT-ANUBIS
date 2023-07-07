using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Transform RagdollRoot;
    [SerializeField] private bool startRagdoll = false;

    [HideInInspector] public Rigidbody[] rigidbodies;
    private CharacterJoint[] joints;
    private Collider[] colliders;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigidbodies = RagdollRoot.GetComponentsInChildren<Rigidbody>();
        joints = RagdollRoot.GetComponentsInChildren<CharacterJoint>();
        colliders = RagdollRoot.GetComponentsInChildren<Collider>();

        if (startRagdoll)
        {
            EnableRagdoll();
        }
        else
        {
            EnableAnimator();
        }
    }

    public void EnableRagdoll()
    {
        anim.enabled = false;
        foreach (CharacterJoint joint in joints)
        {
            joint.enableCollision = true;
        }
        foreach(Collider collider in colliders)
        {
            collider.enabled = true;
        }
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
        }
    }

    public void EnableAnimator()
    {
        anim.enabled = true;
        foreach (CharacterJoint joint in joints)
        {
            joint.enableCollision = false;
        }
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.detectCollisions = false;
            rigidbody.useGravity = false;
        }
    }

}
