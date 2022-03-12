namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewCustomBase, IStylable
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListView renderer with items of variable size.
		/// </summary>
		protected class ListViewTypeSize : ListViewTypeRectangle
		{
			/// <summary>
			/// Items sizes.
			/// </summary>
			protected readonly Dictionary<TItem, Vector2> ItemSizes = new Dictionary<TItem, Vector2>();

			readonly List<TItem> ItemsToRemove = new List<TItem>();

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeSize"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public ListViewTypeSize(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
			}

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			public override void CalculateMaxVisibleItems()
			{
				CalculateItemsSizes(Owner.DataSource, false);

				MaxVisibleItems = CalculateMaxVisibleItems(Owner.DataSource);
			}

			/// <summary>
			/// Calculates the maximum count of the visible items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <returns>Maximum count of the visible items.</returns>
			protected virtual int CalculateMaxVisibleItems(ObservableList<TItem> items)
			{
				if (!Owner.Virtualization)
				{
					return Owner.DataSource.Count;
				}

				if (items.Count == 0)
				{
					return 0;
				}

				var spacing = Owner.LayoutBridge.GetSpacing();
				var min = MinSize(items);

				var size = GetScrollRectSize();
				var result = MinVisibleItems;

				size -= min;

				if (size > 0)
				{
					result += Mathf.FloorToInt(size / (min + spacing));
				}

				return result + 1;
			}

			/// <summary>
			/// Get the minimal size of the specified items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <returns>Minimal size.</returns>
			protected virtual float MinSize(ObservableList<TItem> items)
			{
				if (items.Count == 0)
				{
					return 1f;
				}

				var result = GetItemSize(items[0]);

				for (int i = 1; i < items.Count; i++)
				{
					result = Mathf.Min(result, GetItemSize(items[i]));
				}

				return Mathf.Max(1f, result);
			}

			/// <inheritdoc/>
			public override Vector2 GetItemFullSize(int index)
			{
				return GetItemFullSize(Owner.DataSource[index]);
			}

			/// <summary>
			/// Gets the size of the item.
			/// </summary>
			/// <returns>The item size.</returns>
			/// <param name="item">Item.</param>
			protected Vector2 GetItemFullSize(TItem item)
			{
				Vector2 result;
				if (!ItemSizes.TryGetValue(item, out result))
				{
					result = Owner.ItemSize;
				}

				return result;
			}

			/// <summary>
			/// Gets the size of the item.
			/// </summary>
			/// <returns>The item size.</returns>
			/// <param name="item">Item.</param>
			protected float GetItemSize(TItem item)
			{
				var size = GetItemFullSize(item);
				return Owner.IsHorizontal() ? size.x : size.y;
			}

			/// <summary>
			/// Calculates the size of the top filler.
			/// </summary>
			/// <returns>The top filler size.</returns>
			public override float TopFillerSize()
			{
				return GetItemPosition(Visible.FirstVisible) - Owner.LayoutBridge.GetMargin();
			}

			/// <summary>
			/// Calculates the size of the bottom filler.
			/// </summary>
			/// <returns>The bottom filler size.</returns>
			public override float BottomFillerSize()
			{
				var last = Owner.DisplayedIndexLast + 1;
				var size = last < 0 ? 0f : GetItemsSize(last, Owner.DataSource.Count - last);
				if (size > 0f)
				{
					size += Owner.LayoutBridge.GetSpacing();
				}

				return size;
			}

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var first_visible_index = Mathf.Max(0, GetIndexAtPosition(GetPosition()));

				if (Owner.LoopedListAvailable)
				{
					return first_visible_index;
				}

				if (strict)
				{
					return first_visible_index;
				}

				return Mathf.Min(first_visible_index, Mathf.Max(0, Owner.DataSource.Count - MinVisibleItems));
			}

			/// <summary>
			/// Gets the last index of the visible.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				var last_visible_index = GetIndexAtPosition(GetPosition() + GetScrollRectSize());

				return strict ? last_visible_index : last_visible_index + 2;
			}

			/// <summary>
			/// Gets the width of the items.
			/// </summary>
			/// <returns>The items width.</returns>
			/// <param name="start">Start index.</param>
			/// <param name="count">Items count.</param>
			protected float GetItemsSize(int start, int count)
			{
				if (count == 0)
				{
					return 0f;
				}

				var width = 0f;
				var n = Owner.LoopedListAvailable ? start + count : Mathf.Min(start + count, Owner.DataSource.Count);
				for (int i = start; i < n; i++)
				{
					width += GetItemSize(Owner.DataSource[VisibleIndex2ItemIndex(i)]);
				}

				width += Owner.LayoutBridge.GetSpacing() * (count - 1);

				return Mathf.Max(0, width);
			}

			/// <summary>
			/// Gets the position of the start border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPosition(int index)
			{
				var n = Mathf.Min(index, Owner.DataSource.Count);
				var size = 0f;
				for (int i = 0; i < n; i++)
				{
					size += GetItemSize(Owner.DataSource[i]);
				}

				return size + (Owner.LayoutBridge.GetSpacing() * index) + Owner.LayoutBridge.GetMargin();
			}

			/// <summary>
			/// Gets the position of the end border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionBorderEnd(int index)
			{
				return GetItemPosition(index) + GetItemSize(index);
			}

			/// <summary>
			/// Gets the position to display item at the center of the ScrollRect viewport.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionMiddle(int index)
			{
				return GetItemPosition(index) + (GetItemSize(index) / 2f) - (GetScrollRectSize() / 2f);
			}

			/// <summary>
			/// Gets the position to display item at the bottom of the ScrollRect viewport.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionBottom(int index)
			{
				return GetItemPosition(index) + GetItemSize(index) + Owner.LayoutBridge.GetMargin() - Owner.LayoutBridge.GetSpacing() - GetScrollRectSize();
			}

			/// <summary>
			/// Remove old items from saved sizes.
			/// </summary>
			/// <param name="items">New items.</param>
			protected virtual void RemoveOldItems(ObservableList<TItem> items)
			{
				foreach (var item in ItemSizes.Keys)
				{
					if (!items.Contains(item))
					{
						ItemsToRemove.Add(item);
					}
				}

				foreach (var item in ItemsToRemove)
				{
					ItemSizes.Remove(item);
				}

				ItemsToRemove.Clear();
			}

			/// <summary>
			/// Calculate and sets the width of the items.
			/// </summary>
			/// <param name="items">Items.</param>
			/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
			public override void CalculateItemsSizes(ObservableList<TItem> items, bool forceUpdate = true)
			{
				RemoveOldItems(items);

				if (Owner.PrecalculateItemSizes)
				{
					if (forceUpdate)
					{
						for (int index = 0; index < items.Count; index++)
						{
							var item = items[index];

							var template = Owner.ComponentsPool.GetTemplate(index);
							template.EnableTemplate();

							ItemSizes[item] = CalculateComponentSize(item, template);
						}
					}
					else
					{
						for (int index = 0; index < items.Count; index++)
						{
							var item = items[index];
							if (!ItemSizes.ContainsKey(item))
							{
								var template = Owner.ComponentsPool.GetTemplate(index);
								template.EnableTemplate();

								ItemSizes[item] = CalculateComponentSize(item, template);
							}
						}
					}

					Owner.ComponentsPool.DisableTemplates();
				}
				else
				{
					if (forceUpdate)
					{
						for (int index = 0; index < items.Count; index++)
						{
							var item = items[index];
							ItemSizes[item] = Owner.ItemSize;
						}
					}
					else
					{
						for (int index = 0; index < items.Count; index++)
						{
							var item = items[index];
							if (!ItemSizes.ContainsKey(item))
							{
								ItemSizes[item] = Owner.ItemSize;
							}
						}
					}
				}
			}

			/// <summary>
			/// Gets the index of the item at he specified position.
			/// </summary>
			/// <returns>The index.</returns>
			/// <param name="position">Position.</param>
			int GetIndexAtPosition(float position)
			{
				var result = GetIndexAtPosition(position, NearestType.Before);
				if (result >= Owner.DataSource.Count)
				{
					result = Owner.DataSource.Count - 1;
				}

				return result;
			}

			/// <summary>
			/// Gets the index of the item at he specified position.
			/// </summary>
			/// <returns>The index.</returns>
			/// <param name="position">Position.</param>
			/// <param name="type">Type.</param>
			int GetIndexAtPosition(float position, NearestType type)
			{
				var spacing = Owner.LayoutBridge.GetSpacing();
				var index = 0;
				for (int i = 0; i < Owner.DataSource.Count; i++)
				{
					index = i;

					var item_size = GetItemSize(i);
					if (i > 0)
					{
						item_size += spacing;
					}

					if (position < item_size)
					{
						break;
					}

					position -= item_size;
				}

				switch (type)
				{
					case NearestType.Auto:
						var item_size = GetItemSize(index);
						if (position >= (item_size / 2f))
						{
							index += 1;
						}

						break;
					case NearestType.Before:
						break;
					case NearestType.After:
						index += 1;
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported NearestType: {0}", EnumHelper<NearestType>.ToString(type)));
				}

				return index;
			}

			/// <summary>
			/// Adds the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public override void AddCallback(TComponent item)
			{
				item.onResize.AddListener(OnItemSizeChanged);
			}

			/// <summary>
			/// Removes the callback.
			/// </summary>
			/// <param name="item">Item.</param>
			public override void RemoveCallback(TComponent item)
			{
				item.onResize.RemoveListener(OnItemSizeChanged);
			}

			/// <summary>
			/// Handle component size changed event.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <param name="size">New size.</param>
			protected void OnItemSizeChanged(int index, Vector2 size)
			{
				UpdateItemSize(index, size);
			}

			/// <summary>
			/// Update saved size of item.
			/// </summary>
			/// <param name="index">Item index.</param>
			/// <param name="newSize">New size.</param>
			/// <returns>true if size different; otherwise, false.</returns>
			protected virtual bool UpdateItemSize(int index, Vector2 newSize)
			{
				if (!Owner.IsValid(index))
				{
					return false;
				}

				newSize = ValidateSize(newSize);

				var item = Owner.DataSource[index];
				var current_size = GetItemFullSize(item);

				var is_equals = Owner.IsHorizontal()
					? Mathf.Approximately(current_size.x, newSize.x)
					: Mathf.Approximately(current_size.y, newSize.y);

				ItemSizes[item] = newSize;

				if (is_equals)
				{
					return false;
				}

				Owner.SetNeedResize();

				return true;
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			/// <param name="type">Preferable nearest index.</param>
			public override int GetNearestIndex(Vector2 point, NearestType type)
			{
				var pos_block = Owner.IsHorizontal() ? point.x : Mathf.Abs(point.y);
				var index = GetIndexAtPosition(pos_block, type);

				return Mathf.Min(index, Owner.DataSource.Count);
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			public override int GetNearestItemIndex()
			{
				return GetIndexAtPosition(GetPosition());
			}

			/// <summary>
			/// Get the size of the ListView.
			/// </summary>
			/// <returns>The size.</returns>
			public override float ListSize()
			{
				if (Owner.DataSource.Count == 0)
				{
					return 0;
				}

				var size = 0f;
				foreach (var item in Owner.DataSource)
				{
					size += GetItemSize(item);
				}

				return size + (Owner.DataSource.Count * Owner.LayoutBridge.GetSpacing()) - Owner.LayoutBridge.GetSpacing();
			}
		}
	}
}