namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// EasyLayout.
	/// Warning: using RectTransform relative size with positive size delta (like 100% + 10) with ContentSizeFitter can lead to infinite increased size.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/New UI Widgets/Layout/EasyLayout")]
	public class EasyLayout : LayoutGroup, IObservable, INotifyPropertyChanged
	{
		readonly List<LayoutElementInfo> elements = new List<LayoutElementInfo>();

		readonly Stack<LayoutElementInfo> elementsCache = new Stack<LayoutElementInfo>();

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs when a properties values changed except PaddingInner property.
		/// </summary>
		[SerializeField]
		public UnityEvent SettingsChanged = new UnityEvent();

		[SerializeField]
		[FormerlySerializedAs("GroupPosition")]
		Anchors groupPosition = Anchors.UpperLeft;

		/// <summary>
		/// The group position.
		/// </summary>
		public Anchors GroupPosition
		{
			get
			{
				return groupPosition;
			}

			set
			{
				if (groupPosition != value)
				{
					groupPosition = value;
					NotifyPropertyChanged("GroupPosition");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Stacking")]
		Axis mainAxis = Axis.Horizontal;

		/// <summary>
		/// The stacking type.
		/// </summary>
		[Obsolete("Replaced with MainAxis.")]
		public Stackings Stacking
		{
			get
			{
				if (MainAxis == Axis.Horizontal)
				{
					return Stackings.Horizontal;
				}

				return Stackings.Vertical;
			}

			set
			{
				MainAxis = (value == Stackings.Horizontal) ? Axis.Horizontal : Axis.Vertical;
			}
		}

		/// <summary>
		/// Main axis.
		/// </summary>
		public Axis MainAxis
		{
			get
			{
				return mainAxis;
			}

			set
			{
				if (mainAxis != value)
				{
					mainAxis = value;
					NotifyPropertyChanged("Axis");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("LayoutType")]
		LayoutTypes layoutType = LayoutTypes.Compact;

		/// <summary>
		/// The type of the layout.
		/// </summary>
		public LayoutTypes LayoutType
		{
			get
			{
				return layoutType;
			}

			set
			{
				if (layoutType != value)
				{
					LayoutGroup = null;
					layoutType = value;
					NotifyPropertyChanged("LayoutType");
				}
			}
		}

		EasyLayoutBaseType layoutGroup;

		/// <summary>
		/// Layout group.
		/// </summary>
		protected EasyLayoutBaseType LayoutGroup
		{
			get
			{
				if (layoutGroup == null)
				{
					layoutGroup = GetLayoutGroup();
					layoutGroup.OnElementChanged += ElementChanged;
				}

				return layoutGroup;
			}

			set
			{
				if (layoutGroup != null)
				{
					layoutGroup.OnElementChanged -= ElementChanged;
				}

				layoutGroup = value;

				if (layoutGroup != null)
				{
					layoutGroup.OnElementChanged += ElementChanged;
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CompactConstraint")]
		CompactConstraints compactConstraint = CompactConstraints.Flexible;

		/// <summary>
		/// Which constraint to use for the Grid layout.
		/// </summary>
		public CompactConstraints CompactConstraint
		{
			get
			{
				return compactConstraint;
			}

			set
			{
				if (compactConstraint != value)
				{
					compactConstraint = value;
					NotifyPropertyChanged("CompactConstraint");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CompactConstraintCount")]
		int compactConstraintCount = 1;

		/// <summary>
		/// How many elements there should be along the constrained axis.
		/// </summary>
		public int CompactConstraintCount
		{
			get
			{
				return Mathf.Max(1, compactConstraintCount);
			}

			set
			{
				if (compactConstraintCount != value)
				{
					compactConstraintCount = value;
					NotifyPropertyChanged("CompactConstraintCount");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("GridConstraint")]
		GridConstraints gridConstraint = GridConstraints.Flexible;

		/// <summary>
		/// Which constraint to use for the Grid layout.
		/// </summary>
		public GridConstraints GridConstraint
		{
			get
			{
				return gridConstraint;
			}

			set
			{
				if (gridConstraint != value)
				{
					gridConstraint = value;
					NotifyPropertyChanged("GridConstraint");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("GridConstraintCount")]
		int gridConstraintCount = 1;

		/// <summary>
		/// How many cells there should be along the constrained axis.
		/// </summary>
		public int GridConstraintCount
		{
			get
			{
				return Mathf.Max(1, gridConstraintCount);
			}

			set
			{
				if (gridConstraintCount != value)
				{
					gridConstraintCount = value;
					NotifyPropertyChanged("GridConstraintCount");
				}
			}
		}

		/// <summary>
		/// Constraint count.
		/// </summary>
		public int ConstraintCount
		{
			get
			{
				if (LayoutType == LayoutTypes.Compact)
				{
					return CompactConstraintCount;
				}
				else
				{
					return GridConstraintCount;
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("RowAlign")]
		HorizontalAligns rowAlign = HorizontalAligns.Left;

		/// <summary>
		/// The row align.
		/// </summary>
		public HorizontalAligns RowAlign
		{
			get
			{
				return rowAlign;
			}

			set
			{
				if (rowAlign != value)
				{
					rowAlign = value;
					NotifyPropertyChanged("RowAlign");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("InnerAlign")]
		InnerAligns innerAlign = InnerAligns.Top;

		/// <summary>
		/// The inner align.
		/// </summary>
		public InnerAligns InnerAlign
		{
			get
			{
				return innerAlign;
			}

			set
			{
				if (innerAlign != value)
				{
					innerAlign = value;
					NotifyPropertyChanged("InnerAlign");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("CellAlign")]
		Anchors cellAlign = Anchors.UpperLeft;

		/// <summary>
		/// The cell align.
		/// </summary>
		public Anchors CellAlign
		{
			get
			{
				return cellAlign;
			}

			set
			{
				if (cellAlign != value)
				{
					cellAlign = value;
					NotifyPropertyChanged("CellAlign");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Spacing")]
		Vector2 spacing = new Vector2(5, 5);

		/// <summary>
		/// The spacing.
		/// </summary>
		public Vector2 Spacing
		{
			get
			{
				return spacing;
			}

			set
			{
				if (spacing != value)
				{
					spacing = value;
					NotifyPropertyChanged("Spacing");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Symmetric")]
		bool symmetric = true;

		/// <summary>
		/// Symmetric margin.
		/// </summary>
		public bool Symmetric
		{
			get
			{
				return symmetric;
			}

			set
			{
				if (symmetric != value)
				{
					symmetric = value;
					NotifyPropertyChanged("Symmetric");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Margin")]
		Vector2 margin = new Vector2(5, 5);

		/// <summary>
		/// The margin.
		/// </summary>
		public Vector2 Margin
		{
			get
			{
				return margin;
			}

			set
			{
				if (margin != value)
				{
					margin = value;
					NotifyPropertyChanged("Margin");
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		Padding marginInner;

		/// <summary>
		/// The margin.
		/// Should be used by ListView related scripts.
		/// </summary>
		public Padding MarginInner
		{
			get
			{
				return marginInner;
			}

			set
			{
				if (marginInner != value)
				{
					marginInner = value;
					NotifyPropertyChanged("MarginInner");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("PaddingInner")]
		[HideInInspector]
		Padding paddingInner;

		/// <summary>
		/// The padding.
		/// Should be used by ListView related scripts.
		/// </summary>
		public Padding PaddingInner
		{
			get
			{
				return paddingInner;
			}

			set
			{
				if (paddingInner != value)
				{
					paddingInner = value;
					NotifyPropertyChanged("PaddingInner", false);
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginTop")]
		float marginTop = 5f;

		/// <summary>
		/// The margin top.
		/// </summary>
		public float MarginTop
		{
			get
			{
				return marginTop;
			}

			set
			{
				if (marginTop != value)
				{
					marginTop = value;
					NotifyPropertyChanged("MarginTop");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginBottom")]
		float marginBottom = 5f;

		/// <summary>
		/// The margin bottom.
		/// </summary>
		public float MarginBottom
		{
			get
			{
				return marginBottom;
			}

			set
			{
				if (marginBottom != value)
				{
					marginBottom = value;
					NotifyPropertyChanged("MarginBottom");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginLeft")]
		float marginLeft = 5f;

		/// <summary>
		/// The margin left.
		/// </summary>
		public float MarginLeft
		{
			get
			{
				return marginLeft;
			}

			set
			{
				if (marginLeft != value)
				{
					marginLeft = value;
					NotifyPropertyChanged("MarginLeft");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("MarginRight")]
		float marginRight = 5f;

		/// <summary>
		/// The margin right.
		/// </summary>
		public float MarginRight
		{
			get
			{
				return marginRight;
			}

			set
			{
				if (marginRight != value)
				{
					marginRight = value;
					NotifyPropertyChanged("MarginRight");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("RightToLeft")]
		bool rightToLeft = false;

		/// <summary>
		/// The right to left stacking.
		/// </summary>
		public bool RightToLeft
		{
			get
			{
				return rightToLeft;
			}

			set
			{
				if (rightToLeft != value)
				{
					rightToLeft = value;
					NotifyPropertyChanged("RightToLeft");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("TopToBottom")]
		bool topToBottom = true;

		/// <summary>
		/// The top to bottom stacking.
		/// </summary>
		public bool TopToBottom
		{
			get
			{
				return topToBottom;
			}

			set
			{
				if (topToBottom != value)
				{
					topToBottom = value;
					NotifyPropertyChanged("TopToBottom");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("SkipInactive")]
		bool skipInactive = true;

		/// <summary>
		/// The skip inactive.
		/// </summary>
		public bool SkipInactive
		{
			get
			{
				return skipInactive;
			}

			set
			{
				if (skipInactive != value)
				{
					skipInactive = value;
					NotifyPropertyChanged("SkipInactive");
				}
			}
		}

		[SerializeField]
		bool resetRotation;

		/// <summary>
		/// Reset rotation for the controlled elements.
		/// </summary>
		public bool ResetRotation
		{
			get
			{
				return resetRotation;
			}

			set
			{
				if (resetRotation != value)
				{
					resetRotation = value;
					NotifyPropertyChanged("ResetRotation");
				}
			}
		}

		/// <summary>
		/// The filter.
		/// </summary>
		[Obsolete("Replaced with ShouldIgnore")]
		public Func<IEnumerable<GameObject>, IEnumerable<GameObject>> Filter
		{
			get
			{
				throw new NotSupportedException("Obsolete.");
			}

			set
			{
				throw new NotSupportedException("Obsolete.");
			}
		}

		Func<RectTransform, bool> shouldIgnore;

		/// <summary>
		/// The filter.
		/// </summary>
		public Func<RectTransform, bool> ShouldIgnore
		{
			get
			{
				return shouldIgnore;
			}

			set
			{
				if (shouldIgnore != value)
				{
					shouldIgnore = value;
					NotifyPropertyChanged("ShouldIgnore");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ChildrenWidth")]
		ChildrenSize childrenWidth;

		/// <summary>
		/// How to control width of the children.
		/// </summary>
		public ChildrenSize ChildrenWidth
		{
			get
			{
				return childrenWidth;
			}

			set
			{
				if (childrenWidth != value)
				{
					childrenWidth = value;
					NotifyPropertyChanged("ChildrenWidth");
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("ChildrenHeight")]
		ChildrenSize childrenHeight;

		/// <summary>
		/// How to control height of the children.
		/// </summary>
		public ChildrenSize ChildrenHeight
		{
			get
			{
				return childrenHeight;
			}

			set
			{
				if (childrenHeight != value)
				{
					childrenHeight = value;
					NotifyPropertyChanged("ChildrenHeight");
				}
			}
		}

		[SerializeField]
		EasyLayoutFlexSettings flexSettings = new EasyLayoutFlexSettings();

		/// <summary>
		/// Settings for the Flex layout type.
		/// </summary>
		public EasyLayoutFlexSettings FlexSettings
		{
			get
			{
				return flexSettings;
			}

			set
			{
				if (flexSettings != value)
				{
					flexSettings.OnChange -= FlexSettingsChanged;
					flexSettings = value;
					flexSettings.OnChange += FlexSettingsChanged;
					NotifyPropertyChanged("FlexSettings");
				}
			}
		}

		[SerializeField]
		EasyLayoutStaggeredSettings staggeredSettings = new EasyLayoutStaggeredSettings();

		/// <summary>
		/// Settings for the Staggered layout type.
		/// </summary>
		public EasyLayoutStaggeredSettings StaggeredSettings
		{
			get
			{
				return staggeredSettings;
			}

			set
			{
				if (staggeredSettings != value)
				{
					staggeredSettings.OnChange -= StaggeredSettingsChanged;
					staggeredSettings = value;
					staggeredSettings.OnChange += StaggeredSettingsChanged;
					NotifyPropertyChanged("StaggeredSettings");
				}
			}
		}

		[SerializeField]
		EasyLayoutEllipseSettings ellipseSettings = new EasyLayoutEllipseSettings();

		/// <summary>
		/// Settings for the Ellipse layout type.
		/// </summary>
		public EasyLayoutEllipseSettings EllipseSettings
		{
			get
			{
				return ellipseSettings;
			}

			set
			{
				if (ellipseSettings != value)
				{
					ellipseSettings.OnChange -= EllipseSettingsChanged;
					ellipseSettings = value;
					ellipseSettings.OnChange += EllipseSettingsChanged;
					NotifyPropertyChanged("EllipseSettings");
				}
			}
		}

		/// <summary>
		/// Control width of children.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetPreferred instead.")]
		public bool ControlWidth;

		/// <summary>
		/// Control height of children.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetPreferred instead.")]
		[FormerlySerializedAs("ControlHeight")]
		public bool ControlHeight;

		/// <summary>
		/// Sets width of the children to maximum width from them.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetMaxFromPreferred instead.")]
		[FormerlySerializedAs("MaxWidth")]
		public bool MaxWidth;

		/// <summary>
		/// Sets height of the children to maximum height from them.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetMaxFromPreferred instead.")]
		[FormerlySerializedAs("MaxHeight")]
		public bool MaxHeight;

		/// <summary>
		/// Internal size.
		/// </summary>
		public Vector2 InternalSize
		{
			get
			{
				var size = rectTransform.rect.size;
				var padding = PaddingInner;
				size.x -= MarginFullHorizontal + padding.Horizontal;
				size.y -= MarginFullVertical + padding.Vertical;

				return size;
			}
		}

		/// <summary>
		/// Current sizes info.
		/// </summary>
		public GroupSize CurrentSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the size of the inner block.
		/// </summary>
		/// <value>The size of the inner block.</value>
		public Vector2 BlockSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the UI size.
		/// </summary>
		/// <value>The UI size.</value>
		public Vector2 UISize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Size in elements.
		/// </summary>
		public Vector2 Size
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the minimum width.
		/// </summary>
		/// <value>The minimum width.</value>
		public override float minWidth
		{
			get
			{
				return (ChildrenWidth == ChildrenSize.DoNothing) ? CurrentSize.Width : CurrentSize.MinWidth;
			}
		}

		/// <summary>
		/// Gets the minimum height.
		/// </summary>
		/// <value>The minimum height.</value>
		public override float minHeight
		{
			get
			{
				return (ChildrenHeight == ChildrenSize.DoNothing) ? CurrentSize.Height : CurrentSize.MinHeight;
			}
		}

		/// <summary>
		/// Gets the preferred width.
		/// </summary>
		/// <value>The preferred width.</value>
		public override float preferredWidth
		{
			get
			{
				return (ChildrenWidth == ChildrenSize.DoNothing) ? CurrentSize.Width : CurrentSize.PreferredWidth;
			}
		}

		/// <summary>
		/// Gets the preferred height.
		/// </summary>
		/// <value>The preferred height.</value>
		public override float preferredHeight
		{
			get
			{
				return (ChildrenHeight == ChildrenSize.DoNothing) ? CurrentSize.Height : CurrentSize.PreferredHeight;
			}
		}

		/// <summary>
		/// Summary horizontal margin.
		/// </summary>
		public float MarginHorizontal
		{
			get
			{
				return Symmetric ? (Margin.x + Margin.x) : (MarginLeft + MarginRight);
			}
		}

		/// <summary>
		/// Summary vertical margin.
		/// </summary>
		public float MarginVertical
		{
			get
			{
				return Symmetric ? (Margin.y + Margin.y) : (MarginTop + MarginBottom);
			}
		}

		/// <summary>
		/// Summary horizontal margin with MarginInner.
		/// </summary>
		public float MarginFullHorizontal
		{
			get
			{
				var margin_external = Symmetric ? (Margin.x + Margin.x) : (MarginLeft + MarginRight);
				return margin_external + MarginInner.Horizontal;
			}
		}

		/// <summary>
		/// Summary vertical margin with MarginInner.
		/// </summary>
		public float MarginFullVertical
		{
			get
			{
				var margin_external = Symmetric ? (Margin.y + Margin.y) : (MarginTop + MarginBottom);
				return margin_external + MarginInner.Vertical;
			}
		}

		/// <summary>
		/// Is horizontal stacking?
		/// </summary>
		public bool IsHorizontal
		{
			get
			{
				return MainAxis == Axis.Horizontal;
			}
		}

		/// <summary>
		/// Size of the main axis.
		/// </summary>
		public float MainAxisSize
		{
			get
			{
				return IsHorizontal
					? rectTransform.rect.width - MarginFullHorizontal
					: rectTransform.rect.height - MarginFullVertical;
			}
		}

		/// <summary>
		/// Size of the sub axis.
		/// </summary>
		public float SubAxisSize
		{
			get
			{
				return !IsHorizontal
					? rectTransform.rect.width - MarginFullHorizontal
					: rectTransform.rect.height - MarginFullVertical;
			}
		}

		/// <summary>
		/// Properties tracker.
		/// </summary>
		protected DrivenRectTransformTracker PropertiesTracker;

		/// <summary>
		/// Children list.
		/// Used if SkipInactive disabled.
		/// </summary>
		protected List<RectTransform> Children = new List<RectTransform>();

		/// <summary>
		/// Property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <param name="invokeSettingsChanged">Should invoke SettingsChanged event?</param>
		protected void NotifyPropertyChanged(string propertyName, bool invokeSettingsChanged = true)
		{
			SetDirty();

			var c_handlers = OnChange;
			if (c_handlers != null)
			{
				c_handlers();
			}

			var handlers = PropertyChanged;
			if (handlers != null)
			{
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}

			if (invokeSettingsChanged)
			{
				SettingsChanged.Invoke();
			}
		}

		void FlexSettingsChanged()
		{
			NotifyPropertyChanged("FlexSettings");
		}

		void StaggeredSettingsChanged()
		{
			NotifyPropertyChanged("StaggeredSettings");
		}

		void EllipseSettingsChanged()
		{
			NotifyPropertyChanged("EllipseSettings");
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			flexSettings.OnChange += FlexSettingsChanged;
			staggeredSettings.OnChange += StaggeredSettingsChanged;
			ellipseSettings.OnChange += EllipseSettingsChanged;
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			PropertiesTracker.Clear();

			base.OnDisable();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (flexSettings != null)
			{
				flexSettings.OnChange -= FlexSettingsChanged;
			}

			if (ellipseSettings != null)
			{
				ellipseSettings.OnChange -= EllipseSettingsChanged;
			}

			if (staggeredSettings != null)
			{
				staggeredSettings.OnChange -= StaggeredSettingsChanged;
			}

			LayoutGroup = null;

			base.OnDestroy();
		}

		/// <summary>
		/// Process the RectTransform removed event.
		/// </summary>
		protected virtual void OnRectTransformRemoved()
		{
			SetDirty();
		}

		/// <summary>
		/// Sets the layout horizontal.
		/// </summary>
		public override void SetLayoutHorizontal()
		{
			UpdateElements();

			PerformLayout(true, ResizeType.Horizontal);
		}

		/// <summary>
		/// Sets the layout vertical.
		/// </summary>
		public override void SetLayoutVertical()
		{
			UpdateElements();

			PerformLayout(true, ResizeType.Vertical);
		}

		/// <summary>
		/// Calculates the layout input horizontal.
		/// </summary>
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			UpdateElements();

			PerformLayout(false, ResizeType.None);
		}

		/// <summary>
		/// Calculates the layout input vertical.
		/// </summary>
		public override void CalculateLayoutInputVertical()
		{
			UpdateElements();

			PerformLayout(false, ResizeType.None);
		}

		/// <summary>
		/// Marks layout to update.
		/// </summary>
		public void NeedUpdateLayout()
		{
			SetDirty();
		}

		/// <summary>
		/// Calculates the size of the layout.
		/// </summary>
		public void CalculateLayoutSize()
		{
			UpdateElements();

			PerformLayout(false);
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}

		/// <summary>
		/// Get children.
		/// </summary>
		/// <returns>Children.</returns>
		protected List<RectTransform> GetChildren()
		{
			if (SkipInactive)
			{
				return rectChildren;
			}

			Children.Clear();
			for (int i = 0; i < rectTransform.childCount; i++)
			{
				Children.Add(rectTransform.GetChild(i) as RectTransform);
			}

			return Children;
		}

		/// <summary>
		/// Update LayoutElements.
		/// </summary>
		protected void UpdateElements()
		{
			ClearElements();

			var children = GetChildren();
			var ignore = ShouldIgnore != null;
			foreach (var rt in children)
			{
				if (ignore && ShouldIgnore(rt))
				{
					continue;
				}

				elements.Add(CreateElement(rt));
			}
		}

		/// <summary>
		/// Reset layout elements list.
		/// </summary>
		protected void ClearElements()
		{
			for (int i = 0; i < elements.Count; i++)
			{
				elementsCache.Push(elements[i]);
			}

			elements.Clear();
		}

		/// <summary>
		/// Create layout element.
		/// </summary>
		/// <param name="elem">Element.</param>
		/// <returns>Element data.</returns>
		protected LayoutElementInfo CreateElement(RectTransform elem)
		{
			var info = (elementsCache.Count > 0) ? elementsCache.Pop() : new LayoutElementInfo();
			var active = SkipInactive || elem.gameObject.activeInHierarchy;
			info.SetElement(elem, active, this);
			if (ResetRotation && (LayoutType != LayoutTypes.Ellipse))
			{
				info.NewEulerAngles = Vector3.zero;
			}

			return info;
		}

		/// <summary>
		/// Gets the margin top.
		/// </summary>
		/// <returns>Top margin.</returns>
		public float GetMarginTop()
		{
			return Symmetric ? Margin.y : MarginTop;
		}

		/// <summary>
		/// Gets the margin bottom.
		/// </summary>
		/// <returns>Bottom margin.</returns>
		public float GetMarginBottom()
		{
			return Symmetric ? Margin.y : MarginBottom;
		}

		/// <summary>
		/// Gets the margin left.
		/// </summary>
		/// <returns>Left margin.</returns>
		public float GetMarginLeft()
		{
			return Symmetric ? Margin.x : MarginLeft;
		}

		/// <summary>
		/// Gets the margin right.
		/// </summary>
		/// <returns>Right margin.</returns>
		public float GetMarginRight()
		{
			return Symmetric ? Margin.x : MarginRight;
		}

		/// <summary>
		/// Get layout group.
		/// </summary>
		/// <returns>Layout group.</returns>
		protected EasyLayoutBaseType GetLayoutGroup()
		{
			switch (LayoutType)
			{
				case LayoutTypes.Compact:
					return new EasyLayoutCompact();
				case LayoutTypes.Grid:
					return new EasyLayoutGrid();
				case LayoutTypes.Flex:
					return new EasyLayoutFlex();
				case LayoutTypes.Staggered:
					return new EasyLayoutStaggered();
				case LayoutTypes.Ellipse:
					return new EasyLayoutEllipse();
				default:
					Debug.LogWarning(string.Format("Unknown layout type: {0}", EnumHelper<LayoutTypes>.ToString(LayoutType)));
					break;
			}

			return null;
		}

		/// <summary>
		/// Perform layout.
		/// </summary>
		/// <param name="setPositions">Is need to set elements position?</param>
		/// <param name="resizeType">Resize type.</param>
		protected void PerformLayout(bool setPositions, ResizeType resizeType = ResizeType.Horizontal | ResizeType.Vertical)
		{
			if (LayoutGroup == null)
			{
				Debug.LogWarning(string.Format("Layout group not found: {0}", EnumHelper<LayoutTypes>.ToString(LayoutType)));
				return;
			}

			PropertiesTracker.Clear();

			LayoutGroup.LoadSettings(this);
			CurrentSize = LayoutGroup.PerformLayout(elements, setPositions, resizeType);

			BlockSize = elements.Count > 0 ? new Vector2(CurrentSize.Width, CurrentSize.Height) : Vector2.zero;

			CurrentSize += new Vector2(MarginFullHorizontal, MarginFullVertical);
			UISize = new Vector2(CurrentSize.Width, CurrentSize.Height);
		}

		/// <summary>
		/// Process element changed event.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="properties">Properties.</param>
		protected virtual void ElementChanged(RectTransform element, DrivenTransformProperties properties)
		{
			PropertiesTracker.Add(this, element, properties);
		}

		/// <summary>
		/// Set element size.
		/// </summary>
		/// <param name="element">Element.</param>
		[Obsolete("No more used.")]
		public void SetElementSize(LayoutElementInfo element)
		{
			var driven_properties = DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.AnchoredPositionZ;

			if (ChildrenWidth != ChildrenSize.DoNothing)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaX;
				element.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, element.NewWidth);
			}

			if (ChildrenHeight != ChildrenSize.DoNothing)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaY;
				element.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, element.NewHeight);
			}

			if (LayoutType == LayoutTypes.Ellipse || ResetRotation)
			{
				driven_properties |= DrivenTransformProperties.Rotation;
				element.Rect.localEulerAngles = element.NewEulerAngles;
			}

			if (LayoutType == LayoutTypes.Ellipse)
			{
				driven_properties |= DrivenTransformProperties.Pivot;
				element.Rect.pivot = element.NewPivot;
			}

			PropertiesTracker.Add(this, element.Rect, driven_properties);
		}

		/// <summary>
		/// Get element position in the group.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <returns>Position.</returns>
		public EasyLayoutPosition GetElementPosition(RectTransform element)
		{
			return LayoutGroup.GetElementPosition(element);
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			Upgrade();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected override void OnValidate()
		{
			LayoutGroup = null;

			SetDirty();
		}
		#endif

		[SerializeField]
		[HideInInspector]
		int version = 0;

		/// <summary>
		/// Upgrade to keep compatibility between versions.
		/// </summary>
		public virtual void Upgrade()
		{
			#pragma warning disable 0618
			if (version == 0)
			{
				if (ControlWidth)
				{
					ChildrenWidth = MaxWidth ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}

				if (ControlHeight)
				{
					ChildrenHeight = MaxHeight ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}

				version = 1;
			}
			#pragma warning restore 0618
		}

		/// <summary>
		/// Get debug information.
		/// </summary>
		/// <returns>Debug information.</returns>
		public virtual string GetDebugInfo()
		{
			var sb = new System.Text.StringBuilder();
			GetDebugInfo(sb);

			return sb.ToString();
		}

		/// <summary>
		/// Get debug information.
		/// </summary>
		/// <param name="sb">String builder.</param>
		public virtual void GetDebugInfo(System.Text.StringBuilder sb)
		{
			sb.Append("RectTransform.size: ");
			sb.Append(rectTransform.rect.size.ToString());
			sb.AppendLine();

			sb.Append("localScale: ");
			sb.Append(rectTransform.localScale.ToString());
			sb.AppendLine();

			sb.Append("Main Axis: ");
			sb.Append(EnumHelper<Axis>.ToString(MainAxis));
			sb.AppendLine();

			sb.Append("Type: ");
			sb.Append(EnumHelper<LayoutTypes>.ToString(LayoutType));
			sb.AppendLine();

			switch (LayoutType)
			{
				case LayoutTypes.Compact:
					sb.Append("\tGroup Position: ");
					sb.Append(EnumHelper<Anchors>.ToString(GroupPosition));
					sb.AppendLine();

					sb.Append("\tRow Align: ");
					sb.Append(EnumHelper<HorizontalAligns>.ToString(RowAlign));
					sb.AppendLine();

					sb.Append("\tInner Align: ");
					sb.Append(EnumHelper<InnerAligns>.ToString(InnerAlign));
					sb.AppendLine();

					sb.Append("\tCompact Constraint: ");
					sb.Append(EnumHelper<CompactConstraints>.ToString(CompactConstraint));
					sb.AppendLine();

					sb.Append("\tCompact Constraint Count: ");
					sb.Append(CompactConstraintCount);
					sb.AppendLine();
					break;
				case LayoutTypes.Grid:
					sb.Append("\tGroup Position: ");
					sb.Append(EnumHelper<Anchors>.ToString(GroupPosition));
					sb.AppendLine();

					sb.Append("\tCell Align: ");
					sb.Append(EnumHelper<Anchors>.ToString(CellAlign));
					sb.AppendLine();

					sb.Append("\tGrid Constraint: ");
					sb.Append(EnumHelper<GridConstraints>.ToString(GridConstraint));
					sb.AppendLine();

					sb.Append("\tGrid Constraint Count: ");
					sb.Append(GridConstraintCount);
					sb.AppendLine();
					break;
				case LayoutTypes.Flex:
					FlexSettings.GetDebugInfo(sb);
					break;
				case LayoutTypes.Staggered:
					StaggeredSettings.GetDebugInfo(sb);
					break;
				case LayoutTypes.Ellipse:
					EllipseSettings.GetDebugInfo(sb);
					break;
				default:
					sb.AppendLine("\tUnknown type: no details");
					break;
			}

			sb.Append("PaddingInner: ");
			sb.Append(PaddingInner.ToString());
			sb.AppendLine();

			sb.Append("Spacing: ");
			sb.Append(Spacing.ToString());
			sb.AppendLine();

			sb.Append("Margin Symmetric: ");
			sb.Append(Symmetric);
			sb.AppendLine();

			if (Symmetric)
			{
				sb.Append("Margin: ");
				sb.Append(Margin.ToString());
				sb.AppendLine();
			}
			else
			{
				sb.Append("Margin Left: ");
				sb.Append(MarginLeft);
				sb.AppendLine();

				sb.Append("Margin Right: ");
				sb.Append(MarginRight);
				sb.AppendLine();

				sb.Append("Margin Top: ");
				sb.Append(MarginTop);
				sb.AppendLine();

				sb.Append("Margin Bottom: ");
				sb.Append(MarginBottom);
				sb.AppendLine();
			}

			sb.Append("TopToBottom: ");
			sb.Append(TopToBottom);
			sb.AppendLine();

			sb.Append("RightToLeft: ");
			sb.Append(RightToLeft);
			sb.AppendLine();

			sb.Append("Skip Inactive: ");
			sb.Append(SkipInactive);
			sb.AppendLine();

			sb.Append("Reset Rotation: ");
			sb.Append(ResetRotation);
			sb.AppendLine();

			sb.Append("Children Width: ");
			sb.Append(EnumHelper<ChildrenSize>.ToString(ChildrenWidth));
			sb.AppendLine();

			sb.Append("Children Height: ");
			sb.Append(EnumHelper<ChildrenSize>.ToString(ChildrenHeight));
			sb.AppendLine();

			sb.Append("Children: ");
			foreach (var c in elements)
			{
				sb.Append(c.Rect.name);
				sb.Append(": ");
				sb.Append(c.Rect.rect.size.ToString());
				sb.AppendLine();
			}
		}
	}
}