namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewCustomBase, IStylable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListView ellipse renderer with items of fixed size.
		/// </summary>
		protected class ListViewTypeEllipse : ListViewTypeBase
		{
			bool isEnabled;

			ScrollListener scrollListener;

			EasyLayoutNS.EasyLayoutEllipseScroll ellipseScroll;

			/// <summary>
			/// Ellipse scroll listener.
			/// </summary>
			protected EasyLayoutNS.EasyLayoutEllipseScroll EllipseScroll
			{
				get
				{
					if (ellipseScroll == null)
					{
						ellipseScroll = Utilities.GetOrAddComponent<EasyLayoutNS.EasyLayoutEllipseScroll>(Owner.Container);
						DefaultInertia = ellipseScroll.Inertia;
					}

					return ellipseScroll;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeEllipse"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public ListViewTypeEllipse(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
				if (Owner.Layout == null)
				{
					Debug.LogWarning("TileViewStaggered requires Container.EasyLayout component.", Owner);
					return;
				}

				if (Owner.Layout.LayoutType != EasyLayoutNS.LayoutTypes.Ellipse)
				{
					// Owner.Layout.LayoutType = EasyLayoutNS.LayoutTypes.Ellipse;
					Debug.LogWarning("EasyLayout type should be set to Ellipse when used ListViewEllipse.", Owner);
				}

				if (Owner.Layout.EllipseSettings.Fill != EasyLayoutNS.EllipseFill.Arc)
				{
					// Owner.Layout.EllipseSettings.Fill = EasyLayoutNS.EllipseFill.Arc;
					Debug.LogWarning("EasyLayout.EllipseSettings.Fill should be set to Arc when used ListViewEllipse.", Owner);
				}

				if (Owner.Layout.EllipseSettings.AngleStepAuto)
				{
					// Owner.Layout.EllipseSettings.AngleStepAuto = false;
					Debug.LogWarning("EasyLayout.EllipseSettings.AngleStepAuto should be disabled when used ListViewEllipse.", Owner);
				}
			}

			/// <inheritdoc/>
			public override bool AllowSetContentSizeFitter
			{
				get
				{
					return false;
				}
			}

			/// <inheritdoc/>
			public override bool AllowControlRectTransform
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Gets the size of the item.
			/// </summary>
			/// <returns>The item size.</returns>
			protected float GetItemSize()
			{
				return Owner.IsHorizontal()
					? Owner.ItemSize.x + Owner.LayoutBridge.GetSpacing()
					: Owner.ItemSize.y + Owner.LayoutBridge.GetSpacing();
			}

			/// <inheritdoc/>
			public override float TopFillerSize()
			{
				var settings = Owner.Layout.EllipseSettings;
				var steps = (settings.AngleScroll < 0f)
					? Mathf.CeilToInt(-settings.AngleScroll / settings.AngleStep)
					: -Mathf.FloorToInt(settings.AngleScroll / settings.AngleStep);

				return steps * settings.AngleStep;
			}

			/// <inheritdoc/>
			public override float BottomFillerSize()
			{
				return 0f;
			}

			/// <inheritdoc/>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var length = Owner.Layout.EllipseSettings.AngleScroll;
				var step = Owner.Layout.EllipseSettings.AngleStep;

				int first_visible_index;
				if (length < 0)
				{
					first_visible_index = strict
						? -Mathf.FloorToInt(-length / step)
						: -Mathf.CeilToInt(-length / step);
				}
				else
				{
					first_visible_index = strict
						? Mathf.CeilToInt(length / step)
						: Mathf.FloorToInt(length / step);
				}

				if (Owner.LoopedListAvailable)
				{
					return first_visible_index;
				}

				first_visible_index = Mathf.Max(0, first_visible_index);
				if (strict)
				{
					return first_visible_index;
				}

				first_visible_index = Mathf.Min(first_visible_index, Mathf.Max(0, Owner.DataSource.Count - MinVisibleItems));
				return first_visible_index;
			}

			/// <inheritdoc/>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				var length = Owner.Layout.EllipseSettings.AngleScroll + Owner.Layout.EllipseSettings.ArcLength;
				var step = Owner.Layout.EllipseSettings.AngleStep;

				var last_visible_index = strict
					? Mathf.FloorToInt(length / step)
					: Mathf.CeilToInt(length / step);

				return last_visible_index - 1;
			}

			/// <inheritdoc/>
			public override float GetItemPosition(int index)
			{
				var block_index = GetBlockIndex(index);
				return block_index * Owner.Layout.EllipseSettings.AngleStep;
			}

			/// <inheritdoc/>
			public override float GetItemPositionBorderEnd(int index)
			{
				return GetItemPosition(index + 1);
			}

			/// <inheritdoc/>
			public override float GetItemPositionMiddle(int index)
			{
				return GetItemPosition(index) - (Owner.Layout.EllipseSettings.ArcLength / 2f);
			}

			/// <inheritdoc/>
			public override float GetItemPositionBottom(int index)
			{
				return GetItemPosition(index) - Owner.Layout.EllipseSettings.ArcLength;
			}

			/// <inheritdoc/>
			public override void CalculateMaxVisibleItems()
			{
				if (!Owner.Virtualization)
				{
					MaxVisibleItems = Owner.DataSource.Count;
					return;
				}

				var length = Owner.Layout.EllipseSettings.ArcLength;
				var step = Owner.Layout.EllipseSettings.AngleStep;

				var max = Mathf.CeilToInt(length / step);

				MaxVisibleItems = Mathf.Max(max + 2, MinVisibleItems);
			}

			/// <inheritdoc/>
			public override int GetNearestIndex(Vector2 point, NearestType type)
			{
				if (Owner.IsSortEnabled())
				{
					return 0;
				}

				if (Owner.Components.Count == 0)
				{
					return 0;
				}

				if (Owner.Components.Count == 1)
				{
					switch (type)
					{
						case NearestType.Auto:
							return 0;
						case NearestType.Before:
							return 0;
						case NearestType.After:
							return 1;
						default:
							throw new NotSupportedException(string.Format("Unknown position: {0}", EnumHelper<NearestType>.ToString(type)));
					}
				}

				int index;

				var nearest = 0;
				var minimal_distance = GetDistance(nearest, point);
				var base_distance = BaseDistance();
				switch (type)
				{
					case NearestType.Auto:
						for (int i = 1; i < Owner.Components.Count; i++)
						{
							var new_distance = GetDistance(i, point);
							if (new_distance < minimal_distance)
							{
								minimal_distance = new_distance;
								nearest = i;
							}
						}

						index = Owner.Components[nearest].Index;
						break;
					case NearestType.Before:
						for (int i = 1; i < Owner.Components.Count; i++)
						{
							var new_distance = GetDistance(i, point);
							if ((new_distance < minimal_distance) && (new_distance < (base_distance / 2f)))
							{
								minimal_distance = new_distance;
								nearest = i;
							}
						}

						index = Owner.Components[nearest].Index;
						break;
					case NearestType.After:
						for (int i = 1; i < Owner.Components.Count; i++)
						{
							var new_distance = GetDistance(i, point);
							if ((new_distance < minimal_distance) && (new_distance < (base_distance / 2f)))
							{
								minimal_distance = new_distance;
								nearest = i;
							}
						}

						index = Owner.Components[nearest].Index + 1;
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported NearestType: {0}", EnumHelper<NearestType>.ToString(type)));
				}

				return index;
			}

			float BaseDistance()
			{
				return Vector2.Distance(
					(Owner.Components[0].transform as RectTransform).localPosition,
					(Owner.Components[1].transform as RectTransform).localPosition);
			}

			float GetDistance(int index, Vector2 point)
			{
				var pos = (Owner.Components[index].transform as RectTransform).localPosition;
				return Vector2.Distance(pos, point);
			}

			/// <inheritdoc/>
			public override int GetNearestItemIndex()
			{
				if (Owner.DataSource.Count == 0)
				{
					return -1;
				}

				return VisibleIndex2ItemIndex(Visible.FirstVisible);
			}

			/// <inheritdoc/>
			public override float ListSize()
			{
				if (Owner.DataSource.Count == 0)
				{
					return 0;
				}

				return (Owner.DataSource.Count - 1) * Owner.Layout.EllipseSettings.AngleStep;
			}

			/// <inheritdoc/>
			public override void ValidateContentSize()
			{
			}

			/// <inheritdoc/>
			public override bool AllowLoopedList
			{
				get
				{
					return !IsTileView && CanScroll;
				}
			}

			/// <inheritdoc/>
			public override bool CanScroll
			{
				get
				{
					return Owner.Layout.EllipseSettings.ArcLength < ListSize();
				}
			}

			/// <inheritdoc/>
			public override void Enable()
			{
				if (isEnabled)
				{
					return;
				}

				EllipseScroll.OnScrollEvent.AddListener(Scroll);
				DefaultInertia = EllipseScroll.Inertia;

				isEnabled = true;
			}

			/// <inheritdoc/>
			public override void Disable()
			{
				if (!isEnabled)
				{
					return;
				}

				EllipseScroll.OnScrollEvent.RemoveListener(Scroll);
				isEnabled = false;
			}

			/// <inheritdoc/>
			public override void ToggleScrollToItemCenter(bool state)
			{
				if (state)
				{
					DefaultInertia = EllipseScroll.Inertia;
					EllipseScroll.Inertia = false;
				}
				else
				{
					EllipseScroll.Inertia = DefaultInertia;
				}
			}

			/// <summary>
			/// Process the scroll event.
			/// </summary>
			protected void Scroll()
			{
				Owner.StopScrollCoroutine();
				Owner.StartScrolling();

				UpdateView();
			}

			PointerEventData pointerEventData;

			/// <inheritdoc/>
			public override void UpdateView()
			{
				base.UpdateView();

				if (pointerEventData == null)
				{
					pointerEventData = new PointerEventData(EventSystem.current);
				}

				scrollListener.ScrollEvent.Invoke(pointerEventData);
			}

			/// <inheritdoc/>
			public override void ResetPosition()
			{
				Owner.ContainerAnchoredPosition = Vector2.zero;

				if (Owner.scrollRect != null)
				{
					Owner.scrollRect.horizontal = false;
					Owner.scrollRect.vertical = false;
					Owner.scrollRect.StopMovement();
					scrollListener = Utilities.GetOrAddComponent<ScrollListener>(Owner.ScrollRect);
				}

				if (Owner.Layout != null)
				{
					Owner.Layout.EllipseSettings.AngleScroll = -Owner.Layout.EllipseSettings.ArcLength / 2f;
				}
			}

			/// <inheritdoc/>
			protected override void ValidatePosition()
			{
				var position = ClampPosition(GetPosition(), Owner.LoopedListAvailable);
				SetPosition(position);
			}

			float ClampPosition(float position, bool looped)
			{
				var size = ListSize();
				if (looped)
				{
					size += Owner.Layout.EllipseSettings.AngleStep;
				}

				if (position < 0)
				{
					position += Mathf.CeilToInt(-position / size) * size;
				}

				if (position > size)
				{
					position -= Mathf.CeilToInt(position / size) * size;
				}

				return position;
			}

			/// <inheritdoc/>
			public override float ValidateScrollPosition(float position)
			{
				return ValidatePosition(position);
			}

			/// <inheritdoc/>
			public override float ValidatePosition(float position)
			{
				if (IsRequiredCenterTheItems())
				{
					position = 0f;
				}
				else if (Owner.LoopedListAvailable)
				{
					ValidatePosition();

					var size = ListSize() + Owner.Layout.EllipseSettings.AngleStep;
					var half = size / 2f;
					var current = GetPosition();
					var new_position = ClampPosition(position, true);

					if (Mathf.Abs(new_position - current) > half)
					{
						if (new_position > current)
						{
							new_position -= size;
						}
						else
						{
							new_position += size;
						}
					}

					if (new_position != position)
					{
						position = new_position;
					}
				}
				else
				{
					var half_arc = Owner.Layout.EllipseSettings.ArcLength / 2f;
					position += half_arc;

					position = ClampPosition(position, false);

					position -= half_arc;
				}

				return position;
			}

			/// <summary>
			/// Set position directly without update view.
			/// </summary>
			/// <param name="value">Position.</param>
			protected void SetPosition(float value)
			{
				Owner.Layout.EllipseSettings.AngleScroll = value;
			}

			/// <inheritdoc/>
			public override void SetPosition(float value, bool updateView = true)
			{
				value = ValidatePosition(value);
				if (Owner.Layout.EllipseSettings.AngleScroll != value)
				{
					Owner.Layout.EllipseSettings.AngleScroll = value;

					if (updateView)
					{
						UpdateView();
					}
				}
			}

			/// <inheritdoc/>
			public override void SetPosition(Vector2 newPosition, bool updateView = true)
			{
				var value = Owner.IsHorizontal() ? newPosition.x : newPosition.y;
				SetPosition(value, updateView);
			}

			/// <inheritdoc/>
			public override Vector2 GetPositionVector()
			{
				return Owner.IsHorizontal()
					? new Vector2(Owner.Layout.EllipseSettings.AngleScroll, 0f)
					: new Vector2(0f, Owner.Layout.EllipseSettings.AngleScroll);
			}

			/// <inheritdoc/>
			public override float GetPosition()
			{
				return Owner.Layout.EllipseSettings.AngleScroll;
			}

			/// <inheritdoc/>
			public override float GetCenterPosition()
			{
				return GetPosition() + (Owner.Layout.EllipseSettings.ArcLength / 2f);
			}

			/// <inheritdoc/>
			public override Vector2 GetPosition(int index)
			{
				var pos = ValidatePosition(GetItemPosition(index));

				return Owner.IsHorizontal()
					? new Vector2(pos, 0f)
					: new Vector2(0f, pos);
			}

			/// <inheritdoc/>
			public override int ScrollPosition2Index(float position)
			{
				var index = Mathf.RoundToInt(position / Owner.Layout.EllipseSettings.AngleStep);

				return index;
			}

			/// <inheritdoc/>
			public override bool IsVisible(int index, float minVisiblePart)
			{
				return Owner.DisplayedIndices.Contains(index);
			}

			/// <inheritdoc/>
			public override float CenteredFillerSize()
			{
				return -(Owner.Layout.EllipseSettings.ArcLength - ListSize()) / 2f;
			}

			/// <inheritdoc/>
			public override bool IsRequiredCenterTheItems()
			{
				if (!Owner.CenterTheItems)
				{
					return false;
				}

				return Owner.Layout.EllipseSettings.ArcLength > ListSize();
			}

			/// <inheritdoc/>
			public override void UpdateLayout()
			{
				if (Owner.LayoutBridge == null)
				{
					return;
				}

				var settings = Owner.Layout.EllipseSettings;
				if (IsRequiredCenterTheItems())
				{
					settings.AngleFiller = CenteredFillerSize();
				}
				else if (Owner.LoopedListAvailable)
				{
					settings.AngleFiller = TopFillerSize();
				}
				else
				{
					settings.AngleFiller = settings.AngleScroll > 0f ? TopFillerSize() : 0f;
				}
			}
		}
	}
}