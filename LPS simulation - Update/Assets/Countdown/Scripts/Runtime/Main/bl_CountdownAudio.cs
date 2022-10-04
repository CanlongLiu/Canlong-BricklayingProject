using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lovatto.Countdown
{
    public class bl_CountdownAudio : MonoBehaviour
    {
        public bl_Countdown countdown;
        [Range(0, 1)] public float volume = 0.7f;
        public AudioClip startSound;
        public AudioClip countSound;
        public AudioClip finishSound;
        public AnimationCurve countPitchEffector = AnimationCurve.Linear(0, 1, 1, 1);

        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            if (countdown == null) return;
            countdown.m_onCountStart.AddListener(OnStartCount);
            countdown.m_onCountChange.AddListener(OnCount);
            if (countdown.finishDelay <= 0)
                countdown.m_onCountFinish.AddListener(OnCountFinish);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            if (countdown == null) return;
            countdown.m_onCountStart.RemoveListener(OnStartCount);
            countdown.m_onCountChange.RemoveListener(OnCount);
            if(countdown.finishDelay <= 0)
            countdown.m_onCountFinish.RemoveListener(OnCountFinish);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnStartCount()
        {
            if(startSound != null) audioSource.pitch = 1;
            PlayClip(startSound);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnCount(int count)
        {
            if (countdown != null)
            {
                float progress = 1 - ((float)count / (float)countdown.startTime);
                if (count <= 0) progress = 1;

                audioSource.pitch = countPitchEffector.Evaluate(progress);
            }
            if (count <= countdown.finishTime && finishSound != null)
            {
                if (countdown.finishDelay <= 0) return;
                OnCountFinish();
                return;
            }
            PlayClip(countSound);
        }

        /// <summary>
        /// 
        /// </summary>
        void OnCountFinish()
        {
            if(finishSound != null) audioSource.pitch = 1;
            PlayClip(finishSound);
        }

        /// <summary>
        /// 
        /// </summary>
        void PlayClip(AudioClip clip)
        {
            if (clip == null) return;

            audioSource.volume = volume;
            audioSource.clip = clip;
            audioSource.Play();
        }

        private AudioSource m_source;
        private AudioSource audioSource
        {
            get
            {
                if(m_source == null)
                {
                    m_source = gameObject.AddComponent<AudioSource>();
                    m_source.playOnAwake = false;
                    m_source.spatialBlend = 0;
                }
                return m_source;
            }
        }
    }
}