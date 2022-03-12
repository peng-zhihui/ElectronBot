namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test changing TileView DefaultItem.
	/// </summary>
	public class TileViewDefaultItemChange : MonoBehaviour
	{
		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		protected TileViewIcons TileView;

		/// <summary>
		/// Original DefaultItem.
		/// </summary>
		[SerializeField]
		protected ListViewIconsItemComponent DefaultItemOriginal;

		/// <summary>
		/// New DefaultItem.
		/// </summary>
		[SerializeField]
		protected ListViewIconsItemComponent DefaultItemNew;

		/// <summary>
		/// Set original default item.
		/// </summary>
		public void SetOriginal()
		{
			TileView.DefaultItem = DefaultItemOriginal;
		}

		/// <summary>
		/// Set new default item.
		/// </summary>
		public void SetNew()
		{
			TileView.DefaultItem = DefaultItemNew;
		}
	}
}