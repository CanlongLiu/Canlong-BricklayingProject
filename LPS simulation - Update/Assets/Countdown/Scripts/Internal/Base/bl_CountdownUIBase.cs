using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.Countdown
{
    public abstract class bl_CountdownUIBase : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract void OnStartCount(bl_Countdown countdown);

        /// <summary>
        /// 
        /// </summary>
        public abstract void OnUpdateCount(int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countRaw"></param>
        public abstract void OnUpdateCountRaw(float countRaw);

        /// <summary>
        /// 
        /// </summary>
        public abstract void OnFinish();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string TimeFormatIfNeeded(int time)
        {
            if (time < 60) return time.ToString();

            if(time < 3600)
            {
                int seconds = time % 60;
                int minutes = time / 60;
                return $"{minutes}:{seconds}";
            }
            else
            {
                int seconds = time % 60;
                int minutes = time / 60;
                int hours = minutes / 60;
                return $"{hours}:{minutes}:{seconds}";
            }
        }

    }
}