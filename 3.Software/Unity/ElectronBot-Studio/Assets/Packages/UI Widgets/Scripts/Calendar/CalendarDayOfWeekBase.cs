namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for Calendar day of week.
	/// </summary>
	public class CalendarDayOfWeekBase : MonoBehaviour
	{
		/// <summary>
		/// Date belongs to this calendar.
		/// </summary>
		[HideInInspector]
		public CalendarBase Calendar;

		/// <summary>
		/// Text component to display day of week.
		/// </summary>
		[SerializeField]
		protected TextAdapter dayAdapter;

		/// <summary>
		/// Text component to display day of week.
		/// </summary>
		public TextAdapter DayAdapter
		{
			get
			{
				return dayAdapter;
			}

			set
			{
				dayAdapter = value;

				DateChanged();
			}
		}

		/// <summary>
		/// Date.
		/// </summary>
		protected DateTime Date;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public virtual void SetDate(DateTime currentDate)
		{
			Date = currentDate;
			DateChanged();
		}

		/// <summary>
		/// Update displayed date.
		/// </summary>
		protected virtual void DateChanged()
		{
			DayAdapter.text = Date.ToString("ddd", Calendar.Culture);
		}

		/// <summary>
		/// Apply specified style.
		/// </summary>
		/// <param name="styleCalendar">Style for the calendar.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleCalendar styleCalendar, Style style)
		{
			if (DayAdapter != null)
			{
				styleCalendar.DayOfWeekText.ApplyTo(DayAdapter.gameObject);
			}

			styleCalendar.DayOfWeekBackground.ApplyTo(GetComponent<Image>());
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleCalendar">Style for the calendar.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleCalendar styleCalendar, Style style)
		{
			if (DayAdapter != null)
			{
				styleCalendar.DayOfWeekText.GetFrom(DayAdapter.gameObject);
			}

			styleCalendar.DayOfWeekBackground.GetFrom(GetComponent<Image>());
		}
	}
}