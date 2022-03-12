namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Linked HashSet.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	public sealed class LinkedHashSet<T> : ICollection<T>
	{
		readonly LinkedList<T> list;

		readonly Dictionary<T, LinkedListNode<T>> dict;

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
		/// </summary>
		public LinkedHashSet()
		{
			list = new LinkedList<T>();
			dict = new Dictionary<T, LinkedListNode<T>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Required.")]
		public LinkedHashSet(IEnumerable<T> source)
		{
			list = new LinkedList<T>();
			dict = new Dictionary<T, LinkedListNode<T>>();

			foreach (var item in source)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		public LinkedHashSet(T[] source)
		{
			list = new LinkedList<T>();
			dict = new Dictionary<T, LinkedListNode<T>>();

			foreach (var item in source)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		public LinkedHashSet(List<T> source)
		{
			list = new LinkedList<T>();
			dict = new Dictionary<T, LinkedListNode<T>>();

			foreach (var item in source)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedHashSet{T}"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		public LinkedHashSet(ObservableList<T> source)
		{
			list = new LinkedList<T>();
			dict = new Dictionary<T, LinkedListNode<T>>();

			foreach (var item in source)
			{
				Add(item);
			}
		}

		/// <summary>
		/// Get items.
		/// </summary>
		/// <returns>Items list.</returns>
		public List<T> Items()
		{
			return new List<T>(list);
		}

		/// <summary>
		/// Get items.
		/// </summary>
		/// <param name="output">Output.</param>
		public void GetItems(List<T> output)
		{
			output.AddRange(list);
		}

		/// <summary>
		/// Get first item.
		/// </summary>
		/// <returns>First item.</returns>
		public T First()
		{
			return list.First.Value;
		}

		/// <summary>
		/// Get last item.
		/// </summary>
		/// <returns>Last item.</returns>
		public T Last()
		{
			return list.Last.Value;
		}

		/// <summary>
		/// Removes all elements that match the conditions defined by the specified predicate from a LinkedHashSet{T} collection.
		/// </summary>
		/// <returns>The number of elements that were removed from the LinkedHashSet{T} collection.</returns>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the elements to remove.</param>
		public int RemoveWhere(Predicate<T> match)
		{
			var result = 0;

			var keys = new List<T>(dict.Keys);
			foreach (var item in keys)
			{
				if (match(item))
				{
					Remove(item);
					result += 1;
				}
			}

			return result;
		}

		#region ICollection implementation

		/// <summary>
		/// Adds an object to the LinkedHashSet{T}.
		/// </summary>
		/// <param name="item">The object to add to the LinkedHashSet{T}.</param>
		public void Add(T item)
		{
			if (!dict.ContainsKey(item))
			{
				list.AddLast(item);
				dict.Add(item, list.Last);
			}
		}

		/// <summary>
		/// Removes all items from the LinkedHashSet{T}.
		/// </summary>
		public void Clear()
		{
			dict.Clear();
			list.Clear();
		}

		/// <summary>
		/// Determines whether the LinkedHashSet{T} contains a specific value.
		/// </summary>
		/// <returns>true if item is found in the LinkedHashSet{T}; otherwise, false.</returns>
		/// <param name="item">The object to locate in the LinkedHashSet{T}.</param>
		public bool Contains(T item)
		{
			return dict.ContainsKey(item);
		}

		/// <summary>
		/// Copies the elements of the LinkedHashSet{T} to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from LinkedHashSet{T}. The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the LinkedHashSet{T}.
		/// </summary>
		/// <returns><c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>. This method returns false if item is not found in the LinkedHashSet{T} object.</returns>
		/// <param name="item">The object to remove from the LinkedHashSet{T}.</param>
		public bool Remove(T item)
		{
			if (dict.ContainsKey(item))
			{
				list.Remove(dict[item]);
				dict.Remove(item);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the number of elements contained in the LinkedHashSet{T}.
		/// </summary>
		/// <value>The number of elements contained in the LinkedHashSet{T}.</value>
		public int Count
		{
			get
			{
				return dict.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the LinkedHashSet{T} is read only.
		/// </summary>
		/// <value><c>true</c> if the LinkedHashSet{T} is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region IEnumerable implementation

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		public LinkedList<T>.Enumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion
	}
}