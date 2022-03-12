namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;

	/// <summary>
	/// Drag support.
	/// Drop component should implement IDropSupport{TItem} with same type.
	/// </summary>
	/// <typeparam name="TItem">Type of draggable data.</typeparam>
	public abstract class DragSupport<TItem> : BaseDragSupport, ICancelHandler
	{
		/// <summary>
		/// Raycast results.
		/// </summary>
		[NonSerialized]
		protected readonly List<RaycastResult> RaycastResults = new List<RaycastResult>();

		/// <summary>
		/// Allow drag.
		/// </summary>
		[SerializeField]
		public bool AllowDrag = true;

		/// <summary>
		/// Gets or sets the data.
		/// Data will be passed to Drop component.
		/// </summary>
		/// <value>The data.</value>
		public TItem Data
		{
			get;
			protected set;
		}

		/// <summary>
		/// Cursors.
		/// </summary>
		[SerializeField]
		public Cursors Cursors;

		/// <summary>
		/// The Allow drop cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D AllowDropCursor;

		/// <summary>
		/// The Allow drop cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 AllowDropCursorHotSpot = new Vector2(4, 2);

		/// <summary>
		/// The Denied drop cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DeniedDropCursor;

		/// <summary>
		/// The Denied drop cursor hot spot.
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

		[SerializeField]
		[FormerlySerializedAs("dragHandle")]
		DragSupportHandle handle;

		/// <summary>
		/// Drag handle.
		/// </summary>
		public DragSupportHandle Handle
		{
			get
			{
				return handle;
			}

			set
			{
				if (handle != null)
				{
					handle.OnInitializePotentialDragEvent.RemoveListener(OnInitializePotentialDrag);
					handle.OnBeginDragEvent.RemoveListener(OnBeginDrag);
					handle.OnDragEvent.RemoveListener(OnDrag);
					handle.OnEndDragEvent.RemoveListener(OnEndDrag);
				}

				handle = (value != null)
					? value
					: Utilities.GetOrAddComponent<DragSupportHandle>(this);

				if (handle != null)
				{
					handle.OnInitializePotentialDragEvent.AddListener(OnInitializePotentialDrag);
					handle.OnBeginDragEvent.AddListener(OnBeginDrag);
					handle.OnDragEvent.AddListener(OnDrag);
					handle.OnEndDragEvent.AddListener(OnEndDrag);
				}
			}
		}

		/// <summary>
		/// Drag handle.
		/// </summary>
		[Obsolete("Renamed to Handle.")]
		public DragSupportHandle DragHandle
		{
			get
			{
				return Handle;
			}

			set
			{
				Handle = value;
			}
		}

		/// <summary>
		/// Event on start drag.
		/// </summary>
		[SerializeField]
		public UnityEvent StartDragEvent = new UnityEvent();

		/// <summary>
		/// Event on end drag.
		/// </summary>
		[SerializeField]
		public UnityEvent EndDragEvent = new UnityEvent();

		bool isInited;

		/// <summary>
		/// The current drop target.
		/// </summary>
		protected IDropSupport<TItem> CurrentTarget;

		/// <summary>
		/// The current drop target.
		/// </summary>
		protected IAutoScroll CurrentAutoScrollTarget;

		/// <summary>
		/// If this object is dragged?
		/// </summary>
		protected bool IsDragged;

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

			Handle = handle;
		}

		/// <summary>
		/// Set target.
		/// </summary>
		/// <param name="newTarget">New target.</param>
		/// <param name="eventData">Event data.</param>
		protected virtual void SetTarget(IDropSupport<TItem> newTarget, PointerEventData eventData)
		{
			if (CurrentTarget == newTarget)
			{
				return;
			}

			if (CurrentTarget != null)
			{
				CurrentTarget.DropCanceled(Data, eventData);
			}

			OnTargetChanged(CurrentTarget, newTarget);

			CurrentTarget = newTarget;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			Init();
		}

		/// <summary>
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		public virtual bool CanDrag(PointerEventData eventData)
		{
			return AllowDrag;
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected abstract void InitDrag(PointerEventData eventData);

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public virtual void Dropped(bool success)
		{
			Data = default(TItem);
		}

		/// <summary>
		/// Process OnInitializePotentialDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			SetTarget(null, eventData);
		}

		/// <summary>
		/// Process OnBeginDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (CanDrag(eventData))
			{
				StartDragEvent.Invoke();

				EventSystem.current.SetSelectedGameObject(gameObject);
				IsDragged = true;
				InitDrag(eventData);

				FillRaycasts(eventData, RaycastResults);
				FindCurrentTarget(eventData, RaycastResults);
			}
		}

		/// <summary>
		/// Find current target.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="raycasts">Raycast results.</param>
		protected virtual void FindCurrentTarget(PointerEventData eventData, List<RaycastResult> raycasts)
		{
			var new_target = FindTarget(eventData, RaycastResults);

			SetTarget(new_target, eventData);

			if (UICursor.CanSet(this))
			{
				var cursor = (CurrentTarget != null) ? GetAllowedCursor() : GetDeniedCursor();
				UICursor.Set(this, cursor);
			}
		}

		/// <summary>
		/// Get allowed cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetAllowedCursor()
		{
			if (Cursors != null)
			{
				return Cursors.Allowed;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.Allowed;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Get denied cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetDeniedCursor()
		{
			if (Cursors != null)
			{
				return Cursors.Denied;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.Denied;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Process OnDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			FillRaycasts(eventData, RaycastResults);
			FindCurrentTarget(eventData, RaycastResults);

			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ParentCanvas as RectTransform, eventData.position, eventData.pressEventCamera, out point))
			{
				return;
			}

			DragPoint.localPosition = point;

			var target = FindAutoScrollTarget(eventData, RaycastResults);

			if (target != CurrentAutoScrollTarget)
			{
				if (CurrentAutoScrollTarget != null)
				{
					CurrentAutoScrollTarget.AutoScrollStop();
				}

				CurrentAutoScrollTarget = target;
			}

			if (CurrentAutoScrollTarget != null)
			{
				CurrentAutoScrollTarget.AutoScrollStart(eventData, OnDrag);
			}
		}

		/// <summary>
		/// Process current drop target changed event.
		/// </summary>
		/// <param name="old">Previous drop target.</param>
		/// <param name="current">Current drop target.</param>
		protected virtual void OnTargetChanged(IDropSupport<TItem> old, IDropSupport<TItem> current)
		{
		}

		/// <summary>
		/// Fill raycast results.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="raycasts">Output.</param>
		protected virtual void FillRaycasts(PointerEventData eventData, List<RaycastResult> raycasts)
		{
			raycasts.Clear();

			EventSystem.current.RaycastAll(eventData, raycasts);
		}

		/// <summary>
		/// Finds the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="eventData">Event data.</param>
		/// <param name="raycasts">Raycast results.</param>
		protected virtual IDropSupport<TItem> FindTarget(PointerEventData eventData, List<RaycastResult> raycasts)
		{
			foreach (var raycast in raycasts)
			{
				if (!raycast.isValid)
				{
					continue;
				}

				#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				var target = raycast.gameObject.GetComponent<IDropSupport<TItem>>();
				#else
				var target = raycast.gameObject.GetComponent(typeof(IDropSupport<T>)) as IDropSupport<T>;
				#endif
				if (target != null)
				{
					return CheckTarget(target, eventData) ? target : null;
				}
			}

			return null;
		}

		/// <summary>
		/// Finds the auto-scroll target.
		/// </summary>
		/// <returns>The auto-scroll  target.</returns>
		/// <param name="eventData">Event data.</param>
		/// <param name="raycasts">Raycast results.</param>
		protected virtual IAutoScroll FindAutoScrollTarget(PointerEventData eventData, List<RaycastResult> raycasts)
		{
			foreach (var raycast in raycasts)
			{
				if (!raycast.isValid)
				{
					continue;
				}

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				var target = raycast.gameObject.GetComponent<IAutoScroll>();
#else
				var target = raycast.gameObject.GetComponent(typeof(IAutoScroll)) as IAutoScroll;
#endif
				if (target != null)
				{
					return target;
				}
			}

			return null;
		}

		/// <summary>
		/// Check if target can receive drop.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if target can receive drop; otherwise false.</returns>
		protected virtual bool CheckTarget(IDropSupport<TItem> target, PointerEventData eventData)
		{
			return target.CanReceiveDrop(Data, eventData);
		}

		/// <summary>
		/// Process OnEndDrag event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			FillRaycasts(eventData, RaycastResults);
			FindCurrentTarget(eventData, RaycastResults);

			if (CurrentTarget != null)
			{
				CurrentTarget.Drop(Data, eventData);

				Dropped(true);
			}
			else
			{
				Dropped(false);
			}

			if (CurrentAutoScrollTarget != null)
			{
				CurrentAutoScrollTarget.AutoScrollStop();
				CurrentAutoScrollTarget = null;
			}

			ResetCursor();

			EndDragEvent.Invoke();
		}

		/// <summary>
		/// Process disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			if (!IsDragged)
			{
				return;
			}

			if (CurrentTarget != null)
			{
				CurrentTarget.DropCanceled(Data, null);
			}

			Dropped(false);

			if (CurrentAutoScrollTarget != null)
			{
				CurrentAutoScrollTarget.AutoScrollStop();
				CurrentAutoScrollTarget = null;
			}

			ResetCursor();

			EndDragEvent.Invoke();
		}

		/// <summary>
		/// Reset cursor.
		/// </summary>
		protected void ResetCursor()
		{
			IsDragged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if ((DragPoints != null) && (ParentCanvas != null) && DragPoints.ContainsKey(ParentCanvas.GetInstanceID()))
			{
				DragPoints.Remove(ParentCanvas.GetInstanceID());
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Process the cancel event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnCancel(BaseEventData eventData)
		{
			if (!IsDragged)
			{
				return;
			}

			if (CurrentTarget != null)
			{
				CurrentTarget.DropCanceled(Data, null);
			}

			Dropped(false);

			ResetCursor();
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
	}
}