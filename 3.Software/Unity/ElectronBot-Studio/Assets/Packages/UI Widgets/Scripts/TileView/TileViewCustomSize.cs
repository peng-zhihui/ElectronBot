namespace UIWidgets
{
	/// <summary>
	/// Base class for TileView's with items with different widths or heights.
	/// </summary>
	/// <typeparam name="TComponent">Component class.</typeparam>
	/// <typeparam name="TItem">Item class.</typeparam>
	public class TileViewCustomSize<TComponent, TItem> : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		int tileViewCustomSizeVersion = 0;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

			if (tileViewCustomSizeVersion == 0)
			{
				listType = ListViewType.TileViewWithVariableSize;

				tileViewCustomSizeVersion = 1;
			}
		}
	}
}