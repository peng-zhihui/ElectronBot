namespace EasyLayoutNS
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Flexbox-like layout group.
	/// </summary>
	public class EasyLayoutFlex : EasyLayoutFlexOrStaggered
	{
		/// <summary>
		/// Settings.
		/// </summary>
		protected EasyLayoutFlexSettings Settings;

		/// <inheritdoc/>
		public override void LoadSettings(EasyLayout layout)
		{
			base.LoadSettings(layout);

			Settings = layout.FlexSettings;
		}

		/// <summary>
		/// Group elements.
		/// </summary>
		protected override void Group()
		{
			if (!Settings.Wrap)
			{
				for (int i = 0; i < ElementsGroup.Count; i++)
				{
					ElementsGroup.SetPosition(i, IsHorizontal ? 0 : i, IsHorizontal ? i : 0);
				}

				return;
			}

			var base_size = MainAxisSize;
			var size = base_size;
			var spacing = IsHorizontal ? Spacing.x : Spacing.y;

			int main_axis_index = 0;
			int sub_axis_index = 0;

			for (int i = 0; i < ElementsGroup.Count; i++)
			{
				var element = ElementsGroup[i];
				if (sub_axis_index == 0)
				{
					size -= element.AxisSize;
				}
				else if (size >= (element.AxisSize + spacing))
				{
					size -= element.AxisSize + spacing;
				}
				else
				{
					size = base_size - element.AxisSize;

					main_axis_index++;
					sub_axis_index = 0;
				}

				if (IsHorizontal)
				{
					ElementsGroup.SetPosition(i, main_axis_index, sub_axis_index);
				}
				else
				{
					ElementsGroup.SetPosition(i, sub_axis_index, main_axis_index);
				}

				sub_axis_index++;
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
			var size = base.CalculateGroupSize(isHorizontal, spacing, padding);

			var internal_size = ByAxis(InternalSize);

			// add is_horizontal check
			var stretch_main_axis = (Settings.JustifyContent == EasyLayoutFlexSettings.Content.SpaceAround)
				|| (Settings.JustifyContent == EasyLayoutFlexSettings.Content.SpaceBetween)
				|| (Settings.JustifyContent == EasyLayoutFlexSettings.Content.SpaceEvenly);
			if (stretch_main_axis)
			{
				if (isHorizontal)
				{
					size.Width = internal_size.x;
				}
				else
				{
					size.Height = internal_size.x;
				}
			}

			var stretch_sub_axis = (Settings.AlignContent == EasyLayoutFlexSettings.Content.SpaceAround)
				|| (Settings.AlignContent == EasyLayoutFlexSettings.Content.SpaceBetween)
				|| (Settings.AlignContent == EasyLayoutFlexSettings.Content.SpaceEvenly);
			if (stretch_sub_axis)
			{
				if (isHorizontal)
				{
					size.Height = internal_size.y;
				}
				else
				{
					size.Width = internal_size.y;
				}
			}

			return size;
		}

		/// <summary>
		/// Get axis data.
		/// </summary>
		/// <param name="isMainAxis">Is main axis?</param>
		/// <param name="elements">Elements count.</param>
		/// <param name="size">Total size of the elements.</param>
		/// <returns>Axis data.</returns>
		protected override AxisData GetAxisData(bool isMainAxis, int elements, float size)
		{
			var axis = base.GetAxisData(isMainAxis, elements, size);

			var outer_size = isMainAxis ? ByAxis(InternalSize).x : ByAxis(InternalSize).y;
			var align = isMainAxis ? Settings.JustifyContent : Settings.AlignContent;

			if (align == EasyLayoutFlexSettings.Content.End)
			{
				axis.Offset += outer_size - (size + ((elements - 1) * axis.Spacing));
			}
			else if (align == EasyLayoutFlexSettings.Content.Center)
			{
				axis.Offset += (outer_size - (size + ((elements - 1) * axis.Spacing))) / 2f;
			}
			else if (align == EasyLayoutFlexSettings.Content.SpaceAround)
			{
				var space = (outer_size - size) / (elements * 2);
				axis.Offset += space;
				axis.Spacing = space * 2;
			}
			else if (align == EasyLayoutFlexSettings.Content.SpaceBetween)
			{
				if (elements > 1)
				{
					axis.Spacing = (outer_size - size) / (elements - 1);
				}
			}
			else if (align == EasyLayoutFlexSettings.Content.SpaceEvenly)
			{
				var space = (outer_size - size) / (elements + 1);
				axis.Offset += space;
				axis.Spacing = space;
			}

			return axis;
		}

		/// <summary>
		/// Get align rate for the items.
		/// </summary>
		/// <returns>Align rate.</returns>
		protected override float GetItemsAlignRate()
		{
			switch (Settings.AlignItems)
			{
				case EasyLayoutFlexSettings.Items.Start:
					return 0f;
				case EasyLayoutFlexSettings.Items.Center:
					return 0.5f;
				case EasyLayoutFlexSettings.Items.End:
					return 1f;
				default:
					Debug.LogWarning(string.Format("Unknown items align: {0}", EnumHelper<EasyLayoutFlexSettings.Items>.ToString(Settings.AlignItems)));
					break;
			}

			return 0f;
		}
	}
}