namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the calendar.
	/// </summary>
	public class StyleSupportCalendar : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Next month.
		/// </summary>
		[SerializeField]
		public Image NextMonth;

		/// <summary>
		/// Previous month.
		/// </summary>
		[SerializeField]
		public Image PrevMonth;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Calendar.NextMonth.ApplyTo(NextMonth);
			style.Calendar.PrevMonth.ApplyTo(PrevMonth);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Calendar.NextMonth.GetFrom(NextMonth);
			style.Calendar.PrevMonth.GetFrom(PrevMonth);

			return true;
		}
		#endregion
	}
}