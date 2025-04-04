using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    Transform hitPoint;

    SpringJoint sj;

    [Header("Spring")]
    public float spring;
    public float damper, mass;
    [Tooltip("와이어를 걸 수 있는 최대 거리")]
    public float grapDistance = 50f;
    [Tooltip("줄 감기/풀기 속도")]
    public float retractorSpeed;

    [Header("Setting Input")]
    public TMP_InputField springI;
    public TMP_InputField damperI, massI, retractorSpeedI;


    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
        hitPoint = transform.GetChild(0);

        springI.text = spring.ToString();
        damperI.text = damper.ToString();
        massI.text = mass.ToString();
        retractorSpeedI.text = retractorSpeed.ToString();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RopeShoot();
        }
        if (Input.GetMouseButtonUp(0)) {
            EndShoot();
        }


        if (Input.GetMouseButton(1)) {
            ShortenRope(40);
        }
        if (Input.GetKey(KeyCode.Q)) {
            ShortenRope(GetIntValue(retractorSpeedI));
        }
        if (Input.GetKey(KeyCode.E)) {
            ExtendRope();
        }
        

        DrawOutline();
        DrawRope();
    }

    void RopeShoot()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, grapDistance, GrapplingObj)) {
            if (hit.collider.gameObject == gameObject)
                return;
            grapObject = hit.collider.gameObject;

            hitPoint.SetParent(grapObject.transform);
            hitPoint.position = hit.point;

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
            sj.minDistance = dis;
            sj.damper = GetIntValue(damperI);
            sj.spring = GetIntValue(springI);
            sj.massScale = GetIntValue(massI);
        }
    }

    void EndShoot()
    {
        grapObject = null;
        hitPoint.SetParent(this.transform);
        onGrappling = false;
        lr.positionCount = 0;
        Destroy(sj);
    }


    void ShortenRope(float value)
    {
        if (!onGrappling || sj.maxDistance <= 1) 
            return;

        sj.maxDistance = sj.minDistance = sj.maxDistance - value * Time.deltaTime;

        if (sj.maxDistance < 1)
            sj.maxDistance = sj.minDistance = 1;
    }
    void ExtendRope()
    {
        if (!onGrappling || sj.maxDistance > grapDistance) 
            return;

        sj.maxDistance = sj.minDistance = sj.maxDistance + GetIntValue(retractorSpeedI) * Time.deltaTime;
    }
    

    void DrawRope()
    {
        if (onGrappling) {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hitPoint.position);
            sj.connectedAnchor = hitPoint.position;
        }
    }

    void DrawOutline()
    {
        // 마우스가 가리키는 오브젝트의 외곽선 표시
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, grapDistance, GrapplingObj)) {
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
