namespace UIWidgets
{
	using System;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Date ScrollBlock widget.
	/// </summary>
	public class DateScroller : DateBase
	{
		/// <summary>
		/// Date comparison precision.
		/// </summary>
		protected enum Precision
		{
			/// <summary>
			/// Years.
			/// </summary>
			Years = 0,

			/// <summary>
			/// Months.
			/// </summary>
			Months,

			/// <summary>
			/// Days.
			/// </summary>
			Days,

			/// <summary>
			/// Hours.
			/// </summary>
			Hours,

			/// <summary>
			/// Minutes.
			/// </summary>
			Minutes,

			/// <summary>
			/// Seconds.
			/// </summary>
			Seconds,

			/// <summary>
			/// Milliseconds.
			/// </summary>
			Milliseconds,

			/// <summary>
			/// Ticks.
			/// </summary>
			Ticks,
		}

		/// <summary>
		/// If enabled any time period changes will not change other time periods.
		/// </summary>
		[SerializeField]
		[Tooltip("If enabled any time period changes will not change other time periods.")]
		public bool IndependentScroll;

		[SerializeField]
		bool years = true;

		/// <summary>
		/// Display ScrollBlock for the years.
		/// </summary>
		public bool Years
		{
			get
			{
				return years;
			}

			set
			{
				years = value;

				if (YearsScrollBlock != null)
				{
					YearsScrollBlock.gameObject.SetActive(years);
				}
			}
		}

		/// <summary>
		/// ScrollBlock for the years.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("years")]
		[FormerlySerializedAs("YearsScroller")]
		protected ScrollBlockBase YearsScrollBlock;

		/// <summary>
		/// Step to change year.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("years")]
		public int YearsStep = 1;

		/// <summary>
		/// Format to display year.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("years")]
		public string YearsFormat = "yyyy";

		[SerializeField]
		bool months = true;

		/// <summary>
		/// Display ScrollBlock for the months.
		/// </summary>
		public bool Months
		{
			get
			{
				return months;
			}

			set
			{
				months = value;

				if (MonthsScrollBlock != null)
				{
					MonthsScrollBlock.gameObject.SetActive(months);
				}
			}
		}

		/// <summary>
		/// ScrollBlock for the months.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("months")]
		[FormerlySerializedAs("MonthsScroller")]
		protected ScrollBlockBase MonthsScrollBlock;

		/// <summary>
		/// Step to change month.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("months")]
		public int MonthsStep = 1;

		/// <summary>
		/// Format to display month.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("months")]
		public string MonthsFormat = "MMM";

		[SerializeField]
		bool days = true;

		/// <summary>
		/// Display ScrollBlock for the days.
		/// </summary>
		public bool Days
		{
			get
			{
				return days;
			}

			set
			{
				days = value;

				if (DaysScrollBlock != null)
				{
					DaysScrollBlock.gameObject.SetActive(days);
				}
			}
		}

		/// <summary>
		/// ScrollBlock for the days.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("days")]
		[FormerlySerializedAs("DaysScroller")]
		protected ScrollBlockBase DaysScrollBlock;

		/// <summary>
		/// Step to change day.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("days")]
		public int DaysStep = 1;

		/// <summary>
		/// Format to display day.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("days")]
		public string DaysFormat = "dd";

		/// <summary>
		/// Looped clamp the specified date.
		/// </summary>
		/// <param name="d">Date.</param>
		/// <returns>Clamped date.</returns>
		protected virtual DateTime LoopedClamp(DateTime d)
		{
			if (d < DateMin)
			{
				d = DateMax;
			}

			if (d > DateMax)
			{
				d = DateMin;
			}

			return d;
		}

		/// <summary>
		/// Looped increment/decrement for the date.
		/// </summary>
		/// <param name="value">Start date.</param>
		/// <param name="steps">Steps.</param>
		/// <param name="step">Step.</param>
		/// <param name="nextValue">Function increment/decrement for the date.</param>
		/// <returns>Resulted date.</returns>
		protected virtual DateTime LoopedDate(DateTime value, int steps, int step, Func<DateTime, int, DateTime> nextValue)
		{
			for (int i = 0; i < steps; i++)
			{
				value = LoopedClamp(nextValue(value, step));
			}

			return value;
		}

