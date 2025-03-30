using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ChatGPT 도움을 받음.
// 입력 : 유니티에서 마우스 커서 모양을 끄고 마우스의 위치를 화면 정중앙으로 고정시킬 수 있어? 

public class MouseLock : MonoBehaviour
{
    void Start()
    {
        // 마우스 커서 숨기기
        Cursor.visible = false;
        
        // 마우스 커서를 화면 중앙에 고정 (게임 창 내에서만 적용)
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ALT 키로 잠금 해제
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
