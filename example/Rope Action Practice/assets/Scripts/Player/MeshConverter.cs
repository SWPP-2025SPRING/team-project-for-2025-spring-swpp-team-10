using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshConverter : MonoBehaviour
{
    public Mesh sphereMesh, capsuleMesh;
    public static bool isSphere;

    private MeshFilter mesh;
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private CapsuleCollider capsuleCollider;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>();
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        isSphere = false;
        Convert();
    }

    private void Update()
    {
        
    }

    public void Convert()
    {
        if (isSphere) {
            mesh.mesh = capsuleMesh;
            sphereCollider.enabled = false;
            capsuleCollider.enabled = true;

            rb.MovePosition(transform.position + Vector3.up * 0.6f);
            transform.rotation = Quaternion.identity;

            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }
        else {
            mesh.mesh = sphereMesh;
            sphereCollider.enabled = true;
            capsuleCollider.enabled = false;

            rb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
        }

        isSphere = !isSphere;
    }

    public void ConvertToSphere()
    {
        if (!isSphere) Convert();
    }

    public void ConvertToCapsule()
    {
        if (isSphere) Convert();
    }
}
