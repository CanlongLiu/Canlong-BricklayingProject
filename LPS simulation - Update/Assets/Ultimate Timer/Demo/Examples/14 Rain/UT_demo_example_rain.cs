using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_demo_example_rain : MonoBehaviour
{
    public ultimate_timer[] raindrop;
    public ultimate_timer[] ring;
    public ultimate_timer[] spatter_1;
    public ultimate_timer[] spatter_2;
    public ultimate_timer[] spatter_3;
    public ultimate_timer[] spatter_4;



    void OnEnable()
    {
        /*
        raindrop_1_1.StartTimer();
        ring_1_1.TimeCompleted();
        spatter_1_1.TimeCompleted();
        spatter_1_2.TimeCompleted();
        spatter_1_3.TimeCompleted();
        spatter_1_4.TimeCompleted();


        raindrop_2_1.StartTimer();
        ring_2_1.TimeCompleted();
        spatter_2_1.TimeCompleted();
        spatter_2_2.TimeCompleted();
        spatter_2_3.TimeCompleted();
        spatter_2_4.TimeCompleted();
        */

        foreach (ultimate_timer raindropElement in raindrop)
        {
            raindropElement.StartTimer();
        }

        foreach (ultimate_timer ringElement in ring)
        {
            ringElement.TimeCompleted();
        }

        foreach (ultimate_timer spatterElement1 in spatter_1)
        {
            spatterElement1.TimeCompleted();

        }

        foreach (ultimate_timer spatterElement2 in spatter_2)
        {
            spatterElement2.TimeCompleted();

        }

        foreach (ultimate_timer spatterElement3 in spatter_3)
        {
            spatterElement3.TimeCompleted();

        }

        foreach (ultimate_timer spatterElement4 in spatter_4)
        {
            spatterElement4.TimeCompleted();

        }
        
    }
}
