namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the calendar.
	/// </summary>
	[Serializable]
	public class StyleCalendar : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the current date.
		/// </summary>
		[SerializeField]
		public StyleText CurrentDate;

		/// <summary>
		/// Style for the current month.
		/// </summary>
		[SerializeField]
		public StyleText CurrentMonth;

		/// <summary>
		/// Style for the previous month.
		/// </summary>
		[SerializeField]
		public StyleImage PrevMonth;

		/// <summary>
		/// Style for the next month.
		/// </summary>
		[SerializeField]
		public StyleImage NextMonth;

		/// <summary>
		/// Style for the day of week background.
		/// </summary>
		[SerializeField]
		public StyleImage DayOfWeekBackground;

		/// <summary>
		/// Style for the day of week text.
		/// </summary>
		[SerializeField]
		public StyleText DayOfWeekText;

		/// <summary>
		/// Style for the days background.
		/// </summary>
		[SerializeField]
		public StyleImage DaysBackground;

		/// <summary>
		/// Style for the day background.
		/// </summary>
		[SerializeField]
		public StyleImage DayBackground;

		/// <summary>
		/// Style for the selected day background.
		/// </summary>
		[SerializeField]
		public Sprite SelectedDayBackground;

		/// <summary>
		/// Style for the day text.
		/// </summary>
		[SerializeField]
		public StyleText DayText;

		/// <summary>
		/// The color for the selected day.
		/// </summary>
		[SerializeField]
		public Color ColorSelectedDay = Color.white;

		/// <summary>
		/// The color for the weekend.
		/// </summary>
		[SerializeField]
		public Color ColorWeekend = Color.red;

		/// <summary>
		/// The color for the current month.
		/// </summary>
		[SerializeField]
		public Color ColorCurrentMonth = Color.white;

		/// <summary>
		/// The color for the other month.
		/// </summary>
		[SerializeField]
		public Color ColorOtherMonth = Color.gray;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			CurrentDate.SetDefaultValues();
			CurrentMonth.SetDefaultValues();
			PrevMonth.SetDefaultValues();
			NextMonth.SetDefaultValues();
			DayOfWeekBackground.SetDefaultValues();
			DayOfWeekText.SetDefaultValues();
			DaysBackground.SetDefaultValues();
			DayBackground.SetDefaultValues();
			DayText.SetDefaultValues();
		}
#endif
	}
}