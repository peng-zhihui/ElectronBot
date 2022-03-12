namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// DatePicker.
	/// </summary>
	public class DatePicker : Picker<DateTime, DatePicker>
	{
		/// <summary>
		/// Calendar.
		/// </summary>
		[SerializeField]
		public DateBase Calendar;

		/// <summary>
		/// If true select date only when date changes; otherwise select date on click.
		/// </summary>
		[SerializeField]
		public bool DateChangeOnly = false;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(DateTime defaultValue)
		{
			Calendar.Date = defaultValue;

			Calendar.OnDateChanged.AddListener(DateChange);
			Calendar.OnDateClick.AddListener(DateClick);
		}

		/// <summary>
		/// Process date time change.
		/// </summary>
		/// <param name="dt">Selected value.</param>
		protected void DateChange(DateTime dt)
		{
			if (DateChangeOnly)
			{
				Selected(dt);
			}
		}

		/// <summary>
		/// Process date time click.
		/// </summary>
		/// <param name="dt">Selected value.</param>
		protected void DateClick(DateTime dt)
		{
			if (!DateChangeOnly)
			{
				Selected(dt);
			}
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			Calendar.OnDateChanged.RemoveListener(DateChange);
			Calendar.OnDateClick.RemoveListener(DateClick);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			Calendar.SetStyle(style);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			Calendar.GetStyle(style);

			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}