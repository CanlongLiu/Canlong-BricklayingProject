using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using GVRI;

public class DesignUIController : MonoBehaviour
{
    public HandIntegration hand;
    public GameObject attachPoint;
    public bool copy_orientation = true;
    public bool toggleable = false;
    public GameObject UI;
    protected bool lastFrameValue = false;
    protected virtual void Update()
    {

        if (!hand) return; //need a hand

        bool value = false;
        if (hand.device.TryGetFeatureValue(CommonUsages.primaryButton, out value)) // I change primary2DAxisClick to primaryButton
        {
            Input(value);
        }
        lastFrameValue = value;
    }
    void SetOrientation()
    {
        var target = attachPoint.transform;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }

    bool isOpen = false;

    protected virtual void Input(bool value)
    {

        if (toggleable)
        {
            if (!isOpen && value && !lastFrameValue)
            {// player started press
                SetOrientation();
                UI.SetActive(true);
            }

            if (!value && lastFrameValue)
            {// player stopped press
                if (!isOpen) isOpen = true;
                else
                {
                    isOpen = false;
                    UI.SetActive(false);
                }
            }
        }
        else
        {
            if (value && !lastFrameValue)
            {// player started press
                SetOrientation();
                UI.SetActive(true);
            }

            if (!value && lastFrameValue)
            {// player stopped press
                UI.SetActive(false);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


}
