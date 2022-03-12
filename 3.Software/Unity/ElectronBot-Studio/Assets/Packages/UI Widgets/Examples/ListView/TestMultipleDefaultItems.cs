namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test multiple DefaultItems.
	/// </summary>
	public class TestMultipleDefaultItems : MonoBehaviour
	{
		/// <summary>
		/// Template selector.
		/// </summary>
		protected class Selector : IListViewTemplateSelector<ListViewIconsItemComponent, ListViewIconsItemDescription>
		{
			/// <summary>
			/// Templates.
			/// </summary>
			public ListViewIconsItemComponent[] Templates;

			/// <inheritdoc/>
			public ListViewIconsItemComponent[] AllTemplates()
			{
				return Templates;
			}

			/// <inheritdoc/>
			public ListViewIconsItemComponent Select(int index, ListViewIconsItemDescription item)
			{
				return Templates[index % 2];
			}
		}

		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		protected ListViewIcons ListView;

		/// <summary>
		/// Template 1.
		/// </summary>
		[SerializeField]
		protected ListViewIconsItemComponent Template1;

		/// <summary>
		/// Template 2.
		/// </summary>
		[SerializeField]
		protected ListViewIconsItemComponent Template2;

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Start()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			ListView.TemplateSelector = new Selector()
			{
				Templates = new ListViewIconsItemComponent[] { Template1, Template2 },
			};
		}
	}
}