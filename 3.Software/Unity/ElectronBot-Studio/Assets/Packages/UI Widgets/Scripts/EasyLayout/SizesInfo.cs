namespace EasyLayoutNS
{
	using System.Collections.Generic;

	/// <summary>
	/// Sizes info.
	/// </summary>
	public struct SizesInfo : System.IEquatable<SizesInfo>
	{
		/// <summary>
		/// The summary minimum size.
		/// </summary>
		public float TotalMin;

		/// <summary>
		/// The summary preferred size.
		/// </summary>
		public float TotalPreferred;

		/// <summary>
		/// The summary flexible size.
		/// </summary>
		public float TotalFlexible;

		/// <summary>
		/// The sizes.
		/// </summary>
		public Size[] Sizes;

		/// <summary>
		/// Serves as a hash function for a object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return TotalMin.GetHashCode() ^ TotalPreferred.GetHashCode() ^ TotalFlexible.GetHashCode() ^ Sizes.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified System.Object is equal to the current size.
		/// </summary>
		/// <param name="obj">The System.Object to compare with the current size.</param>
		/// <returns><c>true</c> if the specified System.Object is equal to the current size; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is SizesInfo))
			{
				return false;
			}

			return Equals((SizesInfo)obj);
		}

		/// <summary>
		/// Determines whether the specified size is equal to the current size.
		/// </summary>
		/// <param name="other">The size to compare with the current size.</param>
		/// <returns><c>true</c> if the specified size is equal to the current size; otherwise, <c>false</c>.</returns>
		public bool Equals(SizesInfo other)
		{
			if (TotalMin != other.TotalMin)
			{
				return false;
			}

			if (TotalPreferred != other.TotalPreferred)
			{
				return false;
			}

			if (TotalFlexible != other.TotalFlexible)
			{
				return false;
			}

			return Sizes == other.Sizes;
		}

		/// <summary>
		/// Compare sizes.
		/// </summary>
		/// <param name="sizesInfo1">First size.</param>
		/// <param name="sizesInfo2">Second size.</param>
		/// <returns>True if sizes are equals; otherwise false.</returns>
		public static bool operator ==(SizesInfo sizesInfo1, SizesInfo sizesInfo2)
		{
			return sizesInfo1.Equals(sizesInfo2);
		}

		/// <summary>
		/// Compare sizes.
		/// </summary>
		/// <param name="sizesInfo1">First size.</param>
		/// <param name="sizesInfo2">Second size.</param>
		/// <returns>True if sizes are not equals; otherwise false.</returns>
		public static bool operator !=(SizesInfo sizesInfo1, SizesInfo sizesInfo2)
		{
			return !sizesInfo1.Equals(sizesInfo2);
		}

		/// <summary>
		/// Gets the sizes info.
		/// </summary>
		/// <returns>The sizes info.</returns>
		/// <param name="sizes">Sizes.</param>
		public static SizesInfo GetSizesInfo(Size[] sizes)
		{
			var result = new SizesInfo() { Sizes = sizes };
			for (int i = 0; i < sizes.Length; i++)
			{
				result.TotalMin += sizes[i].Min;
				result.TotalPreferred += sizes[i].Preferred;
				result.TotalFlexible += sizes[i].Flexible;
			}

			if (result.TotalFlexible == 0f)
			{
				for (int i = 0; i < sizes.Length; i++)
				{
					sizes[i].Flexible = 1f;
				}

				result.TotalFlexible += sizes.Length;
			}

			return result;
		}

		/// <summary>
		/// Gets the widths sizes info.
		/// </summary>
		/// <returns>The widths sizes info.</returns>
		/// <param name="elems">Elements.</param>
		public static SizesInfo GetWidths(List<LayoutElementInfo> elems)
		{
			var sizes = new Size[elems.Count];
			for (int i = 0; i < elems.Count; i++)
			{
				sizes[i] = new Size()
				{
					Min = elems[i].MinWidth,
					Preferred = elems[i].PreferredWidth,
					Flexible = elems[i].FlexibleWidth,
				};
			}

			return GetSizesInfo(sizes);
		}

		/// <summary>
		/// Gets the heights sizes info.
		/// </summary>
		/// <returns>The heights sizes info.</returns>
		/// <param name="elems">Elements.</param>
		public static SizesInfo GetHeights(List<LayoutElementInfo> elems)
		{
			var sizes = new Size[elems.Count];
			for (int i = 0; i < elems.Count; i++)
			{
				sizes[i] = new Size()
				{
					Min = elems[i].MinHeight,
					Preferred = elems[i].PreferredHeight,
					Flexible = elems[i].FlexibleHeight,
				};
			}

			return GetSizesInfo(sizes);
		}

		/// <summary>
		/// Gets the widths sizes info.
		/// </summary>
		/// <returns>The widths sizes info.</returns>
		/// <param name="group">Elements group.</param>
		public static SizesInfo GetWidths(LayoutElementsGroup group)
		{
			var sizes = new Size[group.Columns];
			for (int column = 0; column < group.Columns; column++)
			{
				sizes[column] = Size.MaxWidths(group.GetColumn(column));
			}

			return GetSizesInfo(sizes);
		}

		/// <summary>
		/// Gets the heights sizes info.
		/// </summary>
		/// <returns>The heights sizes info.</returns>
		/// <param name="group">Elements group.</param>
		public static SizesInfo GetHeights(LayoutElementsGroup group)
		{
			var sizes = new Size[group.Rows];
			for (int row = 0; row < group.Rows; row++)
			{
				sizes[row] = Size.MaxHeights(group.GetRow(row));
			}

			return GetSizesInfo(sizes);
		}
	}
}