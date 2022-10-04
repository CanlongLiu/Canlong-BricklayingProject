using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UT_example_functions : MonoBehaviour
{
    //Simple pre-made functions that carry out a console Debug.Log messages for testing


    public void StartGame()
    {
        Debug.Log("Game Started! - " + gameObject.name);
    }

    public void GameOver()
    {
        Debug.Log("Game Over!! - " + gameObject.name);
    }


    public void TimerComplete()
    {
        Debug.Log("Timer complete! - " + gameObject.name);
    }

}