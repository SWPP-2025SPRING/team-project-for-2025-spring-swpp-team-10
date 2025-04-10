using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SphereMovement : MonoBehaviour
{
    [Tooltip("이동 시 가해지는 힘")]
    public float movePower = 1000;
    [Tooltip("최대 속도")]
    public float maxVelocity = 20;


    [Header("Setting Input")]
    [SerializeField] public TMP_InputField movePowerI;
    [SerializeField] public TMP_InputField maxVelocityI;


    private void Start()
    {
        ChangeInputFieldText(movePowerI, movePower.ToString());
        ChangeInputFieldText(maxVelocityI, maxVelocity.ToString());
    }

    private void Update()
    {
        GetInputField();   
    }


    // AddForce : https://www.youtube.com/watch?v=8dFDRWCQ3Hs 참고
    public void Move()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 moveDir = GetInputMoveDir();

        float addSpeed, accelSpeed, currentSpeed;

        currentSpeed = Vector2.Dot(new Vector2(rb.velocity.x, rb.velocity.z), new Vector2(moveDir.x, moveDir.z));
        addSpeed = maxVelocity - currentSpeed;
        if (addSpeed <= 0)
            return;
        accelSpeed = Mathf.Min(addSpeed, movePower * Time.deltaTime);
        rb.AddForce(moveDir * accelSpeed, ForceMode.Force);
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


    void GetInputField()
    {
        movePower = GetFloatValue(movePower, movePowerI);
        maxVelocity = GetFloatValue(maxVelocity, maxVelocityI);
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
