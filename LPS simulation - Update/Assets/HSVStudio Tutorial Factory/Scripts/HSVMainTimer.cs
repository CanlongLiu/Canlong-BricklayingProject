using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSVStudio.Tutorial
{
	/// <summary>
	/// This is used to do timing actions call
	/// </summary>
	public class HSVMainTimer : MonoBehaviour
	{
		public static HSVMainTimer time;

		private List<Timer> timers;

		private List<int> removalPending;

		private int idCounter;


		private class Timer
		{
			public int id;
			public bool isActive;

			public float rate;
			public int ticks;
			public int ticksElapsed;
			public float last;
			public Action callBack;

			public Timer(int id_, float rate_, int ticks_, Action callback_)
			{
				id = id_;
				rate = rate_ < 0 ? 0 : rate_;
				ticks = ticks_ < 0 ? 0 : ticks_;
				callBack = callback_;
				last = 0;
				ticksElapsed = 0;
				isActive = true;
			}

			public void Tick()
			{
				last += Time.deltaTime;

				if (isActive && last >= rate)
				{
					last = 0;
					ticksElapsed++;
					callBack.Invoke();

					if (ticks > 0 && ticks == ticksElapsed)
					{
						isActive = false;
						HSVMainTimer.time.RemoveTimer(id);
					}
				}
			}
		}

		private void Awake()
		{
			time = this;
			timers = new List<Timer>();
			removalPending = new List<int>();
		}

		/// <summary>
		/// Creates new timer
		/// </summary>
		/// <param name="rate">Tick rate</param>
		/// <param name="callBack">Callback method</param>
		/// <returns>Time GUID</returns>
		public int AddTimer(float rate, Action callBack)
		{
			return AddTimer(rate, 0, callBack);
		}

		/// <summary>
		/// Creates new timer
		/// </summary>
		/// <param name="rate">Tick rate</param>
		/// <param name="ticks">Number of ticks before timer removal</param>
		/// <param name="callBack">Callback method</param>
		/// <returns>Timer GUID</returns>
		public int AddTimer(float rate, int ticks, Action callBack)
		{
            //Debug.Log("Adding Timer " + rate + " tick: " + ticks);
            Timer newTimer = new Timer(++idCounter, rate, ticks, callBack);
			timers.Add(newTimer);
			return newTimer.id;
		}

		/// <summary>
		/// Removes timer
		/// </summary>
		/// <param name="timerId">Timer GUID</param>
		public void RemoveTimer(int timerId)
		{
			if (!removalPending.Contains(timerId))
			{
				removalPending.Add(timerId);
			}
		}

		/// <summary>
		/// Timer removal queue handler
		/// </summary>
		private void Remove()
		{
			if (removalPending.Count > 0)
			{
				foreach (int id in removalPending)
					for (int i = 0; i < timers.Count; i++)
						if (timers[i].id == id)
						{
							timers.RemoveAt(i);
							break;
						}

				removalPending.Clear();
			}
		}

		/// <summary>
		/// Updates timers
		/// </summary>
		private void Tick()
		{
			for (int i = 0; i < timers.Count; i++)
				timers[i].Tick();
		}

		// Update is called once per frame
		private void Update()
		{
			Remove();
			Tick();
		}
	}
}