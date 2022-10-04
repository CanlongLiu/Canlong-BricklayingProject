using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_timescale_manager : MonoBehaviour
{
    /*
    This script will speed up or slow down the overall time scale of your project.
    Time Scale is the scale at which time passes.
    This can be used for slow motion effects or to speed up your application. When timeScale is 1.0, time passes as fast as real time. When timeScale is 0.5 time passes 2x slower than realtime.
    When timeScale is set to zero your application acts as if paused if all your functions are frame rate independent. Negative values are ignored.
    */

    [Range(0, 2)]
    public float speedOfTime = 1.0f;

    void Update()
    {
        Time.timeScale = speedOfTime;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
