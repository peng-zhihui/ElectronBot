namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Drag support of selected items for the ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class ListViewIconsMultipleDragSupport : DragSupport<List<ListViewIconsItemDescription>>
	{
		/// <summary>
		/// Delete selected items after drop.
		/// </summary>
		[SerializeField]
		public bool DeleteAfterDrop;

		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewIcons ListView;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			ListView = GetComponent<ListViewIcons>();
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			ListView = GetComponent<ListViewIcons>();
			Data = ListView.SelectedItems;
		}

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			if (success && DeleteAfterDrop)
			{
				ListView.DataSource.BeginUpdate();

				foreach (var item in Data)
				{
					ListView.DataSource.Remove(item);
				}

				ListView.DataSource.EndUpdate();
			}

			base.Dropped(success);
		}
	}
}