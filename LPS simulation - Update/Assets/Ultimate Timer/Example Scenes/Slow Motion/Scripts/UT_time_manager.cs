using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_time_manager : MonoBehaviour
{
    public string inputKey = "space";
    public bool toggleInput; //If true pressing once activates until pressing again. If false, then stays active until key release
    public ultimate_timer timerScriptSlowTime;
    public ultimate_timer timerScriptResumeTime;

    private bool slowIsActive;

    void Update()
    {
        if (toggleInput)
        {
            if (Input.GetKeyDown(inputKey))
            {
                if (!slowIsActive)
                {
                    timerScriptSlowTime.StartTimer();
                    timerScriptResumeTime.ResetTimer();
                    slowIsActive = true;
                    Debug.Log("Slow Time");
                }
                else
                {
                    timerScriptSlowTime.ResetTimer();
                    timerScriptResumeTime.StartTimer();
                    slowIsActive = false;
                    Debug.Log("Resume Time");
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(inputKey))
            {
                timerScriptSlowTime.StartTimer();
                timerScriptResumeTime.ResetTimer();
                Debug.Log("Slow Time");
            }

            if (Input.GetKeyUp(inputKey))
            {
                timerScriptSlowTime.ResetTimer();
                timerScriptResumeTime.StartTimer();
                Debug.Log("Resume Time");
            }
        }
    }
}
