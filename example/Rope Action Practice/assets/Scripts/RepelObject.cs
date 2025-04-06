using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player가 가까이 오면 밀어내는 오브젝트
public class RepelObject : MonoBehaviour
{
    public float maxPushForce = 100f;       // 최대 밀어내는 힘
    public float repelRadius = 5f;          // 반응 범위
    public float stopDistance = 1f;         // 이 거리보다 멀면 아무것도 안함
    public Transform repelCenter;          // 밀어내는 중심 위치 (기본은 자기 자신)

    private void FixedUpdate()
    {
        Vector3 center = repelCenter != null ? repelCenter.position : transform.position;

        Collider[] hits = Physics.OverlapSphere(center, repelRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    float distance = Vector3.Distance(center, rb.position);

                    if (distance < stopDistance)
                        continue;

                    Vector3 direction = (rb.position - center).normalized;

                    // 거리 반비례 힘 계산 (가까울수록 세게 밀림)
                    float clampedDistance = Mathf.Max(distance, 0.1f);
                    float pushForce = maxPushForce / (clampedDistance * clampedDistance);

                    pushForce = Mathf.Min(pushForce, maxPushForce);

                    rb.AddForce(direction * pushForce);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = repelCenter != null ? repelCenter.position : transform.position;
        Gizmos.DrawWireSphere(center, repelRadius);
    }
}