		/// <summary>
		/// Add years to the specified date.
		/// </summary>
		/// <param name="date">Date.</param>
		/// <param name="years">Years to add.</param>
		/// <returns>New date.</returns>
		protected virtual DateTime AddYears(DateTime date, int years)
		{
			return date.AddYears(years);
		}

		/// <summary>
		/// Increment/decrement current date by years.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <returns>Date.</returns>
		protected virtual DateTime LoopedYears(int steps)
		{
			var value = Date;
			var step = YearsStep;
			var increase = steps > 0;
			if (!increase)
			{
				steps = -steps;
				step = -YearsStep;
			}

			if (!IndependentScroll)
			{
				return LoopedDate(value, steps, step, AddYears);
			}

			for (int i = 0; i < steps; i++)
			{
				value = value.AddYears(step);

				var new_years = value.Year;
				if (new_years < DateMin.Year)
				{
					new_years = increase ? DateMax.Year : Mathf.Max(DateMin.Year, DateTime.MaxValue.Year - YearsStep);
				}

				if (new_years > DateMax.Year)
				{
					new_years = increase ? DateMin.Year : Mathf.Min(DateMax.Year, DateTime.MaxValue.Year - YearsStep);
				}

				if (value.Year != new_years)
				{
					value = value.AddYears(new_years - value.Year);
				}
			}

			return value;
		}

		/// <summary>
		/// Add months to the specified date.
		/// </summary>
		/// <param name="date">Date.</param>
		/// <param name="months">Months to add.</param>
		/// <returns>New date.</returns>
		protected virtual DateTime AddMonths(DateTime date, int months)
		{
			return date.AddMonths(months);
		}

		/// <summary>
		/// Increment/decrement current date by months.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <returns>Date.</returns>
		protected virtual DateTime LoopedMonths(int steps)
		{
			var value = Date;
			var step = MonthsStep;
			var increase = steps > 0;
			if (!increase)
			{
				steps = -steps;
				step = -step;
			}

			if (!IndependentScroll)
			{
				return LoopedDate(value, steps, step, AddMonths);
			}

			for (int i = 0; i < steps; i++)
			{
				value = value.AddMonths(step);
				value = CopyTimePeriods(value, Precision.Years);

				var new_months = value.Month;
				if (DateTimeEquals(Date, DateMin, Precision.Years) && (new_months < DateMin.Month))
				{
					new_months = increase ? DateMax.Month : Mathf.Max(DateMin.Month, 12 - MonthsStep);
				}

				if (DateTimeEquals(Date, DateMax, Precision.Years) && (new_months > DateMax.Month))
				{
					new_months = increase ? DateMin.Month : Mathf.Min(DateMax.Month, 12 - MonthsStep);
				}

				if (value.Month != new_months)
				{
					value = value.AddMonths(new_months - value.Month);
				}
			}

			return value;
		}

		/// <summary>
		/// Add days to the specified date.
		/// </summary>
		/// <param name="date">Date.</param>
		/// <param name="days">Days to add.</param>
		/// <returns>New date.</returns>
		protected virtual DateTime AddDays(DateTime date, int days)
		{
			return date.AddDays(days);
		}

		/// <summary>
		/// Increment/decrement current date by days.
		/// </summary>
		/// <param name="steps">Steps.</param>
		/// <returns>Date.</returns>
		protected virtual DateTime LoopedDays(int steps)
		{
			var value = Date;
			var step = DaysStep;
			var increase = steps > 0;
			if (!increase)
			{
				steps = -steps;
				step = -step;
			}

			if (!IndependentScroll)
			{
				return LoopedDate(value, steps, step, AddDays);
			}

			for (int i = 0; i < steps; i++)
			{
				value = value.AddDays(step);
				value = CopyTimePeriods(value, Precision.Months);

				var days_in_month = DateTime.DaysInMonth(Date.Year, Date.Month);
				var new_days = value.Day;
				if (DateTimeEquals(Date, DateMin, Precision.Months) && (new_days < DateMin.Day))
				{
					new_days = increase ? DateMax.Day : Mathf.Max(DateMin.Day, days_in_month - DaysStep);
				}

				if (DateTimeEquals(Date, DateMax, Precision.Months) && (new_days > DateMax.Day))
				{
					new_days = increase ? DateMin.Day : Mathf.Min(DateMax.Day, days_in_month - DaysStep);
				}

				if (value.Day != new_days)
				{
					value = value.AddDays(new_days - value.Day);
				}
			}

			return value;
		}

