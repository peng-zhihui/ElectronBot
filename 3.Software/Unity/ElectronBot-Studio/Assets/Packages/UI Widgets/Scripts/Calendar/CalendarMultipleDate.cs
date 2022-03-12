namespace UIWidgets
{
	/// <summary>
	/// CalendarMultipleDate.
	/// Display date.
	/// </summary>
	public class CalendarMultipleDate : CalendarDate
	{
		/// <summary>
		/// CalendarMultipleDates.
		/// </summary>
		public CalendarMultipleDates Dates;

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public override void DateChanged()
		{
			DayAdapter.text = CurrentDate.ToString("dd", Calendar.Culture);

			if (Dates.IsSelected(CurrentDate))
			{
				DayAdapter.color = SelectedDay;
				DayImage.sprite = SelectedDayBackground;
			}
			else
			{
				DayImage.sprite = DefaultDayBackground;

				if (Calendar.IsSameMonth(Calendar.DateDisplay, CurrentDate))
				{
					if (Calendar.IsWeekend(CurrentDate) ||
						Calendar.IsHoliday(CurrentDate))
					{
						DayAdapter.color = Weekend;
					}
					else
					{
						DayAdapter.color = CurrentMonth;
					}
				}
				else
				{
					if (Calendar.IsWeekend(CurrentDate) ||
						Calendar.IsHoliday(CurrentDate))
					{
						DayAdapter.color = Weekend * OtherMonth;
					}
					else
					{
						DayAdapter.color = OtherMonth;
					}
				}

				if (CurrentDate < Calendar.DateMin)
				{
					DayAdapter.color *= OtherMonth;
				}
				else if (CurrentDate > Calendar.DateMax)
				{
					DayAdapter.color *= OtherMonth;
				}
			}
		}
	}
}