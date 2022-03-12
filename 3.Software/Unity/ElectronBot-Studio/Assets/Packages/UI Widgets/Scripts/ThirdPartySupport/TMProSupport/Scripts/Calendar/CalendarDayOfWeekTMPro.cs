#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using System;
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// CalendarDayOfWeek TMPro.
	/// Display day of week.
	/// </summary>
	public class CalendarDayOfWeekTMPro : CalendarDayOfWeekBase, IUpgradeable
	{
		/// <summary>
		/// Text component to display day of week.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with DayAdapter.")]
		protected TextMeshProUGUI Day;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			Utilities.GetOrAddComponent(Day, ref dayAdapter);
#pragma warning restore 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}
#endif