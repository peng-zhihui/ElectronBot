namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the button.
	/// </summary>
	public class StyleSupportButton : MonoBehaviour, IStylable
	{
		/// <summary>
		/// The background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// The mask.
		/// </summary>
		[SerializeField]
		public Image Mask;

		/// <summary>
		/// The border.
		/// </summary>
		[SerializeField]
		public Image Border;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public GameObject Text;

		/// <summary>
		/// The image.
		/// </summary>
		[SerializeField]
		public Image Image;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.ButtonSmall.ApplyTo(this);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.ButtonSmall.GetFrom(this);

			return true;
		}
		#endregion
	}
}