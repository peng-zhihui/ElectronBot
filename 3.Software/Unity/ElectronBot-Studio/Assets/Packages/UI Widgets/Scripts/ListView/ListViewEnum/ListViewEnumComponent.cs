namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewEnum item component.
	/// </summary>
	public class ListViewEnumComponent : ListViewItem, IViewData<ListViewEnum.Item>
	{
		/// <summary>
		/// The text adapter.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Strike through.
		/// </summary>
		[SerializeField]
		public Graphic Strikethrough;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public ListViewEnum.Item Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ListViewEnum.Item item)
		{
#if UNITY_EDITOR
			name = item.Name == null ? "DefaultItem " + Index.ToString() : item.Name;
#endif

			Item = item;
			UpdateView();
		}

		/// <summary>
		/// Update display name.
		/// </summary>
		protected void UpdateView()
		{
			if (Name != null)
			{
				Name.text = Item.Name;
			}

			if (Strikethrough != null)
			{
				Strikethrough.enabled = Item.IsObsolete;
			}
		}
	}
}