namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// Observable list.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	[Serializable]
	public class ObservableList<T> : IObservable, IObservableList<T>, IList<T>
	{
		/// <summary>
		/// Occurs when data changed.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when changed collection (added item, removed item, replaced item).
		/// </summary>
		public event OnChange OnCollectionChange;

		/// <summary>
		/// Occurs when changed data of the item in the collection.
		/// </summary>
		public event OnChange OnCollectionItemChange;

		/// <summary>
		/// Occurs when data changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Re-sort items when collection changed.
		/// </summary>
		public bool ResortOnCollectionChanged = true;

		/// <summary>
		/// Re-sort item when collection item changed.
		/// </summary>
		public bool ResortOnCollectionItemChanged = true;

		Comparison<T> comparison;

		/// <summary>
		/// Sorts the elements in the entire ObservableList{T} using the specified System.Comparison{T}
		/// </summary>
		/// <value>The comparison.</value>
		public Comparison<T> Comparison
		{
			get
			{
				return comparison;
			}

			set
			{
				comparison = value;

				if (comparison != null)
				{
					if (ResortOnCollectionChanged)
					{
						CollectionChanged();
					}
					else if (ResortOnCollectionItemChanged)
					{
						CollectionItemChanged();
					}
				}
			}
		}

		/// <summary>
		/// The items.
		/// </summary>
		[SerializeField]
		protected List<T> Items;

		/// <summary>
		/// Observe items.
		/// </summary>
		public bool ObserveItems = true;

		private static readonly bool IsValueType;

		private static readonly bool IsItemsObservable;

		private static readonly bool IsItemsSupportNotifyPropertyChanged;

		static ObservableList()
		{
			var type = typeof(T);
			IsValueType = type.IsValueType;
			IsItemsObservable = !IsValueType && typeof(IObservable).IsAssignableFrom(type);
			IsItemsSupportNotifyPropertyChanged = !IsValueType && typeof(INotifyPropertyChanged).IsAssignableFrom(type);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
		/// </summary>
		public ObservableList()
			: this(true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
		/// </summary>
		/// <param name="capacity">List capacity.</param>
		public ObservableList(int capacity)
			: this(true, capacity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
		/// </summary>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <param name="capacity">List capacity.</param>
		public ObservableList(bool observeItems, int capacity = 0)
		{
			ObserveItems = observeItems;

			Items = new List<T>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="observeItems">Is need to observe items? If true ObservableList.OnChange will be raised on item OnChange or PropertyChanged.</param>
		public ObservableList(List<T> enumerable, bool observeItems = true)
		{
			ObserveItems = observeItems;

			Items = new List<T>(enumerable);

			AddCallbacks(Items);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="observeItems">Is need to observe items? If true ObservableList.OnChange will be raised on item OnChange or PropertyChanged.</param>
		public ObservableList(ObservableList<T> enumerable, bool observeItems = true)
		{
			ObserveItems = observeItems;

			Items = new List<T>(enumerable.Items);

			AddCallbacks(Items);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="observeItems">Is need to observe items? If true ObservableList.OnChange will be raised on item OnChange or PropertyChanged.</param>
		public ObservableList(IEnumerable<T> enumerable, bool observeItems = true)
		{
			ObserveItems = observeItems;

			Items = new List<T>(enumerable);

			AddCallbacks(Items);
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="ObservableList{T}"/> class.
		/// </summary>
		~ObservableList()
		{
			if (Items != null)
			{
				RemoveCallbacks(Items);
			}
		}

		/// <summary>
		/// Check if item is null.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item is null; otherwise false.</returns>
		protected static bool IsNull(T item)
		{
			if (IsValueType)
			{
				return false;
			}

			return item == null;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Required.")]
		void AddCallbacks(IEnumerable<T> items)
		{
			if (IsItemsObservable)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as IObservable).OnChange += CollectionItemChanged;
					}
				}
			}
			else if (IsItemsSupportNotifyPropertyChanged)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as INotifyPropertyChanged).PropertyChanged += ItemPropertyChanged;
					}
				}
			}
		}

		void AddCallbacks(List<T> items)
		{
			if (IsItemsObservable)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as IObservable).OnChange += CollectionItemChanged;
					}
				}
			}
			else if (IsItemsSupportNotifyPropertyChanged)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as INotifyPropertyChanged).PropertyChanged += ItemPropertyChanged;
					}
				}
			}
		}

		void AddCallbacks(T[] items)
		{
			if (IsItemsObservable)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as IObservable).OnChange += CollectionItemChanged;
					}
				}
			}
			else if (IsItemsSupportNotifyPropertyChanged)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as INotifyPropertyChanged).PropertyChanged += ItemPropertyChanged;
					}
				}
			}
		}

		void AddCallback(T item)
		{
			if (IsNull(item))
			{
				return;
			}

			if (IsItemsObservable)
			{
				(item as IObservable).OnChange += CollectionItemChanged;
			}
			else if (IsItemsSupportNotifyPropertyChanged)
			{
				(item as INotifyPropertyChanged).PropertyChanged += ItemPropertyChanged;
			}
		}

		void RemoveCallbacks(List<T> items)
		{
			if (IsItemsObservable)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as IObservable).OnChange -= CollectionItemChanged;
					}
				}
			}
			else if (IsItemsSupportNotifyPropertyChanged)
			{
				foreach (var item in items)
				{
					if (!IsNull(item))
					{
						(item as INotifyPropertyChanged).PropertyChanged -= ItemPropertyChanged;
					}
				}
			}
		}

		void RemoveCallback(T item)
		{
			if (IsNull(item))
			{
				return;
			}

			if (IsItemsObservable)
			{
				(item as IObservable).OnChange -= CollectionItemChanged;
			}
			else if (IsItemsSupportNotifyPropertyChanged)
			{
				(item as INotifyPropertyChanged).PropertyChanged -= ItemPropertyChanged;
			}
		}

		void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (ObserveItems)
			{
				CollectionItemChanged(e.PropertyName);
			}
		}

		/// <summary>
		/// Changed collection.
		/// </summary>
		public void CollectionChanged()
		{
			CollectionChanged(true);
		}

		void CollectionChanged(bool resort)
		{
			if (inUpdate)
			{
				isCollectionChanged = true;
				isChanged = true;
			}
			else
			{
				if (resort && ResortOnCollectionChanged && (comparison != null))
				{
					Items.Sort(comparison);
				}

				var cc_handlers = OnCollectionChange;
				if (cc_handlers != null)
				{
					cc_handlers();
				}

				var c_handlers = OnChange;
				if (c_handlers != null)
				{
					c_handlers();
				}

				var handlers = PropertyChanged;
				if (handlers != null)
				{
					handlers(this, new PropertyChangedEventArgs("Collection"));
				}
			}
		}

		/// <summary>
		/// Changed data of item in collection.
		/// </summary>
		public void CollectionItemChanged()
		{
			CollectionItemChanged(null);
		}

		/// <summary>
		/// Changed data of item in collection.
		/// </summary>
		/// <param name="property">Property name.</param>
		void CollectionItemChanged(string property)
		{
			if (!ObserveItems)
			{
				return;
			}

			if (inUpdate)
			{
				isCollectionItemChanged = true;
				isChanged = true;
			}
			else
			{
				if (ResortOnCollectionItemChanged && (comparison != null))
				{
					Items.Sort(comparison);
				}

				var ci_handlers = OnCollectionItemChange;
				if (ci_handlers != null)
				{
					ci_handlers();
				}

				var c_handlers = OnChange;
				if (c_handlers != null)
				{
					c_handlers();
				}

				var handlers = PropertyChanged;
				if (handlers != null)
				{
					handlers(this, new PropertyChangedEventArgs(property ?? "Collection"));
				}
			}
		}

		bool isCollectionChanged;

		bool isCollectionItemChanged;

		bool isChanged;

		bool inUpdate;

		/// <summary>
		/// Maintains performance while items are added/removed/changed by preventing the widgets from drawing until the EndUpdate method is called.
		/// </summary>
		public void BeginUpdate()
		{
			inUpdate = true;
		}

		/// <summary>
		/// Ends the update and raise OnChange if something was changed.
		/// </summary>
		public void EndUpdate()
		{
			inUpdate = false;
			if ((isCollectionChanged && ResortOnCollectionItemChanged) || (isCollectionItemChanged && ResortOnCollectionItemChanged))
			{
				if (comparison != null)
				{
					Items.Sort(comparison);
				}
			}

			if (isCollectionChanged && (OnCollectionChange != null))
			{
				isCollectionChanged = false;

				var c_handlers = OnCollectionChange;
				if (c_handlers != null)
				{
					c_handlers();
				}
			}

			if (isCollectionItemChanged && (OnCollectionItemChange != null))
			{
				isCollectionItemChanged = false;

				var c_handlers = OnCollectionItemChange;
				if (c_handlers != null)
				{
					c_handlers();
				}
			}

			if (isChanged && (OnChange != null))
			{
				isChanged = false;
				var c_handlers = OnChange;
				if (c_handlers != null)
				{
					c_handlers();
				}

				var handlers = PropertyChanged;
				if (handlers != null)
				{
					handlers(this, new PropertyChangedEventArgs("Collection"));
				}
			}
		}

		/// <summary>
		/// Gets the number of elements contained in the ObservableList{T}.
		/// </summary>
		/// <value>The number of elements contained in the ObservableList{T}.</value>
		public int Count
		{
			get
			{
				return Items.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the ObservableList{T} is read only.
		/// </summary>
		/// <value><c>true</c> if the ObservableList{T} is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public T this[int index]
		{
			get
			{
				return Items[index];
			}

			set
			{
				RemoveCallback(Items[index]);

				Items[index] = value;

				AddCallback(Items[index]);

				CollectionChanged();
			}
		}

		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		public T Get(int index)
		{
			return Items[index];
		}

		/// <summary>
		/// Adds an object to the end of the ObservableList{T}.
		/// </summary>
		/// <param name="item">The object to be added to the end of the ObservableList{T}. The value can be null for reference types.</param>
		public void Add(T item)
		{
			Items.Add(item);

			AddCallback(item);

			CollectionChanged();
		}

		/// <summary>
		/// Removes all elements from the ObservableList{T}.
		/// </summary>
		public void Clear()
		{
			RemoveCallbacks(Items);

			Items.Clear();

			CollectionChanged();
		}

		/// <summary>
		/// Determines whether an element is in the ObservableList{T}.
		/// </summary>
		/// <param name="item">The object to locate in the ObservableList{T}. The value can be null for reference types.</param>
		/// <returns>true if item is found in the ObservableList{T}; otherwise, false.</returns>
		public bool Contains(T item)
		{
			return Items.Contains(item);
		}

		/// <summary>
		/// Copies the entire ObservableList{T} to a compatible one-dimensional array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from ObservableList{T}. The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			Items.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the ObservableList{T}.
		/// </summary>
		/// <returns>A List{T}.Enumerator for the ObservableList{T}.</returns>
		public List<T>.Enumerator GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the ObservableList{T}.
		/// </summary>
		/// <returns>A IEnumerator{T} for the ObservableList{T}.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the entire ObservableList{T}.
		/// </summary>
		/// <param name="item">The object to locate in the ObservableList{T}. The value can be null for reference types.</param>
		/// <returns>The zero-based index of the first occurrence of item within the entire ObservableList{T}, if found; otherwise, –1.</returns>
		public int IndexOf(T item)
		{
			return Items.IndexOf(item);
		}

		/// <summary>
		/// Inserts an element into the ObservableList{T} at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert. The value can be null for reference types.</param>
		public void Insert(int index, T item)
		{
			Items.Insert(index, item);

			AddCallback(item);

			CollectionChanged();
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the ObservableList{T}.
		/// </summary>
		/// <param name="item">The object to remove from the ObservableList{T}. The value can be null for reference types.</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the ObservableList{T}.</returns>
		public bool Remove(T item)
		{
			var result = Items.Remove(item);
			if (result)
			{
				RemoveCallback(item);

				CollectionChanged();
			}

			return result;
		}

		/// <summary>
		/// Removes the element at the specified index of the ObservableList{T}.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		public void RemoveAt(int index)
		{
			RemoveCallback(Items[index]);

			Items.RemoveAt(index);

			CollectionChanged();
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ObservableList{T}.
		/// </summary>
		/// <param name="items">The collection whose elements should be added to the end of the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void AddRange(IEnumerable<T> items)
		{
			Items.AddRange(items);
			AddCallbacks(items);

			CollectionChanged();
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ObservableList{T}.
		/// </summary>
		/// <param name="items">The collection whose elements should be added to the end of the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void AddRange(List<T> items)
		{
			Items.AddRange(items);
			AddCallbacks(items);

			CollectionChanged();
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ObservableList{T}.
		/// </summary>
		/// <param name="items">The collection whose elements should be added to the end of the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void AddRange(ObservableList<T> items)
		{
			Items.AddRange(items.Items);
			AddCallbacks(items.Items);

			CollectionChanged();
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the ObservableList{T}.
		/// </summary>
		/// <param name="items">The collection whose elements should be added to the end of the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void AddRange(T[] items)
		{
			Items.AddRange(items);
			AddCallbacks(items);

			CollectionChanged();
		}

		/// <summary>
		/// Returns a read-only IList{T} wrapper for the current collection.
		/// </summary>
		/// <returns>A ReadOnlyCollection{T} that acts as a read-only wrapper around the current ObservableList{T}.</returns>
		public ReadOnlyCollection<T> AsReadOnly()
		{
			#if NETFX_CORE
			return new System.Collections.ObjectModel.ReadOnlyCollection<T>(Items);
			#else
			return Items.AsReadOnly();
			#endif
		}

		/// <summary>
		/// Searches the entire sorted ObservableList{T} for an element using the default comparer and returns the zero-based index of the element.
		/// </summary>
		/// <param name="item">The object to locate. The value can be null for reference types.</param>
		/// <returns>The zero-based index of item in the sorted ObservableList{T}, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(T item)
		{
			return Items.BinarySearch(item);
		}

		/// <summary>
		/// Searches the entire sorted ObservableList{T} for an element using the specified comparer and returns the zero-based index of the element.
		/// </summary>
		/// <param name="item">The object to locate. The value can be null for reference types.</param>
		/// <param name="comparer">The IComparer{T} implementation to use when comparing elements, or null to use the default comparer Comparer{T}.Default.</param>
		/// <returns>The zero-based index of item in the sorted ObservableList{T}, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(T item, IComparer<T> comparer)
		{
			return Items.BinarySearch(item, comparer);
		}

		/// <summary>
		/// Searches a range of elements in the sorted ObservableList{T} for an element using the specified comparer and returns the zero-based index of the element.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to search.</param>
		/// <param name="count">The length of the range to search.</param>
		/// <param name="item">The object to locate. The value can be null for reference types. </param>
		/// <param name="comparer">The IComparer{T} implementation to use when comparing elements, or null to use the default comparer Comparer{T}.Default.</param>
		/// <returns>The zero-based index of item in the sorted ObservableList{T}, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
		{
			return Items.BinarySearch(index, count, item, comparer);
		}

		/// <summary>
		/// Searches the entire sorted ObservableList{T} for an element using the specified comparison and returns the zero-based index of the element.
		/// </summary>
		/// <param name="item">The object to locate. The value can be null for reference types.</param>
		/// <param name="binaryComparison">The Comparison{T} implementation to use when comparing elements, or null to use the default comparer Comparer{T}.Default.</param>
		/// <returns>The zero-based index of item in the sorted ObservableList{T}, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise complement of Count.</returns>
		public int BinarySearch(T item, Comparison<T> binaryComparison)
		{
			if (binaryComparison == null)
			{
				return BinarySearch(item);
			}

			return BinarySearch(item, new ComparisonComparer<T>(binaryComparison));
		}

		/// <summary>
		/// Converts the elements in the current ObservableList{T} to another type, and returns a list containing the converted elements.
		/// </summary>
		/// <param name="converter">A Converter{TInput, TOutput} delegate that converts each element from one type to another type.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
		/// <returns>A ObservableList{T} of the target type containing the converted elements from the current ObservableList{T}.</returns>
		public ObservableList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter, bool observeItems = true)
		{
			return Convert<TOutput>(converter, observeItems);
		}

		/// <summary>
		/// Converts the elements in the current ObservableList{T} to another type, and returns a list containing the converted elements.
		/// </summary>
		/// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
		/// <param name="converter">A Converter{TInput, TOutput} delegate that converts each element from one type to another type.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <returns>A ObservableList{T} of the target type containing the converted elements from the current ObservableList{T}.</returns>
		public ObservableList<TOutput> Convert<TOutput>(Converter<T, TOutput> converter, bool observeItems = true)
		{
			var result = new ObservableList<TOutput>(observeItems, Items.Count);
			foreach (var item in Items)
			{
				result.Add(converter(item));
			}

			return result;
		}

		/// <summary>
		/// Copies the entire ObservableList{T} to a compatible one-dimensional array, starting at the beginning of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from ObservableList{T}. The Array must have zero-based indexing.</param>
		public void CopyTo(T[] array)
		{
			Items.CopyTo(array);
		}

		/// <summary>
		/// Copies a range of elements from the ObservableList{T} to a compatible one-dimensional array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="index">The zero-based index in the source ObservableList{T} at which copying begins.</param>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from ObservableList{T}. The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		/// <param name="count">The number of elements to copy.</param>
		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			Items.CopyTo(index, array, arrayIndex, count);
		}

		/// <summary>
		/// Determines whether the ObservableList{T} contains elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the elements to search for.</param>
		/// <returns>true if the ObservableList{T} contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
		public bool Exists(Predicate<T> match)
		{
			return Items.Exists(match);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire ObservableList{T}.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.</returns>
		public T Find(Predicate<T> match)
		{
			return Items.Find(match);
		}

		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the elements to search for.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <returns>A ObservableList{T} containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty ObservableList{T}.</returns>
		public ObservableList<T> FindAll(Predicate<T> match, bool observeItems = true)
		{
			return new ObservableList<T>(Items.FindAll(match), observeItems);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire ObservableList{T}.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1.</returns>
		public int FindIndex(Predicate<T> match)
		{
			return Items.FindIndex(match);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the ObservableList{T} that extends from the specified index to the last element.
		/// </summary>
		/// <param name="startIndex">The zero-based starting index of the search.</param>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1.</returns>
		public int FindIndex(int startIndex, Predicate<T> match)
		{
			return Items.FindIndex(startIndex, match);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the ObservableList{T} that starts at the specified index and contains the specified number of elements.
		/// </summary>
		/// <param name="startIndex">The zero-based starting index of the search.</param>
		/// <param name="count">The number of elements in the section to search.</param>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, –1.</returns>
		public int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			return Items.FindIndex(startIndex, count, match);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire ObservableList{T}.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.</returns>
		public T FindLast(Predicate<T> match)
		{
			return Items.FindLast(match);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire ObservableList{T}.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, –1.</returns>
		public int FindLastIndex(Predicate<T> match)
		{
			return Items.FindLastIndex(match);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the ObservableList{T} that extends from the first element to the specified index.
		/// </summary>
		/// <param name="startIndex">The zero-based starting index of the backward search.</param>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, –1.</returns>
		public int FindLastIndex(int startIndex, Predicate<T> match)
		{
			return Items.FindLastIndex(startIndex, match);
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the ObservableList{T} that contains the specified number of elements and ends at the specified index.
		/// </summary>
		/// <param name="startIndex">The zero-based starting index of the backward search.</param>
		/// <param name="count">The number of elements in the section to search.</param>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the element to search for.</param>
		/// <returns>The zero-based index of the last occurrence of an element that matches the conditions defined by match, if found; otherwise, –1.</returns>
		public int FindLastIndex(int startIndex, int count, Predicate<T> match)
		{
			return Items.FindLastIndex(startIndex, count, match);
		}

		/// <summary>
		/// Performs the specified action on each element of the ObservableList{T}.
		/// </summary>
		/// <param name="action">The Action{T} delegate to perform on each element of the ObservableList{T}.</param>
		public void ForEach(Action<T> action)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				action(Items[i]);
			}
		}

		/// <summary>
		/// Creates a shallow copy of a range of elements in the source ObservableList{T}.
		/// </summary>
		/// <param name="index">The zero-based ObservableList{T} index at which the range starts.</param>
		/// <param name="count">The number of elements in the range.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <returns>A shallow copy of a range of elements in the source ObservableList{T}.</returns>
		public ObservableList<T> GetRange(int index, int count, bool observeItems = true)
		{
			return new ObservableList<T>(Items.GetRange(index, count), observeItems);
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the ObservableList{T} that extends from the specified index to the last element.
		/// </summary>
		/// <param name="item">The object to locate in the ObservableList{T}. The value can be null for reference types.</param>
		/// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
		/// <returns>The zero-based index of the first occurrence of item within the range of elements in the ObservableList{T} that extends from index to the last element, if found; otherwise, –1.</returns>
		public int IndexOf(T item, int index)
		{
			return Items.IndexOf(item, index);
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the ObservableList{T} that starts at the specified index and contains the specified number of elements.
		/// </summary>
		/// <param name="item">The object to locate in the ObservableList{T}. The value can be null for reference types.</param>
		/// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
		/// <param name="count">The number of elements in the section to search.</param>
		/// <returns>The zero-based index of the first occurrence of item within the range of elements in the ObservableList{T} that starts at index and contains count number of elements, if found; otherwise, –1.</returns>
		public int IndexOf(T item, int index, int count)
		{
			return Items.IndexOf(item, index, count);
		}

		/// <summary>
		/// Inserts the elements of a collection into the ObservableList{T} at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the new elements should be inserted.</param>
		/// <param name="collection">The collection whose elements should be inserted into the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void InsertRange(int index, IEnumerable<T> collection)
		{
			Items.InsertRange(index, collection);

			AddCallbacks(collection);

			CollectionChanged();
		}

		/// <summary>
		/// Inserts the elements of a collection into the ObservableList{T} at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the new elements should be inserted.</param>
		/// <param name="collection">The collection whose elements should be inserted into the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void InsertRange(int index, List<T> collection)
		{
			Items.InsertRange(index, collection);

			AddCallbacks(collection);

			CollectionChanged();
		}

		/// <summary>
		/// Inserts the elements of a collection into the ObservableList{T} at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the new elements should be inserted.</param>
		/// <param name="collection">The collection whose elements should be inserted into the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void InsertRange(int index, ObservableList<T> collection)
		{
			Items.InsertRange(index, collection.Items);

			AddCallbacks(collection.Items);

			CollectionChanged();
		}

		/// <summary>
		/// Inserts the elements of a collection into the ObservableList{T} at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the new elements should be inserted.</param>
		/// <param name="collection">The collection whose elements should be inserted into the ObservableList{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
		public void InsertRange(int index, T[] collection)
		{
			Items.InsertRange(index, collection);

			AddCallbacks(collection);

			CollectionChanged();
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the last occurrence within the entire ObservableList{T}.
		/// </summary>
		/// <param name="item">The object to locate in the ObservableList{T}. The value can be null for reference types.</param>
		/// <returns>The zero-based index of the last occurrence of item within the entire the ObservableList{T}, if found; otherwise, –1.</returns>
		public int LastIndexOf(T item)
		{
			return Items.LastIndexOf(item);
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the ObservableList{T} that extends from the first element to the specified index.
		/// </summary>
		/// <param name="item">The object to locate in the ObservableList{T}. The value can be null for reference types.</param>
		/// <param name="index">The zero-based starting index of the backward search.</param>
		/// <returns>The zero-based index of the last occurrence of item within the range of elements in the ObservableList{T} that extends from the first element to index, if found; otherwise, –1.</returns>
		public int LastIndexOf(T item, int index)
		{
			return Items.LastIndexOf(item, index);
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the ObservableList{T} that contains the specified number of elements and ends at the specified index.
		/// </summary>
		/// <param name="item">The object to locate in the ObservableList{T}. The value can be null for reference types.</param>
		/// <param name="index">The zero-based starting index of the backward search.</param>
		/// <param name="count">The number of elements in the section to search.</param>
		/// <returns>The zero-based index of the last occurrence of item within the range of elements in the ObservableList{T} that contains count number of elements and ends at index, if found; otherwise, –1.</returns>
		public int LastIndexOf(T item, int index, int count)
		{
			return Items.LastIndexOf(item, index, count);
		}

		/// <summary>
		/// Removes all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the elements to remove.</param>
		/// <returns>The number of elements removed from the ObservableList{T}.</returns>
		public int RemoveAll(Predicate<T> match)
		{
			RemoveCallbacks(Items.FindAll(match));

			var deleted = Items.RemoveAll(match);

			CollectionChanged();

			return deleted;
		}

		/// <summary>
		/// Removes a range of elements from the ObservableList{T}.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range of elements to remove.</param>
		/// <param name="count">The number of elements to remove.</param>
		public void RemoveRange(int index, int count)
		{
			for (int i = index; i < index + count; i++)
			{
				RemoveCallback(Items[i]);
			}

			Items.RemoveRange(index, count);

			CollectionChanged();
		}

		/// <summary>
		/// Reverses the order of the elements in the entire ObservableList{T}.
		/// </summary>
		public void Reverse()
		{
			Items.Reverse();

			CollectionChanged();
		}

		/// <summary>
		/// Reverses the order of the elements in the specified range.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to reverse.</param>
		/// <param name="count">The number of elements in the range to reverse.</param>
		public void Reverse(int index, int count)
		{
			Items.Reverse(index, count);

			CollectionChanged();
		}

		/// <summary>
		/// Sorts the elements in the entire ObservableList{T} using the default comparer.
		/// </summary>
		public void Sort()
		{
			Items.Sort();

			CollectionChanged();
		}

		/// <summary>
		/// Sorts the elements in the entire ObservableList{T} using the specified System.Comparison{T}.
		/// </summary>
		/// <param name="sortComparison">The System.Comparison{T} to use when comparing elements.</param>
		public void Sort(Comparison<T> sortComparison)
		{
			Items.Sort(sortComparison);

			CollectionChanged();
		}

		/// <summary>
		/// Sorts the elements in the entire ObservableList{T} using the specified comparer.
		/// </summary>
		/// <param name="comparer">The IComparer{T} implementation to use when comparing elements, or null to use the default comparer Comparer{T}.Default.</param>
		public void Sort(IComparer<T> comparer)
		{
			Items.Sort(comparer);

			CollectionChanged();
		}

		/// <summary>
		/// Sorts the elements in a range of elements in ObservableList{T} using the specified comparer.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to sort.</param>
		/// <param name="count">The length of the range to sort.</param>
		/// <param name="comparer">The IComparer{T} implementation to use when comparing elements, or null to use the default comparer Comparer{T}.Default.</param>
		public void Sort(int index, int count, IComparer<T> comparer)
		{
			Items.Sort(index, count, comparer);

			CollectionChanged();
		}

		/// <summary>
		/// Copies the elements of the ObservableList{T} to a new array.
		/// </summary>
		/// <returns>An array containing copies of the elements of the ObservableList{T}.</returns>
		public T[] ToArray()
		{
			return Items.ToArray();
		}

		/// <summary>
		/// Sets the capacity to the actual number of elements in the ObservableList{T}, if that number is less than a threshold value.
		/// </summary>
		public void TrimExcess()
		{
			Items.TrimExcess();
		}

		/// <summary>
		/// Determines whether every element in the ObservableList{T} matches the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The Predicate{T} delegate that defines the conditions to check against the elements.</param>
		/// <returns>true if every element in the ObservableList{T} matches the conditions defined by the specified predicate; otherwise, false. If the list has no elements, the return value is true.</returns>
		public bool TrueForAll(Predicate<T> match)
		{
			return Items.TrueForAll(match);
		}

		/// <summary>
		/// Convert this instance to List{T}.
		/// </summary>
		/// <returns>List.</returns>
		public List<T> ToList()
		{
			return new List<T>(Items);
		}

		/// <summary>
		/// Returns reference to the internal list.
		/// </summary>
		/// <returns>List.</returns>
		public List<T> ListReference()
		{
			return Items;
		}
	}
}