		/// <summary>
		/// Copy time periods from current date to the specified date with precision.
		/// </summary>
		/// <param name="value">Initial date.</param>
		/// <param name="precision">Copy precision.</param>
		/// <returns>Date.</returns>
		protected DateTime CopyTimePeriods(DateTime value, Precision precision)
		{
			switch (precision)
			{
				case Precision.Years:
					value = value.AddYears(Date.Year - value.Year);
					return value;
				case Precision.Months:
					value = value.AddYears(Date.Year - value.Year);
					value = value.AddMonths(Date.Month - value.Month);
					return value;
				case Precision.Days:
					value = value.AddYears(Date.Year - value.Year);
					value = value.AddMonths(Date.Month - value.Month);
					value = value.AddDays(Date.Day - value.Day);
					return value;
				case Precision.Hours:
					value = value.AddYears(Date.Year - value.Year);
					value = value.AddMonths(Date.Month - value.Month);
					value = value.AddDays(Date.Day - value.Day);

					value = value.AddHours(Date.Hour - value.Hour);
					return value;
				case Precision.Minutes:
					value = value.AddYears(Date.Year - value.Year);
					value = value.AddMonths(Date.Month - value.Month);
					value = value.AddDays(Date.Day - value.Day);

					value = value.AddHours(Date.Hour - value.Hour);
					value = value.AddMinutes(Date.Minute - value.Minute);
					return value;
				case Precision.Seconds:
					value = value.AddYears(Date.Year - value.Year);
					value = value.AddMonths(Date.Month - value.Month);
					value = value.AddDays(Date.Day - value.Day);

					value = value.AddHours(Date.Hour - value.Hour);
					value = value.AddMinutes(Date.Minute - value.Minute);
					value = value.AddSeconds(Date.Second - value.Second);
					return value;
				case Precision.Milliseconds:
					value = value.AddYears(Date.Year - value.Year);
					value = value.AddMonths(Date.Month - value.Month);
					value = value.AddDays(Date.Day - value.Day);

					value = value.AddHours(Date.Hour - value.Hour);
					value = value.AddMinutes(Date.Minute - value.Minute);
					value = value.AddSeconds(Date.Second - value.Second);

					value = value.AddMilliseconds(Date.Millisecond - value.Millisecond);
					return value;
				case Precision.Ticks:
					return Date;
			}

			return value;
		}

		/// <summary>
		/// Compare date time with specified precision.
		/// </summary>
		/// <param name="date1">Date 1.</param>
		/// <param name="date2">Date 2.</param>
		/// <param name="precision">Compare precision.</param>
		/// <returns>true if dates equals; otherwise false.</returns>
		protected static bool DateTimeEquals(DateTime date1, DateTime date2, Precision precision)
		{
			switch (precision)
			{
				case Precision.Years:
					return date1.Year == date2.Year;
				case Precision.Months:
					return (date1.Year == date2.Year) && (date1.Month == date2.Month);
				case Precision.Days:
					return date1.Date == date2.Date;
				case Precision.Hours:
					return (date1.Date == date2.Date) && (date1.Hour == date2.Hour);
				case Precision.Minutes:
					return (date1.Date == date2.Date) && (date1.Hour == date2.Hour) && (date1.Minute == date2.Minute);
				case Precision.Seconds:
					return (date1.Date == date2.Date) && (date1.Hour == date2.Hour) && (date1.Minute == date2.Minute) && (date1.Second == date2.Second);
				case Precision.Milliseconds:
					return (date1.Date == date2.Date) && (date1.Hour == date2.Hour) && (date1.Minute == date2.Minute) && (date1.Second == date2.Second) && (date1.Millisecond == date2.Millisecond);
				case Precision.Ticks:
					return date1 == date2;
				default:
					throw new NotSupportedException(string.Format("Unknown precision: {0}", EnumHelper<Precision>.ToString(precision)));
			}
		}

		/// <summary>
		/// Get value for the years at specified step.
		/// </summary>
		/// <param name="steps">Step.</param>
		/// <returns>Formatted years.</returns>
		protected virtual string YearsValue(int steps)
		{
			return LoopedYears(steps).ToString(YearsFormat, Culture);
		}

		/// <summary>
		/// Get value for the months at specified step.
		/// </summary>
		/// <param name="steps">Step.</param>
		/// <returns>Formatted months.</returns>
		protected virtual string MonthsValue(int steps)
		{
			return LoopedMonths(steps).ToString(MonthsFormat, Culture);
		}

		/// <summary>
		/// Get value for the days at specified step.
		/// </summary>
		/// <param name="steps">Step.</param>
		/// <returns>Formatted days.</returns>
		protected string DaysValue(int steps)
		{
			return LoopedDays(steps).ToString(DaysFormat, Culture);
		}

		/// <summary>
		/// Updates the calendar.
		/// Should be called only after changing culture settings.
		/// </summary>
		public override void UpdateCalendar()
		{
			if (YearsScrollBlock != null)
			{
				YearsScrollBlock.UpdateView();
			}

			if (MonthsScrollBlock != null)
			{
				MonthsScrollBlock.UpdateView();
			}

			if (DaysScrollBlock != null)
			{
				DaysScrollBlock.UpdateView();
			}
		}

		bool isInitedDate;

		/// <summary>
		/// Increase the current date on one year.
		/// </summary>
		protected void IncreaseYears()
		{
			Date = LoopedYears(1);
		}

		/// <summary>
		/// Decrease the current date on one year.
		/// </summary>
		protected void DecreaseYears()
		{
			Date = LoopedYears(-1);
		}

		/// <summary>
		/// Increase the current date on one month.
		/// </summary>
		protected void IncreaseMonths()
		{
			Date = LoopedMonths(1);
		}

		/// <summary>
		/// Decrease the current date on one month.
		/// </summary>
		protected void DecreaseMonths()
		{
			Date = LoopedMonths(-1);
		}

		/// <summary>
		/// Increase the current date on one day.
		/// </summary>
		protected void IncreaseDays()
		{
			Date = LoopedDays(1);
		}

		/// <summary>
		/// Decrease the current date on one day.
		/// </summary>
		protected void DecreaseDays()
		{
			Date = LoopedDays(-1);
		}

		/// <summary>
		/// Init.
		/// </summary>
		protected override void InitNestedWidgets()
		{
			if (isInitedDate)
			{
				return;
			}

			isInitedDate = true;

			if (YearsScrollBlock != null)
			{
				YearsScrollBlock.gameObject.SetActive(Years);

				YearsScrollBlock.Value = YearsValue;
				YearsScrollBlock.Decrease = DecreaseYears;
				YearsScrollBlock.Increase = IncreaseYears;
				YearsScrollBlock.IsInteractable = IsActive;

				YearsScrollBlock.Init();
			}

			if (MonthsScrollBlock != null)
			{
				MonthsScrollBlock.gameObject.SetActive(Months);

				MonthsScrollBlock.Value = MonthsValue;
				MonthsScrollBlock.Decrease = DecreaseMonths;
				MonthsScrollBlock.Increase = IncreaseMonths;
				MonthsScrollBlock.IsInteractable = IsActive;

				MonthsScrollBlock.Init();
			}

			if (DaysScrollBlock != null)
			{
				DaysScrollBlock.gameObject.SetActive(Days);

				DaysScrollBlock.Value = DaysValue;
				DaysScrollBlock.Decrease = DecreaseDays;
				DaysScrollBlock.Increase = IncreaseDays;
				DaysScrollBlock.IsInteractable = IsActive;

				DaysScrollBlock.Init();
			}
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected override void OnInteractableChange(bool interactableState)
		{
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (YearsScrollBlock != null)
			{
				YearsScrollBlock.SetStyle(style);
			}

			if (MonthsScrollBlock != null)
			{
				MonthsScrollBlock.SetStyle(style);
			}

			if (DaysScrollBlock != null)
			{
				DaysScrollBlock.SetStyle(style);
			}

			style.ScrollBlock.Highlight.ApplyTo(transform.Find("Highlight"));
			style.ScrollBlock.Background.ApplyTo(GetComponent<Image>());

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (YearsScrollBlock != null)
			{
				YearsScrollBlock.GetStyle(style);
			}

			if (MonthsScrollBlock != null)
			{
				MonthsScrollBlock.GetStyle(style);
			}

			if (DaysScrollBlock != null)
			{
				DaysScrollBlock.GetStyle(style);
			}

			style.ScrollBlock.Highlight.GetFrom(transform.Find("Highlight"));
			style.ScrollBlock.Background.GetFrom(GetComponent<Image>());

			return true;
		}
		#endregion
	}
}