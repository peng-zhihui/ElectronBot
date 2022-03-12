namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Collections functions.
	/// </summary>
	public static class UtilitiesCollections
	{
		/// <summary>
		/// Convert list to string.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <param name="values">Values.</param>
		/// <param name="separator">Values separator.</param>
		/// <returns>String.</returns>
		public static string List2String<T>(List<T> values, string separator = "; ")
		{
			var sb = new System.Text.StringBuilder();

			for (int i = 0; i < values.Count; i++)
			{
				var v = values[i];
				if (i == 0)
				{
					sb.Append(v.ToString());
				}
				else
				{
					sb.Append(separator);
					sb.Append(v.ToString());
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Convert list to string.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <param name="values">Values.</param>
		/// <param name="separator">Values separator.</param>
		/// <returns>String.</returns>
		public static string List2String<T>(T[] values, string separator = "; ")
		{
			var sb = new System.Text.StringBuilder();

			for (int i = 0; i < values.Length; i++)
			{
				var v = values[i];
				if (i == 0)
				{
					sb.Append(v.ToString());
				}
				else
				{
					sb.Append(separator);
					sb.Append(v.ToString());
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Create list.
		/// </summary>
		/// <typeparam name="T">Type of the item.</typeparam>
		/// <param name="count">Items count.</param>
		/// <param name="create">Function to create item.</param>
		/// <returns>List.</returns>
		public static ObservableList<T> CreateList<T>(int count, Func<int, T> create)
		{
			var result = new ObservableList<T>(true, count);

			result.BeginUpdate();

			for (int i = 1; i <= count; i++)
			{
				result.Add(create(i));
			}

			result.EndUpdate();

			return result;
		}

		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <typeparam name="T">Item type.</typeparam>
		/// <param name="source">Items.</param>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the elements to search for.</param>
		/// <returns>A List{T} containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty List{T}.</returns>
		public static ObservableList<T> FindAll<T>(List<T> source, Func<T, bool> match)
		{
			var result = new ObservableList<T>();

			FindAll(source, match, result);

			return result;
		}

		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <typeparam name="T">Item type.</typeparam>
		/// <param name="source">Items.</param>
		/// <param name="match">The Predicate{T} delegate that defines the conditions of the elements to search for.</param>
		/// <param name="result">List with founded items.</param>
		public static void FindAll<T>(List<T> source, Func<T, bool> match, ObservableList<T> result)
		{
			for (int i = 0; i < source.Count; i++)
			{
				if (match(source[i]))
				{
					result.Add(source[i]);
				}
			}
		}

		/// <summary>
		/// Get sum of the list.
		/// </summary>
		/// <param name="source">List to sum.</param>
		/// <returns>Sum.</returns>
		public static float Sum(List<float> source)
		{
			var result = 0f;

			for (int i = 0; i < source.Count; i++)
			{
				result += source[i];
			}

			return result;
		}

		/// <summary>
		/// Check is input array not empty and all values are null.
		/// </summary>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="arr">Input array.</param>
		/// <returns>true if input array not empty and all values are null; otherwise false.</returns>
		public static bool AllNull<T>(T[] arr)
			where T : class
		{
			if (arr.Length == 0)
			{
				return false;
			}

			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] != null)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Prints the specified list to log.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="list">List.</param>
		public static void Log<T>(List<T> list)
		{
			var arr = new string[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				arr[i] = list[i].ToString();
			}

			Debug.Log(Time.frameCount.ToString() + ": " + string.Join("; ", arr));
		}
	}
}