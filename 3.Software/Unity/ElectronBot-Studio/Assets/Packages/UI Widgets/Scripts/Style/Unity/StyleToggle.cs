namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the toggle.
	/// </summary>
	[Serializable]
	public class StyleToggle : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the checkmark.
		/// </summary>
		[SerializeField]
		public StyleImage Checkmark;

		/// <summary>
		/// Style for the label.
		/// </summary>
		[SerializeField]
		public StyleText Label;

		/// <summary>
		/// Apply style for the specified toggle.
		/// </summary>
		/// <param name="component">Toggle.</param>
		public virtual void ApplyTo(Toggle component)
		{
			if (component == null)
			{
				return;
			}

			var style_support = component.GetComponent<StyleSupportToggle>();

			if (style_support != null)
			{
				style_support.SetStyle(this);
			}
			else
			{
				Background.ApplyTo(component.transform.Find("Background"));

				Checkmark.ApplyTo(component.graphic as Image);

				Label.ApplyTo(component.transform.Find("Label"));
			}
		}

		/// <summary>
		/// Set style options from the specified component.
		/// </summary>
		/// <param name="component">Toggle.</param>
		public virtual void GetFrom(Toggle component)
		{
			if (component == null)
			{
				return;
			}

			var style_support = component.GetComponent<StyleSupportToggle>();

			if (style_support != null)
			{
				style_support.GetStyle(this);
			}
			else
			{
				Background.GetFrom(component.transform.Find("Background"));

				Checkmark.GetFrom(component.graphic as Image);

				Label.GetFrom(component.transform.Find("Label"));
			}
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Checkmark.SetDefaultValues();
			Label.SetDefaultValues();
		}
#endif
	}
}