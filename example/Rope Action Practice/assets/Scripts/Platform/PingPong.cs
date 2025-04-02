using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
    public enum Type { Transform, Vector3 }
    public Type inputType;
    public Transform start, end;
    public Vector3 startVec, endVec;
    public float speed;
    [Tooltip("다른 움직이는 오브젝트와의 핑퐁 타이밍 조절")]
    public float offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startPos = inputType == Type.Transform ? start.position : startVec;
        Vector3 endPos = inputType == Type.Transform ? end.position : endVec;
        transform.position = Vector3.Lerp(startPos, endPos, Mathf.PingPong(offset + Time.time * speed, 1));
    }
}


