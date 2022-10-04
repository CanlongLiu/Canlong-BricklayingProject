using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lovatto.Countdown
{
    public class bl_UGUIText : bl_CountdownText
    {
        public Text textInstance;

        public override string text { get => textInstance.text; set => textInstance.text = value; }

        public override void SetText(string text)
        {
           
        }
    }
}