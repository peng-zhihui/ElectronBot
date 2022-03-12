namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// TreeViewSample template selector.
	/// </summary>
	public class TreeViewMultipleDefaultItems : MonoBehaviour, IListViewTemplateSelector<TreeViewSampleComponent, ListNode<ITreeViewSampleItem>>
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		protected TreeViewSample TreeView;

		/// <summary>
		/// ContinentTemplate.
		/// </summary>
		[SerializeField]
		protected TreeViewSampleComponentContinent ContinentTemplate;

		/// <summary>
		/// CountryTemplate.
		/// </summary>
		[SerializeField]
		protected TreeViewSampleComponentCountry CountryTemplate;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			TreeView.TemplateSelector = this;
		}

		/// <inheritdoc/>
		public TreeViewSampleComponent[] AllTemplates()
		{
			return new TreeViewSampleComponent[] { ContinentTemplate, CountryTemplate };
		}

		/// <inheritdoc/>
		public TreeViewSampleComponent Select(int index, ListNode<ITreeViewSampleItem> item)
		{
			if (item.Node.Item is TreeViewSampleItemContinent)
			{
				return ContinentTemplate;
			}

			return CountryTemplate;
		}
	}
}