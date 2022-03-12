namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Base class for DrivesListViewComponent.
	/// </summary>
	public class DrivesListViewComponentBase : ListViewItem, IViewData<FileSystemEntry>
	{
		/// <summary>
		/// Text component to display drive name.
		/// </summary>
		[SerializeField]
		protected TextAdapter NameAdapter;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		/// <value>Current item.</value>
		public FileSystemEntry Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(FileSystemEntry item)
		{
			Item = item;

			NameAdapter.text = Item.DisplayName;
		}

		/// <inheritdoc/>
		public override void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.SetStyle(styleBackground, styleText, style);

			if (NameAdapter != null)
			{
				styleText.ApplyTo(NameAdapter.gameObject);
			}
		}

		/// <inheritdoc/>
		public override void GetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.GetStyle(styleBackground, styleText, style);

			if (NameAdapter != null)
			{
				styleText.GetFrom(NameAdapter.gameObject);
			}
		}
	}
}