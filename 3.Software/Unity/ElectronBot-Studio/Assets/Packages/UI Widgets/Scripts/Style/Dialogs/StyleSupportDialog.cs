namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the dialog.
	/// </summary>
	public class StyleSupportDialog : MonoBehaviour, IStylable
	{
		/// <summary>
		/// The title.
		/// </summary>
		[SerializeField]
		public GameObject Title;

		/// <summary>
		/// The content background.
		/// </summary>
		[SerializeField]
		public Image ContentBackground;

		/// <summary>
		/// The delimiter.
		/// </summary>
		[SerializeField]
		public Image Delimiter;

		/// <summary>
		/// The buttons.
		/// </summary>
		[SerializeField]
		public List<StyleSupportButton> Buttons;

		/// <summary>
		/// The close button.
		/// </summary>
		[SerializeField]
		public StyleSupportButtonClose CloseButton;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Dialog.ContentBackground.ApplyTo(ContentBackground);
			style.Dialog.Delimiter.ApplyTo(Delimiter);

			if (Buttons != null)
			{
				foreach (var button in Buttons)
				{
					style.Dialog.Button.ApplyTo(button);
				}
			}

			CloseButton.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Dialog.ContentBackground.GetFrom(ContentBackground);
			style.Dialog.Delimiter.GetFrom(Delimiter);

			if (Buttons != null)
			{
				foreach (var button in Buttons)
				{
					style.Dialog.Button.GetFrom(button);
				}
			}

			CloseButton.GetStyle(style);

			return true;
		}
		#endregion
	}
}