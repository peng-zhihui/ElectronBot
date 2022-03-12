namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the button close.
	/// </summary>
	public class StyleSupportButtonClose : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public Image Background;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public GameObject Text;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.ButtonClose.Background.ApplyTo(Background);
			style.ButtonClose.Text.ApplyTo(Text);

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.ButtonClose.Background.GetFrom(Background);
			style.ButtonClose.Text.GetFrom(Text);

			return true;
		}
		#endregion
	}
}