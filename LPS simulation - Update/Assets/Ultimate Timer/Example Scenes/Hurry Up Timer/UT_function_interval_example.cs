using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UT_function_interval_example : MonoBehaviour
{

    public GameObject countdownText;
    public TextMeshProUGUI message;


    private void Start()
    {
        ResetExample();
    }

    public void GameOver()
    {
        countdownText.SetActive(false);
        message.text = "- Game Over! -";
    }

    public void TriggerWarning()
    {
        message.text = "Hurry Up!";
    }

    public void ResetExample()
    {
        countdownText.SetActive(true);
        message.text = "Complete the task.";
    }
}
