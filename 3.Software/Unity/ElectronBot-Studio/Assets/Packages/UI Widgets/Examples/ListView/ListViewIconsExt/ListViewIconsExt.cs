namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// ListViewIcons extended with Visible and Interactable options.
	/// </summary>
	public class ListViewIconsExt : ListViewCustom<ListViewIconsItemComponentExt, ListViewIconsItemDescriptionExt>
	{
		/// <summary>
		/// Extended DataSource.
		/// </summary>
		protected ObservableList<ListViewIconsItemDescriptionExt> dataSourceExt;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public virtual ObservableList<ListViewIconsItemDescriptionExt> DataSourceExt
		{
			get
			{
				if (dataSourceExt == null)
				{
					dataSourceExt = new ObservableList<ListViewIconsItemDescriptionExt>(dataSource);
					dataSourceExt.OnChange += CollectionChanged;

					CollectionChanged();
				}

				return dataSourceExt;
			}

			set
			{
				if (dataSourceExt != null)
				{
					dataSourceExt.OnChange -= CollectionChanged;
				}

				dataSourceExt = value;
				dataSourceExt.OnChange += CollectionChanged;

				CollectionChanged();
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (dataSourceExt == null)
			{
				if (dataSource == null)
				{
					dataSource = new ObservableList<ListViewIconsItemDescriptionExt>(customItems);
					dataSource.OnChange += UpdateItems;
				}

				dataSourceExt = new ObservableList<ListViewIconsItemDescriptionExt>(dataSource);
				dataSourceExt.OnChange += CollectionChanged;

				CollectionChanged();
			}

			base.Init();

			DataSource.ObserveItems = false;
		}

		/// <summary>
		/// Updates the data source.
		/// </summary>
		protected void CollectionChanged()
		{
			DataSource.BeginUpdate();

			DataSource.Clear();

			foreach (var item in DataSourceExt)
			{
				if (item.Visible)
				{
					DataSource.Add(item);
				}
			}

			DataSource.EndUpdate();
		}

		/// <summary>
		/// Tests the visible.
		/// </summary>
		/// <param name="index">Index.</param>
		public void TestVisible(int index)
		{
			DataSourceExt[index].Visible = !DataSourceExt[index].Visible;
		}

		/// <summary>
		/// Tests the interactable.
		/// </summary>
		/// <param name="index">Index.</param>
		public void TestInteractable(int index)
		{
			DataSourceExt[index].Interactable = !DataSourceExt[index].Interactable;
		}

		/// <summary>
		/// The color of the disabled item.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("DisabledColor")]
		public Color ItemDisabledColor = Color.gray;

		/// <summary>
		/// The background color of the disabled item.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("DisabledBackgroundColor")]
		public Color ItemDisabledBackgroundColor = Color.gray;

		/// <summary>
		/// Determines whether if item with specified index can be selected.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="index">Index.</param>
		protected override bool CanBeSelected(int index)
		{
			return DataSource[index].Interactable;
		}

		/// <summary>
		/// Determines whether if item with specified index can be deselected.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="index">Index.</param>
		protected override bool CanBeDeselected(int index)
		{
			return DataSource[index].Interactable;
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewIconsItemComponentExt component)
		{
			if (component == null)
			{
				return;
			}

			if (DataSource[component.Index].Interactable)
			{
				base.HighlightColoring(component);
			}
			else
			{
				component.GraphicsColoring(ItemDisabledColor, ItemDisabledBackgroundColor, FadeDuration);
			}
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(ListViewIconsItemComponentExt component)
		{
			if (component == null)
			{
				return;
			}

			if (DataSource[component.Index].Interactable)
			{
				base.SelectColoring(component);
			}
			else
			{
				component.GraphicsColoring(ItemDisabledColor, ItemDisabledBackgroundColor, FadeDuration);
			}
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(ListViewIconsItemComponentExt component)
		{
			if (component == null)
			{
				return;
			}

			if (DataSource[component.Index].Interactable)
			{
				base.DefaultColoring(component);
			}
			else
			{
				component.GraphicsColoring(ItemDisabledColor, ItemDisabledBackgroundColor, FadeDuration);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (dataSourceExt != null)
			{
				dataSourceExt.OnChange -= CollectionChanged;
			}

			base.OnDestroy();
		}
	}
}