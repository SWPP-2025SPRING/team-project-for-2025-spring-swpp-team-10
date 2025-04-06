using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;
using System.Data.Common;

public class Player : MonoBehaviour
{
    [Tooltip("이동 시 가해지는 힘")]
    public float power = 1000;
    [Tooltip("최대 속도")]
    public float maxVelocity = 20;
    [Tooltip("점프 시 가해지는 힘")]
    public float jumpPower = 600;
    public Vector3 initPos;

    public GroundCheck groundCheck;
    public Transform cam;


    [Header("Boost")]
    [Range(0, 1)] public float currentBoostEnergy;
    [Tooltip("1초 당 쓰는 에너지")]
    public float energyUsageRatePerSeconds;
    [Tooltip("부스트 시작 시 순간적으로 쓰는 에너지 (에너지가 해당 값 이상이어야 부스트 발동 가능)")]
    public float burstEnergyUsage;
    [Tooltip("부스트 상태가 아닐 때 1초 당 회복되는 에너지")]
    public float energyRecoveryRatePerSeconds;
    [Tooltip("순간 가속")]
    public float burstBoostPower = 1000;
    [Tooltip("지속적인 가속")]
    public float boostPower = 400;
    public bool isBoost;


    [Header("Setting Input")]
    public TMP_InputField powerI;
    public TMP_InputField massI;


    private Rigidbody rb;
    private MeshConverter meshConverter;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshConverter = GetComponent<MeshConverter>();

        currentBoostEnergy = 1;
        isBoost = false;

        powerI.text = power.ToString();
        massI.text = rb.mass.ToString();
    }

  
    void Update()
    {
        if (MeshConverter.isSphere) AddForce();
        else AddVelocity();

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.R) || transform.position.y < -100)
            Init();

        Boost();
        BoostEnergyControl();

        rb.mass = GetFloatValue(massI);
    }


    void Init()
    {
        transform.position = initPos;
        rb.velocity = Vector3.zero;
    }


    // AddForce : https://www.youtube.com/watch?v=8dFDRWCQ3Hs 참고
    void AddForce()
    {
        Vector3 moveDir = GetInputMoveDir();

        float addSpeed, accelSpeed, currentSpeed;

        currentSpeed = Vector2.Dot(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(moveDir.x, moveDir.z));
        addSpeed = maxVelocity - currentSpeed;
        if (addSpeed <= 0)
            return;
        accelSpeed = Mathf.Min(addSpeed, GetIntValue(powerI) * Time.deltaTime);
        rb.AddForce(moveDir * accelSpeed, ForceMode.Force);

        if (rb.velocity.magnitude > maxVelocity) {
            //Debug.Log(rb.velocity.magnitude + " vel : " + new Vector2(rb.velocity.x, rb.velocity.z) + ", forceDir : " + new Vector2(moveVec.x, moveVec.z));
        }
    }

    void AddVelocity()
    {
        Vector3 moveDir = GetInputMoveDir();

        float addSpeed, accelSpeed, currentSpeed;

        float _maxVelocity = Input.GetKey(KeyCode.LeftShift) ? maxVelocity * 1.2f : maxVelocity / 1.2f;

        currentSpeed = Vector2.Dot(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(moveDir.x, moveDir.z));
        addSpeed = _maxVelocity - currentSpeed;
        if (addSpeed <= 0)
            return;
        accelSpeed = Mathf.Min(addSpeed, _maxVelocity * 2f * Time.deltaTime);
        Debug.Log(addSpeed + "," + _maxVelocity * Time.deltaTime);
        rb.velocity += moveDir * accelSpeed;
    }

    Vector3 GetInputMoveDir()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 forwardVec = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
        Vector3 rightVec = new Vector3(cam.right.x, 0, cam.right.z).normalized;
        Vector3 moveVec = (forwardVec * ver + rightVec * hor).normalized;

        return moveVec;
    }


    void Jump()
    {
        if (groundCheck.isGround) {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Acceleration);
        }
    }
    

    void Boost()
    {
        if (!GetComponent<RopeAction>().onGrappling || Input.GetKeyUp(KeyCode.LeftShift) || currentBoostEnergy <= 0) {
            isBoost = false;
            return;
        }

        Vector3 vel = rb.velocity.normalized;
        // 지속성 부스트
        if (isBoost) { 
            rb.AddForce(vel * Time.deltaTime * boostPower, ForceMode.Force);
        }
        // 즉발성 부스트
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentBoostEnergy >= burstEnergyUsage) { 
            isBoost = true;
            rb.AddForce(vel * burstBoostPower, ForceMode.Acceleration);
            currentBoostEnergy -= burstEnergyUsage;
        }
    }

    // 부스터 게이지 조절
    void BoostEnergyControl()
    {
        if (isBoost) { // 부스터 사용중
            currentBoostEnergy -= energyUsageRatePerSeconds * Time.deltaTime;
            if (currentBoostEnergy < 0)
                currentBoostEnergy = 0;
        }
        else {
            if (currentBoostEnergy < 1)
                currentBoostEnergy += energyRecoveryRatePerSeconds * Time.deltaTime;
            else
                currentBoostEnergy = 1;
        }
    }


    // https://coding-shop.tistory.com/255 내용 참고
    void OnCollisionEnter(Collision collision)
    {
        
    }

    int GetIntValue(TMP_InputField inputField)
    {
        if (int.TryParse(inputField.text, out int result))
        {
            return result; // 정수 변환 성공
        }
        return 0; // 변환 실패 시 기본값 0 반환
    }

    float GetFloatValue(TMP_InputField inputField)
    {
        if (float.TryParse(inputField.text, out float result))
        {
            return result; // 정수 변환 성공
        }
        return 0; // 변환 실패 시 기본값 0 반환
    }
}
