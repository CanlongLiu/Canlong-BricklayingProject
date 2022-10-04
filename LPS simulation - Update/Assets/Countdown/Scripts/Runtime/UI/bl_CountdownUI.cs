using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lovatto.Countdown
{
    public class bl_CountdownUI : bl_CountdownUIBase
    {
        public CountDisplayNumber countDisplayNumber = CountDisplayNumber.Integer;
        public string replaceLastWith;
        public bool autoHideOnFinish = true;
        public float decimalsTextSize = 20;
        public string customFormat;
        [Header("Events")]
        public UEvent onCountStart;
        public UEventInt onCountChange;
        public UEvent onCountFinish;

        public bl_CountdownText[] countTexts;

        #region Private members
        [Serializable] public class UEvent : UnityEvent { }
        [Serializable] public class UEventInt : UnityEvent<int> { }
        private bl_Countdown Countdown;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override void OnStartCount(bl_Countdown countdown)
        {
            Countdown = countdown;
            var text = GetCountText(countdown.startTime);
            SetCountText(text);
            gameObject.SetActive(true);
            onCountStart?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        public override void OnUpdateCount(int count)
        {
            var text = GetCountText(count);
            if (countDisplayNumber == CountDisplayNumber.Integer)
            {            
                if (!string.IsNullOrEmpty(replaceLastWith) && Countdown != null && Countdown.IsLastValue(count))
                {
                    text = replaceLastWith;
                }
                SetCountText(text);
            }
            onCountChange?.Invoke(count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countRaw"></param>
        public override void OnUpdateCountRaw(float countRaw)
        {
            if (countDisplayNumber == CountDisplayNumber.OneDecimal)
                SetCountText(countRaw.ToString($"0<size={decimalsTextSize}>.0</size>"));
            else if(countDisplayNumber == CountDisplayNumber.TwoDecimal)
                SetCountText(countRaw.ToString($"0<size={decimalsTextSize}>.00</size>"));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnFinish()
        {
            onCountFinish?.Invoke();
            if (autoHideOnFinish) gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetCountText(string text)
        {
            if (countTexts == null || countTexts.Length <= 0) return;


            foreach (var cText in countTexts)
            {
                if (cText == null) continue;
                cText.text = text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string GetCountText(int count)
        {
            var text = TimeFormatIfNeeded(count);
            //if you want to show the count in a custom format
            if (!string.IsNullOrEmpty(customFormat))
            {
                text = string.Format(customFormat, count.ToString());
            }
            return text;
        }

        [Serializable]
        public enum CountDisplayNumber
        {
            Integer,
            OneDecimal,
            TwoDecimal,
        }
    }
}