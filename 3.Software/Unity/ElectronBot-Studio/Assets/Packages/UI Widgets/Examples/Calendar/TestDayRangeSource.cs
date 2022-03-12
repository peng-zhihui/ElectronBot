namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Test day range source.
	/// </summary>
	public class TestDayRangeSource : MonoBehaviour
	{
		/// <summary>
		/// The start date.
		/// </summary>
		[SerializeField]
		public DateTime DateStart;

		/// <summary>
		/// The end date.
		/// </summary>
		[SerializeField]
		public DateTime DateEnd;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			DateStart = DateTime.Now.AddDays(-5);
			DateEnd = DateTime.Now.AddDays(9);
		}
	}
}