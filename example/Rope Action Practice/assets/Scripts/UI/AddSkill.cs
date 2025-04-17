using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
