namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.l10n;

	/// <summary>
	/// ListViewEnum.
	/// </summary>
	public class ListViewEnum : ListViewCustom<ListViewEnumComponent, ListViewEnum.Item>
	{
		/// <summary>
		/// Item.
		/// </summary>
		public struct Item
		{
			/// <summary>
			/// Value.
			/// </summary>
			public readonly long Value;

			/// <summary>
			/// Name.
			/// </summary>
			public readonly string Name;

			/// <summary>
			/// Is value obsolete?
			/// </summary>
			public readonly bool IsObsolete;

			/// <summary>
			/// Initializes a new instance of the <see cref="Item"/> struct.
			/// </summary>
			/// <param name="value">Value.</param>
			/// <param name="name">Name.</param>
			/// <param name="isObsolete">Is value obsolete?</param>
			public Item(long value, string name, bool isObsolete)
			{
				Value = value;
				Name = name;
				IsObsolete = isObsolete;
			}
		}

		/// <summary>
		/// Create wrapper of the specified enum type.
		/// </summary>
		/// <typeparam name="TEnum">Enum type.</typeparam>
		/// <param name="showObsolete">Show obsolete values?</param>
		/// <param name="converter">Value converter from long to TEnum.</param>
		/// <returns>Wrapper.</returns>
		public ListViewEnum<TEnum> UseEnum<TEnum>(bool showObsolete = false, Func<long, TEnum> converter = null)
#if CSHARP_7_3_OR_NEWER
		where TEnum : struct, Enum
#else
		where TEnum : struct
#endif
		{
			return new ListViewEnum<TEnum>(this, showObsolete, converter);
		}
	}
}