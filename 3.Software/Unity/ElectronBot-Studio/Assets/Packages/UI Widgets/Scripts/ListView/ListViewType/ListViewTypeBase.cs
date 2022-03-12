namespace UIWidgets
{
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewCustomBase, IStylable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// Base class for the ListView renderer.
		/// </summary>
		protected abstract class ListViewTypeBase
		{
			/// <summary>
			/// Visibility data.
			/// </summary>
			protected struct Visibility : System.IEquatable<Visibility>
			{
				/// <summary>
				/// First visible index.
				/// </summary>
				public int FirstVisible;

				/// <summary>
				/// Count of the visible items.
				/// </summary>
				public int Items;

				/// <summary>
				/// Last visible index.
				/// </summary>
				public int LastVisible
				{
					get
					{
						return FirstVisible + Items;
					}
				}

				/// <summary>
				/// Determines whether the specified object is equal to the current object.
				/// </summary>
				/// <param name="obj">The object to compare with the current object.</param>
				/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
				public override bool Equals(object obj)
				{
					if (obj is Visibility)
					{
						return Equals((Visibility)obj);
					}

					return false;
				}

				/// <summary>
				/// Determines whether the specified object is equal to the current object.
				/// </summary>
				/// <param name="other">The object to compare with the current object.</param>
				/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
				public bool Equals(Visibility other)
				{
					return FirstVisible == other.FirstVisible && Items == other.Items;
				}

				/// <summary>
				/// Hash function.
				/// </summary>
				/// <returns>A hash code for the current object.</returns>
				public override int GetHashCode()
				{
					return FirstVisible ^ Items;
				}

				/// <summary>
				/// Compare specified visibility data.
				/// </summary>
				/// <param name="obj1">First data.</param>
				/// <param name="obj2">Second data.</param>
				/// <returns>true if the data are equal; otherwise, false.</returns>
				public static bool operator ==(Visibility obj1, Visibility obj2)
				{
					return obj1.Equals(obj2);
				}

				/// <summary>
				/// Compare specified visibility data.
				/// </summary>
				/// <param name="obj1">First data.</param>
				/// <param name="obj2">Second data.</param>
				/// <returns>true if the data not equal; otherwise, false.</returns>
				public static bool operator !=(Visibility obj1, Visibility obj2)
				{
					return !obj1.Equals(obj2);
				}
			}

			/// <summary>
			/// Minimal count of the visible items.
			/// </summary>
			protected const int MinVisibleItems = 2;

			/// <summary>
			/// Maximal count of the visible items.
			/// </summary>
			public int MaxVisibleItems
			{
				get;
				protected set;
			}

			/// <summary>
			/// Visibility info.
			/// </summary>
			protected Visibility Visible;

			/// <summary>
			/// Owner.
			/// </summary>
			protected ListViewCustom<TComponent, TItem> Owner;

			/// <summary>
			/// Default inertia state.
			/// </summary>
			protected bool DefaultInertia;

			/// <summary>
			/// Is looped list allowed?
			/// </summary>
			/// <returns>True if looped list allowed; otherwise false.</returns>
			public virtual bool IsTileView
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Allow owner to set ContentSizeFitter settings.
			/// </summary>
			public virtual bool AllowSetContentSizeFitter
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Allow owner to control Container.RectTransform.
			/// </summary>
			public virtual bool AllowControlRectTransform
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Allow looped ListView.
			/// </summary>
			public abstract bool AllowLoopedList
			{
				get;
			}

			/// <summary>
			/// Can scroll?
			/// </summary>
			public abstract bool CanScroll
			{
				get;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeBase"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			protected ListViewTypeBase(ListViewCustom<TComponent, TItem> owner)
			{
				Owner = owner;
			}

			/// <summary>
			/// Enable this instance.
			/// </summary>
			public abstract void Enable();

			/// <summary>
			/// Disable this instance.
			/// </summary>
			public abstract void Disable();

			/// <summary>
			/// Reset position.
			/// </summary>
			public abstract void ResetPosition();

			/// <summary>
			/// Validate position.
			/// </summary>
			protected abstract void ValidatePosition();

			/// <summary>
			/// Validate position.
			/// </summary>
			/// <param name="position">Position.</param>
			/// <returns>Validated position.</returns>
			public abstract float ValidatePosition(float position);

			/// <summary>
			/// Toggle scroll to nearest item center.
			/// </summary>
			/// <param name="state">State.</param>
			public abstract void ToggleScrollToItemCenter(bool state);

			/// <summary>
			/// Scroll to the nearest item center.
			/// </summary>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required")]
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required")]
			public void ScrollToItemCenter()
			{
				var center_position = GetCenterPosition();
				var index_at_center = ScrollPosition2Index(center_position);
				var index = VisibleIndex2ItemIndex(index_at_center);
				var middle_position = GetItemPositionMiddle(index);
				var valid_position = ValidateScrollPosition(middle_position);

				if (!Mathf.Approximately(GetPosition(), valid_position))
				{
					Owner.ScrollToPositionAnimated(valid_position, Owner.ScrollInertia, Owner.ScrollUnscaledTime, () => Owner.ScrollCenter = ScrollCenterState.Disable);
				}
			}

			/// <summary>
			/// Validate scroll position.
			/// </summary>
			/// <param name="position">Position.</param>
			/// <returns>Validated position.</returns>
			public abstract float ValidateScrollPosition(float position);

			/// <summary>
			/// Validate position.
			/// </summary>
			/// <param name="position">Position.</param>
			/// <returns>Validated position.</returns>
			public Vector2 ValidatePosition(Vector2 position)
			{
				if (Owner.IsHorizontal())
				{
					position.x = ValidatePosition(position.x);
				}
				else
				{
					position.y = ValidatePosition(position.y);
				}

				return position;
			}

			/// <summary>
			/// Sets the scroll value.
			/// </summary>
			/// <param name="value">Value.</param>
			/// <param name="updateView">Call ScrollUpdate() if position changed.</param>
			public abstract void SetPosition(float value, bool updateView = true);

			/// <summary>
			/// Sets the scroll value.
			/// </summary>
			/// <param name="newPosition">Value.</param>
			/// <param name="updateView">Update view if position changed.</param>
			public abstract void SetPosition(Vector2 newPosition, bool updateView = true);

			/// <summary>
			/// Gets the scroll value in ListView direction.
			/// </summary>
			/// <returns>The scroll value.</returns>
			public abstract Vector2 GetPositionVector();

			/// <summary>
			/// Gets the scroll value in ListView direction.
			/// </summary>
			/// <returns>The scroll value.</returns>
			public abstract float GetPosition();

			/// <summary>
			/// Gets the center position in ListView direction.
			/// </summary>
			/// <returns>Center position.</returns>
			public abstract float GetCenterPosition();

			/// <summary>
			/// Get scroll position for the specified index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>Scroll position</returns>
			public abstract Vector2 GetPosition(int index);

			/// <summary>
			/// Get item index at scroll position.
			/// </summary>
			/// <param name="position">Scroll position.</param>
			/// <returns>Index.</returns>
			public abstract int ScrollPosition2Index(float position);

			/// <summary>
			/// Is visible item with specified index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="minVisiblePart">The minimal visible part of the item to consider item visible.</param>
			/// <returns>true if item visible; false otherwise.</returns>
			public abstract bool IsVisible(int index, float minVisiblePart);

			/// <summary>
			/// Update view.
			/// </summary>
			public virtual void UpdateView()
			{
				if (!IsVirtualizationSupported())
				{
					return;
				}

				ValidatePosition();

				if (!UpdateDisplayedIndices())
				{
					return;
				}

				Owner.SetDisplayedIndices(IsTileView);
			}

			/// <summary>
			/// Updates the layout bridge.
			/// </summary>
			public abstract void UpdateLayout();

			/// <summary>
			/// Get the top filler size to center the items.
			/// </summary>
			/// <returns>Size.</returns>
			public abstract float CenteredFillerSize();

			/// <summary>
			/// Determines whether is required center the list items.
			/// </summary>
			/// <returns><c>true</c> if required center the list items; otherwise, <c>false</c>.</returns>
			public abstract bool IsRequiredCenterTheItems();

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			public abstract void CalculateMaxVisibleItems();

			/// <summary>
			/// Compare LayoutElements by layoutPriority.
			/// </summary>
			/// <param name="x">First LayoutElement.</param>
			/// <param name="y">Second LayoutElement.</param>
			/// <returns>Result of the comparison.</returns>
			protected static int LayoutElementsComparison(ILayoutElement x, ILayoutElement y)
			{
				return -x.layoutPriority.CompareTo(y.layoutPriority);
			}

			/// <summary>
			/// Calculates the size of the item.
			/// </summary>
			/// <param name="reset">Reset item size.</param>
			/// <returns>Item size.</returns>
			public virtual Vector2 GetItemSize(bool reset = false)
			{
				Owner.DefaultItem.gameObject.SetActive(true);

				var rt = Owner.DefaultItem.transform as RectTransform;

				Owner.LayoutElements.Clear();
				Compatibility.GetComponents<ILayoutElement>(Owner.DefaultItem.gameObject, Owner.LayoutElements);
				Owner.LayoutElements.Sort(LayoutElementsComparison);

				var size = Owner.ItemSize;

				if ((size.x == 0f) || reset)
				{
					size.x = Mathf.Max(Mathf.Max(PreferredWidth(Owner.LayoutElements), rt.rect.width), 1f);
					if (float.IsNaN(size.x))
					{
						size.x = 1f;
					}
				}

				if ((size.y == 0f) || reset)
				{
					size.y = Mathf.Max(Mathf.Max(PreferredHeight(Owner.LayoutElements), rt.rect.height), 1f);
					if (float.IsNaN(size.y))
					{
						size.y = 1f;
					}
				}

				Owner.DefaultItem.gameObject.SetActive(false);

				return size;
			}

			static float PreferredHeight(List<ILayoutElement> elems)
			{
				if (elems.Count == 0)
				{
					return 0f;
				}

				var priority = elems[0].layoutPriority;
				var result = -1f;
				for (int i = 0; i < elems.Count; i++)
				{
					if ((result > -1f) && (elems[i].layoutPriority < priority))
					{
						break;
					}

					result = Mathf.Max(Mathf.Max(result, elems[i].minHeight), elems[i].preferredHeight);
					priority = elems[i].layoutPriority;
				}

				return result;
			}

			static float PreferredWidth(List<ILayoutElement> elems)
			{
				if (elems.Count == 0)
				{
					return 0f;
				}

				var priority = elems[0].layoutPriority;
				var result = -1f;
				for (int i = 0; i < elems.Count; i++)
				{
					if ((result > -1f) && (elems[i].layoutPriority < priority))
					{
						break;
					}

					result = Mathf.Max(Mathf.Max(result, elems[i].minWidth), elems[i].preferredWidth);
					priority = elems[i].layoutPriority;
				}

				return result;
			}

			/// <summary>
			/// Calculates the size of the top filler.
			/// </summary>
			/// <returns>The top filler size.</returns>
			public abstract float TopFillerSize();

			/// <summary>
			/// Calculates the size of the bottom filler.
			/// </summary>
			/// <returns>The bottom filler size.</returns>
			public abstract float BottomFillerSize();

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public abstract int GetFirstVisibleIndex(bool strict = false);

			/// <summary>
			/// Gets the last visible index.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public abstract int GetLastVisibleIndex(bool strict = false);

			/// <summary>
			/// Gets the position of the start border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPosition(int index);

			/// <summary>
			/// Gets the position of the end border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionBorderEnd(int index);

			/// <summary>
			/// Gets the position to display item at the center of the ScrollRect viewport.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionMiddle(int index);

			/// <summary>
			/// Gets the position to display item at the bottom of the ScrollRect viewport.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public abstract float GetItemPositionBottom(int index);

			/// <summary>
			/// Calculate and sets the sizes of the items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
			public virtual void CalculateItemsSizes(ObservableList<TItem> items, bool forceUpdate = true)
			{
			}

			/// <summary>
			/// Calculates the size of the component for the specified item.
			/// </summary>
			/// <returns>The component size.</returns>
			/// <param name="item">Item.</param>
			/// <param name="template">Template.</param>
			protected virtual Vector2 CalculateComponentSize(TItem item, Template template)
			{
				Owner.SetData(template.Template, item);

				LayoutRebuilder.ForceRebuildLayoutImmediate(Owner.Container);

				var size = ValidateSize(template.Template.RectTransform.rect.size);

				return size;
			}

			/// <summary>
			/// Size value is acceptable?
			/// </summary>
			/// <param name="value">Value.</param>
			/// <returns>true if value is acceptable; false otherwise.</returns>
			protected bool AcceptableSize(float value)
			{
				if (float.IsNaN(value))
				{
					return false;
				}

				if (float.IsInfinity(value))
				{
					return false;
				}

				return value >= 1f;
			}

			/// <summary>
			/// Validate size.
			/// </summary>
			/// <param name="size">Size.</param>
			/// <returns>Correct size.</returns>
			protected Vector2 ValidateSize(Vector2 size)
			{
				if (!AcceptableSize(size.x))
				{
					size.x = 1f;
				}

				if (!AcceptableSize(size.y))
				{
					size.y = 1f;
				}

				return size;
			}

			/// <summary>
			/// Calculates the size of the component for the item with the specified index.
			/// </summary>
			/// <returns>The component size.</returns>
			/// <param name="index">Index.</param>
			public virtual Vector2 GetItemFullSize(int index)
			{
				return Owner.ItemSize;
			}

			/// <summary>
			/// Gets the size of the item.
			/// </summary>
			/// <returns>The item size.</returns>
			/// <param name="index">Item index.</param>
			protected float GetItemSize(int index)
			{
				var size = GetItemFullSize(index);
				return Owner.IsHorizontal() ? size.x : size.y;
			}

			/// <summary>
			/// Adds the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public virtual void AddCallback(TComponent item)
			{
			}

			/// <summary>
			/// Removes the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public virtual void RemoveCallback(TComponent item)
			{
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			public int GetNearestIndex(Vector2 point)
			{
				return GetNearestIndex(point, NearestType.Auto);
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			/// <param name="type">Preferable nearest index.</param>
			public abstract int GetNearestIndex(Vector2 point, NearestType type);

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			public abstract int GetNearestItemIndex();

			/// <summary>
			/// Get the size of the ListView.
			/// </summary>
			/// <returns>The size.</returns>
			public abstract float ListSize();

			/// <summary>
			/// Get block index by item index.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <returns>Block index.</returns>
			protected virtual int GetBlockIndex(int index)
			{
				return index;
			}

			/// <summary>
			/// Gets the items per block count.
			/// </summary>
			/// <returns>The items per block.</returns>
			public virtual int GetItemsPerBlock()
			{
				return 1;
			}

			/// <summary>
			/// Determines is virtualization supported.
			/// </summary>
			/// <returns><c>true</c> if virtualization supported; otherwise, <c>false</c>.</returns>
			public virtual bool IsVirtualizationSupported()
			{
				return IsVirtualizationPossible();
			}

			/// <summary>
			/// Determines whether this instance can be virtualized.
			/// </summary>
			/// <returns><c>true</c> if this instance can virtualized; otherwise, <c>false</c>.</returns>
			public virtual bool IsVirtualizationPossible()
			{
				var has_scrollrect = Owner.scrollRect != null;
				var has_container = Owner.Container != null;
				var layout = has_container
					? ((Owner.layout != null) ? Owner.layout : Owner.Container.GetComponent<LayoutGroup>())
					: null;
				var valid_layout = false;
				if (layout != null)
				{
					var is_easy_layout = layout is EasyLayout;
					valid_layout = Owner.RequireEasyLayout
						? is_easy_layout
						: (is_easy_layout || (layout is HorizontalOrVerticalLayoutGroup));
				}

				return has_scrollrect && valid_layout;
			}

			/// <summary>
			/// Process the item move event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			/// <param name="item">Item.</param>
			/// <returns>true if was moved to the next item; otherwise false.</returns>
			public virtual bool OnItemMove(AxisEventData eventData, ListViewItem item)
			{
				var step = 0;
				switch (eventData.moveDir)
				{
					case MoveDirection.Left:
						if (Owner.IsHorizontal())
						{
							step = -1;
						}

						break;
					case MoveDirection.Right:
						if (Owner.IsHorizontal())
						{
							step = 1;
						}

						break;
					case MoveDirection.Up:
						if (!Owner.IsHorizontal())
						{
							step = -1;
						}

						break;
					case MoveDirection.Down:
						if (!Owner.IsHorizontal())
						{
							step = 1;
						}

						break;
				}

				if (step == 0)
				{
					return false;
				}

				var target = GetSelectableComponentIndex(item.Index, step);

				return Owner.Navigate(eventData, target);
			}

			/// <summary>
			/// Get index of the next selectable component.
			/// </summary>
			/// <param name="currentIndex">Current index.</param>
			/// <param name="step">Step.</param>
			/// <returns>Index of the component to select.</returns>
			protected int GetSelectableComponentIndex(int currentIndex, int step)
			{
				var index = currentIndex + step;
				while (!Owner.CanSelect(index) && Owner.IsValid(index))
				{
					index += step;
				}

				return index;
			}

			/// <summary>
			/// Validates the content size and item size.
			/// </summary>
			public virtual void ValidateContentSize()
			{
			}

			/// <summary>
			/// Get visibility data.
			/// </summary>
			/// <returns>Visibility data.</returns>
			protected virtual Visibility VisibilityData()
			{
				var visible = default(Visibility);

				if (Owner.LoopedListAvailable)
				{
					visible.FirstVisible = GetFirstVisibleIndex();
					visible.Items = Mathf.Min(MaxVisibleItems, Owner.DataSource.Count);
				}
				else if (Owner.Virtualization && IsVirtualizationSupported() && (Owner.DataSource.Count > 0))
				{
					visible.FirstVisible = GetFirstVisibleIndex();
					visible.Items = Mathf.Min(MaxVisibleItems, Owner.DataSource.Count);
					if ((visible.FirstVisible + visible.Items) > Owner.DataSource.Count)
					{
						visible.Items = Owner.DataSource.Count - visible.FirstVisible;
						if (visible.Items < GetItemsPerBlock())
						{
							visible.Items = Mathf.Min(Owner.DataSource.Count, visible.Items + GetItemsPerBlock());
							visible.FirstVisible = Owner.DataSource.Count - visible.Items;
						}
					}
				}
				else
				{
					visible.FirstVisible = 0;
					visible.Items = Owner.DataSource.Count;
				}

				return visible;
			}

			/// <summary>
			/// Update displayed indices.
			/// </summary>
			/// <returns>true if displayed indices changed; otherwise false.</returns>
			public virtual bool UpdateDisplayedIndices()
			{
				var new_visible = VisibilityData();
				if (new_visible == Visible)
				{
					return false;
				}

				Visible = new_visible;

				Owner.DisplayedIndices.Clear();
				for (int i = Visible.FirstVisible; i < Visible.LastVisible; i++)
				{
					Owner.DisplayedIndices.Add(VisibleIndex2ItemIndex(i));
				}

				return true;
			}

			/// <summary>
			/// Convert visible index to item index.
			/// </summary>
			/// <returns>The item index.</returns>
			/// <param name="index">Visible index.</param>
			public virtual int VisibleIndex2ItemIndex(int index)
			{
				if (index < 0)
				{
					index += Owner.DataSource.Count * Mathf.CeilToInt((float)-index / (float)Owner.DataSource.Count);
				}

				return index % Owner.DataSource.Count;
			}

			/// <summary>
			/// Process ListView direction changed.
			/// </summary>
			public virtual void DirectionChanged()
			{
				if (Owner.Layout != null)
				{
					Owner.Layout.MainAxis = !Owner.IsHorizontal() ? Axis.Horizontal : Axis.Vertical;
				}
			}

			/// <summary>
			/// Get debug information.
			/// </summary>
			/// <param name="builder">String builder.</param>
			public virtual void GetDebugInfo(System.Text.StringBuilder builder)
			{
				builder.Append("IsTileView: ");
				builder.Append(IsTileView);
				builder.AppendLine();

				builder.Append("Max Visible Items: ");
				builder.Append(MaxVisibleItems);
				builder.AppendLine();

				builder.AppendLine("Visibility");

				builder.Append("Visibility.FirstVisible: ");
				builder.Append(Visible.FirstVisible);
				builder.AppendLine();

				builder.Append("Visibility.LastVisible: ");
				builder.Append(Visible.LastVisible);
				builder.AppendLine();

				builder.Append("Visibility.Items: ");
				builder.Append(Visible.Items);
				builder.AppendLine();

				builder.Append("First Visible Index: ");
				builder.Append(GetFirstVisibleIndex());
				builder.AppendLine();

				builder.Append("Last Visible Index: ");
				builder.Append(GetLastVisibleIndex());
				builder.AppendLine();

				builder.Append("List Size: ");
				builder.Append(ListSize());
				builder.AppendLine();

				builder.Append("Items Per Block: ");
				builder.Append(GetItemsPerBlock());
				builder.AppendLine();

				builder.Append("Top Filler: ");
				builder.Append(TopFillerSize());
				builder.AppendLine();

				builder.Append("Bottom Filler: ");
				builder.Append(BottomFillerSize());
				builder.AppendLine();

				builder.AppendLine("Items");
				for (int index = 0; index < Owner.DataSource.Count; index++)
				{
					builder.Append("\t");
					builder.Append(index);
					builder.Append(". size: ");
					builder.Append(GetItemFullSize(index).ToString());
					builder.Append("; position: ");
					builder.Append(GetItemPosition(index));
					builder.Append("; block: ");
					builder.Append(GetBlockIndex(index));
					builder.AppendLine();
				}
			}
		}
	}
}