namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Tooltip settings.
	/// </summary>
	[Serializable]
	public struct TooltipSettings
	{
		/// <summary>
		/// Position.
		/// </summary>
		[SerializeField]
		public TooltipPosition Position;

		/// <summary>
		/// Time before tooltip displayed.
		/// </summary>
		[SerializeField]
		public float ShowDelay;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime;

		/// <summary>
		/// Tooltip parent.
		/// </summary>
		[SerializeField]
		public RectTransform Parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="TooltipSettings"/> struct.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="delay">Time before tooltip displayed.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		public TooltipSettings(TooltipPosition position, float delay = 0.3f, RectTransform parent = null, bool unscaledTime = true)
		{
			Position = position;
			ShowDelay = delay;
			UnscaledTime = unscaledTime;
			Parent = parent;
		}
	}
}