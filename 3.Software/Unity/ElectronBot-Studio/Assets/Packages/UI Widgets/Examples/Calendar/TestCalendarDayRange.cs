namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test calendar with dates range.
	/// </summary>
	public class TestCalendarDayRange : CalendarDate // or CalendarDateTMPro if need TMPro support
	{
		/// <summary>
		/// The range.
		/// </summary>
		[SerializeField]
		public TestDayRangeSource Range;

		/// <summary>
		/// The default background for start date.
		/// </summary>
		[SerializeField]
		public Sprite DateStartDefault;

		/// <summary>
		/// The backdgound for start date if start date is last day of week.
		/// </summary>
		[SerializeField]
		public Sprite DateStartLastDayOfWeek;

		/// <summary>
		/// The default background for date in range.
		/// </summary>
		[SerializeField]
		public Sprite DateRangeDefault;

		/// <summary>
		/// The default background for date in range if date is first day of week.
		/// </summary>
		[SerializeField]
		public Sprite DateRangeFirstDayOfWeek;

		/// <summary>
		/// The default background for date in range if date is last day of week.
		/// </summary>
		[SerializeField]
		public Sprite DateRangeLastDayOfWeek;

		/// <summary>
		/// The default background for end date.
		/// </summary>
		[SerializeField]
		public Sprite DateEndDefault;

		/// <summary>
		/// The backdgound for end date if end date is last day of week.
		/// </summary>
		[SerializeField]
		public Sprite DateEndLastDayOfWeek;

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public override void DateChanged()
		{
			// set default text, sprites and colors
			base.DateChanged();

			var is_last_day = CurrentDate.DayOfWeek == GetLastDayOfWeek();
			var is_first_day = CurrentDate.DayOfWeek == Calendar.FirstDayOfWeek;
			if (Calendar.IsSameDay(Range.DateStart, CurrentDate))
			{
				DayImage.sprite = is_last_day ? DateStartLastDayOfWeek : DateStartDefault;
			}
			else if (Calendar.IsSameDay(Range.DateEnd, CurrentDate))
			{
				DayImage.sprite = is_last_day ? DateEndLastDayOfWeek : DateEndDefault;
			}
			else if ((Range.DateStart < CurrentDate) && (CurrentDate < Range.DateEnd))
			{
				if (is_first_day)
				{
					DayImage.sprite = DateRangeFirstDayOfWeek;
				}
				else if (is_last_day)
				{
					DayImage.sprite = DateRangeLastDayOfWeek;
				}
				else
				{
					DayImage.sprite = DateRangeDefault;
				}
			}

			DayImage.color = Color.white;
		}

		DayOfWeek GetLastDayOfWeek()
		{
			var i = (int)Calendar.FirstDayOfWeek;
			i = i == 0 ? 6 : i - 1;
			return (DayOfWeek)i;
		}
	}
}