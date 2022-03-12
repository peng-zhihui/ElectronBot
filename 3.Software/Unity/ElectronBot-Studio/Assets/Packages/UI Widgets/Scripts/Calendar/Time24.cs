namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Time widget for 24 hour format.
	/// </summary>
	public class Time24 : TimeSpinnerBase
	{
		/// <summary>
		/// The input field for the hours.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with InputHoursAdapter")]
		protected InputField InputHours;

		/// <summary>
		/// The input field for the minutes.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with InputMinutesAdapter")]
		protected InputField InputMinutes;

		/// <summary>
		/// The input field for the seconds.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with InputSecondsAdapter")]
		protected InputField InputSeconds;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0618
			Utilities.GetOrAddComponent(InputHours, ref InputHoursAdapter);
			Utilities.GetOrAddComponent(InputMinutes, ref InputMinutesAdapter);
			Utilities.GetOrAddComponent(InputSeconds, ref InputSecondsAdapter);
#pragma warning restore 0618
		}
	}
}