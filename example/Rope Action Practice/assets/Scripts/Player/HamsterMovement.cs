using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterMovement : MonoBehaviour
{
    [Tooltip("걷는 속도")]
    public float walkVelocity = 10;
    [Tooltip("뛰는 속도")]
    public float runVelocity = 20;

    public void Move()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 moveDir = GetInputMoveDir();

        float _maxVelocity = Input.GetKey(KeyCode.LeftShift) ? runVelocity : walkVelocity;
        if (HamsterRope.onGrappling) _maxVelocity *= HamsterRope.speedFactor;

        Vector3 rbVec = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (moveDir == Vector3.zero) {
            float mul = 3f;
            if (rbVec.magnitude > _maxVelocity) mul = 10f;
            rb.velocity -= rbVec.normalized * mul * Time.deltaTime;
            return;
        }

        float addSpeed, accelSpeed, currentSpeed;

        currentSpeed = Vector2.Dot(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(moveDir.x, moveDir.z));
        addSpeed = _maxVelocity - currentSpeed;
        if (addSpeed <= 0)
            return;
        accelSpeed = Mathf.Min(addSpeed, _maxVelocity * 10f * Time.deltaTime);

        rb.velocity += moveDir * accelSpeed;

        if (HamsterRope.onGrappling) HamsterRope.grapRb.velocity = rb.velocity;
    }


    Vector3 GetInputMoveDir()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Transform cam = Camera.main.transform;
        Vector3 forwardVec = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
        Vector3 rightVec = new Vector3(cam.right.x, 0, cam.right.z).normalized;
        Vector3 moveVec = (forwardVec * ver + rightVec * hor).normalized;

        return moveVec;
    }
}
