using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 스킬 선택창 UI 스크립트
// 스킬 획득 스크립트는 PlayerSkill 스크립트
public class SkillUI : MonoBehaviour
{
    [SerializeField] private PlayerSkill skill;
    [SerializeField] private bool isFood;
    [Header("Food")]
    [SerializeField] private Button speed;
    [SerializeField] private Button jump; 
    [Header("Skill")]
    [SerializeField] private Button boost;
    [SerializeField] private Button retractor;
    [SerializeField] private Button gliding;
    [SerializeField] private Button pull;
    [SerializeField] private Button doubleJump;


    void OnEnable()
    {
        if (isFood) {
            speed.interactable = !skill.HasSpeed();
            jump.interactable = !skill.HasJump();
        }
        else {
            boost.interactable = !skill.HasBoost();
            retractor.interactable = !skill.HasRetractor();
            gliding.interactable = !skill.HasGliding();
            pull.interactable = !skill.HasPull();
            doubleJump.interactable = !skill.HasDoubleJump();
        }
    }
}
