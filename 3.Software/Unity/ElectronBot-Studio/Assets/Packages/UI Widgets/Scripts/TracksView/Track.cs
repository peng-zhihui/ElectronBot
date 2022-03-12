namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// Track.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	[Serializable]
	public class Track<TData, TPoint> : IObservable, INotifyPropertyChanged
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		[SerializeField]
		string name;

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				if (name != value)
				{
					name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		[SerializeField]
		ObservableList<TData> data = new ObservableList<TData>();

		/// <summary>
		/// Data.
		/// </summary>
		public ObservableList<TData> Data
		{
			get
			{
				return data;
			}

			set
			{
				if (data != value)
				{
					data.OnChange -= DataChanged;
					data = value;
					data.OnChange += DataChanged;

					DataChanged();

					NotifyPropertyChanged("Data");
				}
			}
		}

		/// <summary>
		/// Max items at the same point.
		/// </summary>
		public int MaxItemsAtSamePoint
		{
			get;
			protected set;
		}

		/// <summary>
		/// Sorted data.
		/// </summary>
		protected List<TData> SortedData = new List<TData>();

		readonly List<TData> visibleItems = new List<TData>();

		/// <summary>
		/// First visible point.
		/// </summary>
		protected TPoint VisibleStart;

		/// <summary>
		/// Last visible point.
		/// </summary>
		protected TPoint VisibleEnd;

		/// <summary>
		/// First visible index.
		/// </summary>
		protected int VisibleIndexStart;

		/// <summary>
		/// Last visible index.
		/// </summary>
		protected int VisibleIndexEnd;

		/// <summary>
		/// Visible items.
		/// </summary>
		public ReadOnlyCollection<TData> VisibleItems
		{
			get
			{
				return visibleItems.AsReadOnly();
			}
		}

		TrackLayout<TData, TPoint> layout;

		/// <summary>
		/// Layout function.
		/// </summary>
		public TrackLayout<TData, TPoint> Layout
		{
			get
			{
				return layout;
			}

			set
			{
				if (layout != value)
				{
					layout = value;

					SetOrder();
				}
			}
		}

		/// <summary>
		/// Set minimal order of the items.
		/// </summary>
		public bool ItemsToTop
		{
			get;
			set;
		}

		bool separateGroups = true;

		/// <summary>
		/// If enabled, each group of the overlapped items will be processed separately; otherwise, all items at once.
		/// </summary>
		public bool SeparateGroups
		{
			get
			{
				return separateGroups;
			}

			set
			{
				if (separateGroups != value)
				{
					separateGroups = value;
					SetOrder();
				}
			}
		}

		/// <summary>
		/// Default items comparison.
		/// </summary>
		/// <param name="x">First item.</param>
		/// <param name="y">Second item.</param>
		/// <returns>Comparison result.</returns>
		protected int DefaultComparison(TData x, TData y)
		{
			var result = x.StartPoint.CompareTo(y.StartPoint);
			if (result != 0)
			{
				return result;
			}

			result = x.EndPoint.CompareTo(y.EndPoint);
			if (result != 0)
			{
				return result;
			}

			return x.Order.CompareTo(y.Order);
		}

		/// <summary>
		/// Items comparison by the order value.
		/// </summary>
		/// <param name="x">First item.</param>
		/// <param name="y">Second item.</param>
		/// <returns>Comparison result.</returns>
		protected int GroupComparison(TData x, TData y)
		{
			var result = x.Order.CompareTo(y.Order);
			if (result != 0)
			{
				return result;
			}

			return -x.FixedOrder.CompareTo(y.FixedOrder);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Track{TData, TPoint}"/> class.
		/// </summary>
		public Track()
		{
			Data.OnChange += DataChanged;
			ItemsToTop = true;
			SetOrder();
		}

		/// <summary>
		/// Get visible items.
		/// </summary>
		/// <param name="output">Output list.</param>
		public void GetVisibleItems(List<TData> output)
		{
			output.AddRange(visibleItems);
		}

		/// <summary>
		/// Count of the items at the specified point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Count of the items.</returns>
		protected int ItemsAtPoint(TPoint point)
		{
			return ItemsBetweenPoint(point, point);
		}

		/// <summary>
		/// Count of the items between specified points.
		/// </summary>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		/// <returns>Count of the items.</returns>
		protected int ItemsBetweenPoint(TPoint startPoint, TPoint endPoint)
		{
			var start = FindIndexStart(startPoint);
			var end = FindIndexEnd(endPoint);

			return end - start + 1;
		}

		/// <summary>
		/// Update SortedData list.
		/// </summary>
		protected void UpdateSortedData()
		{
			SortedData.Clear();
			SortedData.AddRange(Data);
			SortedData.Sort(DefaultComparison);
		}

		/// <summary>
		/// Process data changed event.
		/// </summary>
		protected void DataChanged()
		{
			SetOrder();

			CalculateVisibleItems();

			NotifyPropertyChanged("Data");
		}

		/// <summary>
		/// Group of the overlapped items.
		/// </summary>
		protected List<TData> Group = new List<TData>();

		/// <summary>
		/// Set order of the items.
		/// </summary>
		public void SetOrder()
		{
			UpdateSortedData();

			CalculateOrder();
		}

		/// <summary>
		/// Calculate order.
		/// </summary>
		protected void CalculateOrder()
		{
			if (SortedData.Count == 0)
			{
				MaxItemsAtSamePoint = 1;
				return;
			}

			if (!SeparateGroups)
			{
				Group.AddRange(SortedData);
				MaxItemsAtSamePoint = UpdateOrder(Group) + 1;

				Group.Clear();
				return;
			}

			var first = SortedData[0];
			Group.Add(first);

			var max_order = 0;
			var start = first.StartPoint;
			var end = first.EndPoint;

			for (int i = 1; i < SortedData.Count; i++)
			{
				var point = SortedData[i];
				if (InRange(point, start, end))
				{
					start = Min(start, point.StartPoint);
					end = Max(end, point.EndPoint);
				}
				else
				{
					max_order = Mathf.Max(max_order, UpdateOrder(Group));
					Group.Clear();

					start = point.StartPoint;
					end = point.EndPoint;
				}

				Group.Add(point);
			}

			max_order = Mathf.Max(max_order, UpdateOrder(Group));
			Group.Clear();

			MaxItemsAtSamePoint = max_order + 1;
		}

		/// <summary>
		/// Update items order for the specified group.
		/// </summary>
		/// <param name="group">Group.</param>
		/// <returns>Max order in the group.</returns>
		protected int UpdateOrder(List<TData> group)
		{
			group.Sort(GroupComparison);

			SetOrder(group);

			return GetMaxOrder(group);
		}

		/// <summary>
		/// Find group with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		protected void FindGroup(TData item)
		{
			if (SortedData.Count == 0)
			{
				return;
			}

			if (!SeparateGroups)
			{
				Group.AddRange(SortedData);
				return;
			}

			var start = SortedData[0].StartPoint;
			var end = SortedData[0].EndPoint;
			Group.Add(SortedData[0]);

			for (int i = 1; i < SortedData.Count; i++)
			{
				var point = SortedData[i];
				if (InRange(point, start, end))
				{
					start = Min(start, point.StartPoint);
					end = Max(end, point.EndPoint);
				}
				else
				{
					if (Group.Contains(item))
					{
						return;
					}
					else
					{
						Group.Clear();

						start = point.StartPoint;
						end = point.EndPoint;
					}
				}

				Group.Add(point);
			}
		}

		/// <summary>
		/// Set order of the item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="newOrder">New item order.</param>
		/// <param name="prevOrder">Previous item order.</param>
		public virtual void SetItemOrder(TData item, int newOrder, int prevOrder)
		{
			if (newOrder < 0)
			{
				newOrder = 0;
			}

			if (ItemsToTop)
			{
				SetItemOrderTop(item, newOrder, prevOrder);
			}
			else
			{
				SetItemOrderCustom(item, newOrder, prevOrder);
			}

			SetOrder(Group);

			Group.Clear();

			MaxItemsAtSamePoint = GetMaxOrder(SortedData) + 1;

			NotifyPropertyChanged("Data");
		}

		/// <summary>
		/// Get maximal order for items in the specified list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <returns>Maximal order.</returns>
		protected int GetMaxOrder(List<TData> list)
		{
			var max_order = 0;

			for (int i = 0; i < list.Count; i++)
			{
				max_order = Mathf.Max(max_order, list[i].Order);
			}

			return max_order;
		}

		/// <summary>
		/// Get maximal order for items with FixedOrder in the specified list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <returns>Maximal order.</returns>
		protected int GetMaxFixedOrder(List<TData> list)
		{
			var max_order = 0;

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].FixedOrder)
				{
					max_order = Mathf.Max(max_order, list[i].Order);
				}
			}

			return max_order;
		}

		/// <summary>
		/// Set custom order of the item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="newOrder">New item order.</param>
		/// <param name="prevOrder">Previous item order.</param>
		protected virtual void SetItemOrderCustom(TData item, int newOrder, int prevOrder)
		{
			var max_group_order = 0;
			foreach (var b in SortedData)
			{
				if (ReferenceEquals(b, item) && (prevOrder == -1))
				{
					continue;
				}

				max_group_order = Math.Max(max_group_order, b.Order);
			}

			newOrder = Math.Min(newOrder, max_group_order);

			item.Order = newOrder;
			item.FixedOrder = true;

			UpdateSortedData();

			FindGroup(item);

			Group.Sort(GroupComparison);

			// disable FixedOrder for other items with the same order
			for (int i = 0; i < Group.Count; i++)
			{
				var next_item = Group[i];
				if ((next_item.Order < item.Order) || ReferenceEquals(next_item, item))
				{
					continue;
				}

				if (next_item.Order > item.Order)
				{
					break;
				}

				next_item.FixedOrder = false;
			}

			Group.Sort(GroupComparison);
		}

		/// <summary>
		/// Set minimal order of the item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="newOrder">New item order.</param>
		/// <param name="prevOrder">Previous item order.</param>
		protected virtual void SetItemOrderTop(TData item, int newOrder, int prevOrder)
		{
			item.Order = newOrder;

			UpdateSortedData();

			FindGroup(item);

			Group.Sort(GroupComparison);

			Group.Remove(item);

			if (newOrder < Group.Count)
			{
				Group.Insert(newOrder, item);
			}
			else
			{
				Group.Add(item);
			}
		}

		/// <summary>
		/// Set order for the specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		protected void SetOrder(List<TData> items)
		{
			if (Layout != null)
			{
				Layout.Set(items);
			}
		}

		/// <summary>
		/// Get minimal point.
		/// </summary>
		/// <param name="point1">Point 1.</param>
		/// <param name="point2">Point 2.</param>
		/// <returns>Minimal point.</returns>
		protected TPoint Min(TPoint point1, TPoint point2)
		{
			return point1.CompareTo(point2) >= 0 ? point2 : point1;
		}

		/// <summary>
		/// Get maximum point.
		/// </summary>
		/// <param name="point1">Point 1.</param>
		/// <param name="point2">Point 2.</param>
		/// <returns>Maximum point.</returns>
		protected TPoint Max(TPoint point1, TPoint point2)
		{
			return point1.CompareTo(point2) >= 0 ? point1 : point2;
		}

		/// <summary>
		/// Calculate visible items.
		/// </summary>
		protected void CalculateVisibleItems()
		{
			var new_start = FindIndexStart(VisibleStart);
			var new_end = FindIndexEnd(VisibleEnd);

			if ((new_start == VisibleIndexStart) && (new_end == VisibleIndexEnd))
			{
				return;
			}

			VisibleIndexStart = new_start;
			VisibleIndexEnd = new_end;
			visibleItems.Clear();

			for (int i = VisibleIndexStart; i <= VisibleIndexEnd; i++)
			{
				if (i < 0)
				{
					continue;
				}

				if (IsVisible(SortedData[i]))
				{
					visibleItems.Add(SortedData[i]);
				}
			}

			var dragged_index = GetDraggedIndex();
			var dragged_index_visible = (dragged_index >= VisibleIndexStart) && (dragged_index <= VisibleIndexEnd);
			if ((dragged_index >= 0) && (!dragged_index_visible))
			{
				visibleItems.Add(SortedData[dragged_index]);
			}
		}

		/// <summary>
		/// Get index of the dragged item.
		/// </summary>
		/// <returns>Index.</returns>
		protected int GetDraggedIndex()
		{
			for (int index = 0; index < SortedData.Count; index++)
			{
				if (SortedData[index].IsDragged)
				{
					return index;
				}
			}

			return -1;
		}

		/// <summary>
		/// Is data visible?
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if data visible; otherwise false.</returns>
		protected bool IsVisible(TData item)
		{
			return InRange(item, VisibleStart, VisibleEnd);
		}

		/// <summary>
		/// Is data located between specified points.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		/// <returns>true if data located between specified points; otherwise false.</returns>
		protected bool InRange(TData item, TPoint startPoint, TPoint endPoint)
		{
			var start = (startPoint.CompareTo(item.StartPoint) <= 0)
				|| (startPoint.CompareTo(item.EndPoint) < 0);

			var end = (endPoint.CompareTo(item.StartPoint) > 0)
				|| (endPoint.CompareTo(item.EndPoint) >= 0);

			return start && end;
		}

		/// <summary>
		/// Find index of the first data located at or after specified point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Index of the first data located at or after specified point; -1 if data not found.</returns>
		protected int FindIndexStart(TPoint point)
		{
			// NOTE: replace with binary search
			for (int i = 0; i < SortedData.Count; i++)
			{
				if ((point.CompareTo(SortedData[i].StartPoint) <= 0)
					|| (point.CompareTo(SortedData[i].EndPoint) <= 0))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Find index of the last data located at or after specified point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Index of the last data located at or after specified point; -1 if data not found.</returns>
		protected int FindIndexEnd(TPoint point)
		{
			// NOTE: replace with binary search
			for (int i = SortedData.Count - 1; i >= 0; i--)
			{
				if ((point.CompareTo(SortedData[i].StartPoint) >= 0)
					|| (point.CompareTo(SortedData[i].EndPoint) >= 0))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Reset visible range.
		/// </summary>
		public void VisibleRangeReset()
		{
			VisibleIndexStart = -1;
			VisibleIndexEnd = -1;
			visibleItems.Clear();
		}

		/// <summary>
		/// Set visible range.
		/// </summary>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		public void VisibleRangeSet(TPoint startPoint, TPoint endPoint)
		{
			VisibleStart = startPoint;
			VisibleEnd = endPoint;

			CalculateVisibleItems();
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raise PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
		{
			var c_handlers = OnChange;
			if (c_handlers != null)
			{
				c_handlers();
			}

			var handlers = PropertyChanged;
			if (handlers != null)
			{
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Copy data to target.
		/// </summary>
		/// <param name="target">Target.</param>
		public void CopyTo(Track<TData, TPoint> target)
		{
			target.Name = Name;
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (data != null)
			{
				data.OnChange -= DataChanged;
			}
		}
	}
}