namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// FileListViewComponentBase.
	/// </summary>
	public class FileListViewComponentBase : ListViewItem, IViewData<FileSystemEntry>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected TextAdapter NameAdapter;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(NameAdapter), };
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Icon.
		/// </summary>
		[SerializeField]
		protected Image Icon;

		/// <summary>
		/// Directory icon.
		/// </summary>
		[SerializeField]
		protected Sprite DirectoryIcon;

		/// <summary>
		/// Current item.
		/// </summary>
		protected FileSystemEntry Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(FileSystemEntry item)
		{
			Item = item;

			Icon.sprite = GetIcon(item);
			Icon.color = Icon.sprite == null ? Color.clear : Color.white;

			NameAdapter.text = item.DisplayName;
		}

		/// <summary>
		/// Get icon for specified FileSystemEntry.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Icon for specified FileSystemEntry.</returns>
		public virtual Sprite GetIcon(FileSystemEntry item)
		{
			if (item.IsDirectory)
			{
				return DirectoryIcon;
			}

			return null;
		}
	}
}