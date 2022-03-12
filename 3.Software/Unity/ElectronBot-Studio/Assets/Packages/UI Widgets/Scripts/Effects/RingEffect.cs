namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Ring effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Ring Effect")]
	public class RingEffect : UV1Effect
	{
		/// <summary>
		/// IDs of ring shader properties.
		/// </summary>
		protected struct RingShaderIDs : IEquatable<RingShaderIDs>
		{
			/// <summary>
			/// Ring color ID.
			/// </summary>
			public readonly int RingColor;

			/// <summary>
			/// Thickness ID.
			/// </summary>
			public readonly int Thickness;

			/// <summary>
			/// Padding ID.
			/// </summary>
			public readonly int Padding;

			/// <summary>
			/// Resolution ID.
			/// </summary>
			public readonly int Resolution;

			/// <summary>
			/// Transparent ID.
			/// </summary>
			public readonly int Transparent;

			private RingShaderIDs(int ringColor, int thickness, int padding, int resolution, int transparent)
			{
				RingColor = ringColor;
				Thickness = thickness;
				Padding = padding;
				Resolution = resolution;
				Transparent = transparent;
			}

			/// <summary>
			/// Get RingShaderIDs instance.
			/// </summary>
			public static RingShaderIDs Instance
			{
				get
				{
					return new RingShaderIDs(
						Shader.PropertyToID("_RingColor"),
						Shader.PropertyToID("_Thickness"),
						Shader.PropertyToID("_Padding"),
						Shader.PropertyToID("_Resolution"),
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
				if (obj is RingShaderIDs)
				{
					return Equals((RingShaderIDs)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(RingShaderIDs other)
			{
				return RingColor == other.RingColor
					&& Thickness == other.Thickness
					&& Padding == other.Padding
					&& Resolution == other.Resolution
					&& Transparent == other.Transparent;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return RingColor ^ Thickness ^ Padding ^ Resolution ^ Transparent;
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(RingShaderIDs left, RingShaderIDs right)
			{
				return left.Equals(right);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(RingShaderIDs left, RingShaderIDs right)
			{
				return !left.Equals(right);
			}
		}

		[SerializeField]
		Color ringColor = Color.white;

		/// <summary>
		/// Ring color.
		/// </summary>
		public Color RingColor
		{
			get
			{
				return ringColor;
			}

			set
			{
				ringColor = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		float thickness = 10f;

		/// <summary>
		/// Thickness.
		/// </summary>
		public float Thickness
		{
			get
			{
				return thickness;
			}

			set
			{
				thickness = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		float padding = 0f;

		/// <summary>
		/// Padding.
		/// </summary>
		public float Padding
		{
			get
			{
				return padding;
			}

			set
			{
				padding = value;
				UpdateMaterial();
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
				transparentBackground = value;
				UpdateMaterial();
			}
		}

		/// <summary>
		/// Ring shader ids.
		/// </summary>
		protected static RingShaderIDs ShaderIDs = RingShaderIDs.Instance;

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected override void SetMaterialProperties()
		{
			if (EffectMaterial != null)
			{
				var size = RectTransform.rect.size;
				var resolution = Mathf.Max(size.x, size.y);

				EffectMaterial.SetColor(ShaderIDs.RingColor, ringColor);
				EffectMaterial.SetFloat(ShaderIDs.Thickness, thickness);
				EffectMaterial.SetFloat(ShaderIDs.Padding, padding);
				EffectMaterial.SetFloat(ShaderIDs.Resolution, resolution);
				EffectMaterial.SetFloat(ShaderIDs.Transparent, transparentBackground ? 1 : 0);
			}
		}
	}
}