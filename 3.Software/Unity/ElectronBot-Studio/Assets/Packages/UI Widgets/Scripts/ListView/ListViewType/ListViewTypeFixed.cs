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
		protected class ListViewTypeFixed : ListViewTypeRectangle
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewTypeFixed"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public ListViewTypeFixed(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
			}

			/// <summary>
			/// Enable this instance.
			/// </summary>
			public override void Enable()
			{
				base.Enable();

				if (Owner.Layout != null)
				{
					var children_size = Owner.IsHorizontal()
						? Owner.Layout.ChildrenWidth
						: Owner.Layout.ChildrenHeight;

					if (children_size != EasyLayoutNS.ChildrenSize.DoNothing)
					{
						var field = Owner.IsHorizontal() ? "ChildrenWidth" : "ChildrenHeight";
						var template = "ListType does not match with Container.EasyLayout settings and this can cause scroll problems. Please change ListType to ListViewWithVariableSize or EasyLayout.{0} to DoNothing.";
						Debug.LogWarning(string.Format(template, field), Owner);
					}
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
				return Mathf.Max(0, (Owner.DataSource.Count - Owner.DisplayedIndexLast - 1) * GetItemSize());
			}

			/// <summary>
			/// Gets the first index of the visible.
			/// </summary>
			/// <returns>The first visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var first_visible_index = strict
					? Mathf.CeilToInt(GetPosition() / GetItemSize())
					: Mathf.FloorToInt(GetPosition() / GetItemSize());
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

			/// <summary>
			/// Gets the last visible index.
			/// </summary>
			/// <returns>The last visible index.</returns>
			/// <param name="strict">If set to <c>true</c> strict.</param>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				var window = GetPosition() + GetScrollRectSize();
				var last_visible_index = strict
					? Mathf.FloorToInt(window / GetItemSize())
					: Mathf.CeilToInt(window / GetItemSize());

				return last_visible_index - 1;
			}

			/// <summary>
			/// Gets the position of the start border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPosition(int index)
			{
				var block_index = GetBlockIndex(index);
				return (block_index * GetItemSize()) + Owner.LayoutBridge.GetMargin();
			}

			/// <summary>
			/// Gets the position of the end border of the item.
			/// </summary>
			/// <returns>The position.</returns>
			/// <param name="index">Index.</param>
			public override float GetItemPositionBorderEnd(int index)
			{
				var block_index = GetBlockIndex(index + 1);
				var spacing = block_index == 0 ? 0f : Owner.LayoutBridge.GetSpacing();
				return GetItemPosition(index + 1) - spacing;
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
			/// Calculates the maximum count of the visible items.
			/// </summary>
			public override void CalculateMaxVisibleItems()
			{
				if (!Owner.Virtualization)
				{
					MaxVisibleItems = Owner.DataSource.Count;
					return;
				}

				var max = Owner.IsHorizontal()
					? Mathf.CeilToInt(Owner.ScrollRectSize.x / Owner.ItemSize.x)
					: Mathf.CeilToInt(Owner.ScrollRectSize.y / Owner.ItemSize.y);

				MaxVisibleItems = Mathf.Max(max + 1, MinVisibleItems);
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			/// <param name="point">Point.</param>
			/// <param name="type">Preferable nearest index.</param>
			public override int GetNearestIndex(Vector2 point, NearestType type)
			{
				var pos = Owner.IsHorizontal() ? point.x : Mathf.Abs(point.y);

				int index;
				switch (type)
				{
					case NearestType.Auto:
						index = Mathf.RoundToInt(pos / GetItemSize());
						break;
					case NearestType.Before:
						index = Mathf.FloorToInt(pos / GetItemSize());
						break;
					case NearestType.After:
						index = Mathf.CeilToInt(pos / GetItemSize());
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported NearestType: {0}", EnumHelper<NearestType>.ToString(type)));
				}

				return Mathf.Min(index, Owner.DataSource.Count);
			}

			/// <summary>
			/// Gets the index of the nearest item.
			/// </summary>
			/// <returns>The nearest item index.</returns>
			public override int GetNearestItemIndex()
			{
				return Mathf.Clamp(Mathf.RoundToInt(GetPosition() / GetItemSize()), 0, Owner.DataSource.Count - 1);
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

				var blocks = Mathf.CeilToInt((float)Owner.DataSource.Count / (float)Owner.GetItemsPerBlock());
				return (blocks * GetItemSize()) - Owner.LayoutBridge.GetSpacing();
			}

			/// <summary>
			/// Validates the content size and item size.
			/// </summary>
			public override void ValidateContentSize()
			{
				var spacing_x = Owner.GetItemSpacingX();
				var spacing_y = Owner.GetItemSpacingY();

				var height = Owner.ScrollRectSize.y + spacing_y - Owner.LayoutBridge.GetFullMarginY();
				var width = Owner.ScrollRectSize.x + spacing_x - Owner.LayoutBridge.GetFullMarginX();

				int per_block;
				if (Owner.IsHorizontal())
				{
					per_block = Mathf.FloorToInt(height / (Owner.ItemSize.y + spacing_y));
					per_block = Mathf.Max(1, per_block);
					per_block = Owner.LayoutBridge.RowsConstraint(per_block);
				}
				else
				{
					per_block = Mathf.FloorToInt(width / (Owner.ItemSize.x + spacing_x));
					per_block = Mathf.Max(1, per_block);
					per_block = Owner.LayoutBridge.ColumnsConstraint(per_block);
				}

				if (per_block > 1)
				{
					// Debug.LogWarning("More that one item per row or column, consider change DefaultItem size or set layout constraint or use TileViewWithFixedSize (TileViewWithVariableSize)", Owner);
				}
			}
		}
	}
}