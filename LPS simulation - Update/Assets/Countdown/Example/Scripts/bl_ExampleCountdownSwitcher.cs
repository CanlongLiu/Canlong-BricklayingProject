using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bl_ExampleCountdownSwitcher : MonoBehaviour
{
    public GameObject[] countdowns;
    public Text nameText;

    private int current = 0;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Switch(false);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Switch(true);
    }

    public void Switch(bool forward)
    {
        if (forward) current = (current + 1) % countdowns.Length;
        else
        {
            if (current <= 0) current = countdowns.Length - 1;
            else current--;
        }
        foreach (var item in countdowns)
        {
            item.gameObject.SetActive(false);
        }
        countdowns[current].SetActive(true);
        nameText.text = $"<size=25>SHOWING</size>\n{countdowns[current].name.ToUpper()}";
    }
}