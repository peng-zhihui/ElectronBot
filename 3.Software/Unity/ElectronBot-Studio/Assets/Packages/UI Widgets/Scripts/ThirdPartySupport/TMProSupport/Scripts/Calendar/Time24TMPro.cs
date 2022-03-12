#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using System;
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// Time widget for 24 hour format.
	/// </summary>
	public class Time24TMPro : TimeSpinnerBase
	{
		/// <summary>
		/// The input field for the hours.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with AMPMTextAdapter")]
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		protected TMP_InputField InputHours;
#else
		protected UnityEngine.UI.InputField InputHours;
#endif

		/// <summary>
		/// The input field for the minutes.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with AMPMTextAdapter")]
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		protected TMP_InputField InputMinutes;
#else
		protected UnityEngine.UI.InputField InputMinutes;
#endif

		/// <summary>
		/// The input field for the seconds.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with AMPMTextAdapter")]
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		protected TMP_InputField InputSeconds;
#else
		protected UnityEngine.UI.InputField InputSeconds;
#endif

		/// <summary>
		/// Init the input.
		/// </summary>
		protected override void InitInput()
		{
			Upgrade();
		}

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
#endif