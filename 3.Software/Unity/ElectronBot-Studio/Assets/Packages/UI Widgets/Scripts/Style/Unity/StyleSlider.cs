namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the slider.
	/// </summary>
	[Serializable]
	public class StyleSlider : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the fill.
		/// </summary>
		[SerializeField]
		public StyleImage Fill;

		/// <summary>
		/// Style for the handle.
		/// </summary>
		[SerializeField]
		public StyleImage Handle;

		/// <summary>
		/// Apply style to the specified slider.
		/// </summary>
		/// <param name="component">Slider.</param>
		public virtual void ApplyTo(Slider component)
		{
			if (component == null)
			{
				return;
			}

			var style_support = component.GetComponent<StyleSupportSlider>();

			if (style_support != null)
			{
				style_support.SetStyle(this);
			}
			else
			{
				Background.ApplyTo(component.transform.Find("Background"));

				Fill.ApplyTo(component.fillRect);
				Handle.ApplyTo(component.handleRect);
			}
		}

		/// <summary>
		/// Set style options from the specified slider.
		/// </summary>
		/// <param name="component">Slider.</param>
		public virtual void GetFrom(Slider component)
		{
			if (component == null)
			{
				return;
			}

			var style_support = component.GetComponent<StyleSupportSlider>();

			if (style_support != null)
			{
				style_support.GetStyle(this);
			}
			else
			{
				Background.GetFrom(component.transform.Find("Background"));

				Fill.GetFrom(component.fillRect);
				Handle.GetFrom(component.handleRect);
			}
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Fill.SetDefaultValues();
			Handle.SetDefaultValues();
		}
#endif
	}
}