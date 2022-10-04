using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Lovatto.Countdown
{
    public class bl_TMPText : bl_CountdownText
    {
        public TextMeshProUGUI textInstance;

        public override string text { get => textInstance.text; set => textInstance.text = value; }

        public override void SetText(string text)
        {

        }
    }
}