namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Reorder TileView items.
	/// </summary>
	public class TileViewReorder : MonoBehaviour
	{
		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		public TileViewSample Tiles;

		/// <summary>
		/// Original items.
		/// </summary>
		public ObservableList<TileViewItemSample> OriginalItems = new ObservableList<TileViewItemSample>();

		/// <summary>
		/// Empty item.
		/// </summary>
		[SerializeField]
		public TileViewItemSample EmptyItem = null;

		/// <summary>
		/// Per row.
		/// </summary>
		protected int PerRow = 1;

		/// <summary>
		/// Per column.
		/// </summary>
		protected int PerColumn = 1;

		/// <summary>
		/// Per page.
		/// </summary>
		protected int PerPage = 1;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			OriginalItems = UtilitiesCollections.CreateList(40, x =>
			{
				return new TileViewItemSample()
				{
					Name = string.Format("Tile {0}", x.ToString()),
					Capital = string.Empty,
					Area = Random.Range(10, 10 * 6),
					Population = Random.Range(100, 100 * 6),
				};
			});

			OriginalItems.OnChange += ReorderItems;

			SizeChanged();

			var listener = Utilities.GetOrAddComponent<ResizeListener>(Tiles.ScrollRect);
			listener.OnResizeNextFrame.AddListener(SizeChanged);
		}

		/// <summary>
		/// Handle the size changed event.
		/// </summary>
		public void SizeChanged()
		{
			var size = (Tiles.ScrollRect.transform as RectTransform).rect.size;
			var margin = Tiles.GetLayoutMargin();
			size.x -= margin.x + margin.y;
			size.y -= margin.z + margin.w;

			PerRow = Mathf.Max(1, Mathf.FloorToInt((size.x + Tiles.GetItemSpacingX()) / (Tiles.GetDefaultItemWidth() + Tiles.GetItemSpacingX())));
			PerColumn = Mathf.Max(1, Mathf.FloorToInt((size.y + Tiles.GetItemSpacingY()) / (Tiles.GetDefaultItemHeight() + Tiles.GetItemSpacingY())));
			PerPage = PerRow * PerColumn;

			ReorderItems();
		}

		/// <summary>
		/// Reorder items.
		/// </summary>
		public void ReorderItems()
		{
			var items = Tiles.DataSource;
			items.BeginUpdate();
			items.Clear();

			var total = Mathf.CeilToInt((float)OriginalItems.Count / (float)PerPage) * PerPage;
			for (int i = 0; i < total; i++)
			{
				var k = ((i / PerPage) * PerPage) + ((i % PerColumn) * PerRow) + ((i % PerPage) / PerColumn);
				items.Add((k < OriginalItems.Count) ? OriginalItems[k] : EmptyItem);
			}

			items.EndUpdate();
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (OriginalItems != null)
			{
				OriginalItems.OnChange -= ReorderItems;
			}

			if ((Tiles != null) && (Tiles.ScrollRect != null))
			{
				var listener = Tiles.ScrollRect.GetComponent<ResizeListener>();
				if (listener != null)
				{
					listener.OnResizeNextFrame.RemoveListener(SizeChanged);
				}
			}
		}
	}
}