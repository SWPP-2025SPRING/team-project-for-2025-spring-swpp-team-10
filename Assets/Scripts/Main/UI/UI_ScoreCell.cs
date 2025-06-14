using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ScoreCell : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Image highlightImage; // 하이라이트용 배경(없으면 null로 두세요)

    public void SetRank(int rank)
    {
        rankText.text = rank.ToString();
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetTime(string time)
    {
        timeText.text = time;
    }

    public void SetHighlight(bool isHighlight)
    {
        if (highlightImage != null)
            highlightImage.enabled = isHighlight;
        else
        {
            // 하이라이트 이미지가 없으면 텍스트 색상으로 구분
            Color c = isHighlight ? Color.yellow : Color.white;
            nameText.color = c;
            timeText.color = c;
            rankText.color = c;
        }
    }
}