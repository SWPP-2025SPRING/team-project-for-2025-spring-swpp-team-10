using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    public enum Type { Transform, Vector3 }
    public Type inputType;
    public Transform start, end;
    public Vector3 startVec, endVec;
    [Tooltip("Start -> End까지 가는 데 걸리는 시간")]
    public float moveTime;
    [Tooltip("다른 움직이는 오브젝트와의 핑퐁 타이밍 조절")]
    public float offset;

    void Update()
    {
        Vector3 startPos = inputType == Type.Transform ? start.position : startVec;
        Vector3 endPos = inputType == Type.Transform ? end.position : endVec;
        transform.position = Vector3.Lerp(startPos, endPos, Mathf.PingPong(offset + Time.time / moveTime, 1));
    }
}


