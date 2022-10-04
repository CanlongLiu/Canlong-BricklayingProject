using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_demo_example_timer_complete : MonoBehaviour
{
    public ultimate_timer timer02;
    public ultimate_timer timer03;



    void OnEnable()
    {
        timer02.ResetTimer();
        timer03.ResetTimer();
    }
}
