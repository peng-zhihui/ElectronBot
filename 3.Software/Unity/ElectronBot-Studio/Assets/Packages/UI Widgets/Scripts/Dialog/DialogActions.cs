namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Dialog actions.
	/// Key - button name.
	/// Value - action on click.
	/// </summary>
	[Obsolete("No more used. Replaced with DialogButton[].")]
	public class DialogActions : IList<DialogButton>
	{
		readonly List<DialogButton> buttons = new List<DialogButton>();

		/// <summary>
		/// Gets the number of elements contained in the dictionary.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				return buttons.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets or sets the element with the specified key.
		/// </summary>
		/// <returns>The element with the specified key.</returns>
		/// <param name="key">The key of the element to get or set.</param>
		[Obsolete("Should not be used.")]
		public Func<bool> this[string key]
		{
			get
			{
				return GetButton(key).ActionBool;
			}

			set
			{
				GetButton(key).ActionBool = value;
			}
		}

		DialogButton GetButton(string key)
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				if (buttons[i].Label == key)
				{
					return buttons[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Gets an ICollection{string} containing the keys of the dictionary.
		/// </summary>
		/// <value>The keys.</value>
		public List<string> Keys
		{
			get
			{
				var result = new List<string>();
				foreach (var button in buttons)
				{
					result.Add(button.Label);
				}

				return result;
			}
		}

		/// <summary>
		/// Gets an ICollection{Func{bool}} containing the values in the dictionary.
		/// </summary>
		/// <value>The values.</value>
		[Obsolete("Should not be used.")]
		public List<Func<bool>> Values
		{
			get
			{
				var result = new List<Func<bool>>();
				foreach (var button in buttons)
				{
					result.Add(button.ActionBool);
				}

				return result;
			}
		}

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <returns>The element at the specified index.</returns>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		public DialogButton this[int index]
		{
			get
			{
				return buttons[index];
			}

			set
			{
				buttons[index] = value;
			}
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void Add(KeyValuePair<string, Func<bool>> item)
		{
			Add(item.Key, item.Value, 0);
		}

		/// <summary>
		/// Add the specified key and value.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public void Add(string key, Func<bool> value)
		{
			Add(key, value, 0);
		}

		/// <summary>
		/// Add the specified key and value.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		/// <param name="buttonIndex">Index of the button.</param>
		public void Add(string key, Func<bool> value, int buttonIndex)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Key is null.");
			}

			if (ContainsKey(key))
			{
				throw new ArgumentException(string.Format("An element with the same key ({0}) already exists.", key), "key");
			}

#pragma warning disable 0618
			buttons.Add(new DialogButton(key, value, buttonIndex));
#pragma warning restore 0618
		}

		/// <summary>
		/// Removes all items.
		/// </summary>
		public void Clear()
		{
			buttons.Clear();
		}

		/// <summary>
		/// Determines whether contains a specific value.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if contains a specific value; otherwise false.</returns>
		[Obsolete("Should not be used.")]
		public bool Contains(KeyValuePair<string, Func<bool>> item)
		{
			return ContainsKey(item.Key);
		}

		/// <summary>
		/// Determines whether the IDictionary{TKey, TValue} contains an element with the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <returns><c>true</c>, if key is exists, <c>false</c> otherwise.</returns>
		public bool ContainsKey(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Key is null.");
			}

			for (int i = 0; i < buttons.Count; i++)
			{
				if (buttons[i].Label == key)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Copies the elements of the KeyValuePair{string, Func{bool}} to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">Array.</param>
		/// <param name="arrayIndex">Array index.</param>
		[Obsolete("Should not be used.")]
		public void CopyTo(KeyValuePair<string, Func<bool>>[] array, int arrayIndex)
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				array[arrayIndex + i] = new KeyValuePair<string, Func<bool>>(buttons[i].Label, buttons[i].ActionBool);
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>Returns an enumerator that iterates through a collection.</returns>
		public List<DialogButton>.Enumerator GetEnumerator()
		{
			return buttons.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the DialogActions{T}.
		/// </summary>
		/// <returns>A DialogActions{T}.Enumerator for the DialogActions{T}.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		IEnumerator<DialogButton> IEnumerable<DialogButton>.GetEnumerator()
		{
			return buttons.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>Returns an enumerator that iterates through a collection.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return buttons.GetEnumerator();
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the dictionary.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item exists and item was removed; false otherwise.</returns>
		[Obsolete("Should not be used.")]
		public bool Remove(KeyValuePair<string, Func<bool>> item)
		{
			return Remove(item.Key);
		}

		/// <summary>
		/// Removes the element with the specified key from the dictionary.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <returns>true if key exists and item was removed; false otherwise.</returns>
		public bool Remove(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Key is null.");
			}

			for (int i = 0; i < buttons.Count; i++)
			{
				if (buttons[i].Label == key)
				{
					buttons.RemoveAt(i);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <returns><c>true</c>, if key was found, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		[Obsolete("Should not be used.")]
		public bool TryGetValue(string key, out Func<bool> value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key", "Key is null.");
			}

			for (int i = 0; i < buttons.Count; i++)
			{
				if (buttons[i].Label == key)
				{
					value = buttons[i].ActionBool;
					return true;
				}
			}

			value = default(Func<bool>);
			return false;
		}

		/// <summary>
		/// Searches for the specified object and returns the zero-based index of the first occurrence within the entire DialogActions{T}.
		/// </summary>
		/// <returns>The zero-based index of the first occurrence of item within the entire DialogActions{T}, if found; otherwise, –1.</returns>
		/// <param name="item">The object to locate in the DialogActions{T}. The value can be null for reference types.</param>
		public int IndexOf(DialogButton item)
		{
			return buttons.IndexOf(item);
		}

		/// <summary>
		/// Inserts an element into the DialogActions{T} at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert. The value can be null for reference types.</param>
		public void Insert(int index, DialogButton item)
		{
			buttons.Insert(index, item);
		}

		/// <summary>
		/// Removes the element at the specified index of the DialogActions{T}.
		/// </summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		public void RemoveAt(int index)
		{
			buttons.RemoveAt(index);
		}

		/// <summary>
		/// Adds an object to the end of the DialogActions{T}.
		/// </summary>
		/// <param name="item">The object to be added to the end of the DialogActions{T}. The value can be null for reference types.</param>
		public void Add(DialogButton item)
		{
			buttons.Add(item);
		}

		/// <summary>
		/// Determines whether an element is in the DialogActions{T}.
		/// </summary>
		/// <returns>true if item is found in the DialogActions{T}; otherwise, false.</returns>
		/// <param name="item">The object to locate in the DialogActions{T}. The value can be null for reference types.</param>
		public bool Contains(DialogButton item)
		{
			return buttons.Contains(item);
		}

		/// <summary>
		/// Copies the entire DialogActions{T} to a compatible one-dimensional array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from DialogActions{T}. The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(DialogButton[] array, int arrayIndex)
		{
			buttons.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the DialogActions{T}.
		/// </summary>
		/// <param name="item">The object to remove from the DialogActions{T}. The value can be null for reference types.</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the DialogActions{T}.</returns>
		public bool Remove(DialogButton item)
		{
			return buttons.Remove(item);
		}
	}
}