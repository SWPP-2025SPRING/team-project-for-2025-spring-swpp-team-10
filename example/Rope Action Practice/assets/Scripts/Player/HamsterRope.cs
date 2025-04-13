using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterRope : MonoBehaviour, IRope
{
    public static bool onGrappling { get; private set; }
    public static float speedFactor { get; private set; }
    public static Rigidbody grapRb { get; private set; }

    [SerializeField] private float spring = 1000;
    [SerializeField] private float damper = 1, mass = 10;
    [SerializeField] private float retractorForce;
    [SerializeField] private float retractorMaxSpeed;

    private float grapDistance;
    private float retractorSpeed;

    private Rigidbody rb;
    private SpringJoint sj;
    private RaycastHit hit;
    private Transform hitPoint;
    private Transform grapTransform;


    private void Start()
    {
        onGrappling = false;
        speedFactor = 1f;

        rb = GetComponent<Rigidbody>();

        grapDistance = GetComponent<RopeAction>().grapDistance;
        retractorSpeed = GetComponent<RopeAction>().retractorSpeed;
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

        sj.maxDistance = dis * 1.1f;
        sj.minDistance = dis * 0.9f;
        sj.damper = damper;
        sj.spring = spring;
        sj.massScale = mass;

        onGrappling = true;
        grapRb = hit.collider.gameObject.GetComponent<Rigidbody>();
        grapTransform = hit.collider.transform;
        speedFactor = rb.mass / (rb.mass + grapRb.mass);
    }

    public void EndShoot()
    {
        if (sj != null) {
            Destroy(sj);
            onGrappling = false;
        }
    }

    public void ShortenRope(float value)
    {
        if (sj.maxDistance <= 1) 
            return;

        float _retractorMaxSpeed = retractorMaxSpeed;
        if (value > retractorSpeed) // 마우스 우클릭으로 빠르게 당기는 동작
            _retractorMaxSpeed *= 1.5f;

        Vector3 forceDir = (transform.position - grapTransform.position).normalized;
        if (Vector3.Dot(forceDir, grapRb.velocity) > _retractorMaxSpeed)
            return;

        grapRb.AddForce(forceDir * retractorForce * Time.deltaTime);
        
        sj.maxDistance = Vector3.Distance(transform.position, hitPoint.position) * 1.1f;
        sj.minDistance = Vector3.Distance(transform.position, hitPoint.position) * 0.9f;
    }

    public void ExtendRope()
    {
        if (sj.maxDistance > grapDistance) 
            return;

        Vector3 forceDir = (grapTransform.position - transform.position).normalized;
        if (Vector3.Dot(forceDir, grapRb.velocity) > retractorMaxSpeed)
            return;
        grapRb.AddForce(forceDir * retractorForce * Time.deltaTime);
        
        //sj.maxDistance = sj.minDistance = Vector3.Distance(transform.position, hitPoint.position);// * 0.8f;
        sj.maxDistance = Vector3.Distance(transform.position, hitPoint.position) * 1.1f;
        sj.minDistance = Vector3.Distance(transform.position, hitPoint.position) * 0.9f;
    }

    public void RopeUpdate()
    {
        sj.connectedAnchor = hitPoint.position;
    }
}
