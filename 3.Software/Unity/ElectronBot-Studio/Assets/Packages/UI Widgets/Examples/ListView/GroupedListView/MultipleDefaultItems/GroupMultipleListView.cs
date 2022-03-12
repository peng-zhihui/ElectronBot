namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// ListView with Group header and multiple default items.
	/// </summary>
	public class GroupMultipleListView : ListViewCustom<GroupMultipleComponent, GroupMultipleItem>
	{
		/// <summary>
		/// Template selector.
		/// </summary>
		protected class Selector : IListViewTemplateSelector<GroupMultipleComponent, GroupMultipleItem>
		{
			/// <summary>
			/// Group template.
			/// </summary>
			public GroupMultipleComponent GroupTemplate;

			/// <summary>
			/// Item checkbox template.
			/// </summary>
			public GroupMultipleComponent CheckboxTemplate;

			/// <summary>
			/// Item value template.
			/// </summary>
			public GroupMultipleComponent ValueTemplate;

			/// <inheritdoc/>
			public GroupMultipleComponent[] AllTemplates()
			{
				return new[] { GroupTemplate, CheckboxTemplate, ValueTemplate };
			}

			/// <inheritdoc/>
			public GroupMultipleComponent Select(int index, GroupMultipleItem item)
			{
				switch (item.Mode)
				{
					case GroupMultipleItem.ItemMode.Group:
						return GroupTemplate;
					case GroupMultipleItem.ItemMode.Checkbox:
						return CheckboxTemplate;
					case GroupMultipleItem.ItemMode.Value:
						return ValueTemplate;
				}

				return null;
			}
		}

		/// <summary>
		/// GroupTemplate.
		/// </summary>
		[SerializeField]
		protected GroupMultipleComponent GroupTemplate;

		/// <summary>
		/// ItemTemplate.
		/// </summary>
		[SerializeField]
		protected GroupMultipleComponent CheckboxTemplate;

		/// <summary>
		/// ItemTemplate.
		/// </summary>
		[SerializeField]
		protected GroupMultipleComponent ValueTemplate;

		Selector GroupedTemplateSelector;

		bool isGroupedListViewInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isGroupedListViewInited)
			{
				return;
			}

			isGroupedListViewInited = true;

			GroupedTemplateSelector = new Selector()
			{
				GroupTemplate = GroupTemplate,
				CheckboxTemplate = CheckboxTemplate,
				ValueTemplate = ValueTemplate,
			};

			TemplateSelector = GroupedTemplateSelector;

			base.Init();

			CanSelect = IsItem;
		}

		bool IsItem(int index)
		{
			return DataSource[index].Mode != GroupMultipleItem.ItemMode.Group;
		}
	}
}