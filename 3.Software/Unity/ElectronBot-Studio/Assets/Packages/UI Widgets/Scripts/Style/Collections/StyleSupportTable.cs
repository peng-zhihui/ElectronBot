namespace UIWidgets.Styles
{
	using UnityEngine;

	/// <summary>
	/// Style support for the table.
	/// </summary>
	public class StyleSupportTable : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public TableHeader Header;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			Header.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			Header.GetStyle(style);

			return true;
		}
		#endregion
	}
}