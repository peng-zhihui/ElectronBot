namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the header cell.
	/// </summary>
	public class StyleSupportHeaderCell : MonoBehaviour, IStylable
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
			style.Table.Background.ApplyTo(Background);
			style.Table.HeaderText.ApplyTo(Text);

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.Table.Background.GetFrom(Background);
			style.Table.HeaderText.GetFrom(Text);

			return true;
		}
		#endregion
	}
}