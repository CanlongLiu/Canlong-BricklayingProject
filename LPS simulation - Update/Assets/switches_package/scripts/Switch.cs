using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GVRMUI
{
    public class Switch : MUI
    {
        private bool status = true;
        public GameObject pin; //the pin that will be flipped
        public AudioSource audioData; //sound

        //gets called when the player begins pressing the button
        public override void BeginPress(Transform controllerTransform)
        {//flip the switch
            if (status) status = false;
            else status = true;
            if (pin != null) //flip rotation 
                pin.transform.eulerAngles *= -1;
            //play sound
            if (audioData) audioData.Play();
        }
    }
}
