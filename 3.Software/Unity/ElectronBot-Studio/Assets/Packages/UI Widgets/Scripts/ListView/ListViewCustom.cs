namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Threading;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	/// <typeparam name="TComponent">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed.")]
	[DataBindSupport]
	public partial class ListViewCustom<TComponent, TItem> : ListViewCustomBase, IUpdatable, ILateUpdatable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// Default template selector.
		/// </summary>
		protected class DefaultSelector : IListViewTemplateSelector<TComponent, TItem>
		{
			/// <summary>
			/// Template.
			/// </summary>
			protected TComponent Template;

			/// <summary>
			/// Templates.
			/// </summary>
			protected TComponent[] Templates;

			/// <summary>
			/// Initializes a new instance of the <see cref="DefaultSelector"/> class.
			/// </summary>
			/// <param name="template">Template.</param>
			public DefaultSelector(TComponent template)
			{
				Template = template;
				Templates = new[] { template };
			}

			/// <summary>
			/// Replace template.
			/// </summary>
			/// <param name="template">Template.</param>
			public void Replace(TComponent template)
			{
				Template = template;
				Templates = new[] { template };
			}

			/// <inheritdoc/>
			public TComponent Select(int index, TItem item)
			{
				return Template;
			}

			/// <inheritdoc/>
			public TComponent[] AllTemplates()
			{
				return Templates;
			}
		}

		/// <summary>
		/// Template class.
		/// </summary>
		[Serializable]
		public class Template : ListViewItemTemplate<TComponent>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Template"/> class.
			/// </summary>
			public Template()
				: base()
			{
			}
		}

		/// <summary>
		/// DataSourceEvent.
		/// </summary>
		public class DataSourceEvent : UnityEvent<ListViewCustom<TComponent, TItem>>
		{
		}

		/// <inheritdoc/>
		public override ListViewType ListType
		{
			get
			{
				return listType;
			}

			set
			{
				listType = value;

				if (listRenderer != null)
				{
					listRenderer.Disable();
					listRenderer = null;
				}

				if (isListViewCustomInited)
				{
					SetDefaultItem(defaultItem);
				}
			}
		}

		/// <summary>
		/// The items.
		/// </summary>
		[SerializeField]
		protected List<TItem> customItems = new List<TItem>();

		/// <summary>
		/// Data source.
		/// </summary>
		#if UNITY_2020_1_OR_NEWER
		[NonSerialized]
		#endif
		protected ObservableList<TItem> dataSource;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		[DataBindField]
		public virtual ObservableList<TItem> DataSource
		{
			get
			{
				if (dataSource == null)
				{
					dataSource = new ObservableList<TItem>(customItems);
					dataSource.OnChange += UpdateItems;
				}

				if (!isListViewCustomInited)
				{
					Init();
				}

				return dataSource;
			}

			set
			{
				if (!isListViewCustomInited)
				{
					Init();
				}

				SetNewItems(value, IsMainThread);

				if (IsMainThread)
				{
					ListRenderer.SetPosition(0f);
				}
				else
				{
					DataSourceSetted = true;
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("DefaultItem")]
		TComponent defaultItem;

		/// <summary>
		/// The default item template.
		/// </summary>
		public TComponent DefaultItem
		{
			get
			{
				return defaultItem;
			}

			set
			{
				SetDefaultItem(value);
			}
		}

		#region ComponentPool fields

		/// <summary>
		/// The components list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TComponent> Components = new List<TComponent>();

		/// <summary>
		/// The templates list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<Template> Templates = new List<Template>();

		/// <summary>
		/// The components displayed indices.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<int> ComponentsDisplayedIndices = new List<int>();

		/// <inheritdoc/>
		public override bool DestroyDefaultItemsCache
		{
			get
			{
				return destroyDefaultItemsCache;
			}

			set
			{
				destroyDefaultItemsCache = value;
			}
		}

		ListViewComponentPool componentsPool;

		/// <summary>
		/// The components pool.
		/// Constructor with lists needed to avoid lost connections when instantiated copy of the inited ListView.
		/// </summary>
		protected virtual ListViewComponentPool ComponentsPool
		{
			get
			{
				if (componentsPool == null)
				{
					componentsPool = new ListViewComponentPool(this);
					componentsPool.Init();
				}

				return componentsPool;
			}
		}

		#endregion

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		/// <value>The selected item.</value>
		[DataBindField]
		public TItem SelectedItem
		{
			get
			{
				if (SelectedIndex == -1)
				{
					return default(TItem);
				}

				return DataSource[SelectedIndex];
			}
		}

		/// <summary>
		/// Selected items.
		/// </summary>
		[DataBindField]
		public List<TItem> SelectedItems
		{
			get
			{
				var result = new List<TItem>(selectedIndices.Count);
				GetSelectedItems(result);

				return result;
			}

			set
			{
				SetSelectedItems(value, true);
			}
		}

		[Obsolete("Replaced with DataSource.Comparison.")]
		Func<IEnumerable<TItem>, IEnumerable<TItem>> sortFunc;

		/// <summary>
		/// Sort function.
		/// Deprecated. Replaced with DataSource.Comparison.
		/// </summary>
		[Obsolete("Replaced with DataSource.Comparison.")]
		public Func<IEnumerable<TItem>, IEnumerable<TItem>> SortFunc
		{
			get
			{
				return sortFunc;
			}

			set
			{
				sortFunc = value;
				if (Sort && isListViewCustomInited)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// What to do when the object selected.
		/// </summary>
		[DataBindEvent("SelectedItem", "SelectedItems")]
		[SerializeField]
		public ListViewCustomEvent OnSelectObject = new ListViewCustomEvent();

		/// <summary>
		/// What to do when the object deselected.
		/// </summary>
		[DataBindEvent("SelectedItem", "SelectedItems")]
		[SerializeField]
		public ListViewCustomEvent OnDeselectObject = new ListViewCustomEvent();

		/// <summary>
		/// Called after component instantiated.
		/// </summary>
		public event Action<TComponent> OnComponentCreated;

		/// <summary>
		/// Called before component destroyed.
		/// </summary>
		public event Action<TComponent> OnComponentDestroyed;

		/// <summary>
		/// Called after component activated.
		/// </summary>
		public event Action<TComponent> OnComponentActivated;

		/// <summary>
		/// Called after component cached.
		/// </summary>
		public event Action<TComponent> OnComponentCached;

		#region ListRenderer fields

		/// <summary>
		/// The layout elements of the DefaultItem.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ILayoutElement> LayoutElements = new List<ILayoutElement>();

		#endregion

		[SerializeField]
		[HideInInspector]
		ListViewTypeBase listRenderer;

		/// <summary>
		/// ListView renderer.
		/// </summary>
		protected ListViewTypeBase ListRenderer
		{
			get
			{
				if (listRenderer == null)
				{
					listRenderer = GetRenderer(ListType);
				}

				return listRenderer;
			}

			set
			{
				listRenderer = value;
			}
		}

		/// <inheritdoc/>
		public override int MaxVisibleItems
		{
			get
			{
				Init();

				return ListRenderer.MaxVisibleItems;
			}
		}

		/// <summary>
		/// Selected items cache (to keep valid selected indices with updates).
		/// </summary>
		protected List<TItem> SelectedItemsCache = new List<TItem>();

		/// <inheritdoc/>
		protected override ILayoutBridge LayoutBridge
		{
			get
			{
				if (layoutBridge == null)
				{
					if (Layout != null)
					{
						layoutBridge = new EasyLayoutBridge(Layout, DefaultItem.transform as RectTransform, setContentSizeFitter && ListRenderer.AllowSetContentSizeFitter, ListRenderer.AllowControlRectTransform)
						{
							IsHorizontal = IsHorizontal(),
						};
						ListRenderer.DirectionChanged();
					}
					else
					{
						var hv_layout = Container.GetComponent<HorizontalOrVerticalLayoutGroup>();
						if (hv_layout != null)
						{
							layoutBridge = new StandardLayoutBridge(hv_layout, DefaultItem.transform as RectTransform, setContentSizeFitter && ListRenderer.AllowSetContentSizeFitter);
						}
					}
				}

				return layoutBridge;
			}
		}

		/// <inheritdoc/>
		public override bool LoopedListAvailable
		{
			get
			{
				return LoopedList && Virtualization && ListRenderer.IsVirtualizationSupported() && ListRenderer.AllowLoopedList;
			}
		}

		/// <summary>
		/// Raised when DataSource changed.
		/// </summary>
		public DataSourceEvent OnDataSourceChanged = new DataSourceEvent();

		DefaultSelector defaultTemplateSelector;

		/// <summary>
		/// Default template selector.
		/// </summary>
		protected DefaultSelector DefaultTemplateSelector
		{
			get
			{
				if (defaultTemplateSelector == null)
				{
					if (DefaultItem == null)
					{
						Debug.LogError("ListView.DefaultItem is not specified.", this);
					}

					defaultTemplateSelector = new DefaultSelector(DefaultItem);
				}

				return defaultTemplateSelector;
			}
		}

		IListViewTemplateSelector<TComponent, TItem> templateSelector;

		/// <summary>
		/// Template selector.
		/// </summary>
		public IListViewTemplateSelector<TComponent, TItem> TemplateSelector
		{
			get
			{
				if (templateSelector == null)
				{
					templateSelector = DefaultTemplateSelector;
				}

				return templateSelector;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				if (ReferenceEquals(templateSelector, value))
				{
					return;
				}

				templateSelector = value;

				if ((componentsPool != null) && DestroyDefaultItemsCache)
				{
					componentsPool.Destroy(templateSelector.AllTemplates());
				}

				if (isListViewCustomInited)
				{
					TemplatesChanged();
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("stopScrollAtItemCenter")]
		[Tooltip("Custom scroll inertia until reach item center.")]
		bool scrollInertiaUntilItemCenter;

		/// <summary>
		/// Custom scroll inertia until reach item center.
		/// </summary>
		public bool ScrollInertiaUntilItemCenter
		{
			get
			{
				return scrollInertiaUntilItemCenter;
			}

			set
			{
				if (scrollInertiaUntilItemCenter != value)
				{
					scrollInertiaUntilItemCenter = value;

					ListRenderer.ToggleScrollToItemCenter(scrollInertiaUntilItemCenter);
				}
			}
		}

		/// <summary>
		/// Stop scroll inertia at item center.
		/// </summary>
		[Obsolete("Renamed to ScrollInertiaUntilItemCenter.")]
		public bool StopScrollAtItemCenter
		{
			get
			{
				return ScrollInertiaUntilItemCenter;
			}

			set
			{
				ScrollInertiaUntilItemCenter = value;
			}
		}

		/// <summary>
		/// Scroll inertia.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("StopScrollInertia")]
		public AnimationCurve ScrollInertia = AnimationCurve.Linear(0, 0, 0.15f, 1);

		/// <summary>
		/// Scroll inertia.
		/// </summary>
		[Obsolete("Renamed to ScrollInertia.")]
		public AnimationCurve StopScrollInertia
		{
			get
			{
				return ScrollInertia;
			}

			set
			{
				ScrollInertia = value;
			}
		}

		/// <summary>
		/// Scroll center state.
		/// </summary>
		protected enum ScrollCenterState
		{
			/// <summary>
			/// None.
			/// </summary>
			None = 0,

			/// <summary>
			/// Active.
			/// </summary>
			Active = 1,

			/// <summary>
			/// Disable.
			/// </summary>
			Disable = 2,
		}

		/// <summary>
		/// Current scroll center state.
		/// </summary>
		protected ScrollCenterState ScrollCenter = ScrollCenterState.None;

		/// <summary>
		/// Newly selected indices.
		/// </summary>
		protected List<int> NewSelectedIndices = new List<int>();

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewCustomInited)
			{
				return;
			}

			isListViewCustomInited = true;

			MainThread = Thread.CurrentThread;

			foreach (var template in Templates)
			{
				template.UpdateId();
			}

			base.Init();

			Items = new List<ListViewItem>();

			SelectedItemsCache.Clear();
			GetSelectedItems(SelectedItemsCache);

			DestroyGameObjects = false;

			InitTemplates();

			if (ListRenderer.IsVirtualizationSupported())
			{
				ScrollRect = scrollRect;
				CalculateItemSize();
			}

			SetContentSizeFitter = setContentSizeFitter;

			SetDirection(direction, false);

			UpdateItems();

			if (Layout != null)
			{
				Layout.SettingsChanged.AddListener(SetNeedResize);
			}

			Localization.OnLocaleChanged += LocaleChanged;
		}

		/// <summary>
		/// Gets selected items.
		/// </summary>
		/// <param name="output">Selected items.</param>
		public void GetSelectedItems(List<TItem> output)
		{
			foreach (var index in selectedIndices)
			{
				output.Add(DataSource[index]);
			}
		}

		/// <summary>
		/// Sets selected items.
		/// </summary>
		/// <param name="selectedItems">Selected items.</param>
		/// <param name="deselectCurrent">Deselect currently selected items.</param>
		public void SetSelectedItems(List<TItem> selectedItems, bool deselectCurrent = false)
		{
			if (deselectCurrent)
			{
				DeselectAll();
			}

			foreach (var item in selectedItems)
			{
				Select(DataSource.IndexOf(item));
			}
		}

		/// <summary>
		/// Init templates.
		/// </summary>
		protected void InitTemplates()
		{
			CanSetData = true;

			foreach (var template in TemplateSelector.AllTemplates())
			{
				if (template.gameObject == null)
				{
					Debug.LogError("ListView.TemplateSelector.AllTemplates() has template without gameobject.", this);
					continue;
				}

				template.gameObject.SetActive(true);
				template.FindSelectableObjects();
				template.gameObject.SetActive(false);

				if (!(template is IViewData<TItem>))
				{
					CanSetData = false;
				}

				ComponentsPool.GetTemplate(template);
			}
		}

		/// <summary>
		/// Get template by index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Template.</returns>
		protected virtual TComponent Index2Template(int index)
		{
			return TemplateSelector.Select(index, DataSource[index]);
		}

		/// <inheritdoc/>
		protected override void UpdateLayoutBridgeContentSizeFitter()
		{
			if (LayoutBridge != null)
			{
				LayoutBridge.UpdateContentSizeFitter = SetContentSizeFitter && ListRenderer.AllowSetContentSizeFitter;
			}
		}

		/// <inheritdoc/>
		protected override void SetScrollRect(ScrollRect newScrollRect)
		{
			if (scrollRect != null)
			{
				var old_resize_listener = scrollRect.GetComponent<ResizeListener>();
				if (old_resize_listener != null)
				{
					old_resize_listener.OnResize.RemoveListener(SetNeedResize);
				}

				scrollRect.onValueChanged.RemoveListener(SelectableCheck);
				ListRenderer.Disable();
				scrollRect.onValueChanged.RemoveListener(SelectableSet);
				scrollRect.onValueChanged.RemoveListener(OnScrollRectUpdate);
			}

			scrollRect = newScrollRect;

			if (scrollRect != null)
			{
				var resize_listener = Utilities.GetOrAddComponent<ResizeListener>(scrollRect);
				resize_listener.OnResize.AddListener(SetNeedResize);

				scrollRect.onValueChanged.AddListener(SelectableCheck);
				ListRenderer.Enable();
				scrollRect.onValueChanged.AddListener(SelectableSet);
				scrollRect.onValueChanged.AddListener(OnScrollRectUpdate);

				UpdateScrollRectSize();
			}
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			ComponentsPool.LocaleChanged();
		}

		/// <summary>
		/// Get the rendered of the specified ListView type.
		/// </summary>
		/// <param name="type">ListView type</param>
		/// <returns>Renderer.</returns>
		protected virtual ListViewTypeBase GetRenderer(ListViewType type)
		{
			ListViewTypeBase renderer;
			switch (type)
			{
				case ListViewType.ListViewWithFixedSize:
					renderer = new ListViewTypeFixed(this);
					break;
				case ListViewType.ListViewWithVariableSize:
					renderer = new ListViewTypeSize(this);
					break;
				case ListViewType.TileViewWithFixedSize:
					renderer = new TileViewTypeFixed(this);
					break;
				case ListViewType.TileViewWithVariableSize:
					renderer = new TileViewTypeSize(this);
					break;
				case ListViewType.TileViewStaggered:
					renderer = new TileViewStaggered(this);
					break;
				case ListViewType.ListViewEllipse:
					renderer = new ListViewTypeEllipse(this);
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown ListView type: {0}", EnumHelper<ListViewType>.ToString(type)));
			}

			renderer.Enable();
			renderer.ToggleScrollToItemCenter(ScrollInertiaUntilItemCenter);

			return renderer;
		}

		/// <summary>
		/// Sets the default item.
		/// </summary>
		/// <param name="newDefaultItem">New default item.</param>
		protected virtual void SetDefaultItem(TComponent newDefaultItem)
		{
			if (newDefaultItem == null)
			{
				throw new ArgumentNullException("newDefaultItem");
			}

			defaultItem = newDefaultItem;
			DefaultTemplateSelector.Replace(newDefaultItem);

			if (isListViewCustomInited)
			{
				TemplatesChanged();
			}
		}

		/// <summary>
		/// Process templates changed.
		/// </summary>
		protected virtual void TemplatesChanged()
		{
			InitTemplates();

			CalculateItemSize(true);

			CalculateMaxVisibleItems();

			UpdateView();

			if (scrollRect != null)
			{
				var resizeListener = scrollRect.GetComponent<ResizeListener>();
				if (resizeListener != null)
				{
					resizeListener.OnResize.Invoke();
				}
			}
		}

		/// <inheritdoc/>
		protected override void SetDirection(ListViewDirection newDirection, bool updateView = true)
		{
			direction = newDirection;

			ListRenderer.ResetPosition();

			if (ListRenderer.IsVirtualizationSupported())
			{
				LayoutBridge.IsHorizontal = IsHorizontal();
				ListRenderer.DirectionChanged();

				CalculateMaxVisibleItems();
			}

			if (updateView)
			{
				UpdateView();
			}
		}

		/// <inheritdoc/>
		public override bool IsSortEnabled()
		{
			if (DataSource.Comparison != null)
			{
				return true;
			}

#pragma warning disable 0618
			return Sort && SortFunc != null;
#pragma warning restore 0618
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest index.</returns>
		/// <param name="eventData">Event data.</param>
		/// <param name="type">Preferable nearest index.</param>
		public override int GetNearestIndex(PointerEventData eventData, NearestType type)
		{
			if (IsSortEnabled())
			{
				return -1;
			}

			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Container, eventData.position, eventData.pressEventCamera, out point))
			{
				return DataSource.Count;
			}

			if (!Container.rect.Contains(point))
			{
				return DataSource.Count;
			}

			return GetNearestIndex(point, type);
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		/// <param name="point">Point.</param>
		/// <param name="type">Preferable nearest index.</param>
		public override int GetNearestIndex(Vector2 point, NearestType type)
		{
			var index = ListRenderer.GetNearestIndex(point, type);
			if (index == DataSource.Count)
			{
				return index;
			}

			if (ListRenderer.AllowLoopedList)
			{
				index = ListRenderer.VisibleIndex2ItemIndex(index);
			}

			return index;
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		/// <param name="reset">Reset item size.</param>
		protected virtual void CalculateItemSize(bool reset = false)
		{
			ItemSize = ListRenderer.GetItemSize(reset);
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		protected virtual void CalculateMaxVisibleItems()
		{
			if (!isListViewCustomInited)
			{
				return;
			}

			ListRenderer.CalculateMaxVisibleItems();

			ListRenderer.ValidateContentSize();
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public virtual void Resize()
		{
			ListRenderer.CalculateItemsSizes(DataSource, false);

			NeedResize = false;

			UpdateScrollRectSize();

			CalculateItemSize(true);
			CalculateMaxVisibleItems();
			UpdateView();
		}

		/// <inheritdoc/>
		protected override void SelectItem(int index)
		{
			var component = GetComponent(index);
			SelectColoring(component);

			if (component != null)
			{
				component.StateSelected();
			}
		}

		/// <inheritdoc/>
		protected override void DeselectItem(int index)
		{
			var component = GetComponent(index);
			DefaultColoring(component);

			if (component != null)
			{
				component.StateDefault();
			}
		}

		/// <inheritdoc/>
		protected override void InvokeSelect(int index, bool raiseEvents)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning(string.Format("Incorrect index: {0}", index.ToString()), this);
			}

			SelectedItemsCache.Add(DataSource[index]);

			base.InvokeSelect(index, raiseEvents);

			if (raiseEvents)
			{
				OnSelectObject.Invoke(index);
			}
		}

		/// <inheritdoc/>
		protected override void InvokeDeselect(int index, bool raiseEvents)
		{
			if (!IsValid(index))
			{
				Debug.LogWarning(string.Format("Incorrect index: {0}", index.ToString()), this);
			}

			SelectedItemsCache.Remove(DataSource[index]);

			base.InvokeDeselect(index, raiseEvents);

			if (raiseEvents)
			{
				OnDeselectObject.Invoke(index);
			}
		}

		/// <inheritdoc/>
		public override void UpdateItems()
		{
			SetNewItems(DataSource, IsMainThread);
			IsDataSourceChanged = !IsMainThread;
		}

		/// <inheritdoc/>
		public override void Clear()
		{
			DataSource.Clear();
			ListRenderer.SetPosition(0f);
		}

		/// <inheritdoc/>
		public override void RemoveItemAt(int index)
		{
			DataSource.RemoveAt(index);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(TItem item)
		{
			DataSource.Add(item);

			return DataSource.IndexOf(item);
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed TItem.</returns>
		public virtual int Remove(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index == -1)
			{
				return index;
			}

			DataSource.RemoveAt(index);

			return index;
		}

		/// <summary>
		/// Remove item by the specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public virtual void Remove(int index)
		{
			DataSource.RemoveAt(index);
		}

		/// <summary>
		/// Scrolls to specified item immediately.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ScrollTo(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index > -1)
			{
				ScrollTo(index);
			}
		}

		/// <summary>
		/// Scroll to the specified item with animation.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void ScrollToAnimated(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (index > -1)
			{
				ScrollToAnimated(index);
			}
		}

		/// <inheritdoc/>
		public override void ScrollTo(int index)
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return;
			}

			ListRenderer.SetPosition(ListRenderer.GetPosition(index));
		}

		/// <summary>
		/// Get scroll position.
		/// </summary>
		/// <returns>Position.</returns>
		public override float GetScrollPosition()
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return 0f;
			}

			return ListRenderer.GetPosition();
		}

		/// <summary>
		/// Scrolls to specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		public override void ScrollToPosition(float position)
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return;
			}

			ListRenderer.SetPosition(position);
		}

		/// <summary>
		/// Scrolls to specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		public override void ScrollToPosition(Vector2 position)
		{
			if (!ListRenderer.IsVirtualizationPossible())
			{
				return;
			}

			ListRenderer.SetPosition(position);
		}

		/// <summary>
		/// Is visible item with specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="minVisiblePart">The minimal visible part of the item to consider item visible.</param>
		/// <returns>true if item visible; false otherwise.</returns>
		public override bool IsVisible(int index, float minVisiblePart = 0f)
		{
			if (!ListRenderer.IsVirtualizationSupported())
			{
				return false;
			}

			return ListRenderer.IsVisible(index, minVisiblePart);
		}

		/// <summary>
		/// Starts the scroll coroutine.
		/// </summary>
		/// <param name="coroutine">Coroutine.</param>
		protected virtual void StartScrollCoroutine(IEnumerator coroutine)
		{
			StopScrollCoroutine();
			ScrollCoroutine = coroutine;
			StartCoroutine(ScrollCoroutine);
		}

		/// <summary>
		/// Stops the scroll coroutine.
		/// </summary>
		protected virtual void StopScrollCoroutine()
		{
			if (ScrollCoroutine != null)
			{
				StopCoroutine(ScrollCoroutine);
			}

			ScrollCenter = ScrollCenterState.None;
		}

		/// <inheritdoc/>
		public override void ScrollStop()
		{
			StopScrollCoroutine();
		}

		/// <inheritdoc/>
		public override void ScrollToAnimated(int index)
		{
			StartScrollCoroutine(ScrollToAnimatedCoroutine(index, ScrollMovement, ScrollUnscaledTime));
		}

		/// <inheritdoc/>
		public override void ScrollToAnimated(int index, AnimationCurve animation, bool unscaledTime, Action after = null)
		{
			StartScrollCoroutine(ScrollToAnimatedCoroutine(index, animation, unscaledTime, after));
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(float target)
		{
			ScrollToPositionAnimated(target, ScrollMovement, ScrollUnscaledTime);
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(float target, AnimationCurve animation, bool unscaledTime, Action after = null)
		{
#if CSHARP_7_3_OR_NEWER
			Vector2 Position()
#else
			Func<Vector2> Position = () =>
#endif
			{
				var current_position = ListRenderer.GetPositionVector();
				var target_position = IsHorizontal()
					? new Vector2(ListRenderer.ValidatePosition(-target), current_position.y)
					: new Vector2(current_position.x, ListRenderer.ValidatePosition(target));

				return target_position;
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			StartScrollCoroutine(ScrollToAnimatedCoroutine(Position, animation, unscaledTime, after));
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(Vector2 target)
		{
			ScrollToPositionAnimated(target, ScrollMovement, ScrollUnscaledTime);
		}

		/// <inheritdoc/>
		public override void ScrollToPositionAnimated(Vector2 target, AnimationCurve animation, bool unscaleTime, Action after = null)
		{
#if CSHARP_7_3_OR_NEWER
			Vector2 Position()
#else
			Func<Vector2> Position = () =>
#endif
			{
				return ListRenderer.ValidatePosition(target);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			StartScrollCoroutine(ScrollToAnimatedCoroutine(Position, animation, unscaleTime, after));
		}

		/// <summary>
		/// Scroll to specified index with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="index">Index.</param>
		/// <param name="animation">Animation curve.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="after">Action to run after animation.</param>
		protected virtual IEnumerator ScrollToAnimatedCoroutine(int index, AnimationCurve animation, bool unscaledTime, Action after = null)
		{
#if CSHARP_7_3_OR_NEWER
			Vector2 Position()
#else
			Func<Vector2> Position = () =>
#endif
			{
				return ListRenderer.GetPosition(index);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			return ScrollToAnimatedCoroutine(Position, animation, unscaledTime, after);
		}

		/// <summary>
		/// Get start position for the animated scroll.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Start position.</returns>
		protected virtual Vector2 GetScrollStartPosition(Vector2 target)
		{
			var start = ListRenderer.GetPositionVector();
			if (IsHorizontal())
			{
				start.x = -start.x;
			}

			if (ListRenderer.AllowLoopedList)
			{
				// find shortest distance to target for the looped list
				var list_size = ListRenderer.ListSize() + LayoutBridge.GetSpacing();
				var distance_straight = IsHorizontal()
					? (target.x - start.x)
					: (target.y - start.y);
				var distance_reverse_1 = IsHorizontal()
					? (target.x - (start.x + list_size))
					: (target.y - start.y + list_size);
				var distance_reverse_2 = IsHorizontal()
					? (target.x - (start.x - list_size))
					: (target.y - start.y - list_size);

				if (Mathf.Abs(distance_reverse_1) < Mathf.Abs(distance_straight))
				{
					if (IsHorizontal())
					{
						start.x += list_size;
					}
					else
					{
						start.y += list_size;
					}
				}

				if (Mathf.Abs(distance_reverse_2) < Mathf.Abs(distance_straight))
				{
					if (IsHorizontal())
					{
						start.x -= list_size;
					}
					else
					{
						start.y -= list_size;
					}
				}
			}

			return start;
		}

		/// <summary>
		/// Scroll to specified position with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="targetPosition">Target position.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		[Obsolete("Replaced with ScrollToAnimatedCoroutine(Func<Vector2> targetPosition, AnimationCurve animation, bool unscaledTime, Action after = null).")]
		protected virtual IEnumerator ScrollToAnimatedCoroutine(Func<Vector2> targetPosition, bool unscaledTime)
		{
			return ScrollToAnimatedCoroutine(targetPosition, ScrollMovement, unscaledTime);
		}

		/// <summary>
		/// Scroll to specified position with time coroutine.
		/// </summary>
		/// <returns>The scroll to index with time coroutine.</returns>
		/// <param name="targetPosition">Target position.</param>
		/// <param name="animation">Animation curve.</param>
		/// <param name="unscaledTime">Use unscaled time.</param>
		/// <param name="after">Action to run after animation.</param>
		protected virtual IEnumerator ScrollToAnimatedCoroutine(Func<Vector2> targetPosition, AnimationCurve animation, bool unscaledTime, Action after = null)
		{
			var start = GetScrollStartPosition(targetPosition());

			float delta;
			var animationLength = animation[animation.length - 1].time;
			var startTime = UtilitiesTime.GetTime(unscaledTime);

			do
			{
				delta = UtilitiesTime.GetTime(unscaledTime) - startTime;
				var value = animation.Evaluate(delta);

				var target = targetPosition();
				var pos = start + ((target - start) * value);

				ListRenderer.SetPosition(pos);

				yield return null;
			}
			while (delta < animationLength);

			ListRenderer.SetPosition(targetPosition());

			yield return null;

			ListRenderer.SetPosition(targetPosition());

			if (after != null)
			{
				after();
			}
		}

		/// <summary>
		/// Gets the item position by index.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			return ListRenderer.GetItemPosition(index);
		}

		/// <summary>
		/// Gets the item position by index.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionBorderEnd(int index)
		{
			return ListRenderer.GetItemPositionBorderEnd(index);
		}

		/// <summary>
		/// Gets the item middle position by index.
		/// </summary>
		/// <returns>The item middle position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionMiddle(int index)
		{
			return ListRenderer.GetItemPositionMiddle(index);
		}

		/// <summary>
		/// Gets the item bottom position by index.
		/// </summary>
		/// <returns>The item bottom position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionBottom(int index)
		{
			return ListRenderer.GetItemPositionBottom(index);
		}

		/// <summary>
		/// Called after component instantiated.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ComponentCreated(TComponent component)
		{
			AddCallback(component);

			var c = OnComponentCreated;
			if (c != null)
			{
				c(component);
			}
		}

		/// <summary>
		/// Called before component destroyed.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ComponentDestroyed(TComponent component)
		{
			var c = OnComponentDestroyed;
			if (c != null)
			{
				c(component);
			}

			RemoveCallback(component);
		}

		/// <summary>
		/// Called after component became activated.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ComponentActivated(TComponent component)
		{
			var c = OnComponentActivated;
			if (c != null)
			{
				c(component);
			}
		}

		/// <summary>
		/// Called after component moved to cache.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ComponentCached(TComponent component)
		{
			var c = OnComponentCached;
			if (c != null)
			{
				c(component);
			}
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void AddCallback(TComponent item)
		{
			ListRenderer.AddCallback(item);

			#pragma warning disable 0618
			AddCallback(item as ListViewItem);
			#pragma warning restore 0618
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[Obsolete("Replaced with AddCallback(TComponent item).")]
		protected virtual void AddCallback(ListViewItem item)
		{
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void RemoveCallback(TComponent item)
		{
			if (item == null)
			{
				return;
			}

			ListRenderer.RemoveCallback(item);

			#pragma warning disable 0618
			RemoveCallback(item as ListViewItem);
			#pragma warning restore 0618
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[Obsolete("Replaced with RemoveCallback(TComponent item).")]
		protected virtual void RemoveCallback(ListViewItem item)
		{
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public int Set(TItem item, bool allowDuplicate = true)
		{
			int index;

			if (!allowDuplicate)
			{
				index = DataSource.IndexOf(item);
				if (index == -1)
				{
					index = Add(item);
				}
			}
			else
			{
				index = Add(item);
			}

			Select(index);

			return index;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected virtual void SetData(TComponent component, TItem item)
		{
			if (CanSetData)
			{
				(component as IViewData<TItem>).SetData(item);
			}
		}

		/// <summary>
		/// Gets the default width of the item.
		/// </summary>
		/// <returns>The default item width.</returns>
		public override float GetDefaultItemWidth()
		{
			return ItemSize.x;
		}

		/// <summary>
		/// Gets the default height of the item.
		/// </summary>
		/// <returns>The default item height.</returns>
		public override float GetDefaultItemHeight()
		{
			return ItemSize.y;
		}

		/// <summary>
		/// Sets the displayed indices.
		/// </summary>
		/// <param name="isNewData">Is new data?</param>
		protected virtual void SetDisplayedIndices(bool isNewData = true)
		{
			if (isNewData)
			{
				ComponentsPool.DisplayedIndicesSet(DisplayedIndices, ComponentSetData);
			}
			else
			{
				ComponentsPool.DisplayedIndicesUpdate(DisplayedIndices, ComponentSetData);
			}

			ListRenderer.UpdateLayout();
			Updater.RunOnceNextFrame(ComponentsHighlightedColoring);
		}

		/// <summary>
		/// Process the ScrollRect update event.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void OnScrollRectUpdate(Vector2 position)
		{
			StartScrolling();
		}

		/// <summary>
		/// Set data to component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void ComponentSetData(TComponent component)
		{
			component.ResetDimensionsSize();
			SetData(component, DataSource[component.Index]);
			Coloring(component as ListViewItem);

			if (IsSelected(component.Index))
			{
				component.StateSelected();
			}
			else
			{
				component.StateDefault();
			}
		}

		/// <inheritdoc/>
		public override void UpdateView()
		{
			if (!isListViewCustomInited)
			{
				return;
			}

			ListRenderer.UpdateDisplayedIndices();

			SetDisplayedIndices();

			OnUpdateView.Invoke();
		}

		/// <summary>
		/// Keep selected items on items update.
		/// </summary>
		[SerializeField]
		protected bool KeepSelection = true;

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected virtual void SetNewItems(ObservableList<TItem> newItems, bool updateView = true)
		{
			lock (DataSource)
			{
				DataSource.OnChange -= UpdateItems;

#pragma warning disable 0618
				if (Sort && SortFunc != null)
				{
					newItems.BeginUpdate();

					var sorted = new List<TItem>(SortFunc(newItems));

					newItems.Clear();
					newItems.AddRange(sorted);

					newItems.EndUpdate();
				}
#pragma warning restore 0618

				SilentDeselect(SelectedIndicesList);
				RecalculateSelectedIndices(newItems);

				dataSource = newItems;

				ListRenderer.CalculateItemsSizes(DataSource, false);

				CalculateMaxVisibleItems();

				if (KeepSelection)
				{
					SilentSelect(NewSelectedIndices);
				}

				SelectedItemsCache.Clear();
				GetSelectedItems(SelectedItemsCache);

				DataSource.OnChange += UpdateItems;

				if (updateView)
				{
					UpdateView();
				}
			}
		}

		/// <summary>
		/// Recalculates the selected indices.
		/// </summary>
		/// <param name="newItems">New items.</param>
		protected virtual void RecalculateSelectedIndices(ObservableList<TItem> newItems)
		{
			NewSelectedIndices.Clear();

			foreach (var item in SelectedItemsCache)
			{
				var new_index = newItems.IndexOf(item);
				if (new_index != -1)
				{
					NewSelectedIndices.Add(new_index);
				}
			}
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsValid(int index)
		{
			return (index >= 0) && (index < DataSource.Count);
		}

		/// <summary>
		/// Process the item move event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		/// <param name="eventData">Event data.</param>
		protected override void OnItemMove(int index, ListViewItem item, AxisEventData eventData)
		{
			if (!Navigation)
			{
				return;
			}

			if (ListRenderer.OnItemMove(eventData, item))
			{
				return;
			}

			base.OnItemMove(index, item, eventData);
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void Coloring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				SelectColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			HighlightColoring(component as TComponent);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(TComponent component)
		{
			if (component == null)
			{
				return;
			}

			if (!allowColoring)
			{
				return;
			}

			if (!CanSelect(component.Index))
			{
				return;
			}

			if (IsSelected(component.Index))
			{
				return;
			}

			component.GraphicsColoring(HighlightedColor, HighlightedBackgroundColor, FadeDuration);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			SelectColoring(component as TComponent);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(TComponent component)
		{
			if (component == null)
			{
				return;
			}

			if (!allowColoring)
			{
				return;
			}

			if (IsInteractable())
			{
				component.GraphicsColoring(SelectedColor, SelectedBackgroundColor, FadeDuration);
			}
			else
			{
				component.GraphicsColoring(SelectedColor * DisabledColor, SelectedBackgroundColor * DisabledColor, FadeDuration);
			}
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(ListViewItem component)
		{
			if (component == null)
			{
				return;
			}

			DefaultColoring(component as TComponent);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(TComponent component)
		{
			if (component == null)
			{
				return;
			}

			if (!allowColoring)
			{
				return;
			}

			if (IsInteractable())
			{
				component.GraphicsColoring(DefaultColor, DefaultBackgroundColor, FadeDuration);
			}
			else
			{
				component.GraphicsColoring(DefaultColor * DisabledColor, DefaultBackgroundColor * DisabledColor, FadeDuration);
			}
		}

		/// <summary>
		/// Updates the colors.
		/// </summary>
		/// <param name="instant">Is should be instant color update?</param>
		public override void ComponentsColoring(bool instant = false)
		{
			if (!isListViewCustomInited)
			{
				return;
			}

			if (!allowColoring && instant)
			{
				foreach (var component in ComponentsPool)
				{
					DefaultColoring(component);
				}

				return;
			}

			if (instant)
			{
				var old_duration = FadeDuration;
				FadeDuration = 0f;

				foreach (var component in ComponentsPool)
				{
					Coloring(component);
				}

				FadeDuration = old_duration;
			}
			else
			{
				foreach (var component in ComponentsPool)
				{
					Coloring(component);
				}
			}

			ComponentsHighlightedColoring();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			Localization.OnLocaleChanged -= LocaleChanged;

			if (dataSource != null)
			{
				dataSource.OnChange -= UpdateItems;
				dataSource = null;
			}

			if (layout != null)
			{
				layout.SettingsChanged.RemoveListener(SetNeedResize);
				layout = null;
			}

			layoutBridge = null;

			ScrollRect = null;

			if (componentsPool != null)
			{
				componentsPool.Destroy();
				componentsPool = null;
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="ListViewComponentPool" />.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <returns>A <see cref="ListViewBase.ListViewComponentEnumerator{TComponent, Template}" /> for the <see cref="ListViewComponentPool" />.</returns>
		public ListViewComponentEnumerator<TComponent, Template> GetComponentsEnumerator(PoolEnumeratorMode mode)
		{
			return ComponentsPool.GetEnumerator(mode);
		}

		/// <summary>
		/// Calls the specified action for each component.
		/// </summary>
		/// <param name="func">Action.</param>
		public override void ForEachComponent(Action<ListViewItem> func)
		{
			if (componentsPool != null)
			{
				foreach (var component in GetComponentsEnumerator(PoolEnumeratorMode.All))
				{
					func(component);
				}
			}
		}

		/// <summary>
		/// Calls the specified action for each component.
		/// </summary>
		/// <param name="func">Action.</param>
		public virtual void ForEachComponent(Action<TComponent> func)
		{
			if (componentsPool != null)
			{
				foreach (var component in GetComponentsEnumerator(PoolEnumeratorMode.All))
				{
					func(component);
				}
			}
		}

		/// <summary>
		/// Determines whether item visible.
		/// </summary>
		/// <returns><c>true</c> if item visible; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsItemVisible(int index)
		{
			return DisplayedIndices.Contains(index);
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		public List<int> GetVisibleIndices()
		{
			return new List<int>(DisplayedIndices);
		}

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <param name="output">Output.</param>
		public void GetVisibleIndices(List<int> output)
		{
			output.AddRange(DisplayedIndices);
		}

		/// <summary>
		/// Gets the visible components.
		/// </summary>
		/// <returns>The visible components.</returns>
		public List<TComponent> GetVisibleComponents()
		{
			return new List<TComponent>(Components);
		}

		/// <summary>
		/// Gets the visible components.
		/// </summary>
		/// <param name="output">Output.</param>
		public void GetVisibleComponents(List<TComponent> output)
		{
			output.AddRange(Components);
		}

		/// <summary>
		/// Gets the item component.
		/// </summary>
		/// <returns>The item component.</returns>
		/// <param name="index">Index.</param>
		public TComponent GetItemComponent(int index)
		{
			return GetComponent(index) as TComponent;
		}

		/// <summary>
		/// OnStartScrolling event.
		/// </summary>
		public UnityEvent OnStartScrolling = new UnityEvent();

		/// <summary>
		/// OnEndScrolling event.
		/// </summary>
		public UnityEvent OnEndScrolling = new UnityEvent();

		/// <summary>
		/// Time before raise OnEndScrolling event since last OnScrollRectUpdate event raised.
		/// </summary>
		public float EndScrollDelay = 0.3f;

		/// <summary>
		/// Is ScrollRect now on scrolling state.
		/// </summary>
		protected bool Scrolling;

		/// <summary>
		/// When last scroll event happen?
		/// </summary>
		protected float LastScrollingTime;

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (DataSourceSetted || IsDataSourceChanged)
			{
				var reset_scroll = DataSourceSetted;

				DataSourceSetted = false;
				IsDataSourceChanged = false;

				lock (DataSource)
				{
					CalculateMaxVisibleItems();
					UpdateView();
				}

				if (reset_scroll)
				{
					ListRenderer.SetPosition(0f);
				}
			}

			if (NeedResize)
			{
				Resize();
			}

			if (IsStopScrolling())
			{
				EndScrolling();
			}

			SelectableSet();
		}

		/// <summary>
		/// LateUpdate.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			SelectableSet();
		}

		/// <summary>
		/// Scroll to the nearest item center.
		/// </summary>
		public void ScrollToItemCenter()
		{
			ListRenderer.ScrollToItemCenter();
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			Updater.RunOnceNextFrame(ForceRebuild);

			var old = FadeDuration;
			FadeDuration = 0f;
			ComponentsColoring(true);
			FadeDuration = old;

			Resize();

			Updater.Add(this);
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.Remove(this);
			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Force layout rebuild.
		/// </summary>
		protected virtual void ForceRebuild()
		{
			foreach (var component in ComponentsPool)
			{
				LayoutRebuilder.MarkLayoutForRebuild(component.RectTransform);
			}
		}

		/// <summary>
		/// Start to track scrolling event.
		/// </summary>
		protected virtual void StartScrolling()
		{
			LastScrollingTime = UtilitiesTime.GetTime(true);
			if (Scrolling)
			{
				return;
			}

			Scrolling = true;
			OnStartScrolling.Invoke();
		}

		/// <summary>
		/// Determines whether ScrollRect is stop scrolling.
		/// </summary>
		/// <returns><c>true</c> if ScrollRect is stop scrolling; otherwise, <c>false</c>.</returns>
		protected virtual bool IsStopScrolling()
		{
			if (!Scrolling)
			{
				return false;
			}

			return (LastScrollingTime + EndScrollDelay) <= UtilitiesTime.GetTime(true);
		}

		/// <summary>
		/// Raise OnEndScrolling event.
		/// </summary>
		protected virtual void EndScrolling()
		{
			Scrolling = false;
			OnEndScrolling.Invoke();

			if (ScrollInertiaUntilItemCenter && (ScrollCenter == ScrollCenterState.None) && (AutoScrollCoroutine == null))
			{
				ListRenderer.ScrollToItemCenter();
				ScrollCenter = ScrollCenterState.Active;
			}

			if (ScrollCenter == ScrollCenterState.Disable)
			{
				ScrollCenter = ScrollCenterState.None;
			}
		}

		/// <summary>
		/// Is need to handle resize event?
		/// </summary>
		protected bool NeedResize;

		/// <summary>
		/// Sets the need resize.
		/// </summary>
		protected virtual void SetNeedResize()
		{
			if (!ListRenderer.IsVirtualizationSupported())
			{
				return;
			}

			NeedResize = true;
		}

		/// <summary>
		/// Change DefaultItem size.
		/// </summary>
		/// <param name="size">Size.</param>
		public virtual void ChangeDefaultItemSize(Vector2 size)
		{
			ComponentsPool.SetSize(size);

			CalculateItemSize(true);
			CalculateMaxVisibleItems();
			UpdateView();
		}

		/// <summary>
		/// Select first item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item was found and selected; otherwise false.</returns>
		public bool SelectFirstItem(TItem item)
		{
			var index = DataSource.IndexOf(item);
			if (IsValid(index))
			{
				Select(index);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Select all items.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item was found and selected; otherwise false.</returns>
		public bool SelectAllItems(TItem item)
		{
			var selected = false;
			var start = 0;
			bool is_valid;
			do
			{
				var index = DataSource.IndexOf(item, start);
				is_valid = IsValid(index);
				if (is_valid)
				{
					start = index + 1;
					Select(index);
					selected = true;
				}
			}
			while (is_valid);

			return selected;
		}

		#region DebugInfo

		/// <summary>
		/// Get debug information.
		/// </summary>
		/// <returns>Debug information.</returns>
		public override string GetDebugInfo()
		{
			var sb = new System.Text.StringBuilder();
			sb.Append("Direction: ");
			sb.Append(EnumHelper<ListViewDirection>.ToString(Direction));
			sb.AppendLine();

			sb.Append("Type: ");
			sb.Append(EnumHelper<ListViewType>.ToString(ListType));
			sb.AppendLine();

			sb.Append("Virtualization: ");
			sb.Append(Virtualization);
			sb.AppendLine();

			sb.Append("DataSource.Count: ");
			sb.Append(DataSource.Count);
			sb.AppendLine();

			sb.Append("Container Size: ");
			sb.Append(Container.rect.size.ToString());
			sb.AppendLine();

			sb.Append("Container Scale: ");
			sb.Append(Container.localScale.ToString());
			sb.AppendLine();

			sb.Append("DefaultItem Size: ");
			sb.Append(ItemSize.ToString());
			sb.AppendLine();

			sb.Append("DefaultItem Scale: ");
			sb.Append(DefaultItem.RectTransform.localScale.ToString());
			sb.AppendLine();

			sb.Append("ScrollRect Size: ");
			sb.Append(ScrollRectSize.ToString());
			sb.AppendLine();

			sb.Append("Looped: ");
			sb.Append(LoopedList);
			sb.AppendLine();

			sb.Append("Centered: ");
			sb.Append(CenterTheItems);
			sb.AppendLine();

			sb.Append("Precalculate Sizes: ");
			sb.Append(PrecalculateItemSizes);
			sb.AppendLine();

			sb.Append("DisplayedIndices (count: ");
			sb.Append(DisplayedIndices.Count);
			sb.Append("): ");
			sb.Append(UtilitiesCollections.List2String(DisplayedIndices));
			sb.AppendLine();

			sb.Append("Components Indices (count: ");
			sb.Append(Components.Count);
			sb.Append("): ");
			sb.AppendLine();
			for (int i = 0; i < Components.Count; i++)
			{
				sb.Append(i);
				sb.Append(" ");
				sb.Append(Components[i].name);
				sb.Append(": ");
				sb.Append(Components[i].Index);
				sb.AppendLine();
			}

			sb.Append("StopScrollAtItemCenter: ");
			sb.Append(ScrollInertiaUntilItemCenter);
			sb.AppendLine();

			sb.Append("ScrollCenterState: ");
			sb.Append(EnumHelper<ScrollCenterState>.ToString(ScrollCenter));
			sb.AppendLine();

			sb.Append("ScrollPosition: ");
			sb.Append(ListRenderer.GetPosition());
			sb.AppendLine();

			sb.Append("ScrollVectorPosition: ");
			sb.Append(ListRenderer.GetPositionVector().ToString());
			sb.AppendLine();

			sb.AppendLine();

			sb.AppendLine("#############");
			sb.AppendLine("**Renderer Info**");
			ListRenderer.GetDebugInfo(sb);
			sb.AppendLine();

			sb.AppendLine("#############");
			sb.AppendLine("**Layout Info**");
			if (Layout != null)
			{
				sb.AppendLine("Layout: EasyLayout");
				Layout.GetDebugInfo(sb);
			}
			else
			{
				var layout = Container.GetComponent<LayoutGroup>();
				var layout_type = (layout != null) ? UtilitiesEditor.GetFriendlyTypeName(layout.GetType()) : "null";
				sb.Append("Layout: ");
				sb.Append(layout_type);
				sb.AppendLine();
			}

			return sb.ToString();
		}

		#endregion

		#region ListViewPaginator support

		/// <summary>
		/// Gets the ScrollRect.
		/// </summary>
		/// <returns>The ScrollRect.</returns>
		public override ScrollRect GetScrollRect()
		{
			return ScrollRect;
		}

		/// <summary>
		/// Gets the items count.
		/// </summary>
		/// <returns>The items count.</returns>
		public override int GetItemsCount()
		{
			return DataSource.Count;
		}

		/// <summary>
		/// Gets the items per block count.
		/// </summary>
		/// <returns>The items per block.</returns>
		public override int GetItemsPerBlock()
		{
			return ListRenderer.GetItemsPerBlock();
		}

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex()
		{
			return ListRenderer.GetNearestItemIndex();
		}

		/// <summary>
		/// Gets the size of the DefaultItem.
		/// </summary>
		/// <returns>Size.</returns>
		public override Vector2 GetDefaultItemSize()
		{
			return ItemSize;
		}
		#endregion

		#region Obsolete

		/// <summary>
		/// Gets the visible indices.
		/// </summary>
		/// <returns>The visible indices.</returns>
		[Obsolete("Use GetVisibleIndices()")]
		public List<int> GetVisibleIndicies()
		{
			return GetVisibleIndices();
		}
		#endregion

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void SetStyleDefaultItem(Style style)
		{
			if (componentsPool != null)
			{
				componentsPool.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
			}
			else
			{
				foreach (var template in TemplateSelector.AllTemplates())
				{
					template.Owner = this;
					template.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
				}
			}
		}

		/// <summary>
		/// Sets the style colors.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void SetStyleColors(Style style)
		{
			defaultBackgroundColor = style.Collections.DefaultBackgroundColor;
			defaultColor = style.Collections.DefaultColor;
			highlightedBackgroundColor = style.Collections.HighlightedBackgroundColor;
			highlightedColor = style.Collections.HighlightedColor;
			selectedBackgroundColor = style.Collections.SelectedBackgroundColor;
			selectedColor = style.Collections.SelectedColor;
		}

		/// <summary>
		/// Sets the ScrollRect style.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void SetStyleScrollRect(Style style)
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var viewport = scrollRect.viewport != null ? scrollRect.viewport : Container.parent;
#else
			var viewport = Container.parent;
#endif
			style.Collections.Viewport.ApplyTo(viewport.GetComponent<Image>());

			style.HorizontalScrollbar.ApplyTo(scrollRect.horizontalScrollbar);
			style.VerticalScrollbar.ApplyTo(scrollRect.verticalScrollbar);
		}

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			SetStyleDefaultItem(style);

			SetStyleColors(style);

			SetStyleScrollRect(style);

			style.Collections.MainBackground.ApplyTo(GetComponent<Image>());

			if (StyleTable)
			{
				var image = Utilities.GetOrAddComponent<Image>(Container);
				image.sprite = null;
				image.color = DefaultColor;

				var mask_image = Utilities.GetOrAddComponent<Image>(Container.parent);
				mask_image.sprite = null;

				var mask = Utilities.GetOrAddComponent<Mask>(Container.parent);
				mask.showMaskGraphic = false;

				defaultBackgroundColor = style.Table.Background.Color;
			}

			if (componentsPool != null)
			{
				ComponentsColoring(true);
			}
			else if (defaultItem != null)
			{
				foreach (var template in TemplateSelector.AllTemplates())
				{
					template.SetColors(DefaultColor, DefaultBackgroundColor);
				}
			}

			if (header != null)
			{
				header.SetStyle(style);
			}
			else
			{
				style.ApplyTo(transform.Find("Header"));
			}

			return true;
		}

		/// <summary>
		/// Set style options from the DefaultItem.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void GetStyleDefaultItem(Style style)
		{
			foreach (var template in TemplateSelector.AllTemplates())
			{
				template.Owner = this;
				template.GetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
			}
		}

		/// <summary>
		/// Get style colors.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void GetStyleColors(Style style)
		{
			style.Collections.DefaultBackgroundColor = defaultBackgroundColor;
			style.Collections.DefaultColor = defaultColor;
			style.Collections.HighlightedBackgroundColor = highlightedBackgroundColor;
			style.Collections.HighlightedColor = highlightedColor;
			style.Collections.SelectedBackgroundColor = selectedBackgroundColor;
			style.Collections.SelectedColor = selectedColor;
		}

		/// <summary>
		/// Get style options from the ScrollRect.
		/// </summary>
		/// <param name="style">Style.</param>
		protected virtual void GetStyleScrollRect(Style style)
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			var viewport = scrollRect.viewport != null ? scrollRect.viewport : Container.parent;
#else
			var viewport = Container.parent;
#endif
			style.Collections.Viewport.GetFrom(viewport.GetComponent<Image>());

			style.HorizontalScrollbar.GetFrom(scrollRect.horizontalScrollbar);
			style.VerticalScrollbar.GetFrom(scrollRect.verticalScrollbar);
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			GetStyleDefaultItem(style);

			GetStyleColors(style);

			GetStyleScrollRect(style);

			style.Collections.MainBackground.GetFrom(GetComponent<Image>());

			if (StyleTable)
			{
				style.Table.Background.Color = defaultBackgroundColor;
			}

			if (header != null)
			{
				header.GetStyle(style);
			}
			else
			{
				style.GetFrom(transform.Find("Header"));
			}

			return true;
		}
		#endregion

		#region Selectable

		/// <summary>
		/// Selectable data.
		/// </summary>
		protected struct SelectableData : IEquatable<SelectableData>
		{
			/// <summary>
			/// Is need to update EventSystem.currentSelectedGameObject?
			/// </summary>
			public bool Update;

			/// <summary>
			/// Index of the item with selectable GameObject.
			/// </summary>
			public int Item
			{
				get;
				private set;
			}

			/// <summary>
			/// Index of the selectable GameObject of the item.
			/// </summary>
			public int SelectableGameObject
			{
				get;
				private set;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="SelectableData"/> struct.
			/// </summary>
			/// <param name="item">Index of the item with selectable GameObject.</param>
			/// <param name="selectableGameObject">Index of the selectable GameObject of the item.</param>
			public SelectableData(int item, int selectableGameObject)
			{
				Update = true;
				Item = item;
				SelectableGameObject = selectableGameObject;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return Item ^ SelectableGameObject;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (!(obj is SelectableData))
				{
					return false;
				}

				return Equals((SelectableData)obj);
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(SelectableData other)
			{
				if (Update != other.Update)
				{
					return false;
				}

				if (Item != other.Item)
				{
					return false;
				}

				return SelectableGameObject == other.SelectableGameObject;
			}

			/// <summary>
			/// Compare specified objects.
			/// </summary>
			/// <param name="data1">First object.</param>
			/// <param name="data2">Second object.</param>
			/// <returns>true if the objects are equal; otherwise, false.</returns>
			public static bool operator ==(SelectableData data1, SelectableData data2)
			{
				return data1.Equals(data2);
			}

			/// <summary>
			/// Compare specified objects.
			/// </summary>
			/// <param name="data1">First object.</param>
			/// <param name="data2">Second object.</param>
			/// <returns>true if the objects not equal; otherwise, false.</returns>
			public static bool operator !=(SelectableData data1, SelectableData data2)
			{
				return !data1.Equals(data2);
			}
		}

		/// <summary>
		/// Selectable data.
		/// </summary>
		protected SelectableData NewSelectableData;

		/// <summary>
		/// Get current selected GameObject.
		/// </summary>
		/// <returns>Selected GameObject.</returns>
		protected GameObject GetCurrentSelectedGameObject()
		{
			var es = EventSystem.current;
			if (es == null)
			{
				return null;
			}

			var go = es.currentSelectedGameObject;
			if (go == null)
			{
				return null;
			}

			if (!go.transform.IsChildOf(Container))
			{
				return null;
			}

			return go;
		}

		/// <summary>
		/// Get item component with selected GameObject.
		/// </summary>
		/// <param name="go">Selected GameObject.</param>
		/// <returns>Item component.</returns>
		protected TComponent SelectedGameObject2Component(GameObject go)
		{
			if (go == null)
			{
				return null;
			}

			var t = go.transform;
			foreach (var component in ComponentsPool)
			{
				var item_transform = component.RectTransform;
				if (t.IsChildOf(item_transform) && (t.GetInstanceID() != item_transform.GetInstanceID()))
				{
					return component;
				}
			}

			return null;
		}

		/// <summary>
		/// Find index of the next item.
		/// </summary>
		/// <param name="index">Index of the current item with selected GameObject.</param>
		/// <returns>Index of the next item</returns>
		protected virtual int SelectableFindNextObjectIndex(int index)
		{
			for (int i = 0; i < DataSource.Count; i++)
			{
				var prev_index = index - i;
				var next_index = index + i;
				var prev_valid = IsValid(prev_index);
				var next_valid = IsValid(next_index);
				if (!prev_valid && !next_valid)
				{
					return -1;
				}

				if (IsVisible(next_index))
				{
					return next_index;
				}

				if (IsVisible(prev_index))
				{
					return prev_index;
				}
			}

			return -1;
		}

		/// <inheritdoc/>
		protected override void SelectableCheck()
		{
			var go = GetCurrentSelectedGameObject();
			if (go == null)
			{
				return;
			}

			var component = SelectedGameObject2Component(go);
			if (component == null)
			{
				return;
			}

			if (IsVisible(component.Index))
			{
				return;
			}

			var item_index = SelectableFindNextObjectIndex(component.Index);
			if (!IsValid(item_index))
			{
				return;
			}

			NewSelectableData = new SelectableData(item_index, component.GetSelectableIndex(go));
		}

		/// <inheritdoc/>
		protected override void SelectableSet()
		{
			if (!NewSelectableData.Update)
			{
				return;
			}

			var es = EventSystem.current;
			if ((es == null) || es.alreadySelecting)
			{
				return;
			}

			var component = GetItemComponent(NewSelectableData.Item);
			if (component == null)
			{
				return;
			}

			var go = component.GetSelectableObject(NewSelectableData.SelectableGameObject);
			NewSelectableData.Update = false;

			if (go != null)
			{
				es.SetSelectedGameObject(go);
			}
		}
		#endregion

		#region AutoScroll

		/// <summary>
		/// Auto scroll.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected override IEnumerator AutoScroll()
		{
			var min = 0;
			var max = ListRenderer.CanScroll ? GetItemPositionBottom(DataSource.Count - 1) : 0f;

			while (true)
			{
				var delta = AutoScrollSpeed * UtilitiesTime.GetDeltaTime(ScrollUnscaledTime) * AutoScrollDirection;

				var pos = GetScrollPosition() + delta;
				if (!LoopedListAvailable)
				{
					pos = Mathf.Clamp(pos, min, max);
				}

				ScrollToPosition(pos);

				yield return null;

				if (AutoScrollCallback != null)
				{
					AutoScrollCallback(AutoScrollEventData);
				}
			}
		}
		#endregion
	}
}