namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Table Header.
	/// How to change DefaultItem:
	/// var order = tableHeader.GetColumnsOrder();
	/// tableHeader.RestoreColumnsOrder();
	/// listView.DefaultItem = newDefaultItem;
	/// tableHeader.SetColumnsOrder(order);
	/// </summary>
	[RequireComponent(typeof(LayoutGroup))]
	[AddComponentMenu("UI/New UI Widgets/Collections/Table Header")]
	[DisallowMultipleComponent]
	public class TableHeader : UIBehaviour, IDropSupport<TableHeaderDragCell>, IPointerEnterHandler, IPointerExitHandler, IStylable, ILateUpdatable
	{
		#region Interactable
		[SerializeField]
		bool interactable = true;

		/// <summary>
		/// Is widget interactable.
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		public bool Interactable
		{
			get
			{
				return interactable;
			}

			set
			{
				if (interactable != value)
				{
					interactable = value;
					InteractableChanged();
				}
			}
		}

		/// <summary>
		/// If the canvas groups allow interaction.
		/// </summary>
		protected bool GroupsAllowInteraction = true;

		/// <summary>
		/// The CanvasGroup cache.
		/// </summary>
		protected List<CanvasGroup> CanvasGroupCache = new List<CanvasGroup>();

		/// <summary>
		/// Process the CanvasGroupChanged event.
		/// </summary>
		protected override void OnCanvasGroupChanged()
		{
			var groupAllowInteraction = true;
			var t = transform;
			while (t != null)
			{
				t.GetComponents(CanvasGroupCache);
				var shouldBreak = false;
				foreach (var canvas_group in CanvasGroupCache)
				{
					if (!canvas_group.interactable)
					{
						groupAllowInteraction = false;
						shouldBreak = true;
					}

					shouldBreak |= canvas_group.ignoreParentGroups;
				}

				if (shouldBreak)
				{
					break;
				}

				t = t.parent;
			}

			if (groupAllowInteraction != GroupsAllowInteraction)
			{
				GroupsAllowInteraction = groupAllowInteraction;
				InteractableChanged();
			}
		}

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive()
		{
			return base.IsActive() && GroupsAllowInteraction && Interactable;
		}

		/// <summary>
		/// Is instance interactable?
		/// </summary>
		/// <returns>true if instance interactable; otherwise false</returns>
		public bool IsInteractable()
		{
			return GroupsAllowInteraction && Interactable;
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		protected virtual void InteractableChanged()
		{
			if (!base.IsActive())
			{
				return;
			}

			OnInteractableChange(GroupsAllowInteraction && Interactable);
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected virtual void OnInteractableChange(bool interactableState)
		{
			if (!interactableState)
			{
				if (IsCursorOver)
				{
					IsCursorOver = false;

					ResetCursor();
				}
			}
		}
		#endregion

		/// <summary>
		/// ListView instance.
		/// </summary>
		[SerializeField]
		public ListViewBase List;

		/// <summary>
		/// Allow resize.
		/// </summary>
		[SerializeField]
		public bool AllowResize = true;

		/// <summary>
		/// Allow reorder.
		/// </summary>
		[SerializeField]
		public bool AllowReorder = true;

		/// <summary>
		/// Is now processed cell reorder?
		/// </summary>
		[NonSerialized]
		[HideInInspector]
		public bool ProcessCellReorder = false;

		/// <summary>
		/// Update ListView columns width on drag.
		/// </summary>
		[SerializeField]
		public bool OnDragUpdate = true;

		/// <summary>
		/// The active region in points from left or right border where resize allowed.
		/// </summary>
		[SerializeField]
		public float ActiveRegion = 5;

		/// <summary>
		/// The current camera. For Screen Space - Overlay let it empty.
		/// </summary>
		[NonSerialized]
		protected Camera CurrentCamera;

		/// <summary>
		/// Cursors.
		/// </summary>
		[SerializeField]
		public Cursors Cursors;

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorTexture;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D AllowDropCursor;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 AllowDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DeniedDropCursor;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DeniedDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// The drop indicator.
		/// </summary>
		[SerializeField]
		public LayoutDropIndicator DropIndicator;

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>The RectTransform.</value>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// The cells info.
		/// </summary>
		protected List<TableHeaderCellInfo> CellsInfo = new List<TableHeaderCellInfo>();

		/// <summary>
		/// Header cells.
		/// </summary>
		/// <value>The cells.</value>
		public RectTransform[] Cells
		{
			get
			{
				var result = new RectTransform[CellsInfo.Count];

				for (int i = 0; i < CellsInfo.Count; i++)
				{
					result[i] = CellsInfo[i].Rect;
				}

				return result;
			}
		}

		/// <summary>
		/// Gets a value indicating whether mouse position in active region.
		/// </summary>
		/// <value><c>true</c> if in active region; otherwise, <c>false</c>.</value>
		public bool InActiveRegion
		{
			get
			{
				return CheckInActiveRegion(CompatibilityInput.MousePosition, CurrentCamera);
			}
		}

		/// <summary>
		/// Can change cursor?
		/// </summary>
		protected bool CanChangeCursor
		{
			get
			{
				return UICursor.CanSet(this) && CompatibilityInput.MousePresent && IsCursorOver;
			}
		}

		/// <summary>
		/// Is cursor over gameobject?
		/// </summary>
		protected bool IsCursorOver;

		LayoutElement leftTarget;

		LayoutElement rightTarget;

		bool processDrag;

		LayoutGroup layout;

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Init();
			Refresh();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

#pragma warning disable 0618
			if (DefaultCursorTexture != null)
			{
				UICursor.ObsoleteWarning();
			}
#pragma warning restore 0618
		}

		/// <summary>
		/// Process the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			// do nothing, left for compatibility reason
		}

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ShowDropIndicator(int index)
		{
			if (DropIndicator != null)
			{
				DropIndicator.Show(index, RectTransform);
			}
		}

		/// <summary>
		/// Hides the drop indicator.
		/// </summary>
		protected virtual void HideDropIndicator()
		{
			if (DropIndicator != null)
			{
				DropIndicator.Hide();
			}
		}

		/// <summary>
		/// Restore initial cells order.
		/// </summary>
		[Obsolete("Renamed to RestoreColumnOrder().")]
		public void RestoreOrder()
		{
			RestoreColumnsOrder();
		}

		/// <summary>
		/// Get the columns order.
		/// </summary>
		/// <returns>Columns order.</returns>
		public List<int> GetColumnsOrder()
		{
			var result = new List<int>(CellsInfo.Count);

			GetColumnsOrder(result);

			return result;
		}

		/// <summary>
		/// Get the columns order.
		/// </summary>
		/// <param name="order">Columns order.</param>
		public void GetColumnsOrder(List<int> order)
		{
			order.Clear();

			foreach (var cell in CellsInfo)
			{
				order.Add(cell.Position);
			}
		}

		List<Transform> tempList = new List<Transform>();

		List<int> tempReverseOrder = new List<int>();

		/// <summary>
		/// Set the columns order.
		/// </summary>
		/// <param name="order">New columns order.</param>
		public void SetColumnsOrder(List<int> order)
		{
			// restore original order
			RestoreColumnsOrder();

			// convert list of the new positions to list of the old positions
			for (int i = 0; i < order.Count; i++)
			{
				tempReverseOrder.Add(order.IndexOf(i));
			}

			// restore list components cells order
			List.Init();
			List.ForEachComponent(SetItemColumnOrder);

			for (int new_position = 0; new_position < tempReverseOrder.Count; new_position++)
			{
				var old_position = tempReverseOrder[new_position];
				CellsInfo[old_position].Position = new_position;
				CellsInfo[old_position].Rect.SetAsLastSibling();
			}

			tempReverseOrder.Clear();
		}

		void SetItemColumnOrder(ListViewItem component)
		{
			tempList.Clear();
			var t = component.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				tempList.Add(t.GetChild(i));
			}

			for (int i = 0; i < tempReverseOrder.Count; i++)
			{
				tempList[i].SetAsLastSibling();
			}
		}

		/// <summary>
		/// Restore column order for the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected void RestoreColumnsOrder(ListViewItem component)
		{
			tempList.Clear();

			var t = component.RectTransform;
			for (int i = 0; i < t.childCount; i++)
			{
				tempList.Add(t.GetChild(i));
			}

			foreach (var cell in CellsInfo)
			{
				tempList[cell.Position].SetAsLastSibling();
			}
		}

		/// <summary>
		/// Restore initial cells order.
		/// </summary>
		public void RestoreColumnsOrder()
		{
			// restore order
			if ((List != null) && (CellsInfo != null) && (CellsInfo.Count > 1))
			{
				// restore list components cells order
				List.Init();
				List.ForEachComponent(RestoreColumnsOrder);

				// restore header cells order
				for (int i = 0; i < CellsInfo.Count; i++)
				{
					CellsInfo[i].Position = i;
					CellsInfo[i].Rect.SetSiblingIndex(i);
				}
			}
		}

		/// <summary>
		/// Re-init this instance in case if you remove or add cells manually.
		/// </summary>
		public void Reinit()
		{
			RestoreColumnsOrder();

			// clear cells list
			CellsInfo.Clear();

			// clear cell settings and events
			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var child = RectTransform.GetChild(i);
				child.gameObject.SetActive(true);

				var cell = Utilities.GetOrAddComponent<TableHeaderDragCell>(child);
				cell.Position = -1;

				var events = Utilities.GetOrAddComponent<TableHeaderCell>(child);
				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}

			Refresh();
		}

		/// <summary>
		/// Init cells.
		/// </summary>
		protected void InitCells()
		{
			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var child = RectTransform.GetChild(i);
				var cell = Utilities.GetOrAddComponent<TableHeaderDragCell>(child);

				if (cell.Position == -1)
				{
					cell.Position = CellsInfo.Count;
					cell.TableHeader = this;

					cell.Cursors = Cursors;
					#pragma warning disable 0618
					cell.AllowDropCursor = AllowDropCursor;
					cell.AllowDropCursorHotSpot = AllowDropCursorHotSpot;
					cell.DeniedDropCursor = DeniedDropCursor;
					cell.DeniedDropCursorHotSpot = DeniedDropCursorHotSpot;
					#pragma warning restore 0618

					var events = Utilities.GetOrAddComponent<TableHeaderCell>(child);
					events.OnInitializePotentialDragEvent.AddListener(OnInitializePotentialDrag);
					events.OnBeginDragEvent.AddListener(OnBeginDrag);
					events.OnDragEvent.AddListener(OnDrag);
					events.OnEndDragEvent.AddListener(OnEndDrag);

					CellsInfo.Add(new TableHeaderCellInfo()
					{
						Rect = child as RectTransform,
						LayoutElement = Utilities.GetOrAddComponent<LayoutElement>(child),
						Position = CellsInfo.Count,
					});
				}
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerEnter event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			IsCursorOver = true;
			CurrentCamera = eventData.pressEventCamera;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerExit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOver = false;

			ResetCursor();
		}

		/// <summary>
		/// Process application focus event.
		/// </summary>
		/// <param name="hasFocus">Application has focus?</param>
		protected virtual void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				IsCursorOver = false;
			}
		}

		/// <summary>
		/// Reset cursor.
		/// </summary>
		protected void ResetCursor()
		{
			cursorChanged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// Is cursor changed?
		/// </summary>
		protected bool cursorChanged;

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Update cursors.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (!IsActive())
			{
				return;
			}

			if (!AllowResize)
			{
				return;
			}

			if (!CanChangeCursor)
			{
				return;
			}

			if (processDrag || ProcessCellReorder)
			{
				return;
			}

			if (InActiveRegion)
			{
				UICursor.Set(this, GetCursor());
				cursorChanged = true;
			}
			else if (cursorChanged)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Get cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetCursor()
		{
			if (Cursors != null)
			{
				return Cursors.EastWestArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.EastWestArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Check if event happened in active region.
		/// </summary>
		/// <param name="eventData">Event data</param>
		/// <returns>True if event happened in active region; otherwise false.</returns>
		public virtual bool CheckInActiveRegion(PointerEventData eventData)
		{
			return CheckInActiveRegion(eventData.pressPosition, eventData.pressEventCamera);
		}

		readonly List<TableHeaderCellInfo> cellsInfoOrdered = new List<TableHeaderCellInfo>();

		/// <summary>
		/// CellInfo sorted by position.
		/// </summary>
		protected List<TableHeaderCellInfo> CellsInfoOrdered
		{
			get
			{
				cellsInfoOrdered.Clear();
				cellsInfoOrdered.AddRange(CellsInfo);
				cellsInfoOrdered.Sort(CellComparison);

				return cellsInfoOrdered;
			}
		}

		/// <summary>
		/// Cell comparison.
		/// </summary>
		/// <param name="x">First cell.</param>
		/// <param name="y">Second cell.</param>
		/// <returns>Comparison result.</returns>
		protected static int CellComparison(TableHeaderCellInfo x, TableHeaderCellInfo y)
		{
			return x.Position.CompareTo(y.Position);
		}

		/// <summary>
		/// Check if cursor in active region to resize.
		/// </summary>
		/// <param name="position">Cursor position.</param>
		/// <param name="camera">Camera.</param>
		/// <returns>true if cursor in active region to resize; otherwise, false.</returns>
		protected virtual bool CheckInActiveRegion(Vector2 position, Camera camera)
		{
			Vector2 point;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, position, camera, out point))
			{
				return false;
			}

			var rect = RectTransform.rect;
			if (!rect.Contains(point))
			{
				return false;
			}

			point += new Vector2(rect.width * RectTransform.pivot.x, 0);

			int i = 0;
			foreach (var cell in CellsInfoOrdered)
			{
				if (!cell.ActiveSelf)
				{
					i++;
					continue;
				}

				if (GetTargetIndex(i, -1) != -1)
				{
					if (CheckLeft(cell.Rect, point))
					{
						return true;
					}
				}

				if (GetTargetIndex(i, +1) != -1)
				{
					if (CheckRight(cell.Rect, point))
					{
						return true;
					}
				}

				i++;
			}

			return false;
		}

		float widthLimit;

		float leftTargetWidth;

		float rightTargetWidth;

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			if (!AllowResize)
			{
				return;
			}

			if (ProcessCellReorder)
			{
				return;
			}

			Vector2 point;
			processDrag = false;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out point))
			{
				return;
			}

			var r = RectTransform.rect;
			point += new Vector2(r.width * RectTransform.pivot.x, 0);

			foreach (var cell in CellsInfoOrdered)
			{
				var i = cell.Position;
				if (!cell.ActiveSelf)
				{
					continue;
				}

				if (CheckLeft(cell.Rect, point))
				{
					var left = GetTargetIndex(i, -1);
					if (left != -1)
					{
						processDrag = true;

						var left_cell = GetCellInfo(left);
						leftTarget = left_cell.LayoutElement;
						rightTarget = cell.LayoutElement;

						leftTargetWidth = left_cell.Width;
						rightTargetWidth = cell.Width;

						widthLimit = leftTargetWidth + rightTargetWidth;
						return;
					}
				}

				if (CheckRight(cell.Rect, point))
				{
					var right = GetTargetIndex(i, +1);
					if (right != -1)
					{
						processDrag = true;

						var right_cell = GetCellInfo(right);

						leftTarget = cell.LayoutElement;
						rightTarget = right_cell.LayoutElement;

						leftTargetWidth = cell.Width;
						rightTargetWidth = right_cell.Width;

						widthLimit = leftTargetWidth + rightTargetWidth;
						return;
					}
				}
			}
		}

		TableHeaderCellInfo GetCellInfo(int position)
		{
			foreach (var cell in CellsInfo)
			{
				if (cell.Position == position)
				{
					return cell;
				}
			}

			return null;
		}

		int GetTargetIndex(int index, int direction)
		{
			if ((index + direction) == -1)
			{
				return -1;
			}

			if ((index + direction) == CellsInfo.Count)
			{
				return -1;
			}

			var is_active = GetCellInfo(index + direction).ActiveSelf;

			var result = is_active
				? index + direction
				: GetTargetIndex(index + direction, direction);

			return result;
		}

		/// <summary>
		/// Checks if point in the left region.
		/// </summary>
		/// <returns><c>true</c>, if point in the left region, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">RectTransform.</param>
		/// <param name="point">Point.</param>
		bool CheckLeft(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;
			r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
			r.width = ActiveRegion;

			return r.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="childRectTransform">Child RectTransform.</param>
		/// <param name="point">Point.</param>
		bool CheckRight(RectTransform childRectTransform, Vector2 point)
		{
			var r = childRectTransform.rect;

			r.position += new Vector2(childRectTransform.anchoredPosition.x, 0);
			r.position = new Vector2(r.position.x + r.width - ActiveRegion - 1, r.position.y);
			r.width = ActiveRegion + 1;

			return r.Contains(point);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			ResetCursor();

			ResetChildren();
			if (!OnDragUpdate)
			{
				Resize();
			}

			processDrag = false;
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			cursorChanged = true;
			UICursor.Set(this, GetCursor());

			Vector2 current_point;
			Vector2 original_point;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out original_point);

			var delta = current_point - original_point;

			if (delta.x > 0)
			{
				leftTarget.preferredWidth = Mathf.Min(leftTargetWidth + delta.x, widthLimit - rightTarget.minWidth);
				rightTarget.preferredWidth = widthLimit - leftTarget.preferredWidth;
			}
			else
			{
				rightTarget.preferredWidth = Mathf.Min(rightTargetWidth - delta.x, widthLimit - leftTarget.minWidth);
				leftTarget.preferredWidth = widthLimit - rightTarget.preferredWidth;
			}

			LayoutUtilities.UpdateLayout(layout);

			if (OnDragUpdate)
			{
				Resize();
			}
		}

		/// <summary>
		/// Resets the children widths.
		/// </summary>
		void ResetChildren()
		{
			foreach (var cell in CellsInfo)
			{
				ResetChildrenWidth(cell);
			}
		}

		void ResetChildrenWidth(TableHeaderCellInfo cell)
		{
			cell.LayoutElement.preferredWidth = cell.Rect.rect.width;
		}

		/// <summary>
		/// Resize items in ListView.
		/// </summary>
		public void Resize()
		{
			if (List == null)
			{
				return;
			}

			if (CellsInfo.Count < 2)
			{
				return;
			}

			List.Init();
			List.ForEachComponent(ResizeComponent);
		}

		/// <summary>
		/// Resizes the game object.
		/// </summary>
		/// <param name="go">Game object.</param>
		/// <param name="index">The index.</param>
		void ResizeGameObject(GameObject go, int index)
		{
			var cell = GetCellInfo(index);

			(go.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cell.Width);

			var layoutElement = go.GetComponent<LayoutElement>();
			if (layoutElement)
			{
				layoutElement.minWidth = cell.LayoutElement.minWidth;
				layoutElement.flexibleWidth = cell.LayoutElement.flexibleWidth;
				layoutElement.preferredWidth = cell.Width;
			}
		}

		/// <summary>
		/// Resizes the component.
		/// </summary>
		/// <param name="component">Component.</param>
		void ResizeComponent(ListViewItem component)
		{
			for (int i = 0; i < component.transform.childCount; i++)
			{
				ResizeGameObject(component.transform.GetChild(i).gameObject, i);
			}
		}

		static void ReorderComponent(ListViewItem component, int prevPosition, int newPosition)
		{
			var target = component.transform.GetChild(prevPosition);
			target.SetSiblingIndex(newPosition);
		}

		/// <summary>
		/// Move column from oldColumnPosition to newColumnPosition.
		/// </summary>
		/// <param name="oldColumnPosition">Old column position.</param>
		/// <param name="newColumnPosition">New column position.</param>
		public void Reorder(int oldColumnPosition, int newColumnPosition)
		{
			if (CellsInfo.Count < 2)
			{
				return;
			}

			if (List != null)
			{
				List.Init();

#if CSHARP_7_3_OR_NEWER
				void Action(ListViewItem component)
#else
				Action<ListViewItem> Action = component =>
#endif
				{
					ReorderComponent(component, CellsInfo[oldColumnPosition].Position, CellsInfo[newColumnPosition].Position);
				}
#if !CSHARP_7_3_OR_NEWER
				;
#endif

				List.ForEachComponent(Action);
			}

			var target = CellsInfo[oldColumnPosition].Rect;
			target.SetSiblingIndex(CellsInfo[newColumnPosition].Position);

			for (int i = 0; i < CellsInfo.Count; i++)
			{
				CellsInfo[i].Position = CellsInfo[i].Rect.GetSiblingIndex();
			}
		}

