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
        transform.position = player.position - Vector3.up * 0.4f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform")) {
            groundCount++;
            isGround = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Platform")) {
            if (--groundCount <= 0) {
                isGround = false;
            }
        }
    }
}
