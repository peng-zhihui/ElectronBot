namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// ListViewIcons drag support.
	/// </summary>
	[RequireComponent(typeof(ListViewStringItemComponent))]
	public class ListViewStringDragSupport : ListViewCustomDragSupport<ListViewString, ListViewStringItemComponent, string>
	{
		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected override string GetData(ListViewStringItemComponent component)
		{
			return component.Item;
		}

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected override void SetDragInfoData(string data)
		{
			DragInfo.SetData(data);
		}
	}
}