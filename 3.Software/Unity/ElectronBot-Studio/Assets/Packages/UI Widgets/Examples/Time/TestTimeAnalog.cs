namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Test TimeAnalog.
	/// </summary>
	public class TestTimeAnalog : MonoBehaviour
	{
		/// <summary>
		/// Time widget.
		/// </summary>
		[SerializeField]
		public TimeBase TimeWidget;

		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		public TextAdapter Text;

		/// <summary>
		/// Process the start event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			TimeWidget.OnTimeChanged.AddListener(TimeChanged);
			TimeChanged(TimeWidget.Time);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			TimeWidget.OnTimeChanged.RemoveListener(TimeChanged);
		}

		/// <summary>
		/// Process the time changed event.
		/// </summary>
		/// <param name="time">Time.</param>
		public void TimeChanged(TimeSpan time)
		{
			Text.text = string.Format("{0}:{1}", time.Hours.ToString("D02"), time.Minutes.ToString("D02"));
		}
	}
}