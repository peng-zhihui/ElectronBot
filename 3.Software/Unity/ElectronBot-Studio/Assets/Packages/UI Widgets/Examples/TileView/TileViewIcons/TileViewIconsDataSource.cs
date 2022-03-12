namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileViewIcons DataSource.
	/// </summary>
	[RequireComponent(typeof(TileViewIcons))]
	public class TileViewIconsDataSource : MonoBehaviour
	{
		/// <summary>
		/// Icons.
		/// </summary>
		[SerializeField]
		protected List<Sprite> Icons;

		/// <summary>
		/// Generate and set TileViewIcons DataSource.
		/// </summary>
		protected virtual void Start()
		{
			var n = Icons.Count - 1;
			var tiles = GetComponent<TileViewIcons>();
			tiles.Init();

			var count = 140;
			var items = new ObservableList<ListViewIconsItemDescription>(count);
			items.BeginUpdate();
			for (int i = 0; i < count; i++)
			{
				var item = new ListViewIconsItemDescription()
				{
					Name = string.Format("Tile {0}", i.ToString()),
					Icon = Icons.Count > 0 ? Icons[Random.Range(0, n)] : null,
				};
				items.Add(item);
			}

			items.EndUpdate();

			tiles.DataSource = items;
		}
	}
}