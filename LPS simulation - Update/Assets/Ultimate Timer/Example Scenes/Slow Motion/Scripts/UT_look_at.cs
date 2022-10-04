using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_look_at : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        transform.LookAt(target);
    }
}
