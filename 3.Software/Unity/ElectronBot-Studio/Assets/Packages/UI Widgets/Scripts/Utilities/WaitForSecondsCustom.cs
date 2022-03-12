namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Extensions;
	using UnityEngine;

	/// <summary>
	/// Suspends the coroutine execution for the given amount of seconds.
	/// </summary>
	public class WaitForSecondsCustom : CustomYieldInstruction
	{
		static List<WaitForSecondsCustom> Cache = new List<WaitForSecondsCustom>();

		/// <summary>
		/// The given amount of seconds that the yield instruction will wait for.
		/// </summary>
		public float Seconds
		{
			get;
			protected set;
		}

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		public bool UnscaledTime
		{
			get;
			protected set;
		}

		private bool cached = false;

		private float endTime = -1f;

		/// <summary>
		/// Indicates if coroutine should be kept suspended.
		/// </summary>
		public override bool keepWaiting
		{
			get
			{
				var current_time = UtilitiesTime.GetTime(UnscaledTime);
				if (endTime < 0)
				{
					endTime = current_time + Seconds;
				}

				var waiting = current_time < endTime;
				if (!waiting)
				{
					ToCache();
				}

				return waiting;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WaitForSecondsCustom"/> class.
		/// Creates a yield instruction to wait for a given number of seconds using unscaled time.
		/// </summary>
		/// <param name="seconds">Seconds to wait.</param>
		/// <param name="unscaledTime">Unscaled time.</param>
		private WaitForSecondsCustom(float seconds, bool unscaledTime = false)
		{
			Seconds = seconds;
			UnscaledTime = unscaledTime;
		}

		/// <summary>
		/// Get instance.
		/// </summary>
		/// <param name="seconds">Seconds to wait.</param>
		/// <param name="unscaledTime">Unscaled time.</param>
		/// <returns>Instance.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		public static WaitForSecondsCustom Instance(float seconds, bool unscaledTime = false)
		{
			if (Cache.Count > 0)
			{
				var result = Cache.Pop();
				result.Seconds = seconds;
				result.UnscaledTime = unscaledTime;
				result.cached = false;

				return result;
			}

			return new WaitForSecondsCustom(seconds, unscaledTime);
		}

		private void ToCache()
		{
			if (cached)
			{
				return;
			}

			Cache.Add(this);
			cached = true;

			endTime = -1f;
		}

		#if UNITY_2020_1_OR_NEWER
		/// <summary>
		/// Reset.
		/// </summary>
		public override void Reset()
		{
			ToCache();
		}
		#endif
	}
}