namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Base class for the flex and staggered layout groups.
	/// </summary>
	public abstract class EasyLayoutFlexOrStaggered : EasyLayoutBaseType
	{
		/// <summary>
		/// Axis data.
		/// </summary>
		protected struct AxisData : IEquatable<AxisData>
		{
			/// <summary>
			/// Offset.
			/// </summary>
			public float Offset;

			/// <summary>
			/// Spacing.
			/// </summary>
			public float Spacing;

			/// <summary>
			/// Serves as a hash function for a object.
			/// </summary>
			/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
			public override int GetHashCode()
			{
				return Offset.GetHashCode() ^ Spacing.GetHashCode();
			}

			/// <summary>
			/// Determines whether the specified System.Object is equal to the current axis data.
			/// </summary>
			/// <param name="obj">The System.Object to compare with the current axis data.</param>
			/// <returns><c>true</c> if the specified System.Object is equal to the current axis data; otherwise, <c>false</c>.</returns>
			public override bool Equals(object obj)
			{
				if (!(obj is AxisData))
				{
					return false;
				}

				return Equals((AxisData)obj);
			}

			/// <summary>
			/// Determines whether the specified axis data is equal to the current axis data.
			/// </summary>
			/// <param name="other">The axis data to compare with the current axis data.</param>
			/// <returns><c>true</c> if the specified axis data is equal to the current axis data; otherwise, <c>false</c>.</returns>
			public bool Equals(AxisData other)
			{
				if (Offset != other.Offset)
				{
					return false;
				}

				return Spacing == other.Spacing;
			}

			/// <summary>
			/// Compare axis data.
			/// </summary>
			/// <param name="obj1">First axis data.</param>
			/// <param name="obj2">Second axis data.</param>
			/// <returns>True if data are equals; otherwise false.</returns>
			public static bool operator ==(AxisData obj1, AxisData obj2)
			{
				return obj1.Equals(obj2);
			}

			/// <summary>
			/// Compare axis data.
			/// </summary>
			/// <param name="obj1">First axis data.</param>
			/// <param name="obj2">Seconds axis data.</param>
			/// <returns>True if data are not equals; otherwise false.</returns>
			public static bool operator !=(AxisData obj1, AxisData obj2)
			{
				return !obj1.Equals(obj2);
			}
		}

		/// <summary>
		/// Sizes of blocks at the sub axis.
		/// </summary>
		protected List<GroupSize> SubAxisSizes = new List<GroupSize>();

		/// <summary>
		/// Calculate sizes of the elements.
		/// </summary>
		protected override void CalculateSizes()
		{
			if (IsHorizontal)
			{
				if (ChildrenWidth == ChildrenSize.FitContainer)
				{
					ResizeWidthToFit(false);
				}
				else if (ChildrenWidth == ChildrenSize.SetPreferredAndFitContainer)
				{
					ResizeWidthToFit(true);
				}
				else if (ChildrenWidth == ChildrenSize.ShrinkOnOverflow)
				{
					ShrinkWidthToFit();
				}

				if (ChildrenHeight == ChildrenSize.FitContainer)
				{
					ResizeRowHeightToFit(false);
				}
				else if (ChildrenHeight == ChildrenSize.SetPreferredAndFitContainer)
				{
					ResizeRowHeightToFit(true);
				}
				else if (ChildrenHeight == ChildrenSize.ShrinkOnOverflow)
				{
					ShrinkRowHeightToFit();
				}
			}
			else
			{
				if (ChildrenWidth == ChildrenSize.FitContainer)
				{
					ResizeColumnWidthToFit(false);
				}
				else if (ChildrenWidth == ChildrenSize.SetPreferredAndFitContainer)
				{
					ResizeColumnWidthToFit(true);
				}
				else if (ChildrenWidth == ChildrenSize.ShrinkOnOverflow)
				{
					ShrinkColumnWidthToFit();
				}

				if (ChildrenHeight == ChildrenSize.FitContainer)
				{
					ResizeHeightToFit(false);
				}
				else if (ChildrenHeight == ChildrenSize.SetPreferredAndFitContainer)
				{
					ResizeHeightToFit(true);
				}
				else if (ChildrenHeight == ChildrenSize.ShrinkOnOverflow)
				{
					ShrinkHeightToFit();
				}
			}
		}

		void ResizeColumnWidthToFit(bool increaseOnly)
		{
			ResizeToFit(InternalSize.x, ElementsGroup, Spacing.x, RectTransform.Axis.Horizontal, increaseOnly);
		}

		void ShrinkColumnWidthToFit()
		{
			ShrinkToFit(InternalSize.x, ElementsGroup, Spacing.x, RectTransform.Axis.Horizontal);
		}

		void ResizeHeightToFit(bool increaseOnly)
		{
			var height = InternalSize.y;
			for (int column = 0; column < ElementsGroup.Columns; column++)
			{
				ResizeToFit(height, ElementsGroup.GetColumn(column), Spacing.y, RectTransform.Axis.Vertical, increaseOnly);
			}
		}

		void ShrinkHeightToFit()
		{
			var height = InternalSize.y;
			for (int column = 0; column < ElementsGroup.Columns; column++)
			{
				ShrinkToFit(height, ElementsGroup.GetColumn(column), Spacing.y, RectTransform.Axis.Vertical);
			}
		}

		/// <summary>
		/// Convert input to match direction.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <returns>Converted input.</returns>
		protected Vector2 ByAxis(Vector2 input)
		{
			return IsHorizontal ? input : new Vector2(input.y, input.x);
		}

		/// <summary>
		/// Calculate group size.
		/// </summary>
		/// <returns>Size.</returns>
		protected override GroupSize CalculateGroupSize()
		{
			var spacing = ByAxis(Spacing);
			var padding = ByAxis(new Vector2(PaddingInner.Horizontal, PaddingInner.Vertical));

			return CalculateGroupSize(IsHorizontal, spacing, padding);
		}

		/// <summary>
		/// Calculate positions of the elements.
		/// </summary>
		/// <param name="size">Size.</param>
		protected override void CalculatePositions(Vector2 size)
		{
			var sub_axis = SubAxisData();
			var axis_direction = IsHorizontal ? -1f : 1f;

			var sub_position = sub_axis.Offset * axis_direction;
			var count = IsHorizontal ? ElementsGroup.Rows : ElementsGroup.Columns;
			for (int i = 0; i < count; i++)
			{
				var sub_size = IsHorizontal ? SubAxisSizes[i].Height : SubAxisSizes[i].Width;
				CalculatePositions(i, sub_position, sub_size);

				sub_position += (sub_size + sub_axis.Spacing) * axis_direction;
			}
		}

		/// <summary>
		/// Calculate sizes of the blocks at sub axis.
		/// </summary>
		protected void CalculateSubAxisSizes()
		{
			SubAxisSizes.Clear();

			var count = IsHorizontal ? ElementsGroup.Rows : ElementsGroup.Columns;
			for (int i = 0; i < count; i++)
			{
				var block = IsHorizontal ? ElementsGroup.GetRow(i) : ElementsGroup.GetColumn(i);
				var block_size = default(GroupSize);

				foreach (var element in block)
				{
					block_size.Max(element);
				}

				SubAxisSizes.Add(block_size);
			}
		}

		/// <summary>
		/// Get axis data.
		/// </summary>
		/// <param name="isMainAxis">Is main axis?</param>
		/// <param name="elements">Elements count.</param>
		/// <param name="size">Total size of the elements.</param>
		/// <returns>Axis data</returns>
		protected virtual AxisData GetAxisData(bool isMainAxis, int elements, float size)
		{
			var axis = new AxisData()
			{
				Offset = BaseOffset(isMainAxis),
				Spacing = isMainAxis ? ByAxis(Spacing).x : ByAxis(Spacing).y,
			};

			return axis;
		}

		/// <summary>
		/// Calculate base offset.
		/// </summary>
		/// <param name="isMainAxis">Is main axis?</param>
		/// <returns>Base offset.</returns>
		protected float BaseOffset(bool isMainAxis)
		{
			return IsHorizontal == isMainAxis
				? (Target.rect.width * (-Target.pivot.x)) + MarginLeft
				: (Target.rect.height * (Target.pivot.y - 1f)) + MarginTop;
		}

		/// <summary>
		/// Get sub axis data.
		/// </summary>
		/// <returns>Sub axis data.</returns>
		protected virtual AxisData SubAxisData()
		{
			CalculateSubAxisSizes();

			var size = Sum(SubAxisSizes);
			return GetAxisData(false, SubAxisSizes.Count, IsHorizontal ? size.Height : size.Width);
		}

		/// <summary>
		/// Get main axis data for the block with the specified index.
		/// </summary>
		/// <param name="blockIndex">Block index.</param>
		/// <returns>Main axis data.</returns>
		protected virtual AxisData MainAxisData(int blockIndex)
		{
			var block = IsHorizontal ? ElementsGroup.GetRow(blockIndex) : ElementsGroup.GetColumn(blockIndex);
			var size = 0f;

			foreach (var element in block)
			{
				size += element.AxisSize;
			}

			return GetAxisData(true, block.Count, size);
		}

		/// <summary>
		/// Calculate positions.
		/// </summary>
		/// <param name="blockIndex">Index of the block to calculate positions.</param>
		/// <param name="subAxisOffset">Offset on the sub axis.</param>
		/// <param name="maxSubSize">Maximum size of the sub axis.</param>
		protected void CalculatePositions(int blockIndex, float subAxisOffset, float maxSubSize)
		{
			var block = IsHorizontal ? ElementsGroup.GetRow(blockIndex) : ElementsGroup.GetColumn(blockIndex);

			var axis_direction = IsHorizontal ? 1f : -1f;
			var axis = MainAxisData(blockIndex);

			var position = new Vector2(axis.Offset, subAxisOffset);
			var sub_offset_rate = GetItemsAlignRate();
			foreach (var element in block)
			{
				var sub_axis = subAxisOffset - ((maxSubSize - element.SubAxisSize) * sub_offset_rate * axis_direction);

				element.PositionTopLeft = ByAxis(new Vector2(position.x * axis_direction, sub_axis));
				position.x += element.AxisSize + axis.Spacing;
			}
		}

		/// <summary>
		/// Get align rate for the items.
		/// </summary>
		/// <returns>Align rate.</returns>
		protected virtual float GetItemsAlignRate()
		{
			return 0f;
		}
	}
}