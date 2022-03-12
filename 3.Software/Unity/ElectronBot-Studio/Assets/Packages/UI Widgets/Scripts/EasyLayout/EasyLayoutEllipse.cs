namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using EasyLayoutNS.Extensions;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Ellipse layout group.
	/// </summary>
	public class EasyLayoutEllipse : EasyLayoutBaseType
	{
		readonly List<LayoutElementInfo> EllipseGroup = new List<LayoutElementInfo>();

		/// <summary>
		/// Settings.
		/// </summary>
		protected EasyLayoutEllipseSettings Settings;

		/// <inheritdoc/>
		public override void LoadSettings(EasyLayout layout)
		{
			base.LoadSettings(layout);

			Settings = layout.EllipseSettings;
		}

		/// <summary>
		/// Get element position in the group.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <returns>Position.</returns>
		public override EasyLayoutPosition GetElementPosition(RectTransform element)
		{
			var target_id = element.GetInstanceID();
			for (int x = 0; x < EllipseGroup.Count; x++)
			{
				if (EllipseGroup[x].Rect.GetInstanceID() == target_id)
				{
					return new EasyLayoutPosition(x, 0);
				}
			}

			return new EasyLayoutPosition(-1, -1);
		}

		/// <summary>
		/// Calculate group size.
		/// </summary>
		/// <returns>Size.</returns>
		protected override GroupSize CalculateGroupSize()
		{
			var size = InternalSize;

			if (!Settings.WidthAuto)
			{
				size.x = Settings.Width;
			}

			if (!Settings.HeightAuto)
			{
				size.y = Settings.Height;
			}

			var max_size = FindElementsMaxSize();
			var align = GetAlignRate(Settings.Align) * 2f;

			if (Settings.ElementsRotate)
			{
				var rotation_angle = Mathf.Deg2Rad * Settings.ElementsRotationStart;
				var rotation_sin = Math.Sin(rotation_angle);
				var rotation_cos = Math.Cos(rotation_angle);
				var length = (max_size.x * rotation_cos) + (max_size.y * rotation_sin);
				size.x -= Mathf.Abs((float)length) * (align + 1f);
				size.y -= Mathf.Abs((float)length) * (align + 1f);
			}
			else
			{
				size.x -= max_size.x * (align + 1f);
				size.y -= max_size.y * (align + 1f);
			}

			size.x = Mathf.Max(1f, size.x);
			size.y = Mathf.Max(1f, size.y);

			return new GroupSize(size);
		}

		Vector2 FindElementsMaxSize()
		{
			var max_size = Vector2.zero;

			foreach (var element in ElementsGroup.Elements)
			{
				max_size.x = Mathf.Max(max_size.x, element.Width);
				max_size.y = Mathf.Max(max_size.y, element.Height);
			}

			return max_size;
		}

		/// <summary>
		/// Calculate positions of the elements.
		/// </summary>
		/// <param name="size">Size.</param>
		protected override void CalculatePositions(Vector2 size)
		{
			if (EllipseGroup.Count == 0)
			{
				return;
			}

			var angle_auto = Settings.Fill == EllipseFill.Closed
				? 360f / EllipseGroup.Count
				: Settings.ArcLength / Mathf.Max(1, EllipseGroup.Count - 1);
			var angle_step = Settings.AngleStepAuto ? angle_auto : Settings.AngleStep;

			var angle = Settings.AngleStart + Settings.AngleFiller + Settings.AngleScroll;

			var center = new Vector2(size.x / 2.0f, size.y / 2.0f);
			var align = GetAlignRate(Settings.Align);

			var rotation_angle_rad = Mathf.Deg2Rad * Settings.ElementsRotationStart;
			var rotation_sin = (float)Math.Sin(rotation_angle_rad);
			var rotation_cos = (float)Math.Cos(rotation_angle_rad);

			var pivot = new Vector2(0.5f, 0.5f);
			for (int i = 0; i < EllipseGroup.Count; i++)
			{
				var element = EllipseGroup[i];

				element.NewPivot = pivot;

				var position_angle_rad = Mathf.Deg2Rad * angle;

				var position_sin = Mathf.Sin(position_angle_rad);
				var position_cos = Mathf.Cos(position_angle_rad);

				var element_pos = new Vector2(center.x * position_cos, center.y * position_sin);

				if (Settings.ElementsRotate)
				{
					var length = Mathf.Abs(element.Width * rotation_cos) + Mathf.Abs(element.Height * rotation_sin);
					var align_fix = new Vector2(
						length * position_cos * align,
						length * position_sin * align);
					element_pos += align_fix;
				}
				else
				{
					var align_fix = new Vector2(
						element.Width * position_cos * align,
						element.Height * position_sin * align);
					element_pos += align_fix;
				}

				element.NewEulerAnglesZ = Settings.ElementsRotate ? (angle + Settings.ElementsRotationStart) : 0f;

				element.PositionPivot = element_pos;

				angle -= angle_step;
			}
		}

		static float GetAlignRate(EllipseAlign align)
		{
			switch (align)
			{
				case EllipseAlign.Outer:
					return -0.5f;
				case EllipseAlign.Center:
					return 0f;
				case EllipseAlign.Inner:
					return 0.5f;
				default:
					Debug.LogWarning(string.Format("Unknown ellipse align: {0}", EnumHelper<EllipseAlign>.ToString(align)));
					break;
			}

			return 0f;
		}

		/// <summary>
		/// Calculate sizes of the elements.
		/// </summary>
		protected override void CalculateSizes()
		{
		}

		/// <summary>
		/// Group elements.
		/// </summary>
		protected override void Group()
		{
			EllipseGroup.Clear();
			EllipseGroup.AddRange(ElementsGroup.Elements);

			if (RightToLeft)
			{
				EllipseGroup.Reverse();
			}
		}

		/// <inheritdoc/>
		protected override void SetElementProperties(LayoutElementInfo element, ResizeType resizeType)
		{
			var driven_properties = DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.AnchoredPositionZ;

			if (ChildrenWidth != ChildrenSize.DoNothing)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaX;

				if (resizeType.IsSet(ResizeType.Horizontal))
				{
					element.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, element.NewWidth);
				}
			}

			if (ChildrenHeight != ChildrenSize.DoNothing)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaY;

				if (resizeType.IsSet(ResizeType.Vertical))
				{
					element.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, element.NewHeight);
				}
			}

			driven_properties |= DrivenTransformProperties.Rotation;
			element.Rect.localEulerAngles = element.NewEulerAngles;

			driven_properties |= DrivenTransformProperties.Pivot;
			element.Rect.pivot = element.NewPivot;

			OnElementChangedInvoke(element.Rect, driven_properties);
		}
	}
}