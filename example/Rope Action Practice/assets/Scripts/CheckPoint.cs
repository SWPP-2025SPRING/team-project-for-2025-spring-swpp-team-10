using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// 각 체크포인트에 부착하는 스크립트
public class CheckPoint : MonoBehaviour
{
    [SerializeField] private CheckPointManager manager;
    [Tooltip("체크포인트가 닿으면 활성화할 UI")]
    [SerializeField] private GameObject openUI;

    private Transform octahedron;
    private BoxCollider col;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        octahedron = transform.GetChild(1);
        MoveSeq();
    }

    void MoveSeq()
    {
        octahedron.localPosition = Vector3.up * 2.2f;

        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(0, 0);
        keys[1] = new Keyframe(1, 1);
        AnimationCurve anim = new AnimationCurve(keys);

        Sequence seq = DOTween.Sequence();
        seq.Append(octahedron.DOLocalMoveY(3.3f, 2).SetEase(anim))
           .AppendInterval(0.2f)
           .Append(octahedron.DOLocalMoveY(2.2f, 2).SetEase(anim))
           .AppendInterval(0.2f);
        seq.SetLoops(-1, LoopType.Restart);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Destroy(col);

        octahedron.gameObject.SetActive(false);
        openUI.SetActive(true);

        manager.CheckPoint();
        Time.timeScale = 0f;
    }
}
