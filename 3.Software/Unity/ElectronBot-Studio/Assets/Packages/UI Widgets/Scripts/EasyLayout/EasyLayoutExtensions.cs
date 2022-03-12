namespace EasyLayoutNS.Extensions
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Extensions.
	/// </summary>
	public static class EasyLayoutExtensions
	{
		/// <summary>
		/// Is enum value has specified flag?
		/// </summary>
		/// <param name="value">Enum value.</param>
		/// <param name="flag">Flag.</param>
		/// <returns>true if enum has flag; otherwise false.</returns>
		public static bool IsSet(this ResizeType value, ResizeType flag)
		{
			return (value & flag) == flag;
		}

		/// <summary>
		/// Convert the specified input with converter.
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
			for (int i = 0; i < input.Count; i++)
			{
				output.Add(converter(input[i]));
			}
			
			return output;
			#else
			return input.ConvertAll<TOutput>(converter);
			#endif
		}

		/// <summary>
		/// Apply for each item in the list.
		/// </summary>
		/// <param name="source">List.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this List<T> source, Action<T> action)
		{
			foreach (T element in source)
			{
				action(element);
			}
		}

		/// <summary>
		/// Apply for each item in the list.
		/// </summary>
		/// <param name="source">List.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this T[] source, Action<T> action)
		{
			foreach (T element in source)
			{
				action(element);
			}
		}

		#if NETFX_CORE
		/// <summary>
		/// Apply for each item in the list.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this List<T> list, Action<T> action)
		{
			for (int i = 0; i < list.Count; i++)
			{
				action(list[i]);
			}
		}
		#endif
	}
}