using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform follow;
    public Vector3 offset;
    public float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, follow.position + offset, speed * Time.deltaTime);
    }
}
