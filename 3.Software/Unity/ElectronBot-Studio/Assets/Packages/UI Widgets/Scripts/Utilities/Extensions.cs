namespace UIWidgets.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Text;

	/// <summary>
	/// For each extensions.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// For each with index.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Required.")]
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> handler)
		{
			int i = 0;
			foreach (T item in enumerable)
			{
				handler(item, i);
				i++;
			}
		}

		/// <summary>
		/// For each with index.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this T[] enumerable, Action<T, int> handler)
		{
			int i = 0;
			foreach (T item in enumerable)
			{
				handler(item, i);
				i++;
			}
		}

		/// <summary>
		/// For each with index.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this List<T> enumerable, Action<T, int> handler)
		{
			int i = 0;
			foreach (T item in enumerable)
			{
				handler(item, i);
				i++;
			}
		}

		/// <summary>
		/// For each with index.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this ObservableList<T> enumerable, Action<T, int> handler)
		{
			int i = 0;
			foreach (T item in enumerable)
			{
				handler(item, i);
				i++;
			}
		}

		/// <summary>
		/// For each.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Required.")]
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> handler)
		{
			foreach (T item in enumerable)
			{
				handler(item);
			}
		}

		/// <summary>
		/// For each.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this T[] enumerable, Action<T> handler)
		{
			foreach (T item in enumerable)
			{
				handler(item);
			}
		}

		/// <summary>
		/// For each.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this List<T> enumerable, Action<T> handler)
		{
			foreach (T item in enumerable)
			{
				handler(item);
			}
		}

		/// <summary>
		/// For each.
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="handler">Handler.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this ObservableList<T> enumerable, Action<T> handler)
		{
			foreach (T item in enumerable)
			{
				handler(item);
			}
		}

		/// <summary>
		/// Convert IEnumerable{T} to ObservableList{T}.
		/// </summary>
		/// <returns>The observable list.</returns>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static ObservableList<T> ToObservableList<T>(this IEnumerable<T> enumerable, bool observeItems = true)
		{
			return new ObservableList<T>(enumerable, observeItems);
		}

		/// <summary>
		/// Convert IEnumerable{T} to ObservableList{T}.
		/// </summary>
		/// <returns>The observable list.</returns>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static ObservableList<T> ToObservableList<T>(this T[] enumerable, bool observeItems = true)
		{
			return new ObservableList<T>(enumerable, observeItems);
		}

		/// <summary>
		/// Convert IEnumerable{T} to ObservableList{T}.
		/// </summary>
		/// <returns>The observable list.</returns>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static ObservableList<T> ToObservableList<T>(this List<T> enumerable, bool observeItems = true)
		{
			return new ObservableList<T>(enumerable, observeItems);
		}

		/// <summary>
		/// Convert IEnumerable{T} to ObservableList{T}.
		/// </summary>
		/// <returns>The observable list.</returns>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="observeItems">Is need to observe items?</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static ObservableList<T> ToObservableList<T>(this ObservableList<T> enumerable, bool observeItems = true)
		{
			return new ObservableList<T>(enumerable, observeItems);
		}

		/// <summary>
		/// Sums the float.
		/// </summary>
		/// <returns>The float.</returns>
		/// <param name="list">List.</param>
		[Obsolete("Will be removed in next releases.")]
		public static float SumFloat(this IList<float> list)
		{
			var result = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				result += list[i];
			}

			return result;
		}

		/// <summary>
		/// Sums the float.
		/// </summary>
		/// <returns>The float.</returns>
		/// <param name="list">List.</param>
		/// <param name="calculate">Calculate.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		[Obsolete("Will be removed in next releases.")]
		public static float SumFloat<T>(this IList<T> list, Func<T, float> calculate)
		{
			var result = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				result += calculate(list[i]);
			}

			return result;
		}

		/// <summary>
		/// Convert the specified list with converter.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <param name="converter">Converter.</param>
		/// <typeparam name="TInput">The 1st type parameter.</typeparam>
		/// <typeparam name="TOutput">The 2nd type parameter.</typeparam>
		/// <returns>List with converted items.</returns>
		public static List<TOutput> Convert<TInput, TOutput>(this List<TInput> input, Converter<TInput, TOutput> converter)
		{
			#if NETFX_CORE
			var output = new List<TOutput>(input.Count);
			foreach (var item in input)
			{
				output.Add(converter(item));
			}
			
			return output;
			#else
			return input.ConvertAll<TOutput>(converter);
			#endif
		}

		/// <summary>
		/// Gets all sub nodes.
		/// </summary>
		/// <returns>The all sub nodes.</returns>
		/// <param name="nodes">Nodes.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<TreeNode<T>> GetAllSubNodes<T>(this ObservableList<TreeNode<T>> nodes)
		{
			var all_subnodes = new List<TreeNode<T>>();

			foreach (var node in nodes)
			{
				GetSubNodes(node.Nodes, all_subnodes);
			}

			return all_subnodes;
		}

		/// <summary>
		/// Gets the sub nodes from specified nodes list.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="subnodes">Sub nodes.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void GetSubNodes<T>(ObservableList<TreeNode<T>> nodes, List<TreeNode<T>> subnodes)
		{
			if (nodes == null)
			{
				return;
			}

			foreach (var subnode in nodes)
			{
				subnodes.Add(subnode);
				GetSubNodes(subnode.Nodes, subnodes);
			}
		}

		/// <summary>
		/// Convert list to the string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="list">List.</param>
		/// <param name="format">Format.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static string ToString<T>(this IList<T> list, string format)
			where T : IFormattable
		{
			return list.ToString(format, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Convert list to the string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="list">List.</param>
		/// <param name="format">Format.</param>
		/// <param name="formatProvider">Format provider.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static string ToString<T>(this IList<T> list, string format, IFormatProvider formatProvider)
			where T : IFormattable
		{
			if (list.Count == 0)
			{
				return string.Empty;
			}

			var result = new StringBuilder(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				result.Append(string.Format(format, list[i].ToString(), formatProvider));
			}

			return result.ToString();
		}

		/// <summary>
		/// Convert list to the string.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="list">List.</param>
		/// <param name="format">Format.</param>
		/// <param name="arg1">Argument 1 object.</param>
		/// <param name="formatProvider">Format provider.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		public static string ToString<T>(this IList<T> list, string format, object arg1, IFormatProvider formatProvider)
			where T : IFormattable
		{
			if (list.Count == 0)
			{
				return string.Empty;
			}

			var result = new StringBuilder(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				result.Append(string.Format(format, list[i], arg1, formatProvider));
			}

			return result.ToString();
		}

		/// <summary>
		/// Remove and return the item in the end of the list.
		/// </summary>
		/// <typeparam name="T">Type of the item.</typeparam>
		/// <param name="list">List.</param>
		/// <returns>Last item.</returns>
		public static T Pop<T>(this IList<T> list)
		{
			var n = list.Count - 1;
			var result = list[n];
			list.RemoveAt(n);

			return result;
		}

#if NETFX_CORE
		/// <summary>
		/// Determines if is assignable from the specified source from.
		/// </summary>
		/// <returns><c>true</c> if is assignable from the specified source from; otherwise, <c>false</c>.</returns>
		/// <param name="source">Source.</param>
		/// <param name="from">From.</param>
		static public bool IsAssignableFrom(this Type source, Type from)
		{
			return source.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());
		}

		/// <summary>
		/// Apply action for each item in list.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static public void ForEach<T>(this List<T> list, Action<T> action)
		{
			for (int i = 0; i < list.Count; i++)
			{
				action(list[i]);
			}
		}
#endif
	}
}