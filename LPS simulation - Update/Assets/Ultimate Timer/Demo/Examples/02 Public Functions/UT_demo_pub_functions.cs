using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UT_demo_pub_functions : MonoBehaviour
{
    //This is for the button example on Scene 02  to interact with the timer
    public TMP_InputField setDurationInput;
    public TMP_InputField increaseTimeInput;
    public TMP_InputField decreaseTimeInput;
    public ultimate_timer ultimate_timer_script;


    public void UpdateDuration()
    {
        //Debug.Log("New Duration: " + setDurationInput.text);
        float newDuration = float.Parse(setDurationInput.text);
        ultimate_timer_script.SetDuration(newDuration);
    }

    public void IncreasTime()
    {
        //Debug.Log("Time Increased by: " + increaseTimeInput.text);
        float newIncrease = float.Parse(increaseTimeInput.text);
        ultimate_timer_script.IncreaseTimer(newIncrease);
    }

    public void DecreaseTime()
    {
        //Debug.Log("Time Reduced by: " + decreaseTimeInput.text);
        float newDecrease = float.Parse(decreaseTimeInput.text);
        ultimate_timer_script.DecreaseTimer(newDecrease);
    }
}
