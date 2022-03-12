namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// EasyLayout utilities.
	/// </summary>
	[Obsolete("Renamed to EasyLayoutUtilities.")]
	public static class EasyLayoutUtilites
	{
		/// <summary>
		/// Maximum count of items in group.
		/// </summary>
		/// <param name="group">Group.</param>
		/// <returns>Maximum count.</returns>
		[Obsolete("Replaced with EasyLayoutUtilities.MaxCount().")]
		public static int MaxCount(List<List<LayoutElementInfo>> group)
		{
			return EasyLayoutUtilities.MaxCount(group);
		}

		/// <summary>
		/// Transpose the specified group.
		/// </summary>
		/// <param name="group">Group.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <returns>Transposed list.</returns>
		[Obsolete("Replaced with EasyLayoutUtilities.Transpose().")]
		public static List<List<T>> Transpose<T>(List<List<T>> group)
		{
			return EasyLayoutUtilities.Transpose(group);
		}

		/// <summary>
		/// Get scaled width.
		/// </summary>
		/// <returns>The width.</returns>
		/// <param name="ui">User interface.</param>
		[Obsolete("Replaced with EasyLayoutUtilities.ScaledWidth().")]
		public static float ScaledWidth(RectTransform ui)
		{
			return EasyLayoutUtilities.ScaledWidth(ui);
		}

		/// <summary>
		/// Get scaled height.
		/// </summary>
		/// <returns>The height.</returns>
		/// <param name="ui">User interface.</param>
		[Obsolete("Replaced with EasyLayoutUtilities.ScaledWidth().")]
		public static float ScaledHeight(RectTransform ui)
		{
			return EasyLayoutUtilities.ScaledHeight(ui);
		}
	}
}