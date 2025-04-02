using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class Player : MonoBehaviour
{
    [Tooltip("이동 시 가해지는 힘")]
    public float power;
    [Tooltip("최대 속도 (power에 비례)")]
    public float maxVelocityRatio;
    public Transform cam;
    public Vector3 initPos;


    [Header("Boost")]
    [Range(0, 1)] public float currentBoostEnergy;
    public float energyUsageRatePerSeconds;
    public float burstEnergyUsage;
    public float energyRecoveryRatePerSeconds;
    [Tooltip("순간 가속 (power에 비례)")]
    public float burstBoostRatio = 1.2f;
    [Tooltip("지속적인 가속 (power에 비례)")]
    public float boostRatio = 0.15f;
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
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        Vector3 forwardVec = new Vector3(cam.forward.x, 0, cam.forward.z).normalized;
        Vector3 rightVec = new Vector3(cam.right.x, 0, cam.right.z).normalized;
        Vector3 moveVec = (forwardVec * ver + rightVec * hor).normalized;

        AddForce(moveVec);

        // if (Input.GetKeyDown(KeyCode.Q)) {
        //     Debug.Log(cam.forward + "," + cam.right + "," + moveVec);
        // }

        if (Input.GetKeyDown(KeyCode.R))
            Init();

        Boost();
        BoostEnergyControl();
    }


    // https://www.youtube.com/watch?v=8dFDRWCQ3Hs 참고
    void AddForce(Vector3 moveVec)
    {
        float maxVelocity = GetIntValue(powerI) * maxVelocityRatio;

        float addSpeed, accelSpeed, currentSpeed;

        currentSpeed = Vector2.Dot(new Vector2(rigid.velocity.x, rigid.velocity.z), new Vector2(moveVec.x, moveVec.z));
        addSpeed = maxVelocity - currentSpeed;
        if (addSpeed <= 0)
            return;
        accelSpeed = Mathf.Min(addSpeed, GetIntValue(powerI) * Time.deltaTime);
        rigid.AddForce(moveVec * accelSpeed, ForceMode.Force);

        if (rigid.velocity.magnitude > maxVelocity) {
            Debug.Log(rigid.velocity.magnitude + " " + new Vector2(rigid.velocity.x, rigid.velocity.z) + ", " + new Vector2(moveVec.x, moveVec.z));
        }
    }

    void Init()
    {
        transform.position = initPos;
        rigid.velocity = Vector3.zero;
    }

    void Boost()
    {
        if (!GetComponent<RopeAction>().onGrappling || Input.GetKeyUp(KeyCode.LeftShift) || currentBoostEnergy <= 0) {
            isBoost = false;
            return;
        }

        Vector3 vel = Vector3.zero;
        // 지속성 부스트
        if (isBoost) { 
            vel = rigid.velocity.normalized;
            rigid.AddForce(vel * GetIntValue(powerI) * Time.deltaTime * boostRatio, ForceMode.Force);
        }
        // 즉발성 부스트
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentBoostEnergy >= burstEnergyUsage) { 
            isBoost = true;
            rigid.AddForce(vel * GetIntValue(powerI) * burstBoostRatio, ForceMode.Acceleration);
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

    int GetIntValue(TMP_InputField inputField)
    {
        if (int.TryParse(inputField.text, out int result))
        {
            return result; // 정수 변환 성공
        }
        return 0; // 변환 실패 시 기본값 0 반환
    }
}
