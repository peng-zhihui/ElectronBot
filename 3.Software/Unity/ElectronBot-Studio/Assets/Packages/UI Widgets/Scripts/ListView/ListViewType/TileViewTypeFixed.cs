namespace UIWidgets
{
	using System;
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
		/// TileView renderer with items of fixed size.
		/// </summary>
		protected class TileViewTypeFixed : ListViewTypeFixed
		{
			/// <summary>
			/// Items per row.
			/// </summary>
			protected int ItemsPerRow;

			/// <summary>
			/// Items per column.
			/// </summary>
			protected int ItemsPerColumn;

			/// <summary>
			/// Initializes a new instance of the <see cref="TileViewTypeFixed"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			public TileViewTypeFixed(ListViewCustom<TComponent, TItem> owner)
				: base(owner)
			{
			}

			/// <inheritdoc/>
			public override void Enable()
			{
				base.Enable();

				if (Owner.Layout != null)
				{
					var children_size = Owner.IsHorizontal()
						? Owner.Layout.ChildrenWidth
						: Owner.Layout.ChildrenHeight;

					if (children_size != ChildrenSize.DoNothing)
					{
						var field = Owner.IsHorizontal() ? "ChildrenWidth" : "ChildrenHeight";
						var template = "ListType does not match with Container.EasyLayout settings and this can cause scroll problems. Please change ListType to TileViewWithVariableSize or EasyLayout.{0} to DoNothing.";
						Debug.LogWarning(string.Format(template, field), Owner);
					}
				}
			}

			/// <inheritdoc/>
			public override bool IsTileView
			{
				get
				{
					return true;
				}
			}

			/// <summary>
			/// Gets the blocks count.
			/// </summary>
			/// <returns>The blocks count.</returns>
			/// <param name="items">Items.</param>
			protected int GetBlocksCount(int items)
			{
				return items < 0
					? Mathf.FloorToInt((float)items / (float)GetItemsPerBlock())
					: Mathf.CeilToInt((float)items / (float)GetItemsPerBlock());
			}

			/// <inheritdoc/>
			public override void CalculateMaxVisibleItems()
			{
				var spacing_x = Owner.GetItemSpacingX();
				var spacing_y = Owner.GetItemSpacingY();

				var width = Owner.ScrollRectSize.x + spacing_x - Owner.LayoutBridge.GetFullMarginX();
				var height = Owner.ScrollRectSize.y + spacing_y - Owner.LayoutBridge.GetFullMarginY();

				if (Owner.IsHorizontal())
				{
					ItemsPerRow = Mathf.CeilToInt(width / (Owner.ItemSize.x + spacing_x)) + 1;
					ItemsPerRow = Mathf.Max(MinVisibleItems, ItemsPerRow);

					ItemsPerColumn = Mathf.FloorToInt(height / (Owner.ItemSize.y + spacing_y));
					ItemsPerColumn = Mathf.Max(1, ItemsPerColumn);
					ItemsPerColumn = Owner.LayoutBridge.RowsConstraint(ItemsPerColumn);
				}
				else
				{
					ItemsPerRow = Mathf.FloorToInt(width / (Owner.ItemSize.x + spacing_x));
					ItemsPerRow = Mathf.Max(1, ItemsPerRow);
					ItemsPerRow = Owner.LayoutBridge.ColumnsConstraint(ItemsPerRow);

					ItemsPerColumn = Mathf.CeilToInt(height / (Owner.ItemSize.y + spacing_y)) + 1;
					ItemsPerColumn = Mathf.Max(MinVisibleItems, ItemsPerColumn);
				}

				MaxVisibleItems = Owner.Virtualization ? (ItemsPerRow * ItemsPerColumn) : Owner.DataSource.Count;
			}

			/// <inheritdoc/>
			public override int GetFirstVisibleIndex(bool strict = false)
			{
				var first = base.GetFirstVisibleIndex(strict) * GetItemsPerBlock();

				if (first > (Owner.DataSource.Count - 1))
				{
					first = Owner.DataSource.Count - 2;
				}

				return Mathf.Max(0, first);
			}

			/// <inheritdoc/>
			public override int GetLastVisibleIndex(bool strict = false)
			{
				return ((base.GetLastVisibleIndex(strict) + 1) * GetItemsPerBlock()) - 1;
			}

			/// <inheritdoc/>
			public override float BottomFillerSize()
			{
				var last = Owner.DisplayedIndexLast;
				var blocks = last < 0 ? 0 : GetBlocksCount(Owner.DataSource.Count - last - 1);

				return (blocks == 0) ? 0f : blocks * GetItemSize();
			}

			/// <inheritdoc/>
			public override int GetNearestIndex(Vector2 point, NearestType type)
			{
				// block index
				var pos_block = Owner.IsHorizontal() ? point.x : Mathf.Abs(point.y);

				var block = Mathf.FloorToInt(pos_block / GetItemSize());

				// item index in block
				var pos_elem = Owner.IsHorizontal() ? Mathf.Abs(point.y) : point.x;
				var size = Owner.IsHorizontal() ? Owner.ItemSize.y + Owner.GetItemSpacingY() : Owner.ItemSize.x + Owner.GetItemSpacingX();

				int k;
				switch (type)
				{
					case NearestType.Auto:
						k = Mathf.RoundToInt(pos_elem / size);
						break;
					case NearestType.Before:
						k = Mathf.FloorToInt(pos_elem / size);
						break;
					case NearestType.After:
						k = Mathf.CeilToInt(pos_elem / size);
						break;
					default:
						throw new NotSupportedException(string.Format("Unsupported NearestType: {0}", EnumHelper<NearestType>.ToString(type)));
				}

				return (block * GetItemsPerBlock()) + k;
			}

			/// <inheritdoc/>
			public override int GetNearestItemIndex()
			{
				return base.GetNearestItemIndex() * GetItemsPerBlock();
			}

			/// <inheritdoc/>
			public override int GetItemsPerBlock()
			{
				return Owner.IsHorizontal() ? ItemsPerColumn : ItemsPerRow;
			}

			/// <inheritdoc/>
			protected override int GetBlockIndex(int index)
			{
				return Mathf.FloorToInt((float)index / (float)GetItemsPerBlock());
			}

			/// <inheritdoc/>
			public override bool IsVirtualizationSupported()
			{
				var scrollRectSpecified = Owner.scrollRect != null;
				var containerSpecified = Owner.Container != null;
				var currentLayout = containerSpecified ? ((Owner.Layout != null) ? Owner.Layout : Owner.Container.GetComponent<LayoutGroup>()) : null;
				var validLayout = currentLayout is EasyLayout;

				return scrollRectSpecified && validLayout;
			}

			/// <inheritdoc/>
			public override bool OnItemMove(AxisEventData eventData, ListViewItem item)
			{
				var step = 0;
				switch (eventData.moveDir)
				{
					case MoveDirection.Left:
						step = Owner.IsHorizontal() ? -1 : -GetItemsPerBlock();
						break;
					case MoveDirection.Right:
						step = Owner.IsHorizontal() ? 1 : GetItemsPerBlock();
						break;
					case MoveDirection.Up:
						step = Owner.IsHorizontal() ? -GetItemsPerBlock() : -1;
						break;
					case MoveDirection.Down:
						step = Owner.IsHorizontal() ? GetItemsPerBlock() : 1;
						break;
				}

				if (step == 0)
				{
					return false;
				}

				var target = GetSelectableComponentIndex(item.Index, step);

				return Owner.Navigate(eventData, target);
			}

			/// <inheritdoc/>
			public override void ValidateContentSize()
			{
			}

			/// <inheritdoc/>
			public override void GetDebugInfo(System.Text.StringBuilder builder)
			{
				builder.Append("ItemsPerRow: ");
				builder.Append(ItemsPerRow);
				builder.AppendLine();

				builder.Append("ItemsPerColumn: ");
				builder.Append(ItemsPerColumn);
				builder.AppendLine();
			}
		}
	}
}