using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshConverter : MonoBehaviour
{
    public static bool isSphere;
    [SerializeField] private Mesh sphereMesh, capsuleMesh;

    private MeshFilter mesh;
    private Rigidbody rb;
    private SphereCollider sphereCollider;
    private CapsuleCollider hamsterCollider;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>();
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        hamsterCollider = GetComponent<CapsuleCollider>();

        isSphere = false;
        Convert();
    }

    public void Convert()
    {
        if (isSphere) { // sphere -> capsule
            mesh.mesh = capsuleMesh;
            sphereCollider.enabled = false;
            hamsterCollider.enabled = true;

            rb.MovePosition(transform.position + Vector3.up * 0.6f);
            rb.drag = 1f;
            transform.rotation = Quaternion.identity;

            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }
        else { // capsule -> sphere
            mesh.mesh = sphereMesh;
            sphereCollider.enabled = true;
            hamsterCollider.enabled = false;

            rb.drag = 0.2f;

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

    public void ConvertToHamster()
    {
        if (isSphere) Convert();
    }
}
