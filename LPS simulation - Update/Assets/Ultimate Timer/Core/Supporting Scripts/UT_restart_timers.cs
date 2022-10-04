using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_restart_timers : MonoBehaviour
{
    //Batch action for resetting timers back to 0 without pausing
    //Use this when you want a timer to reset when its being re-enabled

    public enum WhenEnabled
    {
        ClearTimer,
        ResetTimer,
        RestartTimer
    }
    public WhenEnabled whenEnabled;


    public ultimate_timer[] scriptsToClear;

    void OnEnable()
    {
        switch (whenEnabled)
        {
            case WhenEnabled.ClearTimer:
                foreach (ultimate_timer scriptsToClearElement in scriptsToClear)
                {
                    if (scriptsToClearElement != null)
                    {
                        scriptsToClearElement.ClearTimer();
                    }
                }
                break;

            case WhenEnabled.ResetTimer:
                foreach (ultimate_timer scriptsToClearElement in scriptsToClear)
                {
                    if (scriptsToClearElement != null)
                    {
                        scriptsToClearElement.ResetTimer();
                    }
                }
                break;

            case WhenEnabled.RestartTimer:
                foreach (ultimate_timer scriptsToClearElement in scriptsToClear)
                {
                    if (scriptsToClearElement != null)
                    {
                        scriptsToClearElement.RestartTimer();
                    }
                }
                break;
        }
    }
}