#region IDropSupport<TableHeaderCell>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TableHeaderDragCell data, PointerEventData eventData)
		{
			if (!IsActive())
			{
				return false;
			}

			if (!AllowReorder)
			{
				return false;
			}

			var target = FindTarget(eventData);
			if ((target == null) || (target.GetInstanceID() == data.GetInstanceID()))
			{
				return false;
			}

			ShowDropIndicator(CellsInfo[target.Position].Position);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TableHeaderDragCell data, PointerEventData eventData)
		{
			HideDropIndicator();

			var target = FindTarget(eventData);

			Reorder(data.Position, target.Position);
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Cell.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TableHeaderDragCell data, PointerEventData eventData)
		{
			HideDropIndicator();
		}

		/// <summary>
		/// Change value position in specified list.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="oldPosition">Old position.</param>
		/// <param name="newPosition">New position.</param>
		protected static void ChangePosition<T>(List<T> list, int oldPosition, int newPosition)
		{
			var item = list[oldPosition];
			list.RemoveAt(oldPosition);
			list.Insert(newPosition, item);
		}

		readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

		/// <summary>
		/// Get TableHeaderDragCell in position specified with event data.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>TableHeaderDragCell if found; otherwise null.</returns>
		protected TableHeaderDragCell FindTarget(PointerEventData eventData)
		{
			raycastResults.Clear();

			EventSystem.current.RaycastAll(eventData, raycastResults);

			foreach (var raycastResult in raycastResults)
			{
				if (!raycastResult.isValid)
				{
					continue;
				}

				var target = raycastResult.gameObject.GetComponent<TableHeaderDragCell>();
				if ((target != null) && target.transform.IsChildOf(transform))
				{
					return target;
				}
			}

			return null;
		}
