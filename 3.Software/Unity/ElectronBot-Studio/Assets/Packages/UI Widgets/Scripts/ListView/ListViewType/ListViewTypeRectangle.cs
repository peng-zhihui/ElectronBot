namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewCustomBase, IStylable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListView renderer with items of fixed size.
		/// </summary>
		protected abstract class ListViewTypeRectangle : ListViewTypeBase
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeRectangle"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			protected ListViewTypeRectangle(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
			}

			bool isEnabled;

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
					return GetScrollRectSize() < ListSize();
				}
			}

			/// <inheritdoc/>
			public override void ToggleScrollToItemCenter(bool enable)
			{
				if (Owner.ScrollRect == null)
				{
					return;
				}

				if (enable)
				{
					DefaultInertia = Owner.ScrollRect.inertia;
					Owner.ScrollRect.inertia = false;
				}
				else
				{
					Owner.ScrollRect.inertia = DefaultInertia;
				}
			}

			/// <inheritdoc/>
			public override bool IsRequiredCenterTheItems()
			{
				if (!Owner.CenterTheItems)
				{
					return false;
				}

				return GetScrollRectSize() > ListSize();
			}

			/// <inheritdoc/>
			public override float ValidateScrollPosition(float position)
			{
				if (Owner.LoopedListAvailable)
				{
					return ValidatePosition(position);
				}

				return Mathf.Clamp(position, 0, ListSize() - GetScrollRectSize());
			}

			/// <summary>
			/// Gets the size of the scroll.
			/// </summary>
			/// <returns>The scroll size.</returns>
			protected float GetScrollRectSize()
			{
				var scale = Owner.Container.localScale;
				return Owner.IsHorizontal() ? Owner.ScrollRectSize.x / scale.x : Owner.ScrollRectSize.y / scale.y;
			}

			/// <inheritdoc/>
			public override float CenteredFillerSize()
			{
				return (GetScrollRectSize() - ListSize() - Owner.LayoutBridge.GetFullMargin()) / 2f;
			}

			/// <inheritdoc/>
			public override void Enable()
			{
				if (isEnabled)
				{
					return;
				}

				if (Owner.ScrollRect != null)
				{
					DefaultInertia = Owner.ScrollRect.inertia;
					Owner.ScrollRect.onValueChanged.AddListener(OnScroll);
					isEnabled = true;
				}
			}

			/// <inheritdoc/>
			public override void Disable()
			{
				if (!isEnabled)
				{
					return;
				}

				if (Owner.ScrollRect != null)
				{
					Owner.ScrollRect.onValueChanged.RemoveListener(OnScroll);
					isEnabled = false;
				}
			}

			/// <inheritdoc/>
			public override void ResetPosition()
			{
				Owner.ContainerAnchoredPosition = Vector2.zero;

				if (Owner.scrollRect != null)
				{
					Owner.scrollRect.horizontal = Owner.IsHorizontal();
					Owner.scrollRect.vertical = !Owner.IsHorizontal();
					Owner.scrollRect.StopMovement();
				}
			}

			/// <summary>
			/// Process scroll event.
			/// </summary>
			/// <param name="unused">Scroll value.</param>
			protected void OnScroll(Vector2 unused)
			{
				UpdateView();
			}

			/// <inheritdoc/>
			protected override void ValidatePosition()
			{
				var current_position = GetPosition();
				var position = ValidatePosition(current_position);

				if (!Mathf.Approximately(current_position, position))
				{
					SetPosition(position, false);
				}
			}

			/// <inheritdoc/>
			public override float ValidatePosition(float position)
			{
				if (!Owner.LoopedListAvailable)
				{
					return position;
				}

				var list_size = ListSize() + Owner.LayoutBridge.GetFullMargin();
				if (Owner.IsHorizontal())
				{
					if (position < -list_size)
					{
						position += list_size;
					}
					else if (position > 0f)
					{
						position -= list_size;
					}
				}
				else
				{
					if (position > list_size)
					{
						position -= list_size;
					}
					else if (position < 0f)
					{
						position += list_size;
					}
				}

				return position;
			}

			/// <inheritdoc/>
			public override void SetPosition(float value, bool updateView = true)
			{
				if ((Owner.ScrollRect == null) || Owner.ScrollRect.content == null)
				{
					return;
				}

				var current_position = Owner.ContainerAnchoredPosition;
				var new_position = Owner.IsHorizontal()
					? new Vector2(-value, current_position.y)
					: new Vector2(current_position.x, value);

				SetPosition(new_position, updateView);
			}

			/// <inheritdoc/>
			public override void SetPosition(Vector2 newPosition, bool updateView = true)
			{
				newPosition = ValidatePosition(newPosition);

				var current_position = Owner.ContainerAnchoredPosition;
				var diff = (Owner.IsHorizontal() && !Mathf.Approximately(current_position.x, newPosition.x))
						|| (!Owner.IsHorizontal() && !Mathf.Approximately(current_position.y, newPosition.y));

				if (diff)
				{
					Owner.ContainerAnchoredPosition = newPosition;

					if (updateView)
					{
						UpdateView();
					}
				}

				if (Owner.ScrollRect != null)
				{
					Owner.ScrollRect.StopMovement();
				}
			}

			/// <inheritdoc/>
			public override Vector2 GetPositionVector()
			{
				var result = Owner.ContainerAnchoredPosition;
				if (Owner.IsHorizontal())
				{
					result.x = -result.x;
				}

				if (Owner.LoopedListAvailable)
				{
					return result;
				}

				if (float.IsNaN(result.x))
				{
					result.x = 0f;
				}

				if (float.IsNaN(result.y))
				{
					result.y = 0f;
				}

				return result;
			}

			/// <inheritdoc/>
			public override float GetPosition()
			{
				var pos = GetPositionVector();
				return Owner.IsHorizontal() ? pos.x : pos.y;
			}

			/// <inheritdoc/>
			public override float GetCenterPosition()
			{
				return GetPosition() + (GetScrollRectSize() / 2f) - Owner.LayoutBridge.GetMargin();
			}

			/// <inheritdoc/>
			public override Vector2 GetPosition(int index)
			{
				var scroll_main = GetPosition();

				var item_starts = GetItemPosition(index);
				var item_ends = GetItemPositionBottom(index);

				if (item_starts < scroll_main)
				{
					scroll_main = item_starts;
				}
				else if (item_ends > scroll_main)
				{
					scroll_main = item_ends;
				}

				var scroll_secondary = Owner.GetScrollPositionSecondary(index);

				var position = Owner.IsHorizontal()
					? new Vector2(ValidatePosition(-scroll_main), scroll_secondary)
					: new Vector2(scroll_secondary, ValidatePosition(scroll_main));

				return position;
			}

			/// <inheritdoc/>
			public override int ScrollPosition2Index(float position)
			{
				var v = Owner.IsHorizontal() ? new Vector2(position, 0f) : new Vector2(0f, position);
				return GetNearestIndex(v, NearestType.Auto);
			}

			/// <inheritdoc/>
			public override bool IsVisible(int index, float minVisiblePart)
			{
				if (!Owner.IsValid(index))
				{
					return false;
				}

				var viewport_top = GetPosition() + Owner.LayoutBridge.GetMargin();
				var viewport_bottom = viewport_top + GetScrollRectSize() - Owner.LayoutBridge.GetMargin();

				var border_top = GetItemPosition(index);
				var border_bottom = GetItemPositionBorderEnd(index);
				var size = border_bottom - border_top;

				var border_top_visible = (viewport_top <= border_top) && (border_top < viewport_bottom);
				if (border_top_visible)
				{
					var visible_top = Mathf.Max(viewport_top, border_top);
					var visible_bottom = Mathf.Min(viewport_bottom, border_top + size);
					var visible_part = (visible_bottom - visible_top) / size;

					return visible_part >= minVisiblePart;
				}

				var border_bottom_visible = (viewport_top <= border_bottom) && (border_bottom < viewport_bottom);
				if (border_bottom_visible)
				{
					var visible_top = Mathf.Max(viewport_top, border_bottom - size);
					var visible_bottom = Mathf.Min(viewport_bottom, viewport_bottom);
					var visible_part = (visible_bottom - visible_top) / size;

					return visible_part >= minVisiblePart;
				}

				return false;
			}

			/// <inheritdoc/>
			public override void UpdateLayout()
			{
				if (!Owner.Virtualization)
				{
					Owner.LayoutBridge.SetFiller(0f, 0f);

					// Owner.LayoutBridge.UpdateLayout();
					return;
				}

				if (IsRequiredCenterTheItems())
				{
					var filler = CenteredFillerSize();
					Owner.LayoutBridge.SetFiller(filler, 0f);
				}
				else
				{
					var top = TopFillerSize();
					var bottom = BottomFillerSize();
					Owner.LayoutBridge.SetFiller(top, bottom);
				}

				// Owner.LayoutBridge.UpdateLayout();
			}
		}
	}
}