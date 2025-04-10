using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckPointManager : MonoBehaviour
{
    public enum CheckPointItem { Speed, Jump, Boost, Retractor, Gliding, Pull, DoubleJump }

    private int order;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] checkPoints;
    private Transform target;           // 추적할 오브젝트
    [SerializeField] private RectTransform markerUI;     // UI 캔버스에 있는 마커 아이콘
    [SerializeField] private Canvas canvas;              // 월드 공간이 아닌 Screen Space - Overlay 캔버스
    [SerializeField] private TextMeshProUGUI txt;
    [SerializeField] private float borderOffset = 30f;   // 화면 끝에서 조금 안쪽으로 밀어놓기

    
    void Start()
    {
        order = -1;
        CheckPoint();
        for (int i = 1; i < checkPoints.Length; i++)
            checkPoints[i].SetActive(false);
    }

    void Update()
    {
        PutUI();
    }

    private void PutUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);

        bool isBehindCamera = screenPos.z < 0;
        
        // 화면 범위 내인지 확인
        bool isInsideScreen = 
            screenPos.z > 0 &&
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height;

        if (isBehindCamera)
        {
            // 카메라 뒤에 있으면 좌표를 반대로 뒤집음
            screenPos *= -1;
        }

        Vector3 clampedScreenPos = screenPos;

        clampedScreenPos.x = Mathf.Clamp(screenPos.x, borderOffset, Screen.width - borderOffset);
        clampedScreenPos.y = Mathf.Clamp(screenPos.y, borderOffset, Screen.height - borderOffset);

        // 화면에 안 보이는 영역이라면, 무조건 화면의 가장자리에 표시되게 함
        if (!isInsideScreen)
        {
            Vector2 dirVec = new Vector2(clampedScreenPos.x, clampedScreenPos.y) - new Vector2(Screen.width / 2f, Screen.height / 2f);

            Vector2 newDirVec = new Vector2(Mathf.Abs(dirVec.x), Mathf.Abs(dirVec.y));
            // 가장자리에 있지 않다면
            if (newDirVec.x < Screen.width / 2f - borderOffset && newDirVec.y < Screen.height / 2f - borderOffset) {
                newDirVec *= (Screen.width / 2f - borderOffset) / newDirVec.x;
                if (newDirVec.y > Screen.height / 2f - borderOffset) {
                    newDirVec *= (Screen.height / 2f - borderOffset) / newDirVec.y;
                }
            }

            // 가장자리 fitting
            dirVec = new Vector2(newDirVec.x * (dirVec.x > 0 ? 1 : -1), newDirVec.y * (dirVec.y > 0 ? 1 : -1));
            clampedScreenPos = new Vector2(Screen.width / 2f, Screen.height / 2f) + dirVec;
        }

        // UI 위치 반영
        markerUI.position = clampedScreenPos;

        // 거리 계산
        float dist = Vector3.Distance(player.transform.position, target.position);
        markerUI.localScale = Vector3.one * Mathf.Clamp(Mathf.Lerp(1f, 0.5f, dist / 50f), 0.5f, 1f);
        txt.text = $"{dist:F1}m";

        // 방향 회전 (옵션)
        Vector2 dir = ((Vector2)screenPos - new Vector2(Screen.width / 2f, Screen.height / 2f)).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        markerUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // 마커 표시 여부 조정
        // markerUI.gameObject.SetActive(!isInsideScreen);
    }

    // 체크포인트를 먹었을 때 호출
    public void CheckPoint()
    {
        order++;
        checkPoints[order].SetActive(true);
        target = checkPoints[order].transform;
    }
}
