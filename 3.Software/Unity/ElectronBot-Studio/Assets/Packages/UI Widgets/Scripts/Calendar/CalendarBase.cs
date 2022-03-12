namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Calendar base class.
	/// </summary>
	[DataBindSupport]
	public class CalendarBase : DateBase
	{
		[SerializeField]
		DayOfWeek firstDayOfWeek;

		/// <summary>
		/// First day of the week.
		/// </summary>
		public DayOfWeek FirstDayOfWeek
		{
			get
			{
				return firstDayOfWeek;
			}

			set
			{
				firstDayOfWeek = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Container for the dates.
		/// </summary>
		[SerializeField]
		public RectTransform Container;

		[SerializeField]
		CalendarDateBase calendarDateTemplate;

		/// <summary>
		/// Date template.
		/// </summary>
		public CalendarDateBase CalendarDateTemplate
		{
			get
			{
				return calendarDateTemplate;
			}

			set
			{
				calendarDateTemplate = value;

				Init();
				UpdateCalendar();
			}
		}

		/// <summary>
		/// Container for the day of weeks.
		/// </summary>
		[SerializeField]
		public RectTransform HeaderContainer;

		[SerializeField]
		CalendarDayOfWeekBase calendarDayOfWeekTemplate;

		/// <summary>
		/// Day of week template.
		/// </summary>
		public CalendarDayOfWeekBase CalendarDayOfWeekTemplate
		{
			get
			{
				return calendarDayOfWeekTemplate;
			}

			set
			{
				calendarDayOfWeekTemplate = value;

				InitHeader();

				UpdateCalendar();
			}
		}

		DateTime dateDisplay = DateTime.Today;

		/// <summary>
		/// Displayed month in calendar.
		/// </summary>
		public DateTime DateDisplay
		{
			get
			{
				return dateDisplay;
			}

			set
			{
				dateDisplay = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Days in week.
		/// </summary>
		protected int DaysInWeek = 7;

		/// <summary>
		/// Displayed weeks count.
		/// </summary>
		protected int DisplayedWeeks = 6;

		[NonSerialized]
		bool isInitedCalendarBase;

		EasyLayout layout;

		/// <summary>
		/// Container layout.
		/// </summary>
		public EasyLayout Layout
		{
			get
			{
				if (layout == null)
				{
					layout = Container.GetComponent<EasyLayout>();
				}

				return layout;
			}
		}

		/// <summary>
		/// Cache for instantiated days components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<CalendarDateBase> Cache = new List<CalendarDateBase>();

		/// <summary>
		/// Displayed days components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<CalendarDateBase> Days = new List<CalendarDateBase>();

		/// <summary>
		/// Displayed days of week components.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<CalendarDayOfWeekBase> Header = new List<CalendarDayOfWeekBase>();

		/// <summary>
		/// Text to display the current date.
		/// </summary>
		[SerializeField]
		protected TextAdapter dateTextAdapter;

		/// <summary>
		/// Text to display the current date.
		/// </summary>
		public TextAdapter DateTextAdapter
		{
			get
			{
				return dateTextAdapter;
			}

			set
			{
				dateTextAdapter = value;

				UpdateDate();
			}
		}

		/// <summary>
		/// Text to display the current month.
		/// </summary>
		[SerializeField]
		protected TextAdapter monthTextAdapter;

		/// <summary>
		/// Text to display the current month.
		/// </summary>
		public TextAdapter MonthTextAdapter
		{
			get
			{
				return monthTextAdapter;
			}

			set
			{
				monthTextAdapter = value;

				UpdateCalendar();
			}
		}

		/// <summary>
		/// Full initialization of this instance.
		/// </summary>
		public override void Init()
		{
			if (isInitedCalendarBase)
			{
				return;
			}

			isInitedCalendarBase = true;
			dateDisplay = Date;

			base.Init();

			InitHeader();
		}

		/// <summary>
		/// Set date.
		/// </summary>
		/// <param name="value">value.</param>
		protected override void SetDate(DateTime value)
		{
			base.SetDate(value);
			DateDisplay = Date;
		}

		/// <summary>
		/// Init.
		/// </summary>
		protected override void InitNestedWidgets()
		{
			calendarDateTemplate.gameObject.SetActive(false);

			Layout.LayoutType = LayoutTypes.Grid;
			Layout.MainAxis = Axis.Horizontal;
			Layout.GridConstraint = GridConstraints.FixedColumnCount;
			Layout.GridConstraintCount = DaysInWeek;

			foreach (var c in Cache)
			{
				DestroyGameObject(c);
			}

			Cache.Clear();

			foreach (var d in Days)
			{
				DestroyGameObject(d);
			}

			Days.Clear();
		}

		/// <summary>
		/// Destroy gameobject with the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DestroyGameObject(Component component)
		{
			Destroy(component.gameObject);
		}

		/// <summary>
		/// init header.
		/// </summary>
		protected virtual void InitHeader()
		{
			calendarDayOfWeekTemplate.gameObject.SetActive(false);

			foreach (var h in Header)
			{
				DestroyGameObject(h);
			}

			Header.Clear();
		}

		/// <summary>
		/// Displayed days count for specified month.
		/// </summary>
		/// <param name="displayedMonth">Displayed month.</param>
		/// <returns>Displayed days count.</returns>
		protected virtual int GetMaxDisplayedDays(DateTime displayedMonth)
		{
			return DaysInWeek * DisplayedWeeks;
		}

		/// <summary>
		/// Updates the calendar.
		/// Should be called only after changing culture settings.
		/// </summary>
		public override void UpdateCalendar()
		{
			Init();

			UpdateHeader();
			UpdateDays();
			UpdateDate();
		}

		/// <summary>
		/// Update displayed date and month.
		/// </summary>
		protected override void UpdateDate()
		{
			if (dateTextAdapter != null)
			{
				dateTextAdapter.text = Date.ToString("D", Culture);
			}

			if (monthTextAdapter != null)
			{
				monthTextAdapter.text = DateDisplay.ToString("Y", Culture);
			}
		}

		/// <summary>
		/// Update header.
		/// </summary>
		protected virtual void UpdateHeader()
		{
			if (CalendarDayOfWeekTemplate == null)
			{
				return;
			}

			GenerateHeader();

			var firstDay = GetFirstBlockDay(dateDisplay);
			for (int i = 0; i < DaysInWeek; i++)
			{
				Header[i].SetDate(firstDay.AddDays(i));
			}
		}

		/// <summary>
		/// Update days.
		/// </summary>
		protected virtual void UpdateDays()
		{
			GenerateDays(dateDisplay);

			var n = GetMaxDisplayedDays(dateDisplay);
			var firstDay = GetFirstBlockDay(dateDisplay);
			for (int i = 0; i < n; i++)
			{
				Days[i].SetDate(firstDay.AddDays(i));
			}
		}

		/// <summary>
		/// Get first day for displayed month.
		/// </summary>
		/// <param name="day">Displayed month.</param>
		/// <returns>First day for displayed month.</returns>
		protected virtual DateTime GetFirstBlockDay(DateTime day)
		{
			var first = day.AddDays(-Culture.DateTimeFormat.Calendar.GetDayOfMonth(day) + 1);

			while (Culture.DateTimeFormat.Calendar.GetDayOfWeek(first) != FirstDayOfWeek)
			{
				first = first.AddDays(-1);
			}

			return first;
		}

		/// <summary>
		/// Instantiated date component from template.
		/// </summary>
		/// <returns>Date component.</returns>
		protected virtual CalendarDateBase GenerateDay()
		{
			CalendarDateBase day;

			if (Cache.Count > 0)
			{
				day = Cache.Pop();
			}
			else
			{
				day = Compatibility.Instantiate(CalendarDateTemplate);
				day.transform.SetParent(Container, false);
				Utilities.FixInstantiated(CalendarDateTemplate, day);

				day.Calendar = this;
			}

			day.gameObject.SetActive(true);
			day.transform.SetAsLastSibling();

			return day;
		}

		/// <summary>
		/// Create header from day of week template.
		/// </summary>
		protected virtual void GenerateHeader()
		{
			if (CalendarDayOfWeekTemplate == null)
			{
				return;
			}

			for (int i = Header.Count; i < DaysInWeek; i++)
			{
				var day_of_week = Compatibility.Instantiate(CalendarDayOfWeekTemplate);
				day_of_week.transform.SetParent(HeaderContainer, false);
				Utilities.FixInstantiated(CalendarDayOfWeekTemplate, day_of_week);

				day_of_week.Calendar = this;

				day_of_week.gameObject.SetActive(true);

				Header.Add(day_of_week);
			}
		}

		/// <summary>
		/// Create days for displayed month.
		/// </summary>
		/// <param name="displayedDate">Displayed date.</param>
		protected virtual void GenerateDays(DateTime displayedDate)
		{
			var n = GetMaxDisplayedDays(displayedDate);

			if (n > Days.Count)
			{
				for (int i = Days.Count; i < n; i++)
				{
					Days.Add(GenerateDay());
				}
			}
			else if (n < Days.Count)
			{
				for (int i = n; i < Days.Count - n; i++)
				{
					Cache.Add(Days[i]);
					DisableGameObject(Days[i]);
				}

				Days.RemoveRange(n, Days.Count - n);
			}
		}

		/// <summary>
		/// Disabled gameobject with the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DisableGameObject(Component component)
		{
			component.gameObject.SetActive(false);
		}

		/// <summary>
		/// Display next month.
		/// </summary>
		public virtual void NextMonth()
		{
			if (!IsInteractable())
			{
				return;
			}

			DateDisplay = DateDisplay.AddMonths(1);
		}

		/// <summary>
		/// Display previous month.
		/// </summary>
		public virtual void PrevMonth()
		{
			if (!IsInteractable())
			{
				return;
			}

			DateDisplay = DateDisplay.AddMonths(-1);
		}

		/// <summary>
		/// Display next year.
		/// </summary>
		public virtual void NextYear()
		{
			if (!IsInteractable())
			{
				return;
			}

			DateDisplay = DateDisplay.AddYears(1);
		}

		/// <summary>
		/// Display previous year.
		/// </summary>
		public virtual void PrevYear()
		{
			if (!IsInteractable())
			{
				return;
			}

			DateDisplay = DateDisplay.AddYears(-1);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			calendarDateTemplate.SetStyle(style.Calendar, style);

			for (int i = 0; i < Days.Count; i++)
			{
				Days[i].SetStyle(style.Calendar, style);
			}

			for (int i = 0; i < Cache.Count; i++)
			{
				Cache[i].SetStyle(style.Calendar, style);
			}

			calendarDayOfWeekTemplate.SetStyle(style.Calendar, style);

			for (int i = 0; i < Header.Count; i++)
			{
				Header[i].SetStyle(style.Calendar, style);
			}

			style.Calendar.PrevMonth.ApplyTo(transform.Find("MonthToggle/PrevMonth"));
			style.Calendar.NextMonth.ApplyTo(transform.Find("MonthToggle/NextMonth"));

			if (DateTextAdapter != null)
			{
				style.Calendar.CurrentDate.ApplyTo(DateTextAdapter.gameObject);
			}

			if (MonthTextAdapter != null)
			{
				style.Calendar.CurrentMonth.ApplyTo(MonthTextAdapter.gameObject);
			}

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			calendarDateTemplate.GetStyle(style.Calendar, style);

			calendarDayOfWeekTemplate.GetStyle(style.Calendar, style);

			style.Calendar.PrevMonth.GetFrom(transform.Find("MonthToggle/PrevMonth"));
			style.Calendar.NextMonth.GetFrom(transform.Find("MonthToggle/NextMonth"));

			if (DateTextAdapter != null)
			{
				style.Calendar.CurrentDate.GetFrom(DateTextAdapter.gameObject);
			}

			if (MonthTextAdapter != null)
			{
				style.Calendar.CurrentMonth.GetFrom(MonthTextAdapter.gameObject);
			}

			return true;
		}
		#endregion
	}
}