namespace UIWidgets
{
	/// <summary>
	/// Base class for TileView's.
	/// </summary>
	/// <typeparam name="TComponent">Component class.</typeparam>
	/// <typeparam name="TItem">Item class.</typeparam>
	public class TileViewCustom<TComponent, TItem> : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		int tileViewFixedSizeVersion = 0;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

			if (tileViewFixedSizeVersion == 0)
			{
				listType = ListViewType.TileViewWithFixedSize;

				tileViewFixedSizeVersion = 1;
			}
		}
	}
}