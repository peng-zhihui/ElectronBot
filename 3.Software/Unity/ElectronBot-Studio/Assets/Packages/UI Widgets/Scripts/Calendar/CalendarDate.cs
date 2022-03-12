namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// CalendarDate.
	/// Display date.
	/// </summary>
	public class CalendarDate : CalendarDateBase, IUpgradeable
	{
		/// <summary>
		/// Text component to display day.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with DayAdapter")]
		protected Text Day;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			Utilities.GetOrAddComponent(Day, ref dayAdapter);
#pragma warning restore 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}