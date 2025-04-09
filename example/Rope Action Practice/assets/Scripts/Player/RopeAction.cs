using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

// https://www.youtube.com/watch?v=-E_pRXqNYSk 참고
public class RopeAction : MonoBehaviour
{
    [SerializeField] private Transform player;
    [Tooltip("훅을 걸 수 있는 오브젝트의 레이어")]
    public LayerMask GrapplingObj;
    public bool onGrappling = false;

    private Camera cam;
    private RaycastHit hit;
    private LineRenderer lr;
    private GameObject grapObject = null;
    // 해당 스크립트를 가지는 오브젝트의 0번째 자식으로 빈 오브젝트를 할당하기. 와이어를 걸었을 때 후크를 부착하는 포인트가 됨
    private Transform hitPoint; 
    private MeshConverter meshConverter;

    private SpringJoint sj;

    [Header("Spring")]
    [SerializeField] private float spring = 100;
    [SerializeField] private float damper = 1, mass = 10;
    [Tooltip("와이어를 걸 수 있는 최대 거리")]
    public float grapDistance = 50f;
    [Tooltip("줄 감기/풀기 속도")]
    [SerializeField] private float retractorSpeed = 12;

    [Header("Setting Input")]
    [SerializeField] private TMP_InputField springI;
    [SerializeField] private TMP_InputField damperI, massI, retractorSpeedI;


    void Start()
    {
        cam = Camera.main;
        lr = GetComponent<LineRenderer>();
        meshConverter = GetComponent<MeshConverter>();
        hitPoint = transform.GetChild(0);

        ChangeInputFieldText(springI, spring.ToString());
        ChangeInputFieldText(damperI, damper.ToString());
        ChangeInputFieldText(massI, mass.ToString());
        ChangeInputFieldText(retractorSpeedI, retractorSpeed.ToString());
    }

    void Update()
    {
        GetInputField();

        if (Input.GetMouseButtonDown(0)) {
            RopeShoot();
        }
        if (Input.GetMouseButtonUp(0) && onGrappling) {
            EndShoot();
        }

        if (Input.GetMouseButton(1)) {
            ShortenRope(40); // 빠르게 오브젝트에 접근
        }
        if (Input.GetKey(KeyCode.Q)) {
            ShortenRope(retractorSpeed);
        }
        if (Input.GetKey(KeyCode.E)) {
            ExtendRope();
        }
        
        DrawOutline();
        DrawRope();
        ModeConvert();
    }

    void RopeShoot()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, grapDistance, GrapplingObj)) {
            if (hit.collider.gameObject == gameObject) // 자기 자신이면 return
                return;
            grapObject = hit.collider.gameObject;

            // hitPoint 오브젝트트를 훅을 건 오브젝트의 자식으로 설정
            hitPoint.SetParent(grapObject.transform);
            hitPoint.position = hit.point;

            onGrappling = true;

            // LineRenderer 세팅
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, hit.point);

            // SpringJoint 세팅
            float dis = Vector3.Distance(transform.position, hit.point);

            sj = player.gameObject.AddComponent<SpringJoint>();
            sj.autoConfigureConnectedAnchor = false;
            sj.connectedAnchor = hit.point;

            sj.maxDistance = dis;
            sj.minDistance = dis;
            sj.damper = damper;
            sj.spring = spring;
            sj.massScale = mass;

            // 플레이어의 형태를 sphere로 변환
            meshConverter.ConvertToSphere();
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
        
        if (sj.maxDistance < 20) {
            value *= Mathf.Lerp(0.2f, 1, (sj.maxDistance - 1) / 19f); // maxDist가 1일 때는 0.4f, 20일 때는 1f
        }

        sj.maxDistance = sj.minDistance = sj.maxDistance - value * Time.deltaTime;

        if (sj.maxDistance < 1)
            sj.maxDistance = sj.minDistance = 1;
    }
    void ExtendRope()
    {
        if (!onGrappling || sj.maxDistance > grapDistance) 
            return;

        sj.maxDistance = sj.minDistance = sj.maxDistance + retractorSpeed * Time.deltaTime; 
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


    void ModeConvert()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            meshConverter.Convert();
            // Capsule -> Sphere로 변환하면서 로프액션 상태라면 로프를 풀기
            if (!MeshConverter.isSphere && onGrappling)
                EndShoot();
        }
    }


    void GetInputField()
    {
        spring = GetFloatValue(spring, springI);
        damper = GetFloatValue(damper, damperI);
        mass = GetFloatValue(mass, massI);
        retractorSpeed = GetFloatValue(retractorSpeed, retractorSpeedI);
        damper = 1.3f;
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
