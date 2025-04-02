using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOutline : MonoBehaviour
{
    int num = 2;
    public Renderer rd;

    void Start()
    {
        rd = GetComponent<Renderer>();
    }

    void Update()
    {
        if (num < 2) {
            num++;
            rd.materials[1].SetFloat("_scale", 0.3f);
        }
        else {
            rd.materials[1].SetFloat("_scale", 0f);
        }
    }

    public void Draw()
    {
        num = 0;
    }
}
