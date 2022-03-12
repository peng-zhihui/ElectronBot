namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the default Unity button.
	/// </summary>
	[Serializable]
	public class StyleUnityButton : IStyleDefaultValues
	{
		/// <summary>
		/// Use style on buttons with StyleSupportUnityButton script only.
		/// </summary>
		[SerializeField]
		public bool StyleSupportRequired = true;

		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the text.
		/// </summary>
		[SerializeField]
		public StyleText Text;

		/// <summary>
		/// Apply style for the specified button.
		/// </summary>
		/// <param name="component">Button.</param>
		public virtual void ApplyTo(Button component)
		{
			if (component == null)
			{
				return;
			}

			var style_support = component.GetComponent<StyleSupportUnityButton>();
			if (style_support != null)
			{
				style_support.SetStyle(this);
			}
			else if (!StyleSupportRequired)
			{
				Background.ApplyTo(component.GetComponent<Image>());

				Text.ApplyTo(component.transform.Find("Text"));
			}
		}

		/// <summary>
		/// Set style options from the specified button.
		/// </summary>
		/// <param name="component">Button.</param>
		public virtual void GetFrom(Button component)
		{
			if (component == null)
			{
				return;
			}

			var style_support = component.GetComponent<StyleSupportUnityButton>();
			if (style_support != null)
			{
				style_support.GetStyle(this);
			}
			else if (!StyleSupportRequired)
			{
				Background.GetFrom(component.GetComponent<Image>());

				Text.GetFrom(component.transform.Find("Text"));
			}
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Text.SetDefaultValues();
		}
#endif
	}
}