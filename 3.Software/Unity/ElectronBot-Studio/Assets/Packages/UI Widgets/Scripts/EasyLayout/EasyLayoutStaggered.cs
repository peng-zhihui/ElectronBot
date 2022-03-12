namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Easy layout staggered layout.
	/// </summary>
	public class EasyLayoutStaggered : EasyLayoutFlexOrStaggered
	{
		readonly List<GroupSize> BlocksSizes = new List<GroupSize>();

		/// <summary>
		/// Settings.
		/// </summary>
		protected EasyLayoutStaggeredSettings Settings;

		/// <inheritdoc/>
		public override void LoadSettings(EasyLayout layout)
		{
			base.LoadSettings(layout);

			Settings = layout.StaggeredSettings;
		}

		int GetBlockSizes()
		{
			if (ElementsGroup.Count == 0)
			{
				return 0;
			}

			if (Settings.FixedBlocksCount)
			{
				return Mathf.Max(1, Settings.BlocksCount);
			}

			var blocks = 1;

			var size = SubAxisSize - ElementsGroup[0].SubAxisSize;
			var spacing = !IsHorizontal ? Spacing.x : Spacing.y;

			foreach (var element in ElementsGroup.Elements)
			{
				size -= element.SubAxisSize + spacing;
				if (size < 0f)
				{
					break;
				}

				blocks += 1;
			}

			return blocks;
		}

		void InitBlockSizes(int blocks)
		{
			BlocksSizes.Clear();

			EnsureListSize(BlocksSizes, blocks);
			EnsureListSize(Settings.PaddingInnerStart, blocks);
			EnsureListSize(Settings.PaddingInnerEnd, blocks);

			var padding = Settings.PaddingInnerStart;
			for (int i = 0; i < blocks; i++)
			{
				var p = IsHorizontal ? new Vector2(padding[i], 0) : new Vector2(0, padding[i]);
				BlocksSizes[i] += p;
			}
		}

		static void EnsureListSize<T>(List<T> list, int size)
		{
			for (int i = list.Count; i < size; i++)
			{
				list.Add(default(T));
			}
		}

		int NextBlockIndex()
		{
			var index = 0;
			var min_size = BlocksSizes[0];

			for (int i = 1; i < BlocksSizes.Count; i++)
			{
				var size = BlocksSizes[i];
				var is_lower = IsHorizontal ? size.Width < min_size.Width : size.Height < min_size.Height;
				if (is_lower)
				{
					index = i;
					min_size = size;
				}
			}

			return index;
		}

		void InsertToBlock(int block_index, LayoutElementInfo element)
		{
			var block = IsHorizontal
				? ElementsGroup.GetRow(block_index)
				: ElementsGroup.GetColumn(block_index);
			if (IsHorizontal)
			{
				ElementsGroup.SetPosition(element, block_index, block.Count);
			}
			else
			{
				ElementsGroup.SetPosition(element, block.Count, block_index);
			}

			BlocksSizes[block_index] += element;
			if (block.Count > 0)
			{
				BlocksSizes[block_index] += Spacing;
			}
		}

		/// <summary>
		/// Group elements.
		/// </summary>
		protected override void Group()
		{
			var blocks = GetBlockSizes();
			InitBlockSizes(blocks);

			foreach (var element in ElementsGroup.Elements)
			{
				InsertToBlock(NextBlockIndex(), element);
			}

			if (!TopToBottom)
			{
				ElementsGroup.BottomToTop();
			}

			if (RightToLeft)
			{
				ElementsGroup.RightToLeft();
			}
		}

		/// <summary>
		/// Calculate size of the group.
		/// </summary>
		/// <param name="isHorizontal">ElementsGroup are in horizontal order?</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="padding">Padding,</param>
		/// <returns>Size.</returns>
		protected override GroupSize CalculateGroupSize(bool isHorizontal, Vector2 spacing, Vector2 padding)
		{
			var main_axis_size = default(GroupSize);

			var padding_end = Settings.PaddingInnerEnd;

			for (int i = 0; i < BlocksSizes.Count; i++)
			{
				var p = IsHorizontal ? new Vector2(padding_end[i], 0) : new Vector2(0, padding_end[i]);
				main_axis_size.Max(BlocksSizes[i] + p);
			}

			CalculateSubAxisSizes();
			var sub_spacing = !IsHorizontal ? Spacing.x : Spacing.y;

			var sub_size = (SubAxisSizes.Count - 1) * sub_spacing;

			var sub_axis_size = IsHorizontal ? new GroupSize(0, sub_size) : new GroupSize(sub_size, 0);
			sub_axis_size += Sum(SubAxisSizes);

			return IsHorizontal
				? new GroupSize(main_axis_size, sub_axis_size)
				: new GroupSize(sub_axis_size, main_axis_size);
		}

		/// <summary>
		/// Get main axis data.
		/// </summary>
		/// <param name="blockIndex">Block index.</param>
		/// <returns>Main axis data.</returns>
		protected override AxisData MainAxisData(int blockIndex)
		{
			var axis = base.MainAxisData(blockIndex);
			axis.Offset += Settings.PaddingInnerStart[blockIndex];

			return axis;
		}
	}
}