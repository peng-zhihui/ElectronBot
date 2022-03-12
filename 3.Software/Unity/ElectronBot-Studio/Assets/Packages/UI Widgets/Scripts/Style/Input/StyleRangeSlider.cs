namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the RangeSlider.
	/// </summary>
	[Serializable]
	public class StyleRangeSlider : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the usable range.
		/// </summary>
		[SerializeField]
		public StyleImage UsableRange;

		/// <summary>
		/// Style for the fill.
		/// </summary>
		[SerializeField]
		public StyleImage Fill;

		/// <summary>
		/// Style for the minimum handle.
		/// </summary>
		[SerializeField]
		public StyleImage HandleMin;

		/// <summary>
		/// Style for the minimum handle transition.
		/// </summary>
		[SerializeField]
		public Selectable.Transition HandleMinTransition = Selectable.Transition.SpriteSwap;

		/// <summary>
		/// Style for the minimum handle colors.
		/// </summary>
		[SerializeField]
		public ColorBlock HandleMinColors = new ColorBlock()
		{
			normalColor = Color.white,
			highlightedColor = new Color32(245, 245, 245, 255),
			pressedColor = new Color32(200, 200, 200, 255),
			disabledColor = new Color32(200, 200, 200, 128),
			colorMultiplier = 1f,
			fadeDuration = 0.1f,
		};

		/// <summary>
		/// Style for the minimum handle sprites.
		/// </summary>
		[SerializeField]
		public SpriteState HandleMinSprites;

		/// <summary>
		/// Style for the minimum handle animations.
		/// </summary>
		[SerializeField]
		public AnimationTriggers HandleMinAnimation;

		/// <summary>
		/// Style for the maximum handle.
		/// </summary>
		[SerializeField]
		public StyleImage HandleMax;

		/// <summary>
		/// Style for the maximum handle transition.
		/// </summary>
		[SerializeField]
		public Selectable.Transition HandleMaxTransition = Selectable.Transition.SpriteSwap;

		/// <summary>
		/// Style for the maximum handle colors.
		/// </summary>
		[SerializeField]
		public ColorBlock HandleMaxColors;

		/// <summary>
		/// Style for the maximum handle sprites.
		/// </summary>
		[SerializeField]
		public SpriteState HandleMaxSprites;

		/// <summary>
		/// Style for the maximum handle animations.
		/// </summary>
		[SerializeField]
		public AnimationTriggers HandleMaxAnimation;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			UsableRange.SetDefaultValues();
			Fill.SetDefaultValues();
			HandleMin.SetDefaultValues();
			HandleMax.SetDefaultValues();
		}
#endif
	}
}