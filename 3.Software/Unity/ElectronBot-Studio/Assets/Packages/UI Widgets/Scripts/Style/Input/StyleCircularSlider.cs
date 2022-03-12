namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the RangeSlider.
	/// </summary>
	[Serializable]
	public class StyleCircularSlider : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the ring.
		/// </summary>
		[SerializeField]
		public StyleImage Ring;

		/// <summary>
		/// Ring color.
		/// </summary>
		[SerializeField]
		public Color RingColor;

		/// <summary>
		/// Style for the handle.
		/// </summary>
		[SerializeField]
		public StyleImage Handle;

		/// <summary>
		/// Style for the arrow.
		/// </summary>
		[SerializeField]
		public StyleImage Arrow;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Ring.SetDefaultValues();
			Handle.SetDefaultValues();
			Arrow.SetDefaultValues();
		}
#endif
	}
}