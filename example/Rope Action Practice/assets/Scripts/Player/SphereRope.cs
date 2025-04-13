using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRope : MonoBehaviour, IRope 
{
    [SerializeField] private float spring = 10000;
    [SerializeField] private float damper = 1, mass = 10;
    [Tooltip("줄 감기/풀기 속도")]
    [SerializeField] private float retractorSpeed = 12;

    private float grapDistance;

    private SpringJoint sj;
    private RaycastHit hit;
    private Transform hitPoint;


    private void Start()
    {
        grapDistance = GetComponent<RopeAction>().grapDistance;
        hitPoint = GetComponent<RopeAction>().hitPoint;
    }


    public void RopeShoot(RaycastHit hit)
    {
        this.hit = hit;

        // SpringJoint 세팅
        float dis = Vector3.Distance(transform.position, hit.point);

        sj = gameObject.AddComponent<SpringJoint>();
        sj.autoConfigureConnectedAnchor = false;
        sj.connectedAnchor = hit.point;

        sj.maxDistance = dis;
        sj.minDistance = dis;
        sj.damper = damper;
        sj.spring = spring;
        sj.massScale = mass;
    }

    public void EndShoot()
    {
        if (sj != null)
            Destroy(sj);
    }

    public void ShortenRope(float value)
    {
        if (sj.maxDistance <= 1) 
            return;

        if (sj.maxDistance < 20) {
            value *= Mathf.Lerp(0.2f, 1, (sj.maxDistance - 1) / 19f); // maxDist가 1일 때는 0.4f, 20일 때는 1f
        }

        sj.maxDistance = sj.minDistance = sj.maxDistance - value * Time.deltaTime;

        if (sj.maxDistance < 1)
            sj.maxDistance = sj.minDistance = 1;
    }

    public void ExtendRope()
    {
        if (sj.maxDistance > grapDistance) 
            return;

        sj.maxDistance = sj.minDistance = sj.maxDistance + retractorSpeed * Time.deltaTime;
    }

    public void RopeUpdate()
    {
        sj.connectedAnchor = hitPoint.position;
    }
}
