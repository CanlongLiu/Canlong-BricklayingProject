using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lovatto.Countdown
{
    public class bl_Countdown : MonoBehaviour
    {
        #region Public members      
        public int startTime = 10;
        public int finishTime = 0;
        public float countSpeed = 1;
        public float startDelay = 0;
        public float finishDelay = 0;

        [Header("Events")]
        public UEvent m_onCountStart;
        public UEventInt m_onCountChange;
        public UEvent m_onCountFinish;
        public CountdownEvent[] countEvents;

        [Header("References")]
        public bl_CountdownUIBase countdownUI;
        #endregion

        #region Public properties
        public bool IsCounting { get; set; } = false;
        public int CurrentCountValue { get; set; } = 0;

        #endregion

        #region Private members
        [Serializable]public class UEvent : UnityEvent { }
        [Serializable] public class UEventInt : UnityEvent<int> { }
        private Action tempFinishEvents, tempStartEvents;
        private Action<int> tempCountEvent;
        private List<CountdownEvent> tempCountAtEvents = new List<CountdownEvent>();
        #endregion

        /// <summary>
        /// Start the count down from the default start time
        /// </summary>
        public bl_Countdown StartCountdown() => StartCountdown(startTime);

        /// <summary>
        /// Start the countdown from the given start time
        /// </summary>
        /// <param name="startFrom"></param>
        public bl_Countdown StartCountdown(int startFrom)
        {
            if (IsCounting) return this;

            StopAllCoroutines();
            StartCoroutine(DoCountdown(startFrom));
            return this;
        }

        /// <summary>
        /// Cancel the current countdown and hide the UI
        /// </summary>
        public void CancelCountdown()
        {
            StopAllCoroutines();
            IsCounting = false;
            if (countdownUI != null) countdownUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// All the logic of the countdown is executed in this coroutine
        /// </summary>
        /// <returns></returns>
        IEnumerator DoCountdown(int startFrom)
        {
            IsCounting = true;
            if (startDelay > 0) yield return new WaitForSeconds(startDelay);

            int count = startFrom;
            int start = startFrom - finishTime;
            float d = 0;
            CurrentCountValue = startFrom;
            if (count != CurrentCountValue)
            {
                countdownUI?.OnUpdateCount(CurrentCountValue);
                m_onCountChange?.Invoke(CurrentCountValue);
            }
            ResetAllEvents();
            countdownUI?.OnStartCount(this);
            m_onCountStart?.Invoke();
            tempStartEvents?.Invoke();

            while(d < 1)
            {
                d += (Time.deltaTime / start) * countSpeed;
                float progress = start * (1 - d);
                progress = Mathf.Clamp(progress, finishTime, startFrom);
                CurrentCountValue = Mathf.CeilToInt(progress) + finishTime;
                countdownUI?.OnUpdateCountRaw(progress + finishTime);

                if(count != CurrentCountValue)
                {
                    countdownUI?.OnUpdateCount(CurrentCountValue);
                    m_onCountChange?.Invoke(CurrentCountValue);
                    tempCountEvent?.Invoke(CurrentCountValue);
                    CheckEventsForTime(CurrentCountValue);
                    count = CurrentCountValue;
                }
                yield return null;
            }
            if(finishDelay > 0) yield return new WaitForSeconds(finishDelay);

            m_onCountFinish?.Invoke();
            tempFinishEvents?.Invoke();
            countdownUI?.OnFinish();
            IsCounting = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void CheckEventsForTime(int time)
        {
            foreach (var data in tempCountAtEvents) data.Check(time);
            if (countEvents == null || countEvents.Length <= 0) return;

            foreach (var data in countEvents) data.Check(time);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResetAllEvents()
        {
            tempFinishEvents = tempStartEvents = null;
            tempCountEvent = null;
            tempCountAtEvents.Clear();
            if (countEvents == null || countEvents.Length <= 0) return;

            foreach (var data in countEvents) data.Reset();
        }

        /// <summary>
        /// Start the count down from the default start time
        /// </summary>
        public void ButtonStartCountdown() => StartCountdown(startTime);

        /// <summary>
        /// Start the countdown from the given start time
        /// </summary>
        public void ButtonStartCountdown(int start) => StartCountdown(start);

        /// <summary>
        /// Add a callback to be invoked when the countdown start
        /// </summary>
        public bl_Countdown OnStartCount(Action callback)
        {
            tempStartEvents += callback;
            return this;
        }

        /// <summary>
        /// Add a callback to be invoked once the countdown finish
        /// </summary>
        public bl_Countdown OnFinishCount(Action callback)
        {
            tempFinishEvents += callback;
            return this;
        }

        /// <summary>
        /// Add a callback to be invoked each time that the count change
        /// </summary>
        public bl_Countdown OnCountChange(Action<int> callback)
        {
            tempCountEvent += callback;
            return this;
        }

        /// <summary>
        /// Add a callback to be called when the count reached a specific second
        /// </summary>
        public bl_Countdown OnCountAt(int second, UnityAction callback)
        {
            var ev = new CountdownEvent()
            {
                TriggerTime = second,
            };
            ev.onTrigger.AddListener(callback);

            tempCountAtEvents.Add(ev);
            return this;
        }

        /// <summary>
        /// Is the given value the end of the countdown?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsLastValue(int value) => value == finishTime;

        [Serializable]
        public class CountdownEvent
        {
            [TextArea(2,5)] public string Description;
            public int TriggerTime = 1;
            public UEvent onTrigger;

            private bool triggered = false;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="time"></param>
            public void Check(int time)
            {
                if (triggered || time != TriggerTime) return;

                onTrigger?.Invoke();
                triggered = true;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Reset() => triggered = false;
        }
    }
}