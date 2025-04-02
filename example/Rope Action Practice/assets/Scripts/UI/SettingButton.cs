using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class SettingButton : MonoBehaviour
{
    public GameObject[] objs;
    public GameObject highlight;
    bool isOpen = false;

    // Start is called before the first frame update
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
