using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UI_TitleScene : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button startButton;

    private const string PlayerNameKey = "PlayerName";
    private const string PlayerRecordKeyPrefix = "PlayerRecord_";
    private HashSet<string> existingNames = new HashSet<string>();

    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TMP_Text popupMessage;
    [SerializeField] private Button popupYesButton;
    [SerializeField] private Button popupNoButton;


#if UNITY_EDITOR
    [MenuItem("Tools/PlayerPrefs 전체 삭제")]
    public static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs 전체 삭제 완료!");
    }
#endif

    private void Start()
    {
        // 기존 저장된 이름 불러오기
        string savedName = PlayerPrefs.GetString(PlayerNameKey, "");
        if (string.IsNullOrEmpty(savedName))
        {
            // 기본값: Player N
            int n = 1;
            while (PlayerPrefs.HasKey(PlayerRecordKeyPrefix + "Player " + n))
                n++;
            savedName = "Player " + n;
        }
        inputField.text = savedName;

        // 버튼 이벤트 등록
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        string inputName = inputField.text.Trim();
        if (string.IsNullOrEmpty(inputName))
        {
            // 이름이 비어있으면 무시
            Debug.Log("이름을 입력하세요.");
            return;
        }

        // 기존 이름이 있으면 무조건 중복 팝업
        if (PlayerPrefs.HasKey(PlayerRecordKeyPrefix + inputName))
        {
        ShowOverwritePopupRuntime(inputName);
        }
        else
        {
            SaveAndStart(inputName);
        }
    }

   // 런타임용 커스텀 팝업
private void ShowOverwritePopupRuntime(string name)
{
    popupPanel.SetActive(true);
    popupMessage.text = $"이미 '{name}' 이름의 기록이 있습니다.\n덮어쓰시겠습니까?";

    // 기존 리스너 제거
    popupYesButton.onClick.RemoveAllListeners();
    popupNoButton.onClick.RemoveAllListeners();

    popupYesButton.onClick.AddListener(() =>
    {
        popupPanel.SetActive(false);
        SaveAndStart(name);
    });
    popupNoButton.onClick.AddListener(() =>
    {
        popupPanel.SetActive(false);
    });
}
    private void SaveAndStart(string name)
    {
        PlayerPrefs.SetString(PlayerNameKey, name);
        PlayerPrefs.SetInt(PlayerRecordKeyPrefix + name, 1);

        // 이름 리스트에 추가
        string list = PlayerPrefs.GetString("PlayerNameList", "");
        var names = new HashSet<string>(string.IsNullOrEmpty(list) ? new string[0] : list.Split(','));
        names.Add(name);
        PlayerPrefs.SetString("PlayerNameList", string.Join(",", names));

        PlayerPrefs.Save();
        SceneManager.LoadScene("MainScene");
    }
}