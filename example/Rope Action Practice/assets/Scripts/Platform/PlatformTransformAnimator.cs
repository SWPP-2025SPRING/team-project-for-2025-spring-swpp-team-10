using UnityEngine;
using DG.Tweening;

public class PlatformTransformAnimator : MonoBehaviour
{
    [System.Serializable]
    public struct MoveSequence {
        public Vector3 value;
        public float moveTime;
        public float interval;
        public DG.Tweening.Ease ease;
    }

    public enum Type { Position, Rotation, Scale }
    [Tooltip("Trnasform에서 움직일 프로퍼티 설정")]
    public Type modifyType;
    [Header("vec 위치로 moveTime 동안 이동한 뒤 interval 동안 체류 \n0th -> 1th -> ... -> nth -> 0th -> ...")]
    [Tooltip("해당 시간만큼 대기하다가 이동 시작")]
    public float startDelay;
    public MoveSequence[] seqs;

    void Start()
    {
        Init();
        Invoke("SeqStart", startDelay);
    }

    void Init()
    {
        switch (modifyType) {
            case Type.Position:
                transform.position = seqs[0].value;
                break;
            case Type.Rotation:
                transform.rotation = Quaternion.Euler(seqs[0].value);
                break;
            case Type.Scale:  
                transform.localScale = seqs[0].value;
                break;
        }
    }

    void SeqStart()
    {
        Sequence seq = DOTween.Sequence();
        for (int i = 1; i < seqs.Length; i++) {
            seq.Append(Do(i))
               .AppendInterval(seqs[i].interval);
        }
        seq.Append(Do(0))
           .AppendInterval(seqs[0].interval);
        seq.SetLoops(-1, LoopType.Restart);
    }

    // modifyType에 맞는 행동 반환
    Tween Do(int i)
    {
        switch (modifyType) {
            case Type.Position:
                return transform.DOMove(seqs[i].value, seqs[i].moveTime).SetEase(seqs[i].ease);
            case Type.Rotation:
                return transform.DORotate(seqs[i].value, seqs[i].moveTime, RotateMode.FastBeyond360).SetEase(seqs[i].ease);
            default: // Type.Scale:
                return transform.DOScale(seqs[i].value, seqs[i].moveTime).SetEase(seqs[i].ease);
        }
    }
}
