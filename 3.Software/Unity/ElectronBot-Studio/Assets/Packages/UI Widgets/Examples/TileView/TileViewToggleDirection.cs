namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TileView toggle direction.
	/// </summary>
	public class TileViewToggleDirection : MonoBehaviour
	{
		/// <summary>
		/// TileViewSample.
		/// </summary>
		[SerializeField]
		protected TileViewSample Tiles;

		/// <summary>
		/// Vertical scrollbar.
		/// </summary>
		[SerializeField]
		protected Scrollbar VerticalScrollbar;

		/// <summary>
		/// Horizontal scrollbar.
		/// </summary>
		[SerializeField]
		protected Scrollbar HorizontalScrollbar;

		/// <summary>
		/// Vertical paginator.
		/// </summary>
		[SerializeField]
		protected ScrollRectPaginator VerticalPaginator;

		/// <summary>
		/// Horizontal paginator.
		/// </summary>
		[SerializeField]
		protected ScrollRectPaginator HorizontalPaginator;

		/// <summary>
		/// ListView paginator.
		/// </summary>
		[SerializeField]
		protected ListViewPaginator ListViewPaginator;

		/// <summary>
		/// Toggle direction.
		/// </summary>
		public void ToggleDirection()
		{
			if (ListViewPaginator != null)
			{
				ListViewPaginator.StopAnimation();
			}

			if (Tiles.Direction == ListViewDirection.Horizontal)
			{
				VerticalScrollbar.gameObject.SetActive(true);

				Tiles.Direction = ListViewDirection.Vertical;
				Tiles.ScrollRect.horizontalScrollbar.value = 1;
				Tiles.ScrollRect.horizontalScrollbar = null;
				Tiles.ScrollRect.verticalScrollbar = VerticalScrollbar;

				HorizontalScrollbar.gameObject.SetActive(false);

				if (HorizontalPaginator != null)
				{
					HorizontalPaginator.StopAnimation();
					HorizontalPaginator.gameObject.SetActive(false);
				}

				if (VerticalPaginator != null)
				{
					VerticalPaginator.StopAnimation();
					VerticalPaginator.gameObject.SetActive(true);
				}
			}
			else
			{
				HorizontalScrollbar.gameObject.SetActive(true);

				Tiles.Direction = ListViewDirection.Horizontal;
				Tiles.ScrollRect.horizontalScrollbar = HorizontalScrollbar;
				Tiles.ScrollRect.verticalScrollbar.value = 1;
				Tiles.ScrollRect.verticalScrollbar = null;

				VerticalScrollbar.gameObject.SetActive(false);

				if (HorizontalPaginator != null)
				{
					HorizontalPaginator.StopAnimation();
					HorizontalPaginator.gameObject.SetActive(true);
				}

				if (VerticalPaginator != null)
				{
					VerticalPaginator.StopAnimation();
					VerticalPaginator.gameObject.SetActive(false);
				}
			}
		}
	}
}