using UnityEngine;

// 화면 좌상단의 Setting Button 스크립트

public class SettingButton : MonoBehaviour
{
    public GameObject[] objs;
    public GameObject highlight;
    bool isOpen = false;

    void Start()
    {
        foreach (GameObject obj in objs)
            obj.SetActive(false);
    }

    public void onClick()
    {
        isOpen = !isOpen;
        foreach (GameObject obj in objs)
            obj.SetActive(isOpen);
        highlight.SetActive(false);
    }
}
