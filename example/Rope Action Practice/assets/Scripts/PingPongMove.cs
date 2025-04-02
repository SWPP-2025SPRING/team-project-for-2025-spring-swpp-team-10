using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMove : MonoBehaviour
{
    public Transform start, end;
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
        transform.position = Vector3.Lerp(start.position, end.position, Mathf.PingPong(offset + Time.time * speed, 1));
    }
}
