namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the picker.
	/// </summary>
	public class StyleSupportPicker : MonoBehaviour, IStylable
	{
		/// <summary>
		/// The background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// The title text.
		/// </summary>
		[SerializeField]
		public GameObject TitleText;

		/// <summary>
		/// The content background.
		/// </summary>
		[SerializeField]
		public Image ContentBackground;

		/// <summary>
		/// The content text.
		/// </summary>
		[SerializeField]
		public GameObject ContentText;

		/// <summary>
		/// The delimiter.
		/// </summary>
		[SerializeField]
		public Image Delimiter;

		/// <summary>
		/// The buttons.
		/// </summary>
		[SerializeField]
		public List<StyleSupportButton> Buttons = new List<StyleSupportButton>();

		/// <summary>
		/// The stylables.
		/// </summary>
		[SerializeField]
		public List<GameObject> Stylables = new List<GameObject>();

		/// <summary>
		/// The close button at the header.
		/// </summary>
		[SerializeField]
		public StyleSupportButtonClose HeaderCloseButton;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Dialog.Background.ApplyTo(GetComponent<Image>());
			style.Dialog.Title.ApplyTo(TitleText);

			style.Dialog.ContentBackground.ApplyTo(ContentBackground);
			style.Dialog.ContentText.ApplyTo(ContentText);

			style.Dialog.Delimiter.ApplyTo(Delimiter);

			foreach (var button in Buttons)
			{
				style.Dialog.Button.ApplyTo(button);
			}

			for (int i = 0; i < Stylables.Count; i++)
			{
				style.ApplyTo(Stylables[i]);
			}

			HeaderCloseButton.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Dialog.Background.GetFrom(GetComponent<Image>());
			style.Dialog.Title.GetFrom(TitleText);

			style.Dialog.ContentBackground.GetFrom(ContentBackground);
			style.Dialog.ContentText.GetFrom(ContentText);

			style.Dialog.Delimiter.GetFrom(Delimiter);

			foreach (var button in Buttons)
			{
				style.Dialog.Button.GetFrom(button);
			}

			for (int i = 0; i < Stylables.Count; i++)
			{
				style.GetFrom(Stylables[i]);
			}

			HeaderCloseButton.GetStyle(style);

			return true;
		}
		#endregion
	}
}