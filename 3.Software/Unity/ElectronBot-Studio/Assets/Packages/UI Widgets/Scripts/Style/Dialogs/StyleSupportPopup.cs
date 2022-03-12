namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the popup.
	/// </summary>
	public class StyleSupportPopup : MonoBehaviour, IStylable
	{
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
		/// The header close button.
		/// </summary>
		[SerializeField]
		public StyleSupportButtonClose HeaderCloseButton;

		/// <summary>
		/// The close button.
		/// </summary>
		[SerializeField]
		public StyleSupportButton CloseButton;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Dialog.ContentBackground.ApplyTo(ContentBackground);
			style.Dialog.Delimiter.ApplyTo(Delimiter);
			style.Dialog.Button.ApplyTo(CloseButton);

			HeaderCloseButton.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Dialog.ContentBackground.GetFrom(ContentBackground);
			style.Dialog.Delimiter.GetFrom(Delimiter);
			style.Dialog.Button.GetFrom(CloseButton);

			HeaderCloseButton.GetStyle(style);

			return true;
		}
		#endregion
	}
}