namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the Drop-down.
	/// </summary>
	[Serializable]
	public class StyleDropdown : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the selected item text.
		/// </summary>
		[SerializeField]
		public StyleText Label;

		/// <summary>
		/// Style for the arrow.
		/// </summary>
		[SerializeField]
		public StyleImage Arrow;

		/// <summary>
		/// Style for the options background.
		/// </summary>
		[SerializeField]
		public StyleImage OptionsBackground;

		/// <summary>
		/// Style for the item background.
		/// </summary>
		[SerializeField]
		public StyleImage ItemBackground;

		/// <summary>
		/// Style for the check mark.
		/// </summary>
		[SerializeField]
		public StyleImage ItemCheckmark;

		/// <summary>
		/// Style for the item text.
		/// </summary>
		[SerializeField]
		public StyleText ItemLabel;

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		/// <summary>
		/// Apply style to the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="style">Style.</param>
		public virtual void ApplyTo(Dropdown component, Style style)
		{
			if (component == null)
			{
				return;
			}

			Background.ApplyTo(component.GetComponent<Image>());
			Label.ApplyTo(component.captionText);
			Arrow.ApplyTo(component.transform.Find("Arrow"));
			OptionsBackground.ApplyTo(component.template);

			var scroll_rect = component.template.GetComponent<ScrollRect>();
			OptionsBackground.ApplyTo(scroll_rect.viewport);

			if (scroll_rect.horizontalScrollbar != null)
			{
				style.ApplyTo(scroll_rect.horizontalScrollbar.gameObject);
			}

			if (scroll_rect.verticalScrollbar != null)
			{
				style.ApplyTo(scroll_rect.verticalScrollbar.gameObject);
			}

			var item = component.itemText.transform.parent;

			if (item != null)
			{
				ItemBackground.ApplyTo(item.Find("Item Background"));

				var toggle = item.GetComponent<Toggle>();
				if (toggle != null)
				{
					ItemCheckmark.ApplyTo(toggle.graphic as Image);
				}
			}

			ItemLabel.ApplyTo(component.itemText);
		}

		/// <summary>
		/// Set style options from the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="style">Style.</param>
		public virtual void GetFrom(Dropdown component, Style style)
		{
			if (component == null)
			{
				return;
			}

			Background.GetFrom(component.GetComponent<Image>());
			Label.GetFrom(component.captionText);
			Arrow.GetFrom(component.transform.Find("Arrow"));
			OptionsBackground.GetFrom(component.template);

			var scroll_rect = component.template.GetComponent<ScrollRect>();
			OptionsBackground.GetFrom(scroll_rect.viewport);

			if (scroll_rect.horizontalScrollbar != null)
			{
				style.GetFrom(scroll_rect.horizontalScrollbar.gameObject);
			}

			if (scroll_rect.verticalScrollbar != null)
			{
				style.GetFrom(scroll_rect.verticalScrollbar.gameObject);
			}

			var item = component.itemText.transform.parent;

			if (item != null)
			{
				ItemBackground.GetFrom(item.Find("Item Background"));

				var toggle = item.GetComponent<Toggle>();
				if (toggle != null)
				{
					ItemCheckmark.GetFrom(toggle.graphic as Image);
				}
			}

			ItemLabel.GetFrom(component.itemText);
		}
#endif

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Label.SetDefaultValues();
			Arrow.SetDefaultValues();

			OptionsBackground.SetDefaultValues();

			ItemBackground.SetDefaultValues();
			ItemCheckmark.SetDefaultValues();
			ItemLabel.SetDefaultValues();
		}
#endif
	}
}