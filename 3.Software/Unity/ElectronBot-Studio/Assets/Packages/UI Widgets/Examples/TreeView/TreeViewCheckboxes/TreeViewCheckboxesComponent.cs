namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeViewCheckboxes component.
	/// </summary>
	public class TreeViewCheckboxesComponent : TreeViewComponentBase<TreeViewCheckboxesItem>
	{
		TreeViewCheckboxesItem item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public TreeViewCheckboxesItem Item
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
		/// NodeCheckboxChanged event.
		/// </summary>
		public NodeToggleEvent NodeCheckboxChanged = new NodeToggleEvent();

		/// <summary>
		/// Checkbox.
		/// </summary>
		public Toggle Checkbox;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(TreeNode<TreeViewCheckboxesItem> node, int depth)
		{
			Node = node;
			base.SetData(Node, depth);

			Item = (Node == null) ? null : Node.Item;
		}

		/// <summary>
		/// Handle toggle changed event.
		/// </summary>
		public void ToggleChanged()
		{
			if (Item.Selected == Checkbox.isOn)
			{
				return;
			}

			Item.Selected = Checkbox.isOn;
			NodeCheckboxChanged.Invoke(Index);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if ((Icon == null) || (TextAdapter == null))
			{
				return;
			}

			if (Item == null)
			{
				Icon.sprite = null;
				TextAdapter.text = string.Empty;
				Checkbox.isOn = false;
			}
			else
			{
				Icon.sprite = Item.Icon;
				TextAdapter.text = Item.LocalizedName ?? Item.Name;
				Checkbox.isOn = Item.Selected;
			}

			if (SetNativeSize)
			{
				Icon.SetNativeSize();
			}

			// set transparent color if no icon
			Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
			if (Icon != null)
			{
				Icon.sprite = null;
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if (item != null)
			{
				item.OnChange -= UpdateView;
			}

			base.OnDestroy();
		}
	}
}