using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePull : MonoBehaviour
{
    public Transform target; // 당길 물체
    public float springForce = 1000f;
    public float damperForce = 50f;

    void Start()
    {
        var joint = target.gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = GetComponent<Rigidbody>(); // 당기는 주체

        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimitSpring spring = new SoftJointLimitSpring();
        spring.spring = springForce;
        spring.damper = damperForce;

        joint.linearLimitSpring = spring;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = 2f; // 줄 최대 길이
        joint.linearLimit = limit;

        joint.configuredInWorldSpace = false;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = Vector3.zero;
    }
}
