using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lovatto.Countdown;

public class bl_ExampleCountdownUse : MonoBehaviour
{
    public Transform countsRoot;

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var countdown = countsRoot.GetComponentInChildren<bl_Countdown>();
            if (countdown != null) countdown.StartCountdown();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var countdown = countsRoot.GetComponentInChildren<bl_Countdown>();
            if (countdown != null) countdown.CancelCountdown();
        }
    }
}