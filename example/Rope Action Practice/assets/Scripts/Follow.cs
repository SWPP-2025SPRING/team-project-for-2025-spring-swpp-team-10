using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// follow.position + offset의 위치로 자연스럽게 이동하는 컴포넌트

public class Follow : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, follow.position + offset, speed * Time.deltaTime);
    }
}
