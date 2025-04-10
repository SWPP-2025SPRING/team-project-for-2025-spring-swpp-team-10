using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Transform player;
    public bool isGround = false;
    private int groundCount = 0; // 현재 닿아있는 플랫폼의 개수

    void Update()
    {
        if (MeshConverter.isSphere)
            transform.position = player.position - Vector3.up * 0.4f;
        else
            transform.position = player.position - Vector3.up * 0.9f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) {
            groundCount++;
            isGround = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) {
            if (--groundCount <= 0) {
                isGround = false;
            }
        }
    }
}
