using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// https://www.youtube.com/watch?v=-E_pRXqNYSk 참고
public class RopeAction : MonoBehaviour
{
    public Transform player;
    [Tooltip("훅을 걸 수 있는 오브젝트의 레이어")]
    
    public LayerMask GrapplingObj;
    public bool onGrappling = false;

    Camera cam;
    RaycastHit hit;
    LineRenderer lr;
    GameObject grapObject = null;

    SpringJoint sj;

    [Header("Spring")]
    public float spring;
    public float damper, mass; 

    [Header("Setting Input")]
    public TMP_InputField springI;
    public TMP_InputField damperI, massI;


    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();

        springI.text = spring.ToString();
        damperI.text = damper.ToString();
        massI.text = mass.ToString();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RopeShoot();
        }
        if (Input.GetMouseButtonUp(0)) {
            EndShoot();
        }

        DrawOutline();
        DrawRope();
    }

    void RopeShoot()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, GrapplingObj)) {
            if (hit.collider.gameObject == gameObject)
                return;
            grapObject = hit.collider.gameObject;

            onGrappling = true;
            // LineRenderer
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hit.point);

            // SpringJoint
            float dis = Vector3.Distance(transform.position, hit.point);

            sj = player.gameObject.AddComponent<SpringJoint>();
            sj.autoConfigureConnectedAnchor = false;
            sj.connectedAnchor = hit.point;

            sj.maxDistance = dis;
            sj.minDistance = dis * 0.9f;
            sj.damper = GetIntValue(damperI);
            sj.spring = GetIntValue(springI);
            sj.massScale = GetIntValue(massI);
        }
    }

    void EndShoot()
    {
        grapObject = null;
        onGrappling = false;
        lr.positionCount = 0;
        Destroy(sj);
    }

    void DrawRope()
    {
        if (onGrappling) {
            lr.SetPosition(0, transform.position);
        }
    }

    void DrawOutline()
    {
        // 마우스가 가리키는 오브젝트의 외곽선 표시
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, GrapplingObj)) {
            if (hit.collider.gameObject != gameObject)
                hit.collider.gameObject.GetComponent<DrawOutline>().Draw();
        }

        // 현재 잡고 있는 오브젝트의 외곽선 표시
        if (grapObject != null) {
            grapObject.GetComponent<DrawOutline>().Draw();
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
