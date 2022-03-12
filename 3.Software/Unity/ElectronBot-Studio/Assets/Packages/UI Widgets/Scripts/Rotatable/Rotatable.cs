namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Rotatable.
	/// N - north (top).
	/// S - south (bottom).
	/// E - east (right).
	/// W - west (left).
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Rotatable")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class Rotatable : UIBehaviourConditional,
		IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler,
		ILateUpdatable
	{
		/// <summary>
		/// Rotation directions.
		/// </summary>
		[Serializable]
		public struct Directions : IEquatable<Directions>
		{
			/// <summary>
			/// Allow rotate with top left corner.
			/// </summary>
			public bool TopLeft;

			/// <summary>
			/// Allow rotate with top right corner.
			/// </summary>
			public bool TopRight;

			/// <summary>
			/// Allow rotate with bottom left corner.
			/// </summary>
			public bool BottomLeft;

			/// <summary>
			/// Allow rotate with bottom right corner.
			/// </summary>
			public bool BottomRight;

			/// <summary>
			/// Initializes a new instance of the <see cref="UIWidgets.Rotatable.Directions"/> struct.
			/// </summary>
			/// <param name="topLeft">If set to <c>true</c> allow rotation from top left corner.</param>
			/// <param name="topRight">If set to <c>true</c> allow rotation from top right corner.</param>
			/// <param name="bottomLeft">If set to <c>true</c> allow rotation from bottom left corner.</param>
			/// <param name="bottomRight">If set to <c>true</c> allow rotation from bottom right corner.</param>
			public Directions(bool topLeft = true, bool topRight = true, bool bottomLeft = true, bool bottomRight = true)
			{
				TopLeft = topLeft;
				TopRight = topRight;

				BottomLeft = bottomLeft;
				BottomRight = bottomRight;
			}

			/// <summary>
			/// Gets a value indicating whether any direction is allowed.
			/// </summary>
			/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
			public bool Active
			{
				get
				{
					return TopLeft || TopRight || BottomLeft || BottomRight;
				}
			}

			/// <summary>
			/// North-West or South-East.
			/// </summary>
			/// <value><c>true</c> if allowed direction is NWSE; otherwise, <c>false</c>.</value>
			public bool NWSE
			{
				get
				{
					return TopLeft || BottomRight;
				}
			}

			/// <summary>
			/// North-East or South-West.
			/// </summary>
			/// <value><c>true</c> if allowed direction is NESW; otherwise, <c>false</c>.</value>
			public bool NESW
			{
				get
				{
					return TopRight || BottomLeft;
				}
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is Directions)
				{
					return Equals((Directions)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(Directions other)
			{
				return TopLeft == other.TopLeft
					&& TopRight == other.TopRight
					&& BottomLeft == other.BottomLeft
					&& BottomRight == other.BottomRight;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				var code = 0;
				if (TopLeft)
				{
					code += 1;
				}

				if (TopRight)
				{
					code += 2;
				}

				if (BottomLeft)
				{
					code += 4;
				}

				if (BottomRight)
				{
					code += 8;
				}

				return code;
			}

			/// <summary>
			/// Compare specified colors.
			/// </summary>
			/// <param name="directions1">First direction.</param>
			/// <param name="directions2">Second direction.</param>
			/// <returns>true if the directions are equal; otherwise, false.</returns>
			public static bool operator ==(Directions directions1, Directions directions2)
			{
				return directions1.Equals(directions2);
			}

			/// <summary>
			/// Compare specified directions.
			/// </summary>
			/// <param name="directions1">First direction.</param>
			/// <param name="directions2">Second direction.</param>
			/// <returns>true if the directions not equal; otherwise, false.</returns>
			public static bool operator !=(Directions directions1, Directions directions2)
			{
				return !directions1.Equals(directions2);
			}
		}

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

					if (!processDrag)
					{
						ResetCursor();
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Rotate directions.
		/// </summary>
		[SerializeField]
		public Directions RotateDirections = new Directions(true, true, true, true);

		/// <summary>
		/// The active region in points from left or right border where rotation allowed.
		/// </summary>
		[SerializeField]
		[Tooltip("Maximum padding from border where rotation allowed.")]
		public float ActiveRegion = 5;

		/// <summary>
		/// Limit rotation angle.
		/// </summary>
		[SerializeField]
		public bool LimitRotation = false;

		[SerializeField]
		[Tooltip("Allowed value is in range [-180..180]")]
		[EditorConditionBool("LimitRotation")]
		float angleMin = -180f;

		/// <summary>
		/// The minimum angle.
		/// Allowed value is in range [-180..180].
		/// </summary>
		public float AngleMin
		{
			get
			{
				return angleMin;
			}

			set
			{
				angleMin = Mathf.Clamp(value, -180f, 180f);
			}
		}

		[SerializeField]
		[Tooltip("Allowed value is in range [-180..180]")]
		[EditorConditionBool("LimitRotation")]
		float angleMax = 180f;

		/// <summary>
		/// The maximum angle.
		/// Allowed value is in range [-180..180].
		/// </summary>
		public float AngleMax
		{
			get
			{
				return angleMax;
			}

			set
			{
				angleMax = Mathf.Clamp(value, -180f, 180f);
			}
		}

		[SerializeField]
		[Tooltip("Allowed value is in range [0..180). Set 0 to disable.")]
		float angleStep = 1f;

		/// <summary>
		/// Angle step.
		/// </summary>
		public float AngleStep
		{
			get
			{
				return angleStep;
			}

			set
			{
				angleStep = Mathf.Clamp(value, 0f, 180f);
			}
		}

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
		/// The cursor NW texture.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorTopLeftTexture;

		/// <summary>
		/// The cursor NW hot spot.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorTopLeftHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor NE texture.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorTopRightTexture;

		/// <summary>
		/// The cursor NE hot spot.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorTopRightHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor SW texture.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorBottomLeftTexture;

		/// <summary>
		/// The cursor SW hot spot.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorBottomLeftHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor SE texture.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorBottomRightTexture;

		/// <summary>
		/// The cursor SE hot spot.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorBottomRightHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		[EditorConditionBlock("Cursors")]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// Start rotation event.
		/// </summary>
		[SerializeField]
		public RotatableEvent OnStartRotate = new RotatableEvent();

		/// <summary>
		/// During rotation event.
		/// </summary>
		[SerializeField]
		public RotatableEvent OnRotate = new RotatableEvent();

		/// <summary>
		/// End rotation event.
		/// </summary>
		[SerializeField]
		public RotatableEvent OnEndRotate = new RotatableEvent();

		RectTransform rectTransform;

		/// <summary>
		/// Target.
		/// </summary>
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
		/// Is target is self?
		/// </summary>
		protected bool IsTargetSelf;

		RectTransform target;

		/// <summary>
		/// Target.
		/// </summary>
		public RectTransform Target
		{
			get
			{
				if (target == null)
				{
					IsTargetSelf = true;
					target = transform as RectTransform;
				}

				return target;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				IsTargetSelf = value.GetInstanceID() == transform.GetInstanceID();
				target = value;

				if (!IsTargetSelf)
				{
					var le = Utilities.GetOrAddComponent<LayoutElement>(this);
					le.ignoreLayout = true;

					RectTransform.SetParent(target.parent, false);

					CopyRectTransformValues();
				}
			}
		}

		/// <summary>
		/// Current drag regions.
		/// </summary>
		protected Resizable.Regions regions;

		/// <summary>
		/// Drag regions.
		/// </summary>
		protected Resizable.Regions dragRegions;

		/// <summary>
		/// Allow to handle drag event.
		/// </summary>
		protected bool processDrag;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			Init();
		}

		bool isInited;

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
		/// The cursor changed.
		/// </summary>
		protected bool cursorChanged;

		/// <summary>
		/// Is cursor over.
		/// </summary>
		protected bool IsCursorOver;

		/// <summary>
		/// Start rotation angle.
		/// </summary>
		protected float StartAngle;

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

			CurrentCamera = eventData.pressEventCamera;
			IsCursorOver = true;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerExit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOver = false;

			if (!processDrag)
			{
				ResetCursor();
			}
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
		/// Late update.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (!IsActive())
			{
				return;
			}

			if (!CanChangeCursor)
			{
				return;
			}

			if (CompatibilityInput.IsMouseLeftButtonPressed)
			{
				return;
			}

			if (processDrag)
			{
				return;
			}

			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, CompatibilityInput.MousePosition, CurrentCamera, out point))
			{
				return;
			}

			var r = Target.rect;
			if (r.Contains(point))
			{
				UpdateRegions(point);
			}
			else
			{
				regions.Reset();
			}

			UpdateCursor();
		}

		void UpdateRegions(Vector2 point)
		{
			regions.Top = CheckTop(point);
			regions.Bottom = CheckBottom(point);
			regions.Left = CheckLeft(point);
			regions.Right = CheckRight(point);
		}

		/// <summary>
		/// Updates the cursor.
		/// </summary>
		protected virtual void UpdateCursor()
		{
			if (regions.TopLeft && RotateDirections.TopLeft)
			{
				cursorChanged = true;
				UICursor.Set(this, GetTopLeftCursor());
			}
			else if (regions.TopRight && RotateDirections.TopRight)
			{
				cursorChanged = true;
				UICursor.Set(this, GetTopRightCursor());
			}
			else if (regions.BottomLeft && RotateDirections.BottomLeft)
			{
				cursorChanged = true;
				UICursor.Set(this, GetBottomLeftCursor());
			}
			else if (regions.BottomRight && RotateDirections.BottomRight)
			{
				cursorChanged = true;
				UICursor.Set(this, GetBottomRightCursor());
			}
			else if (cursorChanged)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Get top left cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetTopLeftCursor()
		{
			if (Cursors != null)
			{
				return Cursors.NorthWestRotateArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.NorthWestRotateArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Get top right cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetTopRightCursor()
		{
			if (Cursors != null)
			{
				return Cursors.NorthEastRotateArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.NorthEastRotateArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Get bottom left cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetBottomLeftCursor()
		{
			if (Cursors != null)
			{
				return Cursors.SouthWestRotateArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.SouthWestRotateArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Get bottom right cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetBottomRightCursor()
		{
			if (Cursors != null)
			{
				return Cursors.SouthEastRotateArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.SouthEastRotateArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Checks if point in the top region.
		/// </summary>
		/// <returns><c>true</c>, if point in the top region, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		bool CheckTop(Vector2 point)
		{
			var rect = Target.rect;

			rect.position = new Vector2(rect.position.x, rect.position.y + rect.height - ActiveRegion);
			rect.height = ActiveRegion;

			return rect.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		bool CheckBottom(Vector2 point)
		{
			var rect = Target.rect;
			rect.height = ActiveRegion;

			return rect.Contains(point);
		}

		/// <summary>
		/// Checks if point in the left region.
		/// </summary>
		/// <returns><c>true</c>, if point in the left region, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		bool CheckLeft(Vector2 point)
		{
			var rect = Target.rect;
			rect.width = ActiveRegion;

			return rect.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		bool CheckRight(Vector2 point)
		{
			var rect = Target.rect;

			rect.position = new Vector2(rect.position.x + rect.width - ActiveRegion, rect.position.y);
			rect.width = ActiveRegion;

			return rect.Contains(point);
		}

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

			Vector2 point;
			processDrag = false;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.pressPosition, eventData.pressEventCamera, out point))
			{
				return;
			}

			UpdateRegions(point);

			processDrag = IsAllowedDrag();

			dragRegions = regions;

			UpdateCursor();

			if (processDrag)
			{
				var angle = 0f;
				if (regions.TopLeft)
				{
					angle = -135f;
				}
				else if (regions.TopRight)
				{
					angle = -45f;
				}
				else if (regions.BottomLeft && RotateDirections.BottomLeft)
				{
					angle = 135f;
				}
				else if (regions.BottomRight && RotateDirections.BottomRight)
				{
					angle = 45f;
				}

				InitRotate(angle);

				OnStartRotate.Invoke(this);

				if (!eventData.used)
				{
					eventData.Use();
				}
			}
		}

		/// <summary>
		/// Determines whether drag allowed with current active regions and specified directions.
		/// </summary>
		/// <returns><c>true</c> if this instance is allowed drag; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAllowedDrag()
		{
			return (regions.TopLeft && RotateDirections.TopLeft)
				|| (regions.BottomRight && RotateDirections.BottomRight)
				|| (regions.TopRight && RotateDirections.TopRight)
				|| (regions.BottomLeft && RotateDirections.BottomLeft);
		}

		void ResetCursor()
		{
			cursorChanged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			ResetCursor();

			if (processDrag)
			{
				processDrag = false;

				OnEndRotate.Invoke(this);
			}
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			if (eventData.used)
			{
				return;
			}

			eventData.Use();

			Rotate(eventData);
		}

		/// <summary>
		/// Init rotation.
		/// </summary>
		/// <param name="startAngle">Start angle for the rotation.</param>
		public virtual void InitRotate(float startAngle)
		{
			StartAngle = startAngle;
		}

		/// <summary>
		/// Rotate this instance using specified event data.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void Rotate(PointerEventData eventData)
		{
			Vector2 point;
			Target.localRotation = Quaternion.Euler(Vector3.zero);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.position, eventData.pressEventCamera, out point);

			var base_angle = DragPoint2Angle(point);
			Rotate(base_angle);

			OnRotate.Invoke(this);
		}

		/// <summary>
		/// Rotate.
		/// </summary>
		/// <param name="baseAngle">Base angle.</param>
		public virtual void Rotate(float baseAngle)
		{
			var angle = ProcessAngle(baseAngle);

			var rotation = Target.localRotation.eulerAngles;
			rotation.z = angle;
			Target.localRotation = Quaternion.Euler(rotation);

			CopyRectTransformValues();
		}

		/// <summary>
		/// Process angle value.
		/// </summary>
		/// <param name="baseAngle">Initial angle.</param>
		/// <returns>Angle with applied limits and step.</returns>
		protected float ProcessAngle(float baseAngle)
		{
			var angle = (baseAngle + StartAngle) % 360f;

			if (angle > 180f)
			{
				angle -= 360f;
			}

			if (angle <= -180f)
			{
				angle += 360f;
			}

			if (AngleStep > 0)
			{
				angle = Mathf.Round(angle / AngleStep) * AngleStep;
			}

			if (LimitRotation)
			{
				angle = ClampAngle(angle);
			}

			return angle;
		}

		/// <summary>
		/// Get shortest distance between angle.
		/// </summary>
		/// <param name="a">First angle.</param>
		/// <param name="b">Second angle.</param>
		/// <returns>Shortest distance.</returns>
		protected virtual float AngleDistance(float a, float b)
		{
			var d = Mathf.Abs(a - b);
			if (d > 180f)
			{
				d = 360f - d;
			}

			return d;
		}

		/// <summary>
		/// Clamp angle.
		/// </summary>
		/// <param name="angle">Angle.</param>
		/// <returns>Angle limited by AngleMin and AngleMax.</returns>
		protected float ClampAngle(float angle)
		{
			if ((angle >= AngleMin) && (angle <= AngleMax))
			{
				return angle;
			}

			var distance2min = AngleDistance(angle + 180f, AngleMin + 180f);
			var distance2max = AngleDistance(angle + 180f, AngleMax + 180f);
			return (distance2min < distance2max) ? AngleMin : AngleMax;
		}

		/// <summary>
		/// Convert drag point to angle.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Angle.</returns>
		protected float DragPoint2Angle(Vector2 point)
		{
			var size = Target.rect.size;
			var relative = new Vector2(
				point.x + (size.x * (Target.pivot.x - 0.5f)),
				point.y + (size.y * (Target.pivot.y - 0.5f)));

			return Point2Angle(relative);
		}

		/// <summary>
		/// Convert point to the angle.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Angle in range [-180f, 180f].</returns>
		public static float Point2Angle(Vector2 point)
		{
			var angle_rad = Mathf.Atan2(point.y, point.x);
			var angle = angle_rad * Mathf.Rad2Deg;

			angle %= 360f;

			if (angle > 180f)
			{
				angle -= 360f;
			}

			return angle;
		}

		/// <summary>
		/// Copy RectTransform values.
		/// </summary>
		protected void CopyRectTransformValues()
		{
			if (!IsTargetSelf)
			{
				UtilitiesRectTransform.CopyValues(Target, RectTransform);
			}
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

		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			AngleMin = angleMin;
			AngleMax = angleMax;
			AngleStep = angleStep;
		}

		#endif
	}
}