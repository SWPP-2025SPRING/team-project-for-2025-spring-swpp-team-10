using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshConverter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject hamster;
    [SerializeField] private GameObject ball;

    private CapsuleCollider hamCol;
    private Renderer[] hamRds;
    private Rigidbody rb;
    public static bool isSphere;

    private void Start()
    {
        hamCol = hamster.GetComponent<CapsuleCollider>();
        hamRds = hamster.GetComponentsInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();

        //isSphere = true;
        //Convert();
    }

    public void Convert()
    {
        if (isSphere) { // sphere -> hamster
            HamsterSetActive(true);
            ball.SetActive(false);
            animator.SetTrigger("ChangeToHamster");
            
            rb.MovePosition(transform.position + Vector3.up * 0.5f);
            rb.drag = 1f;
            transform.rotation = Quaternion.identity;

            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationY;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;
        }
        else { // hamster -> sphere
            animator.SetTrigger("ChangeToSphere");
            Invoke(nameof(ChangeObject), 0.4f);

            rb.MovePosition(transform.position + Vector3.up * 0.5f);
            rb.drag = 0.2f;

            rb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
        }

        isSphere = !isSphere;
    }

    private void ChangeObject()
    {
        rb.MovePosition(transform.position + Vector3.up * 0.5f);
        ball.SetActive(true);
        HamsterSetActive(false);
    }

    private void HamsterSetActive(bool value)
    {
        hamCol.enabled = value;
        foreach (Renderer rd in hamRds)
            rd.enabled = value;
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