using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INextCheckpointObserver : MonoBehaviour
{
    /// <summary>
    /// 다음 체크포인트 위치가 업데이트되었을 때 호출될 메소드입니다.
    /// </summary>
    /// <param name="nextCheckpointPosition">다음 체크포인트의 위치. 없을 경우 null입니다.</param>
    void OnNextCheckpointUpdated(Vector3? nextCheckpointPosition);
}
