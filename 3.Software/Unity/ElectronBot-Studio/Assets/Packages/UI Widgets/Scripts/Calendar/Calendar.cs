namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Calendar.
	/// </summary>
	public class Calendar : CalendarBase, IUpgradeable
	{
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with DateTextAdapter")]
		Text dateText;

		/// <summary>
		/// Text to display the current date.
		/// </summary>
		[Obsolete("Replaced with DateTextAdapter")]
		public Text DateText
		{
			get
			{
				return dateText;
			}

			set
			{
				dateText = value;

				UpdateDate();
			}
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with MonthTextAdapter")]
		Text monthText;

		/// <summary>
		/// Text to display the current month.
		/// </summary>
		[Obsolete("Replaced with MonthTextAdapter")]
		public Text MonthText
		{
			get
			{
				return monthText;
			}

			set
			{
				monthText = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			Utilities.GetOrAddComponent(dateText, ref dateTextAdapter);
			Utilities.GetOrAddComponent(monthText, ref monthTextAdapter);
#pragma warning restore 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			Compatibility.Upgrade(this);
		}
#endif
	}
}