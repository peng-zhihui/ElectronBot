namespace UIWidgets
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Base class for GroupedList.
	/// </summary>
	/// <typeparam name="T">Items type.</typeparam>
	public abstract class GroupedList<T>
	{
		/// <summary>
		/// Contains groups and items for each group.
		/// Group as key. Items for group as value.
		/// </summary>
		protected Dictionary<T, List<T>> GroupsWithItems = new Dictionary<T, List<T>>();

		/// <summary>
		/// Group comparison.
		/// </summary>
		public Comparison<T> GroupComparison;

		/// <summary>
		/// Dictionary converted to flat list.
		/// </summary>
		public ObservableList<T> Data;

		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected abstract T GetGroup(T item);

		int itemsPerBlock = 1;

		/// <summary>
		/// Items per block (row or column).
		/// </summary>
		public int ItemsPerBlock
		{
			get
			{
				return itemsPerBlock;
			}

			set
			{
				if (itemsPerBlock != value)
				{
					itemsPerBlock = value;
					Update();
				}
			}
		}

		T emptyGroupItem;

		/// <summary>
		/// Empty item to fill group row.
		/// </summary>
		public T EmptyGroupItem
		{
			get
			{
				return emptyGroupItem;
			}

			set
			{
				if (!IsItemsEquals(emptyGroupItem, value))
				{
					emptyGroupItem = value;
					Update();
				}
			}
		}

		T emptyItem;

		/// <summary>
		/// Empty item to fill the last items block.
		/// </summary>
		public T EmptyItem
		{
			get
			{
				return emptyItem;
			}

			set
			{
				if (!IsItemsEquals(emptyItem, value))
				{
					emptyItem = value;
					Update();
				}
			}
		}

		static bool IsItemsEquals(T a, T b)
		{
			return EqualityComparer<T>.Default.Equals(a, b);
		}

		/// <summary>
		/// Add item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void Add(T item)
		{
			var group = GetGroup(item);
			if (!GroupsWithItems.ContainsKey(group))
			{
				GroupsWithItems.Add(group, new List<T>());
			}

			GroupsWithItems[group].Add(item);

			Update();
		}

		/// <summary>
		/// Remove item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item exists and deleted; otherwise, false.</returns>
		public bool Remove(T item)
		{
			var group = GetGroup(item);
			if (!GroupsWithItems.ContainsKey(group))
			{
				return false;
			}

			GroupsWithItems[group].Remove(item);

			if (GroupsWithItems[group].Count == 0)
			{
				GroupsWithItems.Remove(group);
			}

			Update();

			return true;
		}

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear()
		{
			GroupsWithItems.Clear();
			Update();
		}

		/// <summary>
		/// Update data list.
		/// </summary>
		public virtual void Update()
		{
			if (inUpdate)
			{
				isChanged = true;
				return;
			}

			if (Data == null)
			{
				return;
			}

			Data.BeginUpdate();

			Data.Clear();

			InsertGroups();

			Data.EndUpdate();
		}

		/// <summary>
		/// Insert groups with items to the Data list.
		/// </summary>
		protected virtual void InsertGroups()
		{
			var groups = new List<T>(GroupsWithItems.Keys);
			if (GroupComparison != null)
			{
				groups.Sort(GroupComparison);
			}

			for (int group_index = 0; group_index < groups.Count; group_index++)
			{
				var group = groups[group_index];

				Data.Add(group);

				for (var j = 1; j < ItemsPerBlock; j++)
				{
					Data.Add(EmptyGroupItem);
				}

				var items = GroupsWithItems[group];
				Data.AddRange(items);

				if (ItemsPerBlock > 0)
				{
					var n = items.Count % ItemsPerBlock;
					if (n > 0)
					{
						for (var j = n; j < ItemsPerBlock; j++)
						{
							Data.Add(EmptyItem);
						}
					}
				}
			}
		}

		bool inUpdate;
		bool isChanged;

		/// <summary>
		/// Pause data list update.
		/// </summary>
		public void BeginUpdate()
		{
			inUpdate = true;
		}

		/// <summary>
		/// Unpause data list update.
		/// </summary>
		public void EndUpdate()
		{
			inUpdate = false;
			if (isChanged)
			{
				isChanged = false;
				Update();
			}
		}
	}
}