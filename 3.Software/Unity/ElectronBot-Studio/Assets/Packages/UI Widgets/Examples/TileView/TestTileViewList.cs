namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test TileViewList.
	/// </summary>
	public class TestTileViewList : MonoBehaviour
	{
		/// <summary>
		/// File with test data.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("sourceFile")]
		protected TextAsset SourceFile;

		/// <summary>
		/// TileView.
		/// </summary>
		[SerializeField]
		protected TileViewList TileView;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			LoadData();
		}

		/// <summary>
		/// Load data.
		/// </summary>
		protected virtual void LoadData()
		{
			var files = SourceFile.text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			var items = new ObservableList<ListViewIconsItemDescription>();
			foreach (var file in files)
			{
				items.Add(new ListViewIconsItemDescription() { Name = file.Trim() });
			}

			TileView.DataSource = items;
		}
	}
}