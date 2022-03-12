namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileViewSample DataSource.
	/// </summary>
	[RequireComponent(typeof(TileViewSample))]
	public class TileViewSampleDataSource : MonoBehaviour
	{
		/// <summary>
		/// Items.
		/// </summary>
		[SerializeField]
		protected int Items = 40;

		/// <summary>
		/// Start this instances.
		/// </summary>
		protected virtual void Start()
		{
			GenerateItems(Items);
		}

		/// <summary>
		/// Generate DataSource with specified items count.
		/// </summary>
		/// <param name="count">Items count.</param>
		public void GenerateItems(int count)
		{
			var tiles = GetComponent<TileViewSample>();

			tiles.DataSource = UtilitiesCollections.CreateList(count, x =>
			{
				return new TileViewItemSample()
				{
					Name = string.Format("Tile {0}", x.ToString()),
					Capital = string.Empty,
					Area = Random.Range(10, 10 * 6),
					Population = Random.Range(100, 100 * 6),
				};
			});
		}
	}
}