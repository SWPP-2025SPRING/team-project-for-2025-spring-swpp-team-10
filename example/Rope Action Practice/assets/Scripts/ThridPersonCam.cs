using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://jeonhw.tistory.com/17, https://gps-homepage.tistory.com/16 참고
public class ThridPersonCam : MonoBehaviour
{
    public Transform point;
    [SerializeField] float rotSpeed;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float zoomMinDist, zoomMaxDist;

    LayerMask objLayer; // Player 레이어를 제외한 모든 레이어
    float zoom = 10f;
    Vector2 m_Input;

    void Start()
    {
        int playerLayerIndex = LayerMask.NameToLayer("Player");
        objLayer = ~(1 << playerLayerIndex);
    }

    void Rotate()
    {
        if (!Input.GetKey(KeyCode.LeftAlt))
        {
            m_Input.x = Input.GetAxis("Mouse X");
            m_Input.y = -Input.GetAxis("Mouse Y");

            if (m_Input.magnitude != 0)
            {
                Quaternion q = point.rotation;
                float x = q.eulerAngles.x + m_Input.y * rotSpeed;
                x = x > 180 ? x - 360 : x;
                x = Mathf.Clamp(x, -80, 80);
                q.eulerAngles = new Vector3(x, q.eulerAngles.y + m_Input.x * rotSpeed, q.eulerAngles.z);
                point.rotation = q;
            }
        }
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        zoom -= scroll;
        zoom = Mathf.Clamp(zoom, zoomMinDist, zoomMaxDist);
    }

    void CameraUpdate()
    {
        if (Physics.Raycast(point.position, -point.forward, out var hit, zoom, objLayer)) {
            float dis = Vector3.Distance(hit.point, point.position);
            Camera.main.transform.position = point.position - point.forward * (dis - 0.2f);
        }
        else {
            Camera.main.transform.position = point.position - point.forward * zoom;
        }
        Camera.main.transform.LookAt(point.transform);
    }

    public void LateUpdate()
    {
        Rotate();
        Zoom(); 
    }

    void FixedUpdate()
    {
        CameraUpdate();
    }
}
