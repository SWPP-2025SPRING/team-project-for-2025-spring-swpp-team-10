using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_ScoreBoard : MonoBehaviour
{
    [SerializeField] private TMP_Text clearRecordText; // 최종 클리어 기록 텍스트
    [SerializeField] private UI_ScoreCell scoreCellPrefab;
    [SerializeField] private Transform scoreCellContent;

    [SerializeField] private Button titleButton;
    [SerializeField] private Button restartButton;

    private const string PlayerNameKey = "PlayerName";
    private const string PlayerRecordKeyPrefix = "PlayerRecord_";
    private const string PlayerNameListKey = "PlayerNameList";
    [SerializeField] private MainSceneManager mainSceneManager;    

    private void Start()
    {
        titleButton.onClick.AddListener(OnTitleButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);

        // ShowClearRecord();
        // ShowScoreBoard();
    }


    // 예: 게임 클리어 시 호출
    public void SaveClearRecord()
    {
        string playerName = PlayerPrefs.GetString(PlayerNameKey, "Player");
        float playTime = mainSceneManager.PlayTime; // MainSceneManager의 PlayTime 가져오기

        // 기록 저장
        PlayerPrefs.SetFloat(PlayerRecordKeyPrefix + playerName, playTime);

        // 이름 리스트에 추가
        string list = PlayerPrefs.GetString(PlayerNameListKey, "");
        var names = new HashSet<string>(string.IsNullOrEmpty(list) ? new string[0] : list.Split(','));
        names.Add(playerName);
        PlayerPrefs.SetString(PlayerNameListKey, string.Join(",", names));

        PlayerPrefs.Save();
    }

    public void ShowClearRecord()
    {
        string playerName = PlayerPrefs.GetString(PlayerNameKey, "Player");
        float clearTime = PlayerPrefs.GetFloat(PlayerRecordKeyPrefix + playerName, 0f);
        clearRecordText.text = $"{playerName}님의 플레이 시간은\n{FormatTime(clearTime)}";
    }

    public void ShowScoreBoard()
    {
        // 이름 목록 가져오기
        string list = PlayerPrefs.GetString(PlayerNameListKey, "");
        List<string> nameList = string.IsNullOrEmpty(list) ? new List<string>() : new List<string>(list.Split(','));

        // 기록 리스트 생성
        List<(string name, float time)> records = new List<(string, float)>();
        foreach (var name in nameList)
        {
            float t = PlayerPrefs.GetFloat(PlayerRecordKeyPrefix + name, -1f);
            if (t >= 0f) records.Add((name, t));
        }

        // 시간 오름차순 정렬(빠른 순)
        var top3 = records.OrderBy(r => r.time).Take(3).ToList();

        string currentPlayer = PlayerPrefs.GetString(PlayerNameKey, "Player");

        // 기존 셀 삭제
        foreach (Transform child in scoreCellContent)
            Destroy(child.gameObject);

        for (int i = 0; i < top3.Count; i++)
        {
            var cell = Instantiate(scoreCellPrefab, scoreCellContent);
            cell.SetRank(i + 1);
            cell.SetName(top3[i].name);
            cell.SetTime(FormatTime(top3[i].time));
            // 하이라이트 처리
            cell.SetHighlight(top3[i].name == currentPlayer);
        }
    }

    private string FormatTime(float t)
    {
        int totalSeconds = Mathf.FloorToInt(t);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;
        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    private void OnTitleButtonClicked()
    {
        SceneManager.LoadScene("TitleScene");
    }

    private void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}