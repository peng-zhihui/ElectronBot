namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TileViewIcons drag support.
	/// </summary>
	[RequireComponent(typeof(ListViewIconsItemComponent))]
	public class TileViewIconsDragSupport : DragSupport<ListViewIconsItemDescription>
	{
		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		public TileViewIcons TileView;

		/// <summary>
		/// The drag info.
		/// </summary>
		[SerializeField]
		public ListViewIconsItemComponent DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);

		int index;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (DragInfo != null)
			{
				DragInfo.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			var component = GetComponent<ListViewIconsItemComponent>();
			Data = component.Item;
			index = component.Index;

			ShowDragInfo();
		}

		/// <summary>
		/// Shows the drag info.
		/// </summary>
		protected virtual void ShowDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.transform.SetParent(DragPoint, false);
			DragInfo.transform.localPosition = DragInfoOffset;

			DragInfo.SetData(Data);

			DragInfo.gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides the drag info.
		/// </summary>
		protected virtual void HideDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.gameObject.SetActive(false);
		}

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			HideDragInfo();

			// remove used from current ListViewIcons.
			if (success && (TileView != null))
			{
				var first_index = TileView.DataSource.IndexOf(Data);
				var last_index = TileView.DataSource.LastIndexOf(Data);
				if (index == first_index)
				{
					TileView.DataSource.RemoveAt(index);
				}
				else if ((index + 1) == last_index)
				{
					TileView.DataSource.RemoveAt(index + 1);
				}
				else
				{
					TileView.DataSource.Remove(Data);
				}
			}

			base.Dropped(success);
		}
	}
}