namespace UIWidgets
{
	using System;
	using System.Globalization;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// DateTime widget.
	/// </summary>
	public class DateTimeWidget : DateTimeWidgetBase, IStylable
	{
		/// <summary>
		/// The calendar widget.
		/// </summary>
		[SerializeField]
		protected DateBase calendar;

		/// <summary>
		/// The calendar widget.
		/// </summary>
		/// <value>The calendar.</value>
		public DateBase Calendar
		{
			get
			{
				return calendar;
			}

			set
			{
				RemoveListeners();
				calendar = value;
				AddListeners();
			}
		}

		/// <summary>
		/// The time widget.
		/// </summary>
		[SerializeField]
		protected TimeBase time;

		/// <summary>
		/// The time widget.
		/// </summary>
		/// <value>The time.</value>
		public TimeBase Time
		{
			get
			{
				return time;
			}

			set
			{
				RemoveListeners();
				time = value;
				AddListeners();
			}
		}

		/// <summary>
		/// Culture to parse date.
		/// </summary>
		public override CultureInfo Culture
		{
			get
			{
				return calendar.Culture;
			}

			set
			{
				calendar.Culture = value;
			}
		}

		/// <summary>
		/// Is used ScrollBlock widgets?
		/// </summary>
		[FormerlySerializedAs("IsScroller")]
		public bool IsScrollBlocksUsed = false;

		/// <summary>
		/// Is used ScrollBlock widgets?
		/// </summary>
		[Obsolete("Renamed to IsScrollBlocksUsed.")]
		public bool IsScroller
		{
			get
			{
				return IsScrollBlocksUsed;
			}

			set
			{
				IsScrollBlocksUsed = value;
			}
		}

		/// <summary>
		/// Updates the widgets.
		/// </summary>
		protected override void UpdateWidgets()
		{
			calendar.Date = dateTime;
			time.Time = dateTime.TimeOfDay;
		}

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected override void AddListeners()
		{
			calendar.OnDateChanged.AddListener(DateChanged);
			time.OnTimeChanged.AddListener(TimeChanged);
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected override void RemoveListeners()
		{
			calendar.OnDateChanged.RemoveListener(DateChanged);
			time.OnTimeChanged.RemoveListener(TimeChanged);
		}

		/// <summary>
		/// Process changed date.
		/// </summary>
		/// <param name="d">Date.</param>
		protected virtual void DateChanged(DateTime d)
		{
			DateTime = d.Date + DateTime.TimeOfDay;
		}

		/// <summary>
		/// Process changed time.
		/// </summary>
		/// <param name="t">Time.</param>
		protected virtual void TimeChanged(TimeSpan t)
		{
			DateTime = DateTime.Date + t;
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			if (IsScrollBlocksUsed)
			{
				Calendar.SetStyle(style);
				Time.SetStyle(style);

				style.ScrollBlock.Background.ApplyTo(GetComponent<Image>());

				return true;
			}

			return false;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			if (IsScrollBlocksUsed)
			{
				Calendar.GetStyle(style);
				Time.GetStyle(style);

				style.ScrollBlock.Background.GetFrom(GetComponent<Image>());

				return true;
			}

			return false;
		}
		#endregion
	}
}