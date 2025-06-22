using UnityEngine;
using TMPro;

public class TooltipController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform backgroundRectTransform;
    [SerializeField] private Vector2 offset = new Vector2(15f, 10f); // 마우스 포인터와의 거리

    void Update()
    {
        // 마우스 위치를 따라다니도록 위치 업데이트
        // 스크린 좌표계인 Input.mousePosition을 UI가 사용하는 RectTransform 좌표로 변환
        Vector2 mousePos = Input.mousePosition;
        transform.position = mousePos + offset;
    }

    public void SetText(string content)
    {
        tooltipText.text = content;
        tooltipText.ForceMeshUpdate();
    }
}