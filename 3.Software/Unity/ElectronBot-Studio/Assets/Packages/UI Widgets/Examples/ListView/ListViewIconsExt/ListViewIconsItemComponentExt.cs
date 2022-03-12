namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewIconsItem extended component.
	/// </summary>
	public class ListViewIconsItemComponentExt : ListViewIconsItemComponent, IViewData<ListViewIconsItemDescriptionExt>
	{
		/// <summary>
		/// Gets the current item.
		/// </summary>
		public ListViewIconsItemDescriptionExt ItemExt
		{
			get;
			protected set;
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ListViewIconsItemDescriptionExt item)
		{
			ItemExt = item;
			SetData(item as ListViewIconsItemDescription);
		}
	}
}