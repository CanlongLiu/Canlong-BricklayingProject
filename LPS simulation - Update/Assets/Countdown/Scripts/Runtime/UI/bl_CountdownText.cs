using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lovatto.Countdown
{
    public abstract class bl_CountdownText : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public abstract void SetText(string text);
    }
}