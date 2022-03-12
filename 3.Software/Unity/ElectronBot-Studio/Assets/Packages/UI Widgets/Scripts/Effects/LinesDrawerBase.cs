namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the lines drawer.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	public abstract class LinesDrawerBase : UV1Effect
	{
		/// <summary>
		/// IDs of shader properties.
		/// </summary>
		protected struct SnapGridShaderIDs : IEquatable<SnapGridShaderIDs>
		{
			/// <summary>
			/// Line color ID.
			/// </summary>
			public readonly int LineColor;

			/// <summary>
			/// Line thickness ID.
			/// </summary>
			public readonly int LineThickness;

			/// <summary>
			/// Resolution X ID.
			/// </summary>
			public readonly int ResolutionX;

			/// <summary>
			/// Resolution Y ID.
			/// </summary>
			public readonly int ResolutionY;

			/// <summary>
			/// Horizontal lines count ID.
			/// </summary>
			public readonly int HorizontalLinesCount;

			/// <summary>
			/// Horizontal lines ID.
			/// </summary>
			public readonly int HorizontalLines;

			/// <summary>
			/// Vertical lines count ID.
			/// </summary>
			public readonly int VerticalLinesCount;

			/// <summary>
			/// Vertical lines ID.
			/// </summary>
			public readonly int VerticalLines;

			/// <summary>
			/// Transparent ID.
			/// </summary>
			public readonly int Transparent;

			private SnapGridShaderIDs(
				int lineColor,
				int lineThickness,
				int resolutionX,
				int resolutionY,
				int horizontalLinesCount,
				int horizontalLines,
				int verticalLinesCount,
				int verticalLines,
				int transparent)
			{
				LineColor = lineColor;
				LineThickness = lineThickness;
				ResolutionX = resolutionX;
				ResolutionY = resolutionY;
				HorizontalLinesCount = horizontalLinesCount;
				HorizontalLines = horizontalLines;
				VerticalLinesCount = verticalLinesCount;
				VerticalLines = verticalLines;
				Transparent = transparent;
			}

			/// <summary>
			/// Get RingShaderIDs instance.
			/// </summary>
			public static SnapGridShaderIDs Instance
			{
				get
				{
					return new SnapGridShaderIDs(
						Shader.PropertyToID("_LineColor"),
						Shader.PropertyToID("_LineThickness"),
						Shader.PropertyToID("_ResolutionX"),
						Shader.PropertyToID("_ResolutionY"),
						Shader.PropertyToID("_HLinesCount"),
						Shader.PropertyToID("_HLines"),
						Shader.PropertyToID("_VLinesCount"),
						Shader.PropertyToID("_VLines"),
						Shader.PropertyToID("_Transparent"));
				}
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is SnapGridShaderIDs)
				{
					return Equals((SnapGridShaderIDs)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(SnapGridShaderIDs other)
			{
				return LineColor == other.LineColor
					&& LineThickness == other.LineThickness
					&& ResolutionX == other.ResolutionX
					&& ResolutionY == other.ResolutionY
					&& HorizontalLinesCount == other.HorizontalLinesCount
					&& HorizontalLines == other.HorizontalLines
					&& VerticalLinesCount == other.VerticalLinesCount
					&& VerticalLines == other.VerticalLines
					&& Transparent == other.Transparent;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return LineColor ^ LineThickness ^ ResolutionX ^ ResolutionY ^ HorizontalLinesCount ^ HorizontalLines ^ VerticalLinesCount ^ VerticalLines ^ Transparent;
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(SnapGridShaderIDs left, SnapGridShaderIDs right)
			{
				return left.Equals(right);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(SnapGridShaderIDs left, SnapGridShaderIDs right)
			{
				return !left.Equals(right);
			}
		}

		[SerializeField]
		Color lineColor = Color.black;

		/// <summary>
		/// Line color.
		/// </summary>
		public Color LineColor
		{
			get
			{
				return lineColor;
			}

			set
			{
				if (lineColor != value)
				{
					lineColor = value;
					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		float lineThickness = 1f;

		/// <summary>
		/// Line thickness.
		/// </summary>
		public float LineThickness
		{
			get
			{
				return lineThickness;
			}

			set
			{
				if (lineThickness != value)
				{
					lineThickness = value;
					UpdateMaterial();
				}
			}
		}

		[SerializeField]
		[Tooltip("Make the background transparent.")]
		bool transparentBackground = false;

		/// <summary>
		/// Make the background transparent.
		/// </summary>
		public bool TransparentBackground
		{
			get
			{
				return transparentBackground;
			}

			set
			{
				if (transparentBackground != value)
				{
					transparentBackground = value;
					UpdateMaterial();
				}
			}
		}

		/// <summary>
		/// Horizontal lines.
		/// </summary>
		protected List<float> HorizontalLines = new List<float>();

		/// <summary>
		/// Vertical lines.
		/// </summary>
		protected List<float> VerticalLines = new List<float>();

		/// <summary>
		/// Shader horizontal lines.
		/// </summary>
		protected List<float> ShaderHorizontalLines = new List<float>(200);

		/// <summary>
		/// Shader vertical lines.
		/// </summary>
		protected List<float> ShaderVerticalLines = new List<float>(200);

		/// <summary>
		/// Max lines count. Should match with shader setting.
		/// </summary>
		protected int MaxLinesCount = 200;

		/// <summary>
		/// Ring shader ids.
		/// </summary>
		protected static SnapGridShaderIDs ShaderIDs = SnapGridShaderIDs.Instance;

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			base.OnEnable();

			Mode = UVMode.One;

			UpdateLines();
		}

		/// <summary>
		/// Update lines.
		/// </summary>
		protected virtual void UpdateLines()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected override void SetMaterialProperties()
		{
			if (EffectMaterial != null)
			{
				var size = RectTransform.rect.size;

				UpdateShaderLines(HorizontalLines, ShaderHorizontalLines);
				UpdateShaderLines(VerticalLines, ShaderVerticalLines);

				EffectMaterial.SetColor(ShaderIDs.LineColor, lineColor);
				EffectMaterial.SetFloat(ShaderIDs.LineThickness, lineThickness);
				EffectMaterial.SetFloat(ShaderIDs.ResolutionX, size.x);
				EffectMaterial.SetFloat(ShaderIDs.ResolutionY, size.y);
				EffectMaterial.SetFloat(ShaderIDs.Transparent, transparentBackground ? 1 : 0);

				EffectMaterial.SetInt(ShaderIDs.HorizontalLinesCount, HorizontalLines.Count);
				EffectMaterial.SetFloatArray(ShaderIDs.HorizontalLines, ShaderHorizontalLines);

				EffectMaterial.SetInt(ShaderIDs.VerticalLinesCount, VerticalLines.Count);
				EffectMaterial.SetFloatArray(ShaderIDs.VerticalLines, ShaderVerticalLines);
			}
		}

		/// <summary>
		/// Update shader lines.
		/// </summary>
		/// <param name="lines">Lines.</param>
		/// <param name="shaderLines">Shader lines.</param>
		protected void UpdateShaderLines(List<float> lines, List<float> shaderLines)
		{
			shaderLines.Clear();
			shaderLines.AddRange(lines);

			for (int i = shaderLines.Count; i < MaxLinesCount; i++)
			{
				shaderLines.Add(0);
			}
		}
	}
}