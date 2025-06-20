using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CinematicHamsterController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Ending - Escape")]
    [SerializeField] private float escapeSpeed;
    [SerializeField] private float escapeSpeedAdder;
    [SerializeField] private Vector3 escapeRunDirection, escapeJumpDirection;
    
    [Header("Good Ending - RunAway")]
    [SerializeField] private float runAwaySpeed;
    [SerializeField] private Vector3 runAwayStartPos, runAwayStartRot;
    [SerializeField] private Vector3 runAwayEndPos, runAwayEndRot;
    [SerializeField] private Vector3 runAwayDirection;

    private void Awake()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Jump", false);
    }

    public IEnumerator Escape(float runDuration)
    {
        animator.SetBool("Walk", true);
        
        for (float elapsed = 0f; elapsed < runDuration; elapsed += Time.deltaTime)
        {
            transform.localPosition += Time.deltaTime * escapeRunDirection * escapeSpeed;
            yield return null;
        }
        animator.SetBool("Walk", false);
        animator.SetBool("Jump", true);

        while (escapeSpeed >= 0.001f)
        {
            transform.localPosition += Time.deltaTime * escapeJumpDirection * escapeSpeed;
            escapeSpeed -= Time.deltaTime * escapeSpeedAdder;
            yield return null;
        }
    }

    public IEnumerator RunAway(float runAwayDuration)
    {
        transform.localPosition = runAwayStartPos;
        transform.rotation = Quaternion.Euler(runAwayStartRot);
        
        transform.DOLocalMove(Vector3.zero, runAwayDuration).SetEase(Ease.Linear);
        yield return new WaitForSeconds(runAwayDuration);

        transform.rotation = Quaternion.Euler(runAwayEndRot);

        animator.SetBool("Walk", true);
        while (transform.localPosition.x < 20f)
        {
            transform.localPosition += Time.deltaTime * runAwayDirection * runAwaySpeed;
            yield return null;
        }
    }
}
