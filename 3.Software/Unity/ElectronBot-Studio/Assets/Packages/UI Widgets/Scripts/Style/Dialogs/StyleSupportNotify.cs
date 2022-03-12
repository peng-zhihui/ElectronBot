namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the notification.
	/// </summary>
	public class StyleSupportNotify : MonoBehaviour, IStylable
	{
		/// <summary>
		/// The background.
		/// </summary>
		[SerializeField]
		public Image Background;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Notify.Background.ApplyTo(Background);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Notify.Background.GetFrom(Background);

			return true;
		}
		#endregion
	}
}