namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for Calendar date.
	/// </summary>
	public class CalendarDateBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Text component to display day.
		/// </summary>
		[SerializeField]
		protected TextAdapter dayAdapter;

		/// <summary>
		/// Text component to display day.
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
		/// Image component to display day background.
		/// </summary>
		[SerializeField]
		protected Image DayImage;

		/// <summary>
		/// Selected date background.
		/// </summary>
		[SerializeField]
		public Sprite SelectedDayBackground;

		/// <summary>
		/// Selected date color.
		/// </summary>
		[SerializeField]
		public Color SelectedDay = Color.white;

		/// <summary>
		/// Default date background.
		/// </summary>
		[SerializeField]
		public Sprite DefaultDayBackground;

		/// <summary>
		/// Color for date in current month.
		/// </summary>
		[SerializeField]
		public Color CurrentMonth = Color.white;

		/// <summary>
		/// Weekend date color.
		/// </summary>
		[SerializeField]
		public Color Weekend = Color.red;

		/// <summary>
		/// Color for date not in current month.
		/// </summary>
		[SerializeField]
		public Color OtherMonth = Color.gray;

		/// <summary>
		/// Current date to display.
		/// </summary>
		protected DateTime CurrentDate;

		/// <summary>
		/// Date belongs to this calendar.
		/// </summary>
		[HideInInspector]
		public CalendarBase Calendar;

		/// <summary>
		/// Set current date.
		/// </summary>
		/// <param name="currentDate">Current date.</param>
		public virtual void SetDate(DateTime currentDate)
		{
			CurrentDate = currentDate;

			DateChanged();
		}

		/// <summary>
		/// Update displayed date.
		/// </summary>
		public virtual void DateChanged()
		{
			DayAdapter.text = CurrentDate.ToString("dd", Calendar.Culture);

			if (Calendar.IsSameDay(Calendar.Date, CurrentDate))
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

		/// <summary>
		/// OnPoiterDown event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// OnPointerUp event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// PointerClick event.
		/// Change calendar date to clicked date.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!Calendar.IsActive())
			{
				return;
			}

			if (CurrentDate < Calendar.DateMin)
			{
				return;
			}

			if (CurrentDate > Calendar.DateMax)
			{
				return;
			}

			Calendar.Date = CurrentDate;
			Calendar.OnDateClick.Invoke(CurrentDate);
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
				styleCalendar.DayText.ApplyTo(DayAdapter.gameObject);
			}

			styleCalendar.DayBackground.ApplyTo(DayImage);

			DefaultDayBackground = styleCalendar.DayBackground.Sprite;
			SelectedDayBackground = styleCalendar.SelectedDayBackground;

			SelectedDay = styleCalendar.ColorSelectedDay;
			Weekend = styleCalendar.ColorWeekend;

			CurrentMonth = styleCalendar.ColorCurrentMonth;
			OtherMonth = styleCalendar.ColorOtherMonth;

			if (Calendar != null)
			{
				DateChanged();
			}
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
				styleCalendar.DayText.GetFrom(DayAdapter.gameObject);
			}

			styleCalendar.DayBackground.GetFrom(DayImage);

			styleCalendar.DayBackground.Sprite = DefaultDayBackground;
			styleCalendar.SelectedDayBackground = SelectedDayBackground;

			styleCalendar.ColorSelectedDay = SelectedDay;
			styleCalendar.ColorWeekend = Weekend;

			styleCalendar.ColorCurrentMonth = CurrentMonth;
			styleCalendar.ColorOtherMonth = OtherMonth;
		}
	}
}