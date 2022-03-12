namespace UIWidgets
{
	using System;
	using System.Globalization;
	using UIWidgets.Attributes;
	using UnityEngine;

	/// <summary>
	/// DateTime widget.
	/// </summary>
	[DataBindSupport]
	public abstract class DateTimeWidgetBase : MonoBehaviourConditional
	{
		[SerializeField]
		bool currentDateTimeAsDefault = false;

		/// <summary>
		/// The current DateTime.
		/// </summary>
		[SerializeField]
		protected DateTime dateTime;

		/// <summary>
		/// The current DateTime.
		/// </summary>
		/// <value>The date time.</value>
		[DataBindField]
		public virtual DateTime DateTime
		{
			get
			{
				return dateTime;
			}

			set
			{
				Init();

				if (dateTime != value)
				{
					dateTime = value;

					UpdateWidgets();

					OnDateTimeChanged.Invoke(dateTime);
				}
			}
		}

		/// <summary>
		/// The current date time.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("currentDateTimeAsDefault", false)]
		protected string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

		/// <summary>
		/// The format.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("currentDateTimeAsDefault", false)]
		protected string format = "yyyy-MM-dd HH:mm:ss";

		/// <summary>
		/// Event called when date-time changed.
		/// </summary>
		[DataBindEvent("DateTime")]
		public CalendarDateEvent OnDateTimeChanged = new CalendarDateEvent();

		bool isInited;

		/// <summary>
		/// Culture to parse date.
		/// </summary>
		public abstract CultureInfo Culture
		{
			get;
			set;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			dateTime = currentDateTimeAsDefault ? DateTime.Now : DateTime.ParseExact(currentDateTime, format, Culture);

			UpdateWidgets();

			AddListeners();
		}

		/// <summary>
		/// Updates the widgets.
		/// </summary>
		protected abstract void UpdateWidgets();

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}
	}
}