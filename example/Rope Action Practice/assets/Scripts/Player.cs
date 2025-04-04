using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

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


    private Rigidbody rigid;

    
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        currentBoostEnergy = 1;
        isBoost = false;

        powerI.text = power.ToString();
    }

  
    void Update()
    {
        AddForce();
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.R) || transform.position.y < -100)
            Init();

        Boost();
        BoostEnergyControl();
    }


    void Init()
    {
        transform.position = initPos;
        rigid.velocity = Vector3.zero;
    }


    // AddForce : https://www.youtube.com/watch?v=8dFDRWCQ3Hs 참고
    void AddForce()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 forwardVec = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
        Vector3 rightVec = new Vector3(cam.right.x, 0, cam.right.z).normalized;
        Vector3 moveVec = (forwardVec * ver + rightVec * hor).normalized;

        float addSpeed, accelSpeed, currentSpeed;

        currentSpeed = Vector2.Dot(new Vector2(rigid.velocity.x, rigid.velocity.z), new Vector2(moveVec.x, moveVec.z));
        addSpeed = maxVelocity - currentSpeed;
        if (addSpeed <= 0)
            return;
        accelSpeed = Mathf.Min(addSpeed, GetIntValue(powerI) * Time.deltaTime);
        rigid.AddForce(moveVec * accelSpeed, ForceMode.Force);

        if (rigid.velocity.magnitude > maxVelocity) {
            Debug.Log(rigid.velocity.magnitude + " vel : " + new Vector2(rigid.velocity.x, rigid.velocity.z) + ", forceDir : " + new Vector2(moveVec.x, moveVec.z));
        }
    }


    void Jump()
    {
        if (groundCheck.isGround) {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Acceleration);
        }
    }
    

    void Boost()
    {
        if (!GetComponent<RopeAction>().onGrappling || Input.GetKeyUp(KeyCode.LeftShift) || currentBoostEnergy <= 0) {
            isBoost = false;
            return;
        }

        Vector3 vel = rigid.velocity.normalized;
        // 지속성 부스트
        if (isBoost) { 
            rigid.AddForce(vel * Time.deltaTime * boostPower, ForceMode.Force);
        }
        // 즉발성 부스트
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentBoostEnergy >= burstEnergyUsage) { 
            isBoost = true;
            rigid.AddForce(vel * burstBoostPower, ForceMode.Acceleration);
            currentBoostEnergy -= burstEnergyUsage;
        }
    }

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
}
