namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// TreeView with checkboxes.
	/// </summary>
	public class TreeViewCheckboxes : TreeViewCustom<TreeViewCheckboxesComponent, TreeViewCheckboxesItem>
	{
		/// <summary>
		/// NodeCheckboxChanged event.
		/// </summary>
		public NodeEvent OnNodeCheckboxChanged = new NodeEvent();

		void NodeCheckboxChanged(int index)
		{
			OnNodeCheckboxChanged.Invoke(DataSource[index].Node);
		}

		/// <summary>
		/// Add callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void AddCallback(TreeViewCheckboxesComponent item)
		{
			item.NodeCheckboxChanged.AddListener(NodeCheckboxChanged);
			base.AddCallback(item);
		}

		/// <summary>
		/// Remove callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void RemoveCallback(TreeViewCheckboxesComponent item)
		{
			item.NodeCheckboxChanged.RemoveListener(NodeCheckboxChanged);
			base.RemoveCallback(item);
		}
	}
}