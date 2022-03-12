namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Resizable.
	/// N - north (top).
	/// S - south (bottom).
	/// E - east (right).
	/// W - west (left).
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Resizable")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class Resizable : UIBehaviourConditional,
		IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler,
		ILateUpdatable, ISnapGridSupport
	{
		/// <summary>
		/// Resize type.
		/// </summary>
		public enum ResizeType
		{
			/// <summary>
			/// Size.
			/// </summary>
			Size = 0,

			/// <summary>
			/// Scale.
			/// </summary>
			Scale = 1,
		}

		/// <summary>
		/// Resize directions.
		/// </summary>
		[Serializable]
		public struct Directions : IEquatable<Directions>
		{
			/// <summary>
			/// Allow resize with top side.
			/// </summary>
			[SerializeField]
			public bool Top;

			/// <summary>
			/// Allow resize with bottom side.
			/// </summary>
			[SerializeField]
			public bool Bottom;

			/// <summary>
			/// Allow resize with left side.
			/// </summary>
			[SerializeField]
			public bool Left;

			/// <summary>
			/// Allow resize with right side.
			/// </summary>
			[SerializeField]
			public bool Right;

			/// <summary>
			/// Allow resize with top left corner.
			/// </summary>
			[SerializeField]
			public bool TopLeft;

			/// <summary>
			/// Allow resize with top right corner.
			/// </summary>
			[SerializeField]
			public bool TopRight;

			/// <summary>
			/// Allow resize with bottom left corner.
			/// </summary>
			[SerializeField]
			public bool BottomLeft;

			/// <summary>
			/// Allow resize with bottom right corner.
			/// </summary>
			[SerializeField]
			public bool BottomRight;

			/// <summary>
			/// Initializes a new instance of the <see cref="Directions"/> struct.
			/// </summary>
			/// <param name="top">If set to <c>true</c> allow resize from top.</param>
			/// <param name="bottom">If set to <c>true</c> allow resize from bottom.</param>
			/// <param name="left">If set to <c>true</c> allow resize from left.</param>
			/// <param name="right">If set to <c>true</c> allow resize from right.</param>
			/// <param name="topLeft">If set to <c>true</c> allow resize from top left corner.</param>
			/// <param name="topRight">If set to <c>true</c> allow resize from top right corner.</param>
			/// <param name="bottomLeft">If set to <c>true</c> allow resize from bottom left corner.</param>
			/// <param name="bottomRight">If set to <c>true</c> allow resize from bottom right corner.</param>
			public Directions(bool top, bool bottom, bool left, bool right, bool topLeft = true, bool topRight = true, bool bottomLeft = true, bool bottomRight = true)
			{
				Top = top;
				Bottom = bottom;
				Left = left;
				Right = right;

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
					return Top || Bottom || Left || Right || TopLeft || TopRight || BottomLeft || BottomRight;
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
			/// North or South.
			/// </summary>
			/// <value><c>true</c> if allowed direction is NS; otherwise, <c>false</c>.</value>
			public bool NS
			{
				get
				{
					return Top || Bottom;
				}
			}

			/// <summary>
			/// East or West.
			/// </summary>
			/// <value><c>true</c> if allowed direction is EW; otherwise, <c>false</c>.</value>
			public bool EW
			{
				get
				{
					return Left || Right;
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
				return Top == other.Top
					&& Bottom == other.Bottom
					&& Left == other.Left
					&& Right == other.Right
					&& TopLeft == other.TopLeft
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

				if (Top)
				{
					code += 1;
				}

				if (Bottom)
				{
					code += 2;
				}

				if (Left)
				{
					code += 4;
				}

				if (Right)
				{
					code += 8;
				}

				if (TopLeft)
				{
					code += 16;
				}

				if (TopRight)
				{
					code += 32;
				}

				if (BottomLeft)
				{
					code += 64;
				}

				if (BottomRight)
				{
					code += 128;
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

		/// <summary>
		/// Active resize region.
		/// Specifies how position should be changed with resize.
		/// </summary>
		[Serializable]
		public struct Regions : IEquatable<Regions>
		{
			/// <summary>
			/// The top.
			/// </summary>
			[SerializeField]
			public bool Top;

			/// <summary>
			/// The bottom.
			/// </summary>
			[SerializeField]
			public bool Bottom;

			/// <summary>
			/// The left.
			/// </summary>
			[SerializeField]
			public bool Left;

			/// <summary>
			/// The right.
			/// </summary>
			[SerializeField]
			public bool Right;

			/// <summary>
			/// North-West.
			/// </summary>
			/// <value><c>true</c> if cursor mode is NW; otherwise, <c>false</c>.</value>
			[Obsolete("Renamed to the TopLeft.")]
			public bool NW
			{
				get
				{
					return Top && Left;
				}
			}

			/// <summary>
			/// Top left corner.
			/// </summary>
			/// <value><c>true</c> if cursor in the top left corner; otherwise, <c>false</c>.</value>
			public bool TopLeft
			{
				get
				{
					return Top && Left;
				}
			}

			/// <summary>
			/// North-East.
			/// </summary>
			/// <value><c>true</c> if cursor mode is NESW; otherwise, <c>false</c>.</value>
			[Obsolete("Renamed to the TopRight.")]
			public bool NE
			{
				get
				{
					return Top && Right;
				}
			}

			/// <summary>
			/// Top right corner.
			/// </summary>
			/// <value><c>true</c> if cursor in the top right corner; otherwise, <c>false</c>.</value>
			public bool TopRight
			{
				get
				{
					return Top && Right;
				}
			}

			/// <summary>
			/// South-East.
			/// </summary>
			/// <value><c>true</c> if cursor mode is SE; otherwise, <c>false</c>.</value>
			[Obsolete("Renamed to the BottomRight.")]
			public bool SE
			{
				get
				{
					return Bottom && Right;
				}
			}

			/// <summary>
			/// Bottom right corner.
			/// </summary>
			/// <value><c>true</c> if cursor in the bottom right corner; otherwise, <c>false</c>.</value>
			public bool BottomRight
			{
				get
				{
					return Bottom && Right;
				}
			}

			/// <summary>
			/// South-West.
			/// </summary>
			/// <value><c>true</c> if cursor mode is NESW; otherwise, <c>false</c>.</value>
			[Obsolete("Renamed to the BottomLeft.")]
			public bool SW
			{
				get
				{
					return Bottom && Left;
				}
			}

			/// <summary>
			/// Bottom left corner.
			/// </summary>
			/// <value><c>true</c> if cursor in the bottom left corner; otherwise, <c>false</c>.</value>
			public bool BottomLeft
			{
				get
				{
					return Bottom && Left;
				}
			}

			/// <summary>
			/// North-West or South-East.
			/// </summary>
			/// <value><c>true</c> if cursor mode is NWSE; otherwise, <c>false</c>.</value>
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
			/// <value><c>true</c> if cursor mode is NESW; otherwise, <c>false</c>.</value>
			public bool NESW
			{
				get
				{
					return TopRight || BottomLeft;
				}
			}

			/// <summary>
			/// North or South.
			/// </summary>
			/// <value><c>true</c> if cursor mode is NS; otherwise, <c>false</c>.</value>
			public bool NS
			{
				get
				{
					return (Top && !Right) || (Bottom && !Left);
				}
			}

			/// <summary>
			/// East or West.
			/// </summary>
			/// <value><c>true</c> if cursor mode is EW; otherwise, <c>false</c>.</value>
			public bool EW
			{
				get
				{
					return (!Top && Right) || (!Bottom && Left);
				}
			}

			/// <summary>
			/// Is any region active.
			/// </summary>
			/// <value><c>true</c> if any region active; otherwise, <c>false</c>.</value>
			public bool Active
			{
				get
				{
					return Top || Bottom || Left || Right;
				}
			}

			/// <summary>
			/// Reset this instance.
			/// </summary>
			public void Reset()
			{
				Top = false;
				Bottom = false;
				Left = false;
				Right = false;
			}

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
			public override string ToString()
			{
				return string.Format("Top: {0}; Bottom: {1}; Left: {2}; Right: {3}", Top.ToString(), Bottom.ToString(), Left.ToString(), Right.ToString());
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is Regions)
				{
					return Equals((Regions)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(Regions other)
			{
				return Top == other.Top
					&& Bottom == other.Bottom
					&& Left == other.Left
					&& Right == other.Right;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				var code = 0;

				if (Top)
				{
					code += 1;
				}

				if (Bottom)
				{
					code += 2;
				}

				if (Left)
				{
					code += 4;
				}

				if (Right)
				{
					code += 8;
				}

				return code;
			}

			/// <summary>
			/// Compare specified regions.
			/// </summary>
			/// <param name="regions1">First region.</param>
			/// <param name="regions2">Second region.</param>
			/// <returns>true if the regions are equal; otherwise, false.</returns>
			public static bool operator ==(Regions regions1, Regions regions2)
			{
				return regions1.Equals(regions2);
			}

			/// <summary>
			/// Compare specified regions.
			/// </summary>
			/// <param name="regions1">First region.</param>
			/// <param name="regions2">Second region.</param>
			/// <returns>true if the regions not equal; otherwise, false.</returns>
			public static bool operator !=(Regions regions1, Regions regions2)
			{
				return !regions1.Equals(regions2);
			}
		}

		/// <summary>
		/// Allow resize.
		/// </summary>
		[Obsolete("Replaced with Interactable.")]
		public bool AllowResize
		{
			get
			{
				return Interactable;
			}

			set
			{
				Interactable = value;
			}
		}

		#region Interactable
		[SerializeField]
		[FormerlySerializedAs("allowResize")]
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

					if (!ProcessDrag)
					{
						ResetCursor();
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Resize type.
		/// </summary>
		public ResizeType Type = ResizeType.Size;

		[SerializeField]
		[FormerlySerializedAs("ResizeDirections")]
		Directions resizeDirections = new Directions(true, true, true, true);

		/// <summary>
		/// Resize directions.
		/// </summary>
		public Directions ResizeDirections
		{
			get
			{
				return resizeDirections;
			}

			set
			{
				resizeDirections = value;
				OnResizeDirectionsChanged.Invoke(this);
			}
		}

		/// <summary>
		/// Allow resize by corners.
		/// </summary>
		[SerializeField]
		[Tooltip("Disable it if used with Rotatable component.")]
		public bool IncludesCorners = true;

		/// <summary>
		/// Resize step.
		/// </summary>
		[SerializeField]
		public bool IntegerSize = true;

		/// <summary>
		/// Is need to update RectTransform on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateRectTransform = true;

		/// <summary>
		/// Is need to update LayoutElement on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateLayoutElement = true;

		/// <summary>
		/// The active region in points from left or right border where resize allowed.
		/// </summary>
		[SerializeField]
		[Tooltip("Maximum padding from border where resize active.")]
		public float ActiveRegion = 5;

		/// <summary>
		/// Use CanvasScaler.
		/// </summary>
		[SerializeField]
		public bool UseCanvasScaler = true;

		CanvasScaler canvasScaler;

		/// <summary>
		/// Canvas scaler.
		/// </summary>
		protected CanvasScaler CanvasScaler
		{
			get
			{
				if (canvasScaler == null)
				{
					canvasScaler = GetComponentInParent<CanvasScaler>();
				}

				return canvasScaler;
			}
		}

		/// <summary>
		/// Active region scaled according to CanvasScaler.
		/// </summary>
		protected float ActiveRegionScaled
		{
			get
			{
				if (!UseCanvasScaler)
				{
					return ActiveRegion;
				}

				var scaler = CanvasScaler;
				if ((scaler == null) || (scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize))
				{
					return ActiveRegion;
				}

				var ratio = 1f;
				if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
				{
					ratio = Screen.height / scaler.referenceResolution.y;
					if (scaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
					{
						ratio = Mathf.Max(ratio, 1f);
					}
					else if (scaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
					{
						ratio = Mathf.Min(ratio, 1f);
					}
				}
				else if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPhysicalSize)
				{
					ratio = Screen.dpi / scaler.defaultSpriteDPI;
				}

				return ActiveRegion * ratio;
			}
		}

		/// <summary>
		/// The minimum size.
		/// </summary>
		[SerializeField]
		public Vector2 MinSize;

		/// <summary>
		/// The maximum size.
		/// </summary>
		[SerializeField]
		[Tooltip("Set 0 to remove maximum size limit.")]
		public Vector2 MaxSize;

		/// <summary>
		/// The keep aspect ratio.
		/// Aspect ratio applied after MinSize and MaxSize, so if RectTransform aspect ratio not equal MinSize and MaxSize aspect ratio then real size may be outside limit with one of axis.
		/// </summary>
		[SerializeField]
		public bool KeepAspectRatio;

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
		/// The cursor EW texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorEWTexture;

		/// <summary>
		/// The cursor EW hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorEWHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor NS texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorNSTexture;

		/// <summary>
		/// The cursor NS hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorNSHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor NESW texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorNESWTexture;

		/// <summary>
		/// The cursor NESW hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorNESWHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The cursor NWSE texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorNWSETexture;

		/// <summary>
		/// The cursor NWSE hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorNWSEHotSpot = new Vector2(16, 16);

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
		List<SnapGridBase> snapGrids;

		/// <summary>
		/// SnapGrids.
		/// </summary>
		public List<SnapGridBase> SnapGrids
		{
			get
			{
				return snapGrids;
			}

			set
			{
				snapGrids = value;
			}
		}

		[SerializeField]
		Vector2 snapDistance = new Vector2(10f, 10f);

		/// <summary>
		/// Snap distance.
		/// </summary>
		public Vector2 SnapDistance
		{
			get
			{
				return snapDistance;
			}

			set
			{
				snapDistance = value;
			}
		}

		/// <summary>
		/// Start resize event.
		/// </summary>
		public ResizableEvent OnStartResize = new ResizableEvent();

		/// <summary>
		/// During resize event.
		/// </summary>
		public ResizableEvent OnResize = new ResizableEvent();

		/// <summary>
		/// During resize event.
		/// </summary>
		public ResizableDeltaEvent OnResizeDelta = new ResizableDeltaEvent();

		/// <summary>
		/// End resize event.
		/// </summary>
		public ResizableEvent OnEndResize = new ResizableEvent();

		/// <summary>
		/// Resize directions event.
		/// </summary>
		public ResizableEvent OnResizeDirectionsChanged = new ResizableEvent();

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>RectTransform.</value>
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

					targetLayoutElement = null;

					RectTransform.SetParent(target.parent, false);

					CopyRectTransformValues();
				}
			}
		}

		LayoutElement targetLayoutElement;

		/// <summary>
		/// Gets the LayoutElement.
		/// </summary>
		/// <value>LayoutElement.</value>
		public LayoutElement TargetLayoutElement
		{
			get
			{
				if (targetLayoutElement == null)
				{
					targetLayoutElement = Utilities.GetOrAddComponent<LayoutElement>(Target);
				}

				return targetLayoutElement;
			}
		}

		/// <summary>
		/// Current drag regions.
		/// </summary>
		protected Regions regions;

		/// <summary>
		/// Drag regions.
		/// </summary>
		protected Regions DragRegions;

		/// <summary>
		/// Allow to handle drag event.
		/// </summary>
		protected bool ProcessDrag;

		/// <summary>
		/// The cursor changed.
		/// </summary>
		protected bool CursorChanged;

		/// <summary>
		/// Is cursor over.
		/// </summary>
		protected bool IsCursorOver;

		/// <summary>
		/// Position before resize.
		/// </summary>
		protected Vector2 OriginalPosition;

		/// <summary>
		/// Size before resize.
		/// </summary>
		protected Vector2 OriginalSize;

		/// <summary>
		/// Layout preferred size before resize.
		/// </summary>
		protected Vector2 OriginalLayoutSize;

		/// <summary>
		/// Scale before resize.
		/// </summary>
		protected Vector3 OriginalScale;

		/// <summary>
		/// Size with scale before resize.
		/// </summary>
		protected Vector2 OriginalScaledSize;

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
		public void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			var layout = GetComponent<LayoutGroup>();
			if (layout != null)
			{
				LayoutUtilities.UpdateLayout(layout);
			}

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

			if (!ProcessDrag)
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

			if (ProcessDrag)
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
			var active = ActiveRegionScaled;
			regions.Top = CheckTop(point, active);
			regions.Bottom = CheckBottom(point, active);
			regions.Left = CheckLeft(point, active);
			regions.Right = CheckRight(point, active);

			if (!IncludesCorners)
			{
				var any_top_corner = regions.Top && (regions.Left || regions.Right);
				var any_bottom_corner = regions.Bottom && (regions.Left || regions.Right);
				if (any_top_corner || any_bottom_corner)
				{
					regions.Reset();
				}
			}
		}

		/// <summary>
		/// Updates the cursor.
		/// </summary>
		protected virtual void UpdateCursor()
		{
			if ((regions.TopLeft && ResizeDirections.TopLeft) || (regions.BottomRight && ResizeDirections.BottomRight))
			{
				CursorChanged = true;
				UICursor.Set(this, GetNWSECursor());
			}
			else if ((regions.TopRight && ResizeDirections.TopRight) || (regions.BottomLeft && ResizeDirections.BottomLeft))
			{
				CursorChanged = true;
				UICursor.Set(this, GetNESWCursor());
			}
			else if ((regions.Top && ResizeDirections.Top) || (regions.Bottom && ResizeDirections.Bottom))
			{
				CursorChanged = true;
				UICursor.Set(this, GetNSCursor());
			}
			else if ((regions.Left && ResizeDirections.Left) || (regions.Right && ResizeDirections.Right))
			{
				CursorChanged = true;
				UICursor.Set(this, GetEWCursor());
			}
			else if (CursorChanged)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Get NWSE cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetNWSECursor()
		{
			if (Cursors != null)
			{
				return Cursors.NorthWestSouthEastArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.NorthWestSouthEastArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Get NESW cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetNESWCursor()
		{
			if (Cursors != null)
			{
				return Cursors.NorthEastSouthWestArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.NorthEastSouthWestArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Get NS cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetNSCursor()
		{
			if (Cursors != null)
			{
				return Cursors.NorthSouthArrow;
			}

			if (UICursor.Cursors != null)
			{
				return UICursor.Cursors.NorthSouthArrow;
			}

			return default(Cursors.Cursor);
		}

		/// <summary>
		/// Get EW cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetEWCursor()
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
		/// Checks if point in the top region.
		/// </summary>
		/// <returns><c>true</c>, if point in the top region, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		/// <param name="activeRegion">Active region.</param>
		bool CheckTop(Vector2 point, float activeRegion)
		{
			var rect = Target.rect;

			rect.position = new Vector2(rect.position.x, rect.position.y + rect.height - activeRegion);
			rect.height = activeRegion;

			return rect.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		/// <param name="activeRegion">Active region.</param>
		bool CheckBottom(Vector2 point, float activeRegion)
		{
			var rect = Target.rect;
			rect.height = activeRegion;

			return rect.Contains(point);
		}

		/// <summary>
		/// Checks if point in the left region.
		/// </summary>
		/// <returns><c>true</c>, if point in the left region, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		/// <param name="activeRegion">Active region.</param>
		bool CheckLeft(Vector2 point, float activeRegion)
		{
			var rect = Target.rect;
			rect.width = activeRegion;

			return rect.Contains(point);
		}

		/// <summary>
		/// Checks if point in the right region.
		/// </summary>
		/// <returns><c>true</c>, if right was checked, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		/// <param name="activeRegion">Active region.</param>
		bool CheckRight(Vector2 point, float activeRegion)
		{
			var rect = Target.rect;

			rect.position = new Vector2(rect.position.x + rect.width - activeRegion, rect.position.y);
			rect.width = activeRegion;

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
			ProcessDrag = false;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.pressPosition, eventData.pressEventCamera, out point))
			{
				return;
			}

			UpdateRegions(point);

			ProcessDrag = IsAllowedDrag();

			DragRegions = regions;

			UpdateCursor();

			TargetLayoutElement.preferredHeight = Target.rect.height;
			TargetLayoutElement.preferredWidth = Target.rect.width;

			if (ProcessDrag)
			{
				InitResize();

				OnStartResize.Invoke(this);

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
			return (regions.Top && ResizeDirections.Top)
				|| (regions.Bottom && ResizeDirections.Bottom)
				|| (regions.Left && ResizeDirections.Left)
				|| (regions.Right && ResizeDirections.Right)
				|| IsAllowedResizeCorner(regions);
		}

		/// <summary>
		/// Determines whether this instance is allowed resize horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is allowed resize horizontal; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAllowedResizeHorizontal()
		{
			return IsAllowedResizeHorizontal(DragRegions);
		}

		/// <summary>
		/// Determines whether this instance is allowed resize vertical.
		/// </summary>
		/// <returns><c>true</c> if this instance is allowed resize vertical; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAllowedResizeVertical()
		{
			return IsAllowedResizeVertical(DragRegions);
		}

		/// <summary>
		/// Determines whether this instance is allowed resize corner.
		/// </summary>
		/// <param name="regions">Regions.</param>
		/// <returns><c>true</c> if this instance is allowed resize corner; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAllowedResizeCorner(Regions regions)
		{
			return (regions.TopLeft && ResizeDirections.TopLeft)
				|| (regions.BottomRight && ResizeDirections.BottomRight)
				|| (regions.TopRight && ResizeDirections.TopRight)
				|| (regions.BottomLeft && ResizeDirections.BottomLeft);
		}

		/// <summary>
		/// Determines whether this instance is allowed resize horizontal.
		/// </summary>
		/// <param name="regions">Regions.</param>
		/// <returns><c>true</c> if this instance is allowed resize horizontal; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAllowedResizeHorizontal(Regions regions)
		{
			return (regions.Left && ResizeDirections.Left)
				|| (regions.Right && ResizeDirections.Right)
				|| IsAllowedResizeCorner(regions);
		}

		/// <summary>
		/// Determines whether this instance is allowed resize vertical.
		/// </summary>
		/// <param name="regions">Regions.</param>
		/// <returns><c>true</c> if this instance is allowed resize vertical; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAllowedResizeVertical(Regions regions)
		{
			return (regions.Top && ResizeDirections.Top)
				|| (regions.Bottom && ResizeDirections.Bottom)
				|| IsAllowedResizeCorner(regions);
		}

		void ResetCursor()
		{
			CursorChanged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			ResetCursor();

			if (ProcessDrag)
			{
				ProcessDrag = false;

				OnEndResize.Invoke(this);
			}
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!ProcessDrag)
			{
				return;
			}

			if (eventData.used)
			{
				return;
			}

			eventData.Use();

			Vector2 current_point;
			Vector2 original_point;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.position, eventData.pressEventCamera, out current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.pressPosition, eventData.pressEventCamera, out original_point);

			var delta = current_point - original_point;
			Resize(DragRegions, delta);

			OnResize.Invoke(this);
			OnResizeDelta.Invoke(this, DragRegions, delta);
		}

		/// <summary>
		/// Init resize.
		/// </summary>
		public virtual void InitResize()
		{
			OriginalPosition = Target.anchoredPosition;
			OriginalSize = Target.rect.size;
			OriginalScale = Target.localScale;
			OriginalScaledSize = new Vector2(OriginalSize.x * OriginalScale.x, OriginalSize.y * OriginalScale.y);
			OriginalLayoutSize = new Vector2(TargetLayoutElement.preferredWidth, TargetLayoutElement.preferredHeight);
		}

		/// <summary>
		/// Get snap points.
		/// </summary>
		/// <param name="regions">Regions.</param>
		/// <param name="result">Result points.</param>
		/// <param name="snapGrid">SnapGrid.</param>
		protected virtual void GetSnapPoints(Regions regions, List<SnapGridBase.Point> result, SnapGridBase snapGrid)
		{
			if (regions.Left || regions.Top)
			{
				var point = new SnapGridBase.Point(RectTransform, snapGrid, true, true);
				if (!regions.Left)
				{
					point.Left = false;
				}

				if (!regions.Top)
				{
					point.Top = false;
				}

				result.Add(point);
			}

			if (regions.Left || regions.Bottom)
			{
				var point = new SnapGridBase.Point(RectTransform, snapGrid, true, false);
				if (!regions.Left)
				{
					point.Left = false;
				}

				if (!regions.Bottom)
				{
					point.Bottom = false;
				}

				result.Add(point);
			}

			if (regions.Right || regions.Top)
			{
				var point = new SnapGridBase.Point(RectTransform, snapGrid, false, true);
				if (!regions.Right)
				{
					point.Right = false;
				}

				if (!regions.Top)
				{
					point.Top = false;
				}

				result.Add(point);
			}

			if (regions.Right || regions.Bottom)
			{
				var point = new SnapGridBase.Point(RectTransform, snapGrid, false, false);
				if (!regions.Right)
				{
					point.Right = false;
				}

				if (!regions.Bottom)
				{
					point.Bottom = false;
				}

				result.Add(point);
			}
		}

		/// <summary>
		/// Temporary points.
		/// </summary>
		protected List<SnapGridBase.Point> TempPoints = new List<SnapGridBase.Point>();

		/// <summary>
		/// Calculate distance to SnapGrid.
		/// </summary>
		/// <param name="regions">Regions.</param>
		/// <returns>Distance.</returns>
		protected virtual SnapGridBase.Result CalculateDistance(Regions regions)
		{
			if (SnapGrids == null)
			{
				return default(SnapGridBase.Result);
			}

			if (SnapGrids.Count == 0)
			{
				return default(SnapGridBase.Result);
			}

			var distance = new SnapGridBase.Distance(float.MaxValue);

			foreach (var snap_grid in SnapGrids)
			{
				if (snap_grid == null)
				{
					continue;
				}

				GetSnapPoints(regions, TempPoints, snap_grid);

				snap_grid.Snap(TempPoints, ref distance);
				TempPoints.Clear();
			}

			return distance.Snap(snapDistance);
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		/// <param name="regions">Regions specified how position should change with resize.</param>
		/// <param name="delta">Size delta.</param>
		public virtual void Resize(Regions regions, Vector2 delta)
		{
			if (Type == ResizeType.Scale)
			{
				PerformUpdateScale(regions, delta);

				var snap = CalculateDistance(regions);

				if (snap.Delta != Vector2.zero)
				{
					delta += snap.Delta;
					PerformUpdateScale(regions, delta);
				}
			}
			else
			{
				if (UpdateRectTransform)
				{
					PerformUpdateRectTransform(regions, delta);

					var snap = CalculateDistance(regions);

					if (snap.Delta != Vector2.zero)
					{
						delta += snap.Delta;
						PerformUpdateRectTransform(regions, delta);
					}
				}

				if (UpdateLayoutElement)
				{
					PerformUpdateLayoutElement(regions, delta);
				}
			}

			CopyRectTransformValues();
		}

		void PerformUpdateScale(Regions regions, Vector2 delta)
		{
			var size = CalculateSize(regions, OriginalScaledSize, delta);
			var scale = new Vector2(size.x / OriginalSize.x, size.y / OriginalSize.y);

			UpdateAnchoredPosition(regions, OriginalScaledSize, size);

			Target.localScale = scale;
		}

		void UpdateAnchoredPosition(Regions regions, Vector2 oldSize, Vector2 newSize)
		{
			var original_pivot = Target.pivot;

			var sign = new Vector2(
				regions.Right ? original_pivot.x : original_pivot.x - 1f,
				regions.Bottom ? 1f - original_pivot.y : -original_pivot.y);
			var base_delta = newSize - oldSize;
			base_delta = new Vector2(base_delta.x * sign.x, base_delta.y * sign.y);

			var angle_rad = Target.localRotation.eulerAngles.z * Mathf.Deg2Rad;
			var delta = new Vector2(
				(base_delta.x * Mathf.Cos(angle_rad)) + (base_delta.y * Mathf.Sin(angle_rad)),
				(base_delta.x * Mathf.Sin(angle_rad)) - (base_delta.y * Mathf.Cos(angle_rad)));

			Target.anchoredPosition = OriginalPosition + delta;
		}

		Vector2 CalculateSize(Regions regions, Vector2 originalSize, Vector2 delta)
		{
			var size = originalSize;

			if (IsAllowedResizeHorizontal(regions))
			{
				var sign = regions.Right ? +1 : -1;
				size.x = Mathf.Max(MinSize.x, size.x + (sign * delta.x));
				if (MaxSize.x > 0f)
				{
					size.x = Mathf.Min(MaxSize.x, size.x);
				}
			}

			if (IsAllowedResizeVertical(regions))
			{
				var sign = regions.Top ? +1 : -1;
				size.y = Mathf.Max(MinSize.y, size.y + (sign * delta.y));
				if (MaxSize.y > 0f)
				{
					size.y = Mathf.Min(MaxSize.y, size.y);
				}
			}

			if (KeepAspectRatio)
			{
				size = ApplyAspectRatio(size, originalSize);
			}

			if (IntegerSize)
			{
				size = new Vector2(Mathf.Round(size.x), Mathf.Round(size.y));
			}

			return size;
		}

		void PerformUpdateRectTransform(Regions regions, Vector2 delta)
		{
			var size = CalculateSize(regions, OriginalSize, delta);

			UpdateAnchoredPosition(regions, OriginalSize, size);

			Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		}

		/// <summary>
		/// Fix the aspect ratio.
		/// </summary>
		/// <returns>The aspect ratio.</returns>
		/// <param name="newSize">New size.</param>
		/// <param name="originalSize">Base size.</param>
		protected virtual Vector2 ApplyAspectRatio(Vector2 newSize, Vector2 originalSize)
		{
			var size = newSize;

			var aspect_ratio = originalSize.x / originalSize.y;
			var delta = new Vector2(Mathf.Abs(newSize.x - originalSize.x), Mathf.Abs(newSize.y - originalSize.y));
			if (delta.x >= delta.y)
			{
				size.y = size.x / aspect_ratio;
			}
			else
			{
				size.x = size.y * aspect_ratio;
			}

			return size;
		}

		void PerformUpdateLayoutElement(Regions regions, Vector2 delta)
		{
			var size = CalculateSize(regions, OriginalLayoutSize, delta);

			TargetLayoutElement.preferredWidth = size.x;
			TargetLayoutElement.preferredHeight = size.y;
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

		#endif
	}
}