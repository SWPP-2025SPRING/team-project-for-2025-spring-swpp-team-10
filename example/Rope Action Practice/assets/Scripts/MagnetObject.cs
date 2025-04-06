using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//chatGPT로 생성됨.

public class MagnetObject : MonoBehaviour
{
    public float maxPullForce = 100f;          // 끌어당기는 힘
    public float magnetRadius = 5f;        // 감지 범위
    public float snapDistance = 0.5f;      // 달라붙는 거리
    public Transform attachPoint;          // 플레이어가 달라붙을 위치

    private void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, magnetRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    float distance = Vector3.Distance(attachPoint.position, rb.position);

                    if (distance > snapDistance)
                    {
                        Vector3 direction = (attachPoint.position - rb.position).normalized;

                        // 거리 반비례 힘 계산 (거리 너무 작을 때는 최소 거리로 클램프)
                        float clampedDistance = Mathf.Max(distance, 0.1f);
                        float pullForce = maxPullForce / (clampedDistance * clampedDistance);

                        // 너무 큰 힘 방지
                        pullForce = Mathf.Min(pullForce, maxPullForce);

                        rb.AddForce(direction * pullForce);
                    }
                    else
                    {
                        // 가까우면 '착' 붙기
                        rb.velocity = Vector3.zero;
                        rb.MovePosition(attachPoint.position);
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
    }
}
