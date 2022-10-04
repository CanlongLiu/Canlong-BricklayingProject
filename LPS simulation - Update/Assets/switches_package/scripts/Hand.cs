using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GVRMUI
{
    public class Hand : MonoBehaviour
    {
        private MUI mui;

        public XRNode node;
        public InputDevice device;

        bool lastTriggerState = false;

        //its important that it is a fixed update. Update is called multiple times so "justPressed" will not always work
        void FixedUpdate()
        {

            if (!device.isValid)
            {//device cannot give input
                device = InputDevices.GetDeviceAtXRNode(node);
                if (!device.isValid) return; //this happens if for example the controller is turned off
            }

            bool triggerValue = false;
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue))
            {
                //was able to get input from device, so sending the result to input function to handle
                Input(triggerValue);
            }

            lastTriggerState = triggerValue;
        }

        //this is extracted for cleaner input handling
        void Input(bool triggerValue)
        {
            if (!lastTriggerState && triggerValue)
            {//just pressed
                if(mui) mui.BeginPress(gameObject.transform);
            }

            if (lastTriggerState && triggerValue)
            {//holding
                if (mui) mui.Press(gameObject.transform);
            }

            if (lastTriggerState && !triggerValue)
            {//just released
                if (mui) mui.StopPress(gameObject.transform);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (mui) return; //already touching an MUI

            MUI otherMUI = other.GetComponent<MUI>();
            if (otherMUI)
            {
                Debug.Log("touched");
                mui = otherMUI;
                mui.BeginTouch(gameObject.transform);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(mui)
                mui.Touch(gameObject.transform);
        }

        void OnTriggerExit(Collider other)
        {
            if (mui)
            {
                MUI otherMUI = other.GetComponent<MUI>();
                //is it the same that is already registered?
                if(otherMUI == mui)
                {
                    mui.StopTouch(gameObject.transform);
                    mui = null;
                }
            }
        }

    }

}
