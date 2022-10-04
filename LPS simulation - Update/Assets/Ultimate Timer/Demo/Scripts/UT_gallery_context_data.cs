using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UT_gallery_context_data : MonoBehaviour
{

    public UT_gallery galleryScript;
    public TextMeshProUGUI hdr1Text;
    public TextMeshProUGUI hdr2Text;
    public GameObject background_grad;
    public GameObject slideTitleGO;
    public GameObject slideDescGO;
    private TextMeshProUGUI slideTitle;
    private TextMeshProUGUI slideDesc;
    public TextMeshProUGUI nextButtonTop;
    public TextMeshProUGUI nextButtonBot;
    public TextMeshProUGUI prevButtonTop;
    public TextMeshProUGUI prevButtonBot;

    public GameObject scene_3D_Cam_Movement;
    public GameObject scene_3D_Texxture_Change;


    private void OnEnable()
    {
        slideTitle = slideTitleGO.GetComponent<TextMeshProUGUI>();
        slideDesc = slideDescGO.GetComponent<TextMeshProUGUI>();

        scene_3D_Cam_Movement.SetActive(false);
        scene_3D_Texxture_Change.SetActive(false);

        HideTitleDesc();
    }

    void ShowTitleDesc()
    {
        slideTitleGO.SetActive(true);
        slideDescGO.SetActive(true);
    }

    void HideTitleDesc()
    {
        slideTitleGO.SetActive(false);
        slideDescGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (galleryScript.pageIndex == 0)
        {
            hdr1Text.text = "Welcome";
            hdr2Text.text = "Intro Page";
            slideTitle.text = "";
            slideDesc.text = "";
            background_grad.SetActive(true);
            nextButtonTop.text = "Interaction";
            nextButtonBot.text = "Public Functions";
            prevButtonTop.text = "3D Example:";
            prevButtonBot.text = "Texture Change";
            HideTitleDesc();
        }

        if (galleryScript.pageIndex == 1)
        {
            hdr1Text.text = "Interaction";
            hdr2Text.text = "Public Functions";
            slideTitle.text = "<b>Public</b> Functions";
            slideDesc.text = "Ultimate Timer accepts 12 different commands for controlling timer behaviour";
            background_grad.SetActive(true);
            nextButtonTop.text = "Options";
            nextButtonBot.text = "Timer Options";
            prevButtonTop.text = "Welcome";
            prevButtonBot.text = "Intro Page";
            HideTitleDesc();
        }

        if (galleryScript.pageIndex == 2)
        {
            hdr1Text.text = "Options";
            hdr2Text.text = "Timer Options";
            slideTitle.text = "<b>Timer</b> Options";
            slideDesc.text = "Input options for setting Ultimate Timer";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Text Formats";
            prevButtonTop.text = "Interaction";
            prevButtonBot.text = "Public Functions";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 3)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Text Formats";
            slideTitle.text = "<b>Text</b> Formats";
            slideDesc.text = "Ultimate Timer uses <color=#66ccff>TextMeshPro</color> with 3 types of output options";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Image Fill";
            prevButtonTop.text = "Options";
            prevButtonBot.text = "Timer Options";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 4)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Image Fill";
            slideTitle.text = "<b>Image Type:</b> Filled";
            slideDesc.text = "If your <color=#66ccff><i>image type</i></color> is set to <color=#66ccff><i>Filled</i></color>, Ultimate Timer can access the fill amount and calculate against time passed";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Fading Images & Groups";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Text Formats";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 5)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Fading Images & Groups";
            slideTitle.text = "<b>Fading</b> Images & Groups";
            slideDesc.text = "Fade in or out Image or Canvas Groups over time";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Scaling Objects";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Image Fill";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 6)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Scaling Objects";
            slideTitle.text = "<b>Scaling</b> Objects";
            slideDesc.text = "Incrase or decrease scale over time";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Moving UI Objects";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Fading Images & Groups";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 7)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Moving UI Objects";
            slideTitle.text = "<b>Moving</b> UI Objects";
            slideDesc.text = "Set <color=#66ccff>start</color> and <color=#66ccff>end positions</color> to move objects from one point to another over time\nby using one of the four methods below:";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Rotating UI Objects";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Scaling Objects";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 8)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Rotating UI Objects";
            slideTitle.text = "<b>Rotating</b> UI Objects";
            slideDesc.text = "Rotate objects infinitely around specific axis or inherit start and stop rotation values";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Event Intervals";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Moving UI Objects";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 9)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Event Intervals";
            slideTitle.text = "<b>Event</b> Intervals";
            slideDesc.text = "Set timed intervals to call custom events";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Textures & Materials";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Rotating UI Objects";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 10)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Textures & Materials";
            slideTitle.text = "<b>Textures</b> & Materials";
            slideDesc.text = "Update textures, sprites, and materials at custom intervals";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Timer Complete";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Event Intervals";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 11)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Timer Complete";
            slideTitle.text = "<b>Timer</b> Complete";
            slideDesc.text = "Ultimate Timer has 7 built-in functions that can be called when a cycle completes:\nCall Custom Functions, Stop, Reset, Restart, Disable Script, Disable Game Object, and Destroy Game Object";
            background_grad.SetActive(true);
            nextButtonTop.text = "2D Example:";
            nextButtonBot.text = "Speed Lines";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Textures & Materials";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 12)
        {
            hdr1Text.text = "2D Example:";
            hdr2Text.text = "Speed Lines";
            slideTitle.text = "";
            slideDesc.text = "";
            background_grad.SetActive(true);
            nextButtonTop.text = "2D Example";
            nextButtonBot.text = "Rain";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Timer Complete";
            HideTitleDesc();
        }

        if (galleryScript.pageIndex == 13)
        {
            hdr1Text.text = "2D Example:";
            hdr2Text.text = "Rain";
            slideTitle.text = "";
            slideDesc.text = "";
            background_grad.SetActive(true);
            nextButtonTop.text = "Output Options:";
            nextButtonBot.text = "Working with 3D Objects";
            prevButtonTop.text = "2D Example:";
            prevButtonBot.text = "Speed Lines";
            HideTitleDesc();
        }

        if (galleryScript.pageIndex == 14)
        {
            hdr1Text.text = "Output Options:";
            hdr2Text.text = "Working with 3D Objects";
            slideTitle.text = "<b>3D</b> Objects";
            slideDesc.text = "Use Ultimate Timer to move, rotate, and change textures or materials on 3D objects over time";
            background_grad.SetActive(true);
            nextButtonTop.text = "3D Example:";
            nextButtonBot.text = "Multi-Scene Camera Lerp";
            prevButtonTop.text = "2D Example:";
            prevButtonBot.text = "Rain";
            ShowTitleDesc();
        }

        if (galleryScript.pageIndex == 15)
        {
            hdr1Text.text = "3D Example:";
            hdr2Text.text = "Multi-Scene Camera Lerp";
            slideTitle.text = "";
            slideDesc.text = "";
            background_grad.SetActive(false);
            nextButtonTop.text = "3D Example:";
            nextButtonBot.text = "Texture Change";
            prevButtonTop.text = "Output Options:";
            prevButtonBot.text = "Working with 3D Objects";
            HideTitleDesc();
        }

        if (galleryScript.pageIndex == 16)
        {
            hdr1Text.text = "3D Example:";
            hdr2Text.text = "Texture Change";
            slideTitle.text = "";
            slideDesc.text = "";
            background_grad.SetActive(false);
            nextButtonTop.text = "Welcome";
            nextButtonBot.text = "Intro Page";
            prevButtonTop.text = "3D Example:";
            prevButtonBot.text = "Multi-Scene Camera Lerp";
            HideTitleDesc();
        }
    }
}
