namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// TreeViewSample component.
	/// </summary>
	public class TreeViewSampleComponent : TreeViewComponentBase<ITreeViewSampleItem>
	{
		ITreeViewSampleItem item;

		/// <summary>
		/// Item.
		/// </summary>
		public ITreeViewSampleItem Item
		{
			get
			{
				return item;
			}

			set
			{
				if (item != null)
				{
					item.OnChange -= UpdateView;
				}

				item = value;
				if (item != null)
				{
					item.OnChange += UpdateView;
				}

				UpdateView();
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(TreeNode<ITreeViewSampleItem> node, int depth)
		{
			base.SetData(node, depth);

			Item = (node == null) ? null : node.Item;
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (Item == null)
			{
				Icon.sprite = null;
				TextAdapter.text = string.Empty;
			}
			else
			{
				Item.Display(this);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (item != null)
			{
				item.OnChange -= UpdateView;
			}
		}
	}
}