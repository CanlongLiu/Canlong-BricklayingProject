using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class ultimate_timer : MonoBehaviour
{
    //Version 1.1.3
    //1. Paused timers using Countdown will display the max number counting down from instead of zero on initial load
    //2. Texture and Event Intervals now fire on correct interval instead of one behind when Counting Up using text output Seconds  


    #region Public Vars
    public enum CountDirection
    {
        CountDown,
        CountUp,
        CountUpInfinite
    }
    public CountDirection countDirectionValue;

    public enum Timescale
    {
        Scaled,
        Unscaled
    }
    public Timescale timescale;

    public bool pauseTimer;
    public bool showWarnings;

    [Header("Duration")]
    public float hours;
    public float minutes;
    public float seconds = 10f;
    public bool useExternalTimer;
    public ultimate_timer externalTimerScript;


    [System.Serializable]
    public class OutputOptions
    {
        //Text Formating
        public enum TextFormat
        {
            Timecode,
            Percent,
            Score,
            Seconds
        }
        [Header("Text Output")]
        public TextFormat textFormat;
        public TextMeshProUGUI timerText;
        public int scoreMultiplier = 100;
        public bool truncateTimecode;



        //Image Fill
        [Header("Image Fill")]
        public Image fillImage;

        //Image/Canvas Group Alpha
        public enum AlphaFadeType
        {
            NoFade,
            FadeIn,
            FadeOut
        }
        [Header("Image Fade")]
        public AlphaFadeType fadeType;
        public Image imageToFade; //Transparency of a Canvas image
        public CanvasGroup canvasGroupToFade; //Transparency of a Canvas group


        //Transform
        public enum TransformType
        {
            NoMovement,
            Linear,
            EaseIn,
            EaseOut,
            Smooth
        }
        [Header("Transform (UI or 3D)")]
        public TransformType transformEasing;
        public Transform objToMove;
        public Transform objStartPos;
        public Transform objEndPos;


        //Rotation
        public enum RotationDirection
        {
            NoRotation,
            Clockwise,
            CounterClockwise,
            InheritFromStartAndEndRot
        }
        [Header("Rotation (UI or 3D)")]
        public RotationDirection rotationType;
        public enum RotationType
        {
            Linear,
            EaseIn,
            EaseOut,
            Smooth
        }
        public RotationType rotationEasing;
        public Transform objToRotate;
        public Transform objStartRot;
        public Transform objEndRot;
        public bool rotateXAxis;
        public bool rotateYAxis;
        public bool rotateZAxis; //Check only Z if this is a UI image in the Canvas



        //Scale
        public enum ScaleType
        {
            NoScaling,
            Linear,
            EaseIn,
            EaseOut,
            Smooth
        }
        [Header("Scale (UI or 3D)")]
        public ScaleType scaleEasing;
        public Transform objToScale;
        public Transform objStartSize;
        public Transform objEndSize;


        //Time Scale: 1 being normal playback, 0 being totally stopped
        [Header("TimeScale (Slow Motion)")]
        public bool enableTimeScale;
        [Range(0, 1)]
        public float startSpeed = 1f;
        [Range(0, 1)]
        public float endSpeed = 0.5f;


        //Texture
        [System.Serializable]
        public class TextureIntervals
        {
            public string textureName;
            public float textureChangeInterval;
            public Material textureMaterial;
            public Texture textureImage;
            public Sprite textureSprite;
        }

        public enum TextureType
        {
            SetSprite,
            SetImage,
            SetMaterial
        }
        [Header("Texture")]
        public TextureType textureType;
        public GameObject objWithTexture;
        public TextureIntervals[] textureIntervals;
    }

    [Header("Output Options")]
    public OutputOptions outputOptions;





    [System.Serializable]
    public class EventIntervals
    {
        public string eventName;
        public float eventInterval;
        public UnityEvent intervalFunctions;
    }
    [Header("Event Intervals")]
    public EventIntervals[] eventIntervals;




    public enum OnTimerComplete
    {
        Stop,
        Reset,
        Restart,
        DisableGameObject,
        DestroyGameObject,
    }
    [Header("Timer Complete Options")]
    public OnTimerComplete onTimerComplete;
    public bool callInspectorFunction;
    public UnityEvent TimerComplete; //Add any script to this with public functions to call
    #endregion


    #region Private Vars
    [HideInInspector]
    public float totalDuration;
    [HideInInspector]
    public float currentTime;
    [HideInInspector]
    public float currentTimeDecimalPercent;
    [HideInInspector]
    public float currentTimeRotationPercent;
    [HideInInspector]
    public float timerWholePercent;
    [HideInInspector]
    public float actualTimePassed;
    private bool timerCompleted;
    private float xRotValue;
    private float yRotValue;
    private float zRotValue;
    private TimeSpan timeSpan;
    private int score;
    private Color timerAlphaImgColor;
    private float t; //Translate (mover) time for lerps
    private float truncationTime; //Swaps between currentTime and totalDuration for truncating time code in real time versus showing all 00:00:00 off the bat
    private int textureIntervalCounter; //Increments through the array index each time an interval matches real time
    private int functionIntervalCounter; //Increments through the array index each time an interval matches real time
    private float timeScaleValue = 1;
    #endregion


    void OnEnable()
    {
        if (!pauseTimer)
        {
            ClearTimer();
        }

        //At start, display max timer value if counting down, otherwise show zero if counting up
        if (countDirectionValue == CountDirection.CountDown)
        {
            //Convert and add hours and minutes into seconds, then add the seconds
            totalDuration = ((hours * 60) * 60) + (minutes * 60) + seconds;
            currentTime = totalDuration;
        }

        //Use internal or external time formatting based off this script's setting
        if (useExternalTimer)
        {
            FormatTimeExt();
        }
        else
        {
            FormatTime();
        }





        //Make sure the total duration is not a negative number
        //FormatTime();

        if (totalDuration < 0)
        {
            Debug.LogWarning("Your count down start time cannot be less than zero. Please check inspector on: " + gameObject.name);
            pauseTimer = true;
        }
        else
        {
            //pauseTimer = false; //Default to user initiated
        }

        if (showWarnings)
        {
            //Check to see if text, sprites or game object fields are empty and give a warning
            if (outputOptions.timerText == null)
            {
                Debug.LogWarning("Game Object - " + gameObject.name + ":\nYou have not assigned Timer Count Text. This is a time counter text field to your timer.");
            }

            if (outputOptions.fillImage == null)
            {
                Debug.LogWarning("Game Object - " + gameObject.name + ":\nYou have not assigned Timer Fill Img. This is an image that fills over time with your timer.");
            }

            if (outputOptions.objToRotate == null)
            {
                Debug.LogWarning("Game Object - " + gameObject.name + ":\nYou have not assigned Timer Rotate Obj. This is a sprite or 3D game object that will rotate 0-360 with your timer.");
            }

            if (outputOptions.imageToFade == null)
            {
                Debug.LogWarning("Game Object - " + gameObject.name + ":\nYou have not assigned Timer Alpha Image. This manages a sprite's transparency over time.");
            }

            if (outputOptions.canvasGroupToFade == null)
            {
                Debug.LogWarning("Game Object - " + gameObject.name + ":\nYou have not assigned Timer Alpha Canvas Group. This manages a Canvas Group's transparency over time");
            }

            if (outputOptions.objStartPos == null)
            {
                Debug.LogWarning("Game Object - " + gameObject.name + ":\nYou have not assigned Obj Start Position. This is the origin position of the object you want to move over time");
            }

            if (outputOptions.objEndPos == null)
            {
                Debug.LogWarning("Game Object - " + gameObject.name + ":\nYou have not assigned Obj End Position. This is the destination point of the object you want to move over time");
            }
        }
    }


    void Update()
    {
        if (!pauseTimer)
        {
            //Check to see if we're using local h/m/s input or from an exernal Utlimate Timer script
            if (useExternalTimer) //External. We bypass CountUp() and go right for the update functions using external vars
            {
                if (externalTimerScript != null)
                {
                    FormatTimeExt();
                    FormatSpriteFillExt();
                    FormatSpriteAlphaExt();
                    FormatCanvasGroupAlphaExt();
                    RotateObjectExt();
                    MoveObjectExt();
                    ScaleObjectExt();
                    FormatSpeedOfTimeExt();
                    UpdateFunctionIntervalExt();
                    UpdateTextureIntervalExt();
                }
                else
                {
                    useExternalTimer = false;
                    Debug.Log("Can't find external script, defualting to manual input h/m/s");
                }
            }
            else //Using local h/m/s
            {
                switch (countDirectionValue)
                {
                    case CountDirection.CountDown:
                        if (!timerCompleted)
                        {
                            CountDown();
                        }
                        break;

                    case CountDirection.CountUp:
                        if (!timerCompleted)
                        {
                            CountUp();
                        }
                        break;

                    case CountDirection.CountUpInfinite:
                        CountUpInfinite();
                        break;
                }

                //These operate on an interval, so we can update here
                UpdateFunctionInterval();
                UpdateTextureInterval();
            }

        }
    }


    void CountDown()
    {
        if (currentTime > 0)
        {
            #region Parse Time
            switch (timescale)
            {
                case Timescale.Scaled:
                    currentTime -= Time.deltaTime; //Keeps track of manually enterd start values: 17 - 29
                    actualTimePassed += Time.deltaTime; //Tracks raw time that has passed since start
                    break;
                case Timescale.Unscaled:
                    currentTime -= Time.unscaledDeltaTime; //Keeps track of manually enterd start values: 17 - 29
                    actualTimePassed += Time.unscaledDeltaTime; //Tracks raw time that has passed since start
                    break;
            }


            currentTimeDecimalPercent = (actualTimePassed / totalDuration);
            currentTimeRotationPercent = currentTimeDecimalPercent * -360;
            timerWholePercent = Mathf.FloorToInt(currentTimeDecimalPercent * 100);
            #endregion
        }
        else //End time reached
        {
            //CountDownComplete();
            TimeCompleted(); //Calls the function that resets everything and then calls the inspector "TimerComplete.Invoke()" version
        }

        //Text Output
        FormatTime();

        //Sprite Output
        RotateObject();
        MoveObject();
        ScaleObject();
        FormatSpeedOfTime();
        FormatSpriteFill();
        FormatSpriteAlpha();
        FormatCanvasGroupAlpha();
    }

    void CountUp()
    {
        if (currentTime < totalDuration)
        {
            #region Parse Time
            currentTime += Time.deltaTime; //Keeps track of manually enterd start values: 17 - 29
            actualTimePassed += Time.deltaTime; //Tracks raw time that has passed since start

            currentTimeDecimalPercent = (actualTimePassed / totalDuration);
            currentTimeRotationPercent = currentTimeDecimalPercent * -360;
            timerWholePercent = Mathf.FloorToInt(currentTimeDecimalPercent * 100);
            #endregion
        }
        else //End time reached
        {
            //CountUpComplete();
            TimeCompleted(); //Calls the function that resets everything and then calls the inspector "TimerComplete.Invoke()" version
        }

        //Text Output
        FormatTime();


        //Advanced Output
        RotateObject();
        MoveObject();
        ScaleObject();
        FormatSpeedOfTime();
        FormatSpriteFill();
        FormatSpriteAlpha();
        FormatCanvasGroupAlpha();

    }

    void CountUpInfinite()
    {
        currentTime += Time.deltaTime; //This is adding a starter value in Start() so we can start at 13 and go to 21 if we want, we don't have to always start at 0
        actualTimePassed += Time.deltaTime;

        FormatTime();
    }


    #region Format Outputs Local
    void FormatTime()
    {
        if (outputOptions.truncateTimecode)
        {
            truncationTime = currentTime; //Expand timecode 00:00:00 in real time
        }
        else
        {
            truncationTime = totalDuration; //Show complete timecode 00:00:00 based off total time of the timer
        }

        switch (outputOptions.textFormat)
        {
            case OutputOptions.TextFormat.Timecode:
                timeSpan = TimeSpan.FromSeconds(currentTime);

                if (outputOptions.timerText != null)
                {
                    if (truncationTime >= 3600) //1 hours = 3,600 seconds. Show hours formatting if greater
                    {
                        outputOptions.timerText.text = timeSpan.ToString(@"hh\:mm\:ss\.ff");
                    }

                    if (truncationTime >= 60 && truncationTime < 3600) //1 min = 60 seconds. Show minute formatting if greater
                    {
                        outputOptions.timerText.text = timeSpan.ToString(@"mm\:ss\.ff");
                    }

                    if (truncationTime < 60) //1 secend = 1 second. Show seconds formatting if greater
                    {
                        outputOptions.timerText.text = timeSpan.ToString(@"ss\.ff");
                    }
                }

                break;

            case OutputOptions.TextFormat.Percent:
                if (outputOptions.timerText != null)
                {
                    outputOptions.timerText.text = timerWholePercent.ToString() + "%";
                }
                break;

            case OutputOptions.TextFormat.Score:
                if (outputOptions.timerText != null)
                {
                    //score = Mathf.RoundToInt((currentTime) * outputOptions.scoreMultiplier); //Dependent on count up and count down being selected
                    score = Mathf.RoundToInt((timerWholePercent) * outputOptions.scoreMultiplier); //Always cincreases regardless of count up and count down selected
                    outputOptions.timerText.text = string.Format("{0:#,###0}", score);
                }
                break;

            case OutputOptions.TextFormat.Seconds:
                if (outputOptions.timerText != null)
                {
                    if (countDirectionValue == CountDirection.CountUp)
                    {
                        score = Mathf.FloorToInt(currentTime);
                    }
                    else
                    {
                        score = Mathf.CeilToInt(currentTime);
                    }
                    outputOptions.timerText.text = string.Format("{0:#0}", score);
                }
                break;
        }

    }

    void FormatSpriteFill()
    {
        if (outputOptions.fillImage != null)
        {
            outputOptions.fillImage.fillAmount = currentTimeDecimalPercent;
        }
    }

    void FormatSpriteAlpha()
    {
        if (outputOptions.imageToFade != null)
        {
            switch (outputOptions.fadeType)
            {
                case OutputOptions.AlphaFadeType.NoFade:
                    timerAlphaImgColor = outputOptions.imageToFade.color;
                    timerAlphaImgColor.a = 1f;
                    outputOptions.imageToFade.color = timerAlphaImgColor;
                    break;
                case OutputOptions.AlphaFadeType.FadeIn:
                    timerAlphaImgColor = outputOptions.imageToFade.color;
                    timerAlphaImgColor.a = currentTimeDecimalPercent;
                    outputOptions.imageToFade.color = timerAlphaImgColor;
                    break;
                case OutputOptions.AlphaFadeType.FadeOut:
                    timerAlphaImgColor = outputOptions.imageToFade.color;
                    timerAlphaImgColor.a = (1 - currentTimeDecimalPercent);
                    outputOptions.imageToFade.color = timerAlphaImgColor;
                    break;
            }
        }
    }

    void FormatCanvasGroupAlpha()
    {
        if (outputOptions.canvasGroupToFade != null)
        {
            switch (outputOptions.fadeType)
            {
                case OutputOptions.AlphaFadeType.NoFade:
                    outputOptions.canvasGroupToFade.alpha = 1f;
                    break;
                case OutputOptions.AlphaFadeType.FadeIn:
                    outputOptions.canvasGroupToFade.alpha = currentTimeDecimalPercent;
                    break;
                case OutputOptions.AlphaFadeType.FadeOut:
                    outputOptions.canvasGroupToFade.alpha = (1 - currentTimeDecimalPercent);
                    break;
            }
        }
    }

    void RotateObject()
    {
        if (outputOptions.objToRotate != null)
        {

            switch (outputOptions.rotationType)
            {
                case OutputOptions.RotationDirection.NoRotation:
                    xRotValue = 0f;
                    yRotValue = 0f;
                    zRotValue = 0f;

                    outputOptions.objToRotate.localRotation = Quaternion.Euler(new Vector3(xRotValue, yRotValue, zRotValue));
                    break;

                case OutputOptions.RotationDirection.Clockwise:
                    #region Check to see which axis are checked for rotating
                    if (outputOptions.rotateXAxis)
                    {
                        xRotValue = currentTimeRotationPercent;
                    }
                    else
                    {
                        xRotValue = 0f;
                    }

                    if (outputOptions.rotateYAxis)
                    {
                        yRotValue = currentTimeRotationPercent;
                    }
                    else
                    {
                        yRotValue = 0f;
                    }

                    if (outputOptions.rotateZAxis)
                    {
                        zRotValue = currentTimeRotationPercent;
                    }
                    else
                    {
                        zRotValue = 0f;
                    }
                    #endregion

                    outputOptions.objToRotate.localRotation = Quaternion.Euler(new Vector3(xRotValue, yRotValue, zRotValue));
                    break;

                case OutputOptions.RotationDirection.CounterClockwise:
                    #region Check to see which axis are checked for rotating
                    if (outputOptions.rotateXAxis)
                    {
                        xRotValue = (360 - currentTimeRotationPercent);
                    }
                    else
                    {
                        xRotValue = 0f;
                    }

                    if (outputOptions.rotateYAxis)
                    {
                        yRotValue = (360 - currentTimeRotationPercent);
                    }
                    else
                    {
                        yRotValue = 0f;
                    }

                    if (outputOptions.rotateZAxis)
                    {
                        zRotValue = (360 - currentTimeRotationPercent);
                    }
                    else
                    {
                        zRotValue = 0f;
                    }
                    #endregion

                    outputOptions.objToRotate.localRotation = Quaternion.Euler(new Vector3(xRotValue, yRotValue, zRotValue));
                    break;

                case OutputOptions.RotationDirection.InheritFromStartAndEndRot:
                    //Inherit the position easing from the 3D mover game object "TransformType outputOptions.objEndPos" to match rotation
                    t = currentTimeDecimalPercent;

                    switch (outputOptions.rotationEasing)
                    {
                        case OutputOptions.RotationType.Linear:
                            t = t * t * (3f - 2f * t);
                            break;

                        case OutputOptions.RotationType.EaseIn:
                            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                            break;

                        case OutputOptions.RotationType.EaseOut:
                            t = Mathf.Sin(t * Mathf.PI * 0.5f);
                            break;
                        case OutputOptions.RotationType.Smooth:
                            t = t * t * t * (t * (6f * t - 15f) + 10f);
                            break;
                    }

                    //Rotation value goes here
                    outputOptions.objToRotate.localRotation = Quaternion.Slerp(outputOptions.objStartRot.localRotation, outputOptions.objEndRot.localRotation, t);
                    break;
            }


        }
    }

    void MoveObject()
    {
        if (outputOptions.objStartPos != null && outputOptions.objEndPos != null)
        {
            t = currentTimeDecimalPercent;

            switch (outputOptions.transformEasing)
            {
                case OutputOptions.TransformType.NoMovement:
                    t = 0f;
                    break;
                case OutputOptions.TransformType.Linear:
                    //t = t * t * (3f - 2f * t);
                    break;

                case OutputOptions.TransformType.EaseIn:
                    t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;

                case OutputOptions.TransformType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case OutputOptions.TransformType.Smooth:
                    t = t * t * t * (t * (6f * t - 15f) + 10f);
                    break;
            }

            outputOptions.objToMove.position = Vector3.Lerp(outputOptions.objStartPos.position, outputOptions.objEndPos.position, t);
        }
    }

    void ScaleObject()
    {
        if (outputOptions.objStartSize != null && outputOptions.objEndSize != null)
        {
            t = currentTimeDecimalPercent;

            switch (outputOptions.scaleEasing)
            {
                case OutputOptions.ScaleType.NoScaling:
                    t = 0f;
                    break;

                case OutputOptions.ScaleType.Linear:
                    //t = t * t * (3f - 2f * t);
                    break;

                case OutputOptions.ScaleType.EaseIn:
                    t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;

                case OutputOptions.ScaleType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;

                case OutputOptions.ScaleType.Smooth:
                    t = t * t * t * (t * (6f * t - 15f) + 10f);
                    break;
            }

            outputOptions.objToScale.localScale = Vector3.Lerp(outputOptions.objStartSize.localScale, outputOptions.objEndSize.localScale, t);
        }
    }

    void FormatSpeedOfTime()
    {
        if (outputOptions.enableTimeScale)
        {
            //Goes from start speed to end speed over duration
            timeScaleValue = outputOptions.startSpeed - (currentTimeDecimalPercent * (outputOptions.startSpeed - outputOptions.endSpeed));

            Time.timeScale = timeScaleValue;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            //Debug.Log("Current Decimal: " + timeScaleValue);
        }
    }

    void UpdateFunctionInterval() //This is being updated every second
    {
        if (eventIntervals.Length > 0) //If user added at least one interval
        {
            //Compare set timed intervals with current time and execute
            if (functionIntervalCounter < eventIntervals.Length) //If we havent reached max intervals, do another
            {                
                //Do this first, then change depending on the conditions below this block:
                //Current time is now more than the set interval interval. Set for 5 and we're at 5.1
                if (actualTimePassed >= eventIntervals[functionIntervalCounter].eventInterval)
                {
                    eventIntervals[functionIntervalCounter].intervalFunctions.Invoke();
                    //Debug.Log(functionIntervalCounter + " / " + (eventIntervals.Length-1));
                    functionIntervalCounter++;
                }
                else //Current time is less than the next interval. Set for 5 and we're at 4.3
                {
                    //This will ALWAYS be true since there's always the next interval to reach
                }
            }
        }
    }

    void UpdateTextureInterval()
    {

        if (outputOptions.objWithTexture != null && outputOptions.textureIntervals.Length > 0)
        {
            //Compare set timed intervals with current time and execute
            if (textureIntervalCounter < outputOptions.textureIntervals.Length)
            {
                if (actualTimePassed >= outputOptions.textureIntervals[textureIntervalCounter].textureChangeInterval)
                {
                    //Interval Reached
                    //Check to see if Material or Texture (Albedo) was selected
                    //Declare renderer
                    AssignRenderer();

                    //Debug.Log(textureIntervalCounter + " / " + (textureIntervals.Length-1));
                    textureIntervalCounter++;
                }

            }



            //Skips texture to last index for a Stop() function
            if (currentTimeDecimalPercent == 1)
            {
                textureIntervalCounter = (outputOptions.textureIntervals.Length - 1);

                AssignRenderer();
            }

        }

    }

    void AssignRenderer()
    {
        Renderer renderer = outputOptions.objWithTexture.GetComponent<Renderer>();
        switch (outputOptions.textureType)
        {
            case OutputOptions.TextureType.SetSprite:
                //Change Material
                if (outputOptions.textureIntervals[textureIntervalCounter].textureSprite != null)
                {
                    outputOptions.objWithTexture.GetComponent<Image>().sprite = outputOptions.textureIntervals[textureIntervalCounter].textureSprite;
                }
                break;

            case OutputOptions.TextureType.SetMaterial:
                //Change Material
                if (outputOptions.textureIntervals[textureIntervalCounter].textureMaterial != null)
                {
                    //Debug.Log("Change material!");
                    renderer.material = outputOptions.textureIntervals[textureIntervalCounter].textureMaterial;
                }
                break;

            case OutputOptions.TextureType.SetImage:
                //Change Albedo
                if (outputOptions.textureIntervals[textureIntervalCounter].textureImage != null)
                {
                    //Debug.Log("Change sprite!");
                    renderer.material.mainTexture = outputOptions.textureIntervals[textureIntervalCounter].textureImage;

                }
                break;
        }
    }
    #endregion


    #region Format Outputs External
    void FormatTimeExt()
    {
        if (outputOptions.truncateTimecode)
        {
            truncationTime = externalTimerScript.currentTime; //Expand timecode 00:00:00 in real time
        }
        else
        {
            truncationTime = externalTimerScript.totalDuration; //Show complete timecode 00:00:00 based off total time of the timer
        }

        switch (outputOptions.textFormat)
        {
            case OutputOptions.TextFormat.Timecode:
                timeSpan = TimeSpan.FromSeconds(externalTimerScript.currentTime);

                if (outputOptions.timerText != null)
                {
                    if (truncationTime >= 3600) //1 hours = 3,600 seconds. Show hours formatting if greater
                    {
                        outputOptions.timerText.text = timeSpan.ToString(@"hh\:mm\:ss\.ff");
                    }

                    if (truncationTime >= 60 && truncationTime < 3600) //1 min = 60 seconds. Show minute formatting if greater
                    {
                        outputOptions.timerText.text = timeSpan.ToString(@"mm\:ss\.ff");
                    }

                    if (truncationTime < 60) //1 secend = 1 second. Show seconds formatting if greater
                    {
                        outputOptions.timerText.text = timeSpan.ToString(@"ss\.ff");
                    }
                }
                break;

            case OutputOptions.TextFormat.Percent:
                if (outputOptions.timerText != null)
                {
                    outputOptions.timerText.text = externalTimerScript.timerWholePercent.ToString() + "%";
                }
                break;

            case OutputOptions.TextFormat.Score:
                if (outputOptions.timerText != null)
                {
                    //score = Mathf.RoundToInt((externalTimerScript.currentTime) * outputOptions.scoreMultiplier); //Dependent on count up and count down being selected
                    score = Mathf.RoundToInt((externalTimerScript.timerWholePercent) * outputOptions.scoreMultiplier); //Always cincreases regardless of count up and count down selected

                    outputOptions.timerText.text = string.Format("{0:#,###0}", score);
                }
                break;

            case OutputOptions.TextFormat.Seconds:
                if (outputOptions.timerText != null)
                {
                    if (countDirectionValue == CountDirection.CountUp)
                    {
                        score = Mathf.FloorToInt(externalTimerScript.currentTime);
                    }
                    else
                    {
                        score = Mathf.CeilToInt(externalTimerScript.currentTime);
                    }  
                    outputOptions.timerText.text = string.Format("{0:#0}", score);
                }
                break;
        }

    }

    void FormatSpriteFillExt()
    {
        if (outputOptions.fillImage != null)
        {
            outputOptions.fillImage.fillAmount = externalTimerScript.currentTimeDecimalPercent;
        }
    }

    void FormatSpriteAlphaExt()
    {
        if (outputOptions.imageToFade != null)
        {
            switch (outputOptions.fadeType)
            {
                case OutputOptions.AlphaFadeType.NoFade:
                    timerAlphaImgColor = outputOptions.imageToFade.color;
                    timerAlphaImgColor.a = 1f;
                    outputOptions.imageToFade.color = timerAlphaImgColor;
                    break;
                case OutputOptions.AlphaFadeType.FadeIn:
                    timerAlphaImgColor = outputOptions.imageToFade.color;
                    timerAlphaImgColor.a = externalTimerScript.currentTimeDecimalPercent;
                    outputOptions.imageToFade.color = timerAlphaImgColor;
                    break;
                case OutputOptions.AlphaFadeType.FadeOut:
                    timerAlphaImgColor = outputOptions.imageToFade.color;
                    timerAlphaImgColor.a = (1 - externalTimerScript.currentTimeDecimalPercent);
                    outputOptions.imageToFade.color = timerAlphaImgColor;
                    break;
            }
        }
    }

    void FormatCanvasGroupAlphaExt()
    {
        if (outputOptions.canvasGroupToFade != null)
        {
            switch (outputOptions.fadeType)
            {
                case OutputOptions.AlphaFadeType.NoFade:
                    outputOptions.canvasGroupToFade.alpha = 1f;
                    break;
                case OutputOptions.AlphaFadeType.FadeIn:
                    outputOptions.canvasGroupToFade.alpha = externalTimerScript.currentTimeDecimalPercent;
                    break;
                case OutputOptions.AlphaFadeType.FadeOut:
                    outputOptions.canvasGroupToFade.alpha = (1 - externalTimerScript.currentTimeDecimalPercent);
                    break;
            }
        }
    }

    void RotateObjectExt()
    {
        if (outputOptions.objToRotate != null)
        {

            switch (outputOptions.rotationType)
            {
                case OutputOptions.RotationDirection.NoRotation:
                    xRotValue = 0f;
                    yRotValue = 0f;
                    zRotValue = 0f;

                    outputOptions.objToRotate.localRotation = Quaternion.Euler(new Vector3(xRotValue, yRotValue, zRotValue));
                    break;

                case OutputOptions.RotationDirection.Clockwise:
                    #region Check to see which axis are checked for rotating
                    if (outputOptions.rotateXAxis)
                    {
                        xRotValue = externalTimerScript.currentTimeRotationPercent;
                    }
                    else
                    {
                        xRotValue = 0f;
                    }

                    if (outputOptions.rotateYAxis)
                    {
                        yRotValue = externalTimerScript.currentTimeRotationPercent;
                    }
                    else
                    {
                        yRotValue = 0f;
                    }

                    if (outputOptions.rotateZAxis)
                    {
                        zRotValue = externalTimerScript.currentTimeRotationPercent;
                    }
                    else
                    {
                        zRotValue = 0f;
                    }
                    #endregion

                    outputOptions.objToRotate.localRotation = Quaternion.Euler(new Vector3(xRotValue, yRotValue, zRotValue));
                    break;

                case OutputOptions.RotationDirection.CounterClockwise:
                    #region Check to see which axis are checked for rotating
                    if (outputOptions.rotateXAxis)
                    {
                        xRotValue = (360 - externalTimerScript.currentTimeRotationPercent);
                    }
                    else
                    {
                        xRotValue = 0f;
                    }

                    if (outputOptions.rotateYAxis)
                    {
                        yRotValue = (360 - externalTimerScript.currentTimeRotationPercent);
                    }
                    else
                    {
                        yRotValue = 0f;
                    }

                    if (outputOptions.rotateZAxis)
                    {
                        zRotValue = (360 - externalTimerScript.currentTimeRotationPercent);
                    }
                    else
                    {
                        zRotValue = 0f;
                    }
                    #endregion

                    outputOptions.objToRotate.localRotation = Quaternion.Euler(new Vector3(xRotValue, yRotValue, zRotValue));
                    break;

                case OutputOptions.RotationDirection.InheritFromStartAndEndRot:
                    //Inherit the position easing from the 3D mover game object "TransformType outputOptions.objEndPos" to match rotation
                    t = externalTimerScript.currentTimeDecimalPercent;

                    switch (outputOptions.rotationEasing)
                    {
                        case OutputOptions.RotationType.Linear:
                            t = t * t * (3f - 2f * t);
                            break;

                        case OutputOptions.RotationType.EaseIn:
                            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                            break;

                        case OutputOptions.RotationType.EaseOut:
                            t = Mathf.Sin(t * Mathf.PI * 0.5f);
                            break;
                        case OutputOptions.RotationType.Smooth:
                            t = t * t * t * (t * (6f * t - 15f) + 10f);
                            break;
                    }

                    //Rotation value goes here
                    outputOptions.objToRotate.localRotation = Quaternion.Slerp(outputOptions.objStartRot.localRotation, outputOptions.objEndRot.localRotation, t);
                    break;
            }


        }
    }

    void MoveObjectExt()
    {
        if (outputOptions.objStartPos != null && outputOptions.objEndPos != null)
        {
            t = externalTimerScript.currentTimeDecimalPercent;

            switch (outputOptions.transformEasing)
            {
                case OutputOptions.TransformType.NoMovement:
                    t = 0f;
                    break;
                case OutputOptions.TransformType.Linear:
                    //t = t * t * (3f - 2f * t);
                    break;

                case OutputOptions.TransformType.EaseIn:
                    t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;

                case OutputOptions.TransformType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case OutputOptions.TransformType.Smooth:
                    t = t * t * t * (t * (6f * t - 15f) + 10f);
                    break;
            }

            outputOptions.objToMove.position = Vector3.Lerp(outputOptions.objStartPos.position, outputOptions.objEndPos.position, t);
        }
    }

    void ScaleObjectExt()
    {
        if (outputOptions.objStartSize != null && outputOptions.objEndSize != null)
        {
            t = externalTimerScript.currentTimeDecimalPercent;

            switch (outputOptions.scaleEasing)
            {
                case OutputOptions.ScaleType.NoScaling:
                    t = 0f;
                    break;

                case OutputOptions.ScaleType.Linear:
                    //t = t * t * (3f - 2f * t);
                    break;

                case OutputOptions.ScaleType.EaseIn:
                    t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;

                case OutputOptions.ScaleType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;

                case OutputOptions.ScaleType.Smooth:
                    t = t * t * t * (t * (6f * t - 15f) + 10f);
                    break;
            }

            outputOptions.objToScale.localScale = Vector3.Lerp(outputOptions.objStartSize.localScale, outputOptions.objEndSize.localScale, t);
        }
    }

    void FormatSpeedOfTimeExt()
    {
        if (outputOptions.enableTimeScale)
        {
            //Goes from start speed to end speed over duration
            timeScaleValue = outputOptions.startSpeed - (externalTimerScript.currentTimeDecimalPercent * (outputOptions.startSpeed - outputOptions.endSpeed));

            Time.timeScale = timeScaleValue;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Debug.Log("Current Decimal: " + timeScaleValue);
        }
    }

    void UpdateFunctionIntervalExt()
    {
        if (eventIntervals.Length > 0)
        {
            //Compare set timed intervals with current time and execute
            if (functionIntervalCounter < eventIntervals.Length)
            {
                if (externalTimerScript.actualTimePassed >= eventIntervals[functionIntervalCounter].eventInterval)
                {
                    eventIntervals[functionIntervalCounter].intervalFunctions.Invoke();
                    //Debug.Log(functionIntervalCounter + " / " + (eventIntervals.Length-1));
                    functionIntervalCounter++;
                }
            }
        }
    }

    void UpdateTextureIntervalExt()
    {

        if (outputOptions.objWithTexture != null && outputOptions.textureIntervals.Length > 0)
        {
            //Compare set timed intervals with current time and execute
            if (textureIntervalCounter < outputOptions.textureIntervals.Length)
            {
                if (externalTimerScript.actualTimePassed >= outputOptions.textureIntervals[textureIntervalCounter].textureChangeInterval)
                {
                    //Interval Reached
                    //Check to see if Material or Texture (Albedo) was selected
                    //Declare renderer
                    AssignRenderer();

                    //Debug.Log(textureIntervalCounter + " / " + (textureIntervals.Length-1));
                    textureIntervalCounter++;
                }

            }

            //Resets Texture
            if (externalTimerScript.actualTimePassed == 0)
            {
                textureIntervalCounter = 0;
            }

            //Skips texture to last index for a Stop() function
            if (externalTimerScript.currentTimeDecimalPercent == 1)
            {
                textureIntervalCounter = (outputOptions.textureIntervals.Length - 1);

                AssignRenderer();
            }

        }

    }

    void AssignRendererExt()
    {
        Renderer renderer = outputOptions.objWithTexture.GetComponent<Renderer>();
        switch (outputOptions.textureType)
        {
            case OutputOptions.TextureType.SetMaterial:
                //Change Material
                if (outputOptions.textureIntervals[textureIntervalCounter].textureMaterial != null)
                {
                    //Debug.Log("Change material!");
                    renderer.material = outputOptions.textureIntervals[textureIntervalCounter].textureMaterial;
                }
                break;

            case OutputOptions.TextureType.SetImage:
                //Change Albedo
                if (outputOptions.textureIntervals[textureIntervalCounter].textureImage != null)
                {
                    //Debug.Log("Change sprite!");
                    renderer.material.mainTexture = outputOptions.textureIntervals[textureIntervalCounter].textureImage;

                }
                break;
        }
    }
    #endregion




    #region Public Functions

    public void ClearTimer() //Resets timer without forcing a pause value. Use this to reset a timer while respecting the manual PAUSE input bool from the inspector
    {
        timeScaleValue = 1;
        currentTimeDecimalPercent = 0f;
        timerWholePercent = 0;
        currentTimeRotationPercent = 0f;
        actualTimePassed = 0f;
        functionIntervalCounter = 0;
        textureIntervalCounter = 0;
        if (outputOptions.objWithTexture != null && outputOptions.textureIntervals.Length > 0)
        {
            outputOptions.textureIntervals[textureIntervalCounter].textureChangeInterval = 0;
        }

        timerCompleted = false;
        //Convert and add hours and minutes into seconds, then add the seconds
        totalDuration = ((hours * 60) * 60) + (minutes * 60) + seconds;

        switch (countDirectionValue)
        {
            case CountDirection.CountDown:
                currentTime = totalDuration;
                break;

            case CountDirection.CountUp:
                currentTime = 0;
                break;

            case CountDirection.CountUpInfinite:
                currentTime = 0;
                break;
        }


        //We should output to the text, sprites and game objects just in case player resets while paused.
        //Text Output
        FormatTime();

        //Sprite Output
        MoveObject();
        ScaleObject();
        FormatSpeedOfTime();
        RotateObject();
        FormatSpriteFill();
        FormatSpriteAlpha();
        FormatCanvasGroupAlpha();
        UpdateTextureInterval();



        //Debug.Log("Timer Cleared on " + gameObject.name  + " - " + this.GetType().Name);
    }

    public void StartTimer() //Use this to enable a disabled gameobject that the timer is on, reset it, and unpause a timer that might have been previously paused
    {
        gameObject.SetActive(true);
        ClearTimer();
        pauseTimer = false;
    }

    public void PauseTimer()
    {
        pauseTimer = true;
    }

    public void ResumeTimer()
    {
        pauseTimer = false;
    }

    public void ResetTimer() //Clears timer back to Start and pauses timer
    {
        timeScaleValue = 1;
        currentTimeDecimalPercent = 0f;
        timerWholePercent = 0;
        currentTimeRotationPercent = 0f;
        actualTimePassed = 0f;
        functionIntervalCounter = 0;
        textureIntervalCounter = 0;

        if (outputOptions.objWithTexture != null && outputOptions.textureIntervals.Length > 0)
        {
            outputOptions.textureIntervals[textureIntervalCounter].textureChangeInterval = 0;
        }

        timerCompleted = false;
        //Convert and add hours and minutes into seconds, then add the seconds
        totalDuration = ((hours * 60) * 60) + (minutes * 60) + seconds;

        switch (countDirectionValue)
        {
            case CountDirection.CountDown:
                currentTime = totalDuration;
                break;

            case CountDirection.CountUp:
                currentTime = 0;
                break;

            case CountDirection.CountUpInfinite:
                currentTime = 0;
                break;
        }

        //We should output to the text, sprites and game objects just in case player resets while paused.
        //Text Output
        FormatTime();

        //Sprite Output
        MoveObject();
        ScaleObject();
        FormatSpeedOfTime();
        RotateObject();
        FormatSpriteFill();
        FormatSpriteAlpha();
        FormatCanvasGroupAlpha();
        UpdateTextureInterval();

        pauseTimer = true;
    }

    public void RestartTimer() //Clears timer back to Start and plays timer
    {
        ClearTimer();
        pauseTimer = false;
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    public void TimeCompleted() //When this timer reaches end time. Different than "TimerComplete" which triggers the inspector function
    {
        //Triggers external function(s) (if available)
        if (callInspectorFunction)
        {
            TimerComplete.Invoke();
        }

        //Trigger internal function(s)
        switch (countDirectionValue)
        {
            case CountDirection.CountDown:
                currentTime = 0; //count down
                //Debug.Log("[Internal] Count Down complete! - " + gameObject.name);
                break;

            case CountDirection.CountUp:
                currentTime = totalDuration; //count up
                //Debug.Log("[Internal] Count Up complete! - " + gameObject.name);
                break;
        }

        switch (onTimerComplete)
        {
            case OnTimerComplete.DestroyGameObject:
                DestroyGameObject();
                break;

            case OnTimerComplete.Reset:
                ResetTimer();
                break;

            case OnTimerComplete.Restart:
                RestartTimer();
                break;

            case OnTimerComplete.Stop:
                currentTimeDecimalPercent = 1;
                timerWholePercent = 100;
                currentTimeRotationPercent = 360;
                //functionIntervalCounter = 1;
                //textureIntervalCounter = 1;
                timerCompleted = true;

                //Text Output
                FormatTime();

                //Sprite Output
                RotateObject();
                MoveObject();
                ScaleObject();
                FormatSpeedOfTime();
                FormatSpriteFill();
                FormatSpriteAlpha();
                FormatCanvasGroupAlpha();
                UpdateTextureInterval();

                pauseTimer = true;

                break;

            case OnTimerComplete.DisableGameObject:
                currentTimeDecimalPercent = 1;
                timerWholePercent = 100;
                currentTimeRotationPercent = 360;
                //functionIntervalCounter = 1;
                //textureIntervalCounter = 1;
                timerCompleted = true;

                //Text Output
                FormatTime();

                //Sprite Output
                RotateObject();
                MoveObject();
                ScaleObject();
                FormatSpeedOfTime();
                FormatSpriteFill();
                FormatSpriteAlpha();
                FormatCanvasGroupAlpha();
                UpdateTextureInterval();

                gameObject.SetActive(false);
                break;
        }
    }

    public void SetDuration(float newDuration)
    {
        hours = 0f;
        minutes = 0f;
        seconds = newDuration;
        ClearTimer();
        //totalDuration = newDuration;
    }

    public void IncreaseTimer(float newIncrease)
    {
        currentTime += newIncrease;
        actualTimePassed += newIncrease;
    }

    public void DecreaseTimer(float newDecrease)
    {
        currentTime -= newDecrease;
        actualTimePassed -= newDecrease;
    }
    #endregion
}