#endregion

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			CellsInfo.Clear();
			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var child = RectTransform.GetChild(i);
				var events = child.GetComponent<TableHeaderCell>();
				if (events == null)
				{
					continue;
				}

				events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
				events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
				events.OnDragEvent.RemoveListener(OnDrag);
				events.OnEndDragEvent.RemoveListener(OnEndDrag);
			}
		}

		/// <summary>
		/// Refresh header.
		/// </summary>
		public void Refresh()
		{
			if (layout == null)
			{
				layout = GetComponent<LayoutGroup>();
			}

			if (layout != null)
			{
				LayoutUtilities.UpdateLayout(layout);
			}

			InitCells();

			ResetChildren();
			Resize();
		}

		/// <summary>
		/// Change column state.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="active">If set state to active.</param>
		public void ColumnToggle(int index, bool active)
		{
			var target = CellsInfo[index];
			target.Rect.gameObject.SetActive(active);

#if CSHARP_7_3_OR_NEWER
			void Action(ListViewItem component)
#else
			Action<ListViewItem> Action = component =>
#endif
			{
				var child = component.transform.GetChild(target.Position);
				child.gameObject.SetActive(active);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			List.ForEachComponent(Action);

			List.ComponentsColoring();

			Refresh();
		}

		/// <summary>
		/// Disable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ColumnDisable(int index)
		{
			ColumnToggle(index, false);
		}

		/// <summary>
		/// Enable column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void ColumnEnable(int index)
		{
			ColumnToggle(index, true);
		}

		/// <summary>
		/// Add header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		public void AddCell(GameObject cell)
		{
			cell.transform.SetParent(transform, false);
			cell.SetActive(true);

			Refresh();
		}

		/// <summary>
		/// Get cell index for the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		/// <returns>Index of the cell.</returns>
		protected int GetCellIndex(GameObject go)
		{
			for (int i = 0; i < CellsInfo.Count; i++)
			{
				if (CellsInfo[i].Rect.gameObject == go)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Remove header cell.
		/// </summary>
		/// <param name="cell">Cell.</param>
		/// <param name="parent">Parent.</param>
		public void RemoveCell(GameObject cell, RectTransform parent = null)
		{
			var index = GetCellIndex(cell);
			if (index == -1)
			{
				Debug.LogWarning("Cell not in header", cell);
				return;
			}

			cell.SetActive(false);
			cell.transform.SetParent(parent, false);
			if (parent == null)
			{
				Destroy(cell);
			}

			// remove from cells
			CellsInfo.RemoveAt(index);

			// remove events
			var events = Utilities.GetOrAddComponent<TableHeaderCell>(cell);
			events.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
			events.OnBeginDragEvent.RemoveListener(OnBeginDrag);
			events.OnDragEvent.RemoveListener(OnDrag);
			events.OnEndDragEvent.RemoveListener(OnEndDrag);

			// decrease position for cells where >cell_position
			var cell_info = CellsInfo[index];
			for (int i = 0; i < CellsInfo.Count; i++)
			{
				if (CellsInfo[i].Position > cell_info.Position)
				{
					CellsInfo[i].Position -= 1;
				}
			}

			// update list widths
			LayoutUtilities.UpdateLayout(layout);
			Resize();
		}

		#if UNITY_EDITOR

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			CursorsDPISelector.Require(this);

			base.Reset();
		}

		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			// apply style for header
			style.Table.Border.ApplyTo(gameObject);

			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var cell = RectTransform.GetChild(i);
				var style_support = cell.GetComponent<StyleSupportHeaderCell>();

				if (style_support != null)
				{
					style_support.SetStyle(style);
				}
				else
				{
					style.Table.Background.ApplyTo(cell);
					style.Table.HeaderText.ApplyTo(cell.Find("Text"));
				}
			}

			// apply style to list
			style.Table.Border.ApplyTo(List.Container);

			#if CSHARP_7_3_OR_NEWER
			void Action(ListViewItem component)
			#else
			Action<ListViewItem> Action = component =>
			#endif
			{
				style.Table.Border.ApplyTo(component);

				for (int i = 0; i < component.RectTransform.childCount; i++)
				{
					var child = component.RectTransform.GetChild(i);
					style.Table.Background.ApplyTo(child.gameObject);
				}
			}
			#if !CSHARP_7_3_OR_NEWER
			;
			#endif

			List.ForEachComponent(Action);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			// got style from header
			style.Table.Border.GetFrom(gameObject);

			for (int i = 0; i < RectTransform.childCount; i++)
			{
				var cell = RectTransform.GetChild(i);
				var style_support = cell.GetComponent<StyleSupportHeaderCell>();

				if (style_support != null)
				{
					style_support.GetStyle(style);
				}
				else
				{
					style.Table.Background.GetFrom(cell);
					style.Table.HeaderText.GetFrom(cell.Find("Text"));
				}
			}

			// got style from list
			style.Table.Border.GetFrom(List.Container);

			return true;
		}

		#endregion
	}
}