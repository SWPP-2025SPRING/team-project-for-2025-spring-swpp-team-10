using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 화면 좌측 아래의 AddSkill, AddFood 디버그 버튼 전용 스크립트
public class AddSkill : MonoBehaviour
{
    public GameObject food;
    public GameObject skill;

    public void AddSkills()
    {
        skill.SetActive(true);
        Time.timeScale = 0f;
    }

    public void AddFoods()
    {
        food.SetActive(true);
        Time.timeScale = 0f;
    }
}
