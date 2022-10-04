using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.Countdown
{
    public class bl_AnimationPlayRewind : MonoBehaviour
    {
        public string motionName;
        public string hideMotionName;
        public Animator animator;

        private bool firstValue = true;

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            if (animator == null) return;
            animator.Play(motionName, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlayHide()
        {
            if (animator == null) return;
            animator.Play(hideMotionName, 0, 0);
            Invoke(nameof(Disable), animator.GetCurrentAnimatorStateInfo(0).length);
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlayToggled()
        {
            if (animator == null) return;
            var name = firstValue ? motionName : hideMotionName;
            animator.Play(name, 0, 0);
            firstValue = !firstValue;
        }

        /// <summary>
        /// 
        /// </summary>
        void Disable() => gameObject.SetActive(false);
    }
}