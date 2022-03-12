namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SimpleTileView component.
	/// </summary>
	public class SimpleTileViewComponent : ListViewItem, IViewData<SimpleTileViewItem>
	{
		/// <summary>
		/// The name of the item.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with ItemNameAdapter.")]
		public Text ItemName;

		/// <summary>
		/// The text of the item.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with ItemTextAdapter.")]
		public Text ItemText;

		/// <summary>
		/// The name of the item.
		/// </summary>
		[SerializeField]
		public TextAdapter ItemNameAdapter;

		/// <summary>
		/// The text of the item.
		/// </summary>
		[SerializeField]
		public TextAdapter ItemTextAdapter;

		#region IViewData implementation

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SimpleTileViewItem item)
		{
			ItemNameAdapter.text = item.Name;

			ItemTextAdapter.text = item.Text.Replace("\\n", "\n");
		}
		#endregion

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(ItemName, ref ItemNameAdapter);
			Utilities.GetOrAddComponent(ItemText, ref ItemTextAdapter);
#pragma warning restore 0612, 0618
		}
	}
}