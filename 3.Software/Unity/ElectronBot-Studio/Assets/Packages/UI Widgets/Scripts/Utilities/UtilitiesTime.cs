namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Time functions.
	/// </summary>
	public static class UtilitiesTime
	{
		/// <summary>
		/// Function to get time to use with animations.
		/// Can be replaced with custom function.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public static Func<bool, float> GetTime = DefaultGetTime;

		/// <summary>
		/// Function to get delta time to use with animations.
		/// Can be replaced with custom function.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public static Func<bool, float> GetDeltaTime = DefaultGetDeltaTime;

		/// <summary>
		/// Function to get frame count.
		/// Can be replaced with custom function.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public static Func<int> GetFrameCount = DefaultGetFrameCount;

		/// <summary>
		/// Suspends the coroutine execution for the given amount of seconds.
		/// Can be replaced with custom function.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public static Func<float, bool, CustomYieldInstruction> Wait = DefaultWait;

		#if UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void Init()
		{
			GetTime = DefaultGetTime;
			GetDeltaTime = DefaultGetDeltaTime;
			GetFrameCount = DefaultGetFrameCount;
			Wait = DefaultWait;
		}
		#endif

		/// <summary>
		/// Default GetFrameCount function from the default Time class.
		/// </summary>
		/// <returns>Frame count.</returns>
		public static int DefaultGetFrameCount()
		{
			return Time.frameCount;
		}

		/// <summary>
		/// Default GetTime function from the default Time class.
		/// </summary>
		/// <param name="unscaledTime">Return unscaled time.</param>
		/// <returns>Time.</returns>
		public static float DefaultGetTime(bool unscaledTime)
		{
			return unscaledTime ? Time.unscaledTime : Time.time;
		}

		/// <summary>
		/// Suspends the coroutine execution for the given amount of seconds.
		/// </summary>
		/// <param name="seconds">Seconds.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <returns>Coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		public static CustomYieldInstruction DefaultWait(float seconds, bool unscaledTime = false)
		{
			return WaitForSecondsCustom.Instance(seconds, unscaledTime);
		}

		/// <summary>
		/// Default GetUnscaledTime function.
		/// </summary>
		/// <returns>Default Time.unscaledTime.</returns>
		public static float DefaultGetUnscaledTime()
		{
			return Time.unscaledTime;
		}

		/// <summary>
		/// Default GetDeltaTime function from the default Time class.
		/// </summary>
		/// <param name="unscaledTime">Return unscaled delta time.</param>
		/// <returns>Delta Time.</returns>
		public static float DefaultGetDeltaTime(bool unscaledTime)
		{
			return unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
		}

		/// <summary>
		/// Check how much time takes to run specified action.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="log">Text to add to log.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
		public static void CheckRunTime(Action action, string log = "")
		{
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			action();

			sw.Stop();
			var ts = sw.Elapsed;

			string result = string.Format(
				"RunTime {0}:{1}:{2}.{3}; {4}",
				ts.Hours.ToString("00"),
				ts.Minutes.ToString("00"),
				ts.Seconds.ToString("00"),
				(ts.Milliseconds / 10).ToString("0000"),
				log);
			Debug.Log(result);
		}
	}
}