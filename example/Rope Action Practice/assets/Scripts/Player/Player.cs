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
    [SerializeField] private float movePower = 1000;
    [Tooltip("최대 속도")]
    [SerializeField] private float maxVelocity = 20;
    [Tooltip("점프 시 가해지는 힘")]
    [SerializeField] private float jumpPower = 600;
    [SerializeField] private Vector3 initPos;

    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform cam;


    [Header("Boost")]
    [Range(0, 1)] public float currentBoostEnergy;
    [Tooltip("1초 당 쓰는 에너지")]
    [SerializeField] private float energyUsageRatePerSeconds = 0.6f;
    [Tooltip("부스트 시작 시 순간적으로 쓰는 에너지 (에너지가 해당 값 이상이어야 부스트 발동 가능)")]
    public float burstEnergyUsage = 0.2f;
    [Tooltip("부스트 상태가 아닐 때 1초 당 회복되는 에너지")]
    [SerializeField] private float energyRecoveryRatePerSeconds = 0.125f;
    [Tooltip("순간 가속")]
    [SerializeField] private float burstBoostPower = 1000;
    [Tooltip("지속적인 가속")]
    [SerializeField] private float sustainedBoostPower = 400;
    public bool isBoost;

    [Header("Setting Input")]
    [SerializeField] private TMP_InputField movePowerI;
    [SerializeField] private TMP_InputField massI;
    [SerializeField] private TMP_InputField maxVelocityI;
    [SerializeField] private TMP_InputField burstBoostI;
    [SerializeField] private TMP_InputField sustainedBoostI;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI velocityTxt;

    private Rigidbody rb;

    
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentBoostEnergy = 1;
        isBoost = false;

        ChangeInputFieldText(movePowerI, movePower.ToString());
        ChangeInputFieldText(massI, rb.mass.ToString());
        ChangeInputFieldText(maxVelocityI, maxVelocity.ToString());
        ChangeInputFieldText(burstBoostI, burstBoostPower.ToString());
        ChangeInputFieldText(sustainedBoostI, sustainedBoostPower.ToString());
    }

  
    void Update()
    {
        GetInputField();

        if (MeshConverter.isSphere) AddForce();
        else AddVelocity();

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyDown(KeyCode.R) || transform.position.y < -100)
            Init();

        Boost();
        BoostEnergyControl();

        if (velocityTxt != null)
            velocityTxt.text = $"Velocity : {rb.velocity.magnitude:F1}";
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
        accelSpeed = Mathf.Min(addSpeed, movePower * Time.deltaTime);
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
            rb.AddForce(vel * Time.deltaTime * sustainedBoostPower, ForceMode.Force);
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


    void GetInputField()
    {
        movePower = GetFloatValue(movePower, movePowerI);
        rb.mass = GetFloatValue(rb.mass, massI);
        maxVelocity = GetFloatValue(maxVelocity, maxVelocityI);
        burstBoostPower = GetFloatValue(burstBoostPower, burstBoostI);
        sustainedBoostPower = GetFloatValue(sustainedBoostPower, sustainedBoostI);
    }

    void ChangeInputFieldText(TMP_InputField inputField, string s)
    {
        if (inputField != null)
            inputField.text = s;
    }

    float GetFloatValue(float defaultValue, TMP_InputField inputField)
    {
        if (inputField != null && float.TryParse(inputField.text, out float result))
            return result;
        return defaultValue;
    }
}
