#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// Calendar TMPro.
	/// </summary>
	public class CalendarTMPro : CalendarBase, IUpgradeable
	{
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with DateTextAdapter")]
		TextMeshProUGUI dateText;

		/// <summary>
		/// Text to display current date.
		/// </summary>
		[System.Obsolete("Replaced with DateTextAdapter")]
		public TextMeshProUGUI DateText
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
		[System.Obsolete("Replaced with MonthTextAdapter")]
		TextMeshProUGUI monthText;

		/// <summary>
		/// Text to display current month.
		/// </summary>
		[System.Obsolete("Replaced with DateTextAdapter")]
		public TextMeshProUGUI MonthText
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
#endif