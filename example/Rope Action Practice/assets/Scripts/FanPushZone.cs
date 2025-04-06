using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanPushZone : MonoBehaviour
{
    public float pushForce = 20f;          // 바람 세기
    public float pushLength = 5f;          // 바람 닿는 거리 (원기둥 길이)
    public float pushRadius = 1f;          // 바람 퍼지는 반경 (원기둥 반지름)
    public Transform windOrigin;          // 바람 시작 위치 (없으면 자기 자신)

    void FixedUpdate()
    {
        Vector3 origin = windOrigin != null ? windOrigin.position : transform.position;
        Vector3 direction = transform.forward;

        // 원기둥 양 끝 점 계산
        Vector3 p1 = origin;
        Vector3 p2 = origin + direction * pushLength;

        Collider[] hits = Physics.OverlapCapsule(p1, p2, pushRadius);

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.CompareTag("Player"))
            {
                Rigidbody rb = hit.attachedRigidbody;
                rb.AddForce(direction * pushForce);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 origin = windOrigin != null ? windOrigin.position : transform.position;
        Vector3 direction = transform.forward;
        Vector3 p1 = origin;
        Vector3 p2 = origin + direction * pushLength;

        // 캡슐 형태 시각화
        Gizmos.DrawWireSphere(p1, pushRadius);
        Gizmos.DrawWireSphere(p2, pushRadius);
        Gizmos.DrawLine(p1 + transform.right * pushRadius, p2 + transform.right * pushRadius);
        Gizmos.DrawLine(p1 - transform.right * pushRadius, p2 - transform.right * pushRadius);
        Gizmos.DrawLine(p1 + transform.up * pushRadius, p2 + transform.up * pushRadius);
        Gizmos.DrawLine(p1 - transform.up * pushRadius, p2 - transform.up * pushRadius);
    }
}
