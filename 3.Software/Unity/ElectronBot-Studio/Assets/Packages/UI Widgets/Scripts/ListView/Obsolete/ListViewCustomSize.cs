namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView for items with variable size.
	/// </summary>
	/// <typeparam name="TComponent">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public class ListViewCustomSize<TComponent, TItem> : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		[UnityEngine.SerializeField]
		[UnityEngine.HideInInspector]
		int listViewCustomSizeVersion = 0;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

			if (listViewCustomSizeVersion == 0)
			{
				listType = ListViewType.ListViewWithVariableSize;

				listViewCustomSizeVersion = 1;
			}
		}
	}
}