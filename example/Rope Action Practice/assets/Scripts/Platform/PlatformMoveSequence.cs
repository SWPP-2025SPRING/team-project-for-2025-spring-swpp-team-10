using UnityEngine;
using DG.Tweening;

public class PlatformMoveSequence : MonoBehaviour
{
    [System.Serializable]
    public struct MoveSequence {
        public Vector3 vec;
        public float moveTime;
        public float interval;
        public DG.Tweening.Ease ease;
    }

    [Tooltip("초기위치 : 0th vector\nvec 위치로 moveTime 동안 이동한 뒤 interval 동안 체류")]
    public MoveSequence[] moveSeqs;

    void Start()
    {
        transform.position = moveSeqs[0].vec;

        Sequence seq = DOTween.Sequence();
        for (int i = 1; i < moveSeqs.Length; i++) {
            seq.Append(transform.DOMove(moveSeqs[i].vec, moveSeqs[i].moveTime).SetEase(moveSeqs[i].ease))
               .AppendInterval(moveSeqs[i].interval);
        }
        seq.Append(transform.DOMove(moveSeqs[0].vec, moveSeqs[0].moveTime).SetEase(moveSeqs[0].ease))
           .AppendInterval(moveSeqs[0].interval);
        seq.SetLoops(-1, LoopType.Restart);
    }
}
