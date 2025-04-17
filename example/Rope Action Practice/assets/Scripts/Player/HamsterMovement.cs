using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterMovement : MonoBehaviour
{
    [Tooltip("걷는 속도")]
    public float walkVelocity = 10;
    [Tooltip("뛰는 속도")]
    public float runVelocity = 20;

    private Vector3 moveDir = Vector3.zero;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateFunc()
    {
        moveDir = GetInputMoveDir();
        Rotate();
    }

    private void Rotate()
    {
        // 수평 속도가 거의 없으면 회전하지 않음
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.sqrMagnitude < 0.01f) return;

        // 바라볼 방향 (y축 고정)
        Quaternion targetRotation = Quaternion.LookRotation(-flatVel.normalized, Vector3.up);

        // 부드럽게 회전
        float rotateSpeed = 15f;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
    }


    // public float acceleration = 30f;
    // public float deceleration = 20f;
    // public float controlFactor = 0.5f; // 속도가 커도 조작을 가능하게 해주는 정도 (0~1)

    // 움직이고 있다면 true 반환
    public bool Move()
    {
        float _maxVelocity = Input.GetKey(KeyCode.LeftShift) ? runVelocity : walkVelocity;
        // 오브젝트 잡고 움직이는 중
        if (HamsterRope.onGrappling) _maxVelocity *= HamsterRope.speedFactor; 

        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float vel = flatVel.magnitude;
        if (vel > _maxVelocity) { // 속력은 유지하고 천천히 방향키 쪽 방향으로 이동 
            flatVel += moveDir * _maxVelocity * Time.fixedDeltaTime * 5; // 방향키 방향으로
            flatVel = flatVel.normalized * vel; // 속력 유지
            rb.velocity = new Vector3(flatVel.x, rb.velocity.y, flatVel.z);
        }
        else if (moveDir != Vector3.zero) // 즉각적으로 해당 방향으로 이동
            rb.velocity = new Vector3(moveDir.x * _maxVelocity, rb.velocity.y, moveDir.z * _maxVelocity);

        // 오브젝트 잡고 움직이는 중
        if (HamsterRope.onGrappling) HamsterRope.grapRb.velocity = rb.velocity;

        return rb.velocity.sqrMagnitude > 0.01f;

        // Vector3 desiredMove = moveDir * maxSpeed;

        // // 속도 차이 계산
        // Vector3 velocityDiff = desiredMove - flatVel;

        // // 조작 감도 조절 (속도가 빠르면 조작이 약하게 들어가도록)
        // float speedFactor = Mathf.Clamp01(1f - (flatVel.magnitude / maxSpeed));
        // float control = Mathf.Lerp(controlFactor, 1f, speedFactor);

        // // 가속 또는 감속 적용
        // Vector3 forceToAdd = velocityDiff.normalized * acceleration * control;
    }

    // public void Move()
    // {
    //     float _maxVelocity = Input.GetKey(KeyCode.LeftShift) ? runVelocity : walkVelocity;
    //     if (HamsterRope.onGrappling) _maxVelocity *= HamsterRope.speedFactor;

    //     Vector3 rbVec = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    //     if (moveDir == Vector3.zero) {
    //         float mul = 3f;
    //         if (rbVec.magnitude > _maxVelocity) mul = 10f;
    //         rb.velocity -= rbVec.normalized * mul * Time.deltaTime;
    //         return;
    //     }

    //     float addSpeed, accelSpeed, currentSpeed;

    //     currentSpeed = Vector2.Dot(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(moveDir.x, moveDir.z));
    //     addSpeed = _maxVelocity - currentSpeed;
    //     if (addSpeed <= 0)
    //         return;
    //     accelSpeed = Mathf.Min(addSpeed, _maxVelocity * 10f * Time.deltaTime);

    //     rb.velocity += moveDir * accelSpeed;
    //     Debug.Log(moveDir);

    //     if (HamsterRope.onGrappling) HamsterRope.grapRb.velocity = rb.velocity;
    // }


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