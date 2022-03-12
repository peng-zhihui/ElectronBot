namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListView with dynamic items heights.
	/// </summary>
	[Obsolete("Replaced with ListViewString")]
	public class ListViewHeight : ListView
	{
		/// <summary>
		/// Items heights.
		/// </summary>
		protected Dictionary<string, float> Heights = new Dictionary<string, float>();

		/// <summary>
		/// Calculates the maximum count of the visible items.
		/// </summary>
		protected override void CalculateMaxVisibleItems()
		{
			SetItemsHeight(DataSource);

			maxVisibleItems = GetMaxVisibleItems();
		}

		/// <summary>
		/// Get the max count of the visible items.
		/// </summary>
		/// <returns>Maximum count of the visible items,</returns>
		protected int GetMaxVisibleItems()
		{
			var spacing = LayoutBridge.GetSpacing();
			var min = MinSize();

			var size = GetScrollSize();
			var result = 0;
			while (true)
			{
				size -= min;
				if (result > 0)
				{
					size -= spacing;
				}

				if (size < 0)
				{
					break;
				}

				result += 1;
			}

			return result + 2;
		}

		/// <summary>
		/// Get the minimal size of the specified items.
		/// </summary>
		/// <returns>Minimal size.</returns>
		protected virtual float MinSize()
		{
			if (DataSource.Count == 0)
			{
				return 0f;
			}

			var result = Heights[DataSource[0]];

			for (int i = 1; i < DataSource.Count; i++)
			{
				result = Mathf.Min(result, Heights[DataSource[i]]);
			}

			return result;
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		protected override void CalculateItemSize()
		{
			var rect = DefaultItem.transform as RectTransform;
			var layout_elements = new List<ILayoutElement>();
			Compatibility.GetComponents<ILayoutElement>(DefaultItem.gameObject, layout_elements);

			if (ItemSize.y == 0)
			{
				ItemSize.y = Mathf.Max(PreferredHeight(layout_elements), rect.rect.height);
			}

			if (ItemSize.x == 0)
			{
				ItemSize.x = Mathf.Max(PreferredWidth(layout_elements), rect.rect.width);
			}
		}

		static float PreferredHeight(List<ILayoutElement> elements)
		{
			var result = 0f;

			foreach (var elem in elements)
			{
				result = Mathf.Max(Mathf.Max(result, elem.minHeight), elem.preferredHeight);
			}

			return result;
		}

		static float PreferredWidth(List<ILayoutElement> elements)
		{
			var result = 0f;

			foreach (var elem in elements)
			{
				result = Mathf.Max(Mathf.Max(result, elem.minWidth), elem.preferredWidth);
			}

			return result;
		}

		/// <summary>
		/// Scrolls to item with specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		public override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return;
			}

			var top = GetScrollValue();
			var bottom = GetScrollValue() + GetScrollSize();

			var item_starts = ItemStartAt(index);

			var item_ends = ItemEndAt(index) + LayoutBridge.GetMargin();

			if (item_starts < top)
			{
				SetScrollValue(item_starts);
			}
			else if (item_ends > bottom)
			{
				SetScrollValue(item_ends - GetScrollSize());
			}
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		protected override float CalculateBottomFillerSize()
		{
			if (bottomHiddenItems == 0)
			{
				return 0f;
			}

			var height = 0f;
			var start = topHiddenItems + visibleItems;
			for (int i = 0; i < bottomHiddenItems; i++)
			{
				height += GetItemHeight(DataSource[start + i]);
			}

			return height + (LayoutBridge.GetSpacing() * (bottomHiddenItems - 1));
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		protected override float CalculateTopFillerSize()
		{
			if (topHiddenItems == 0)
			{
				return 0f;
			}

			var height = 0f;
			for (int i = 0; i < topHiddenItems; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return Mathf.Max(0, height + (LayoutBridge.GetSpacing() * (topHiddenItems - 1)));
		}

		float GetItemHeight(string item)
		{
			return Heights[item];
		}

		/// <summary>
		/// Gets the item position.
		/// </summary>
		/// <returns>The item position.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPosition(int index)
		{
			var height = 0f;
			var n = Mathf.Min(index, DataSource.Count);
			for (int i = 0; i < n; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Gets the item position bottom.
		/// </summary>
		/// <returns>The item position bottom.</returns>
		/// <param name="index">Index.</param>
		public override float GetItemPositionBottom(int index)
		{
			return GetItemPosition(index) + GetItemHeight(DataSource[index]) + LayoutBridge.GetFullMargin() - GetScrollSize();
		}

		/// <summary>
		/// Total height of items before specified index.
		/// </summary>
		/// <returns>Height.</returns>
		/// <param name="index">Index.</param>
		float ItemStartAt(int index)
		{
			var height = 0f;
			for (int i = 0; i < index; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Total height of items before and with specified index.
		/// </summary>
		/// <returns>The <see cref="int"/>.</returns>
		/// <param name="index">Index.</param>
		float ItemEndAt(int index)
		{
			var height = 0f;
			for (int i = 0; i < index + 1; i++)
			{
				height += GetItemHeight(DataSource[i]);
			}

			return height + (LayoutBridge.GetSpacing() * index);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public override int Add(string item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item", "Item is null.");
			}

			if (!Heights.ContainsKey(item))
			{
				Heights.Add(item, CalculateItemHeight(item));
			}

			return base.Add(item);
		}

		/// <summary>
		/// Calculate and sets the height of the items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		void SetItemsHeight(ObservableList<string> items, bool forceUpdate = true)
		{
			if (forceUpdate)
			{
				Heights.Clear();
			}

			foreach (var item in items)
			{
				if (!Heights.ContainsKey(item))
				{
					Heights.Add(item, CalculateItemHeight(item));
				}
			}
		}

		/// <summary>
		/// Resize this instance.
		/// </summary>
		public override void Resize()
		{
			SetItemsHeight(DataSource, true);

			base.Resize();
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		/// <param name="updateView">Update view.</param>
		protected override void SetNewItems(ObservableList<string> newItems, bool updateView = true)
		{
			SetItemsHeight(newItems);

			base.SetNewItems(newItems, updateView);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public override void RunUpdate()
		{
			if (DataSourceSetted || IsDataSourceChanged)
			{
				var reset_scroll = DataSourceSetted;

				DataSourceSetted = false;
				IsDataSourceChanged = false;

				lock (DataSource)
				{
					SetItemsHeight(DataSource);
					CalculateMaxVisibleItems();

					UpdateView();
				}

				if (reset_scroll)
				{
					SetScrollValue(0f);
				}
			}

			if (NeedResize)
			{
				Resize();
			}
		}

		/// <summary>
		/// Gets the height of the index by.
		/// </summary>
		/// <returns>The index by height.</returns>
		/// <param name="height">Height.</param>
		int GetIndexByHeight(float height)
		{
			var spacing = LayoutBridge.GetSpacing();

			var result = 0;
			for (int index = 0; index < DataSource.Count; index++)
			{
				height -= Heights[DataSource[index]];
				if (index > 0)
				{
					height -= spacing;
				}

				if (height < 0)
				{
					break;
				}

				result += 1;
			}

			return result;
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetLastVisibleIndex(bool strict = false)
		{
			var last_visible_index = GetIndexByHeight(GetScrollValue() + GetScrollSize());

			return strict ? last_visible_index : last_visible_index + 2;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		protected override int GetFirstVisibleIndex(bool strict = false)
		{
			var first_visible_index = GetIndexByHeight(GetScrollValue());

			if (strict)
			{
				return first_visible_index;
			}

			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		LayoutGroup defaultItemLayoutGroup;

		/// <summary>
		/// Gets the height of the item.
		/// </summary>
		/// <returns>The item height.</returns>
		/// <param name="item">Item.</param>
		float CalculateItemHeight(string item)
		{
			if (defaultItemLayoutGroup == null)
			{
				defaultItemLayoutGroup = DefaultItem.GetComponent<LayoutGroup>();
			}

			var height = 0f;
			if (defaultItemLayoutGroup != null)
			{
				DefaultItem.gameObject.SetActive(true);

				DefaultItem.SetData(item);
				LayoutUtilities.UpdateLayout(defaultItemLayoutGroup);

				height = LayoutUtility.GetPreferredHeight(DefaultItem.RectTransform);

				DefaultItem.gameObject.SetActive(false);
			}

			return height;
		}

		#region ListViewPaginator support

		/// <summary>
		/// Gets the index of the nearest item.
		/// </summary>
		/// <returns>The nearest item index.</returns>
		public override int GetNearestItemIndex()
		{
			return GetIndexByHeight(GetScrollValue());
		}
		#endregion
	}
}