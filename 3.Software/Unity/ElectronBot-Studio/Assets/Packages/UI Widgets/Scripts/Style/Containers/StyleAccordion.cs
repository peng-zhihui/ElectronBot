namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the accordion.
	/// </summary>
	[Serializable]
	public class StyleAccordion : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the inactive toggle background.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("ToggleBackground")]
		public StyleImage ToggleDefaultBackground;

		/// <summary>
		/// Style for the inactive toggle background.
		/// </summary>
		[Obsolete("Renamed to ToggleDefaultBackground.")]
		public StyleImage ToggleBackground
		{
			get
			{
				return ToggleDefaultBackground;
			}
		}

		/// <summary>
		/// Style for the active toggle background.
		/// </summary>
		[SerializeField]
		public StyleImage ToggleActiveBackground;

		/// <summary>
		/// Style for the inactive toggle text.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("ToggleText")]
		public StyleText ToggleDefaultText;

		/// <summary>
		/// Style for the inactive toggle text.
		/// </summary>
		[Obsolete("Renamed to ToggleDefaultText.")]
		public StyleText ToggleText
		{
			get
			{
				return ToggleDefaultText;
			}
		}

		/// <summary>
		/// Style for the active toggle text.
		/// </summary>
		[SerializeField]
		public StyleText ToggleActiveText;

		/// <summary>
		/// Style for the content background.
		/// </summary>
		[SerializeField]
		public StyleImage ContentBackground;

		/// <summary>
		/// Style for the content text.
		/// </summary>
		[SerializeField]
		public StyleText ContentText;

		/// <summary>
		/// Apply style for the accordion item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ApplyTo(AccordionItem item)
		{
			if (item == null)
			{
				return;
			}

			if (item.ToggleObject != null)
			{
				if (item.Open)
				{
					ToggleActiveBackground.ApplyTo(item.ToggleObject.GetComponent<Image>());
				}
				else
				{
					ToggleDefaultBackground.ApplyTo(item.ToggleObject.GetComponent<Image>());
				}

				ToggleDefaultText.ApplyTo(item.ToggleObject.transform.Find("Text"));
			}

			if (item.ContentObject != null)
			{
				ContentBackground.ApplyTo(item.ContentObject.GetComponent<Image>());
				ContentText.ApplyTo(item.ContentObject.transform.Find("Text"));
			}
		}

		/// <summary>
		/// Set style options from the accordion item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void GetFrom(AccordionItem item)
		{
			if (item == null)
			{
				return;
			}

			if (item.ToggleObject != null)
			{
				if (item.Open)
				{
					ToggleActiveBackground.GetFrom(item.ToggleObject.GetComponent<Image>());
				}
				else
				{
					ToggleDefaultBackground.GetFrom(item.ToggleObject.GetComponent<Image>());
				}

				ToggleDefaultText.GetFrom(item.ToggleObject.transform.Find("Text"));
			}

			if (item.ContentObject != null)
			{
				ContentBackground.GetFrom(item.ContentObject.GetComponent<Image>());
				ContentText.GetFrom(item.ContentObject.transform.Find("Text"));
			}
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			ToggleDefaultBackground.SetDefaultValues();
			ToggleActiveBackground.SetDefaultValues();
			ToggleDefaultText.SetDefaultValues();
			ToggleActiveText.SetDefaultValues();
			ContentBackground.SetDefaultValues();
			ContentText.SetDefaultValues();
		}
#endif
	}
}