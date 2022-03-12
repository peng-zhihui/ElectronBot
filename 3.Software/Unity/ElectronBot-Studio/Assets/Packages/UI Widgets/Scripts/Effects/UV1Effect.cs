namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// UV1 effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	public abstract class UV1Effect : BaseMeshEffect, IMaterialModifier, IMeshModifier
	{
		/// <summary>
		/// UV mode.
		/// </summary>
		public enum UVMode
		{
			/// <summary>
			/// UV1.y limited by width.
			/// x in [0, 1]; y in [0, 1 / (width / height)]
			/// </summary>
			X = 0,

			/// <summary>
			/// UV1.x limited by height.
			/// x in [0, 1 / (height / width)]; y in [0, 1]
			/// </summary>
			Y = 1,

			/// <summary>
			/// UV1.x is not limited by size.
			/// x in [0, 1]; y in [0, 1]
			/// </summary>
			One = 2,

			/// <summary>
			/// If width more that height then works as X mode; otherwise Y mode.
			/// </summary>
			Max = 3,
		}

		/// <summary>
		/// Shader.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("RippleShader")]
		protected Shader EffectShader;

		/// <summary>
		/// Base material.
		/// </summary>
		[NonSerialized]
		protected Material BaseMaterial;

		/// <summary>
		/// Ring material.
		/// </summary>
		[NonSerialized]
		protected Material EffectMaterial;

		[NonSerialized]
		RectTransform rectTransform;

		/// <summary>
		/// RectTransform component.
		/// </summary>
		protected RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		UVMode mode = UVMode.X;

		/// <summary>
		/// If enabled set UV1.y is limited by horizontal size.
		/// </summary>
		protected UVMode Mode
		{
			get
			{
				return mode;
			}

			set
			{
				if (mode != value)
				{
					mode = value;
					graphic.SetVerticesDirty();
				}
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (graphic != null)
			{
				graphic.SetVerticesDirty();
			}
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			if (graphic != null)
			{
				graphic.SetMaterialDirty();
			}

			base.OnDisable();
		}

		/// <summary>
		/// Process the animation event.
		/// </summary>
		protected override void OnDidApplyAnimationProperties()
		{
			if (graphic != null)
			{
				graphic.SetMaterialDirty();
			}

			base.OnDidApplyAnimationProperties();
		}

		#if UNITY_EDITOR

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (graphic != null)
			{
				UpdateMaterial();
			}
		}

		#endif

		/// <summary>
		/// Process the dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			UpdateMaterial();
		}

		/// <summary>
		/// Add uv1 to the mesh.
		/// </summary>
		/// <param name="vh">Vertex helper.</param>
		public override void ModifyMesh(VertexHelper vh)
		{
			var size = RectTransform.rect.size;
			if ((size.x < 0f) || (size.y < 0f))
			{
				return;
			}

			var vertex = default(UIVertex);

			// get min and max position to calculate uv1
			vh.PopulateUIVertex(ref vertex, 0);
			var min_x = vertex.position.x;
			var max_x = min_x;
			var min_y = vertex.position.y;
			var max_y = min_y;

			for (int i = 1; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				min_x = Math.Min(min_x, vertex.position.x);
				max_x = Math.Max(max_x, vertex.position.x);

				min_y = Math.Min(min_y, vertex.position.y);
				max_y = Math.Max(max_y, vertex.position.y);
			}

			// set uv1 for the shader
			var width = max_x - min_x;
			var height = max_y - min_y;

			Vector2 aspect_ratio;

			switch (Mode)
			{
				case UVMode.X:
					aspect_ratio = new Vector2(1f, size.x / size.y);
					break;
				case UVMode.Y:
					aspect_ratio = new Vector2(size.y / size.x, 1f);
					break;
				case UVMode.One:
					aspect_ratio = Vector2.one;
					break;
				case UVMode.Max:
					aspect_ratio = (size.x >= size.y)
						? new Vector2(1f, size.x / size.y)
						: new Vector2(size.y / size.x, 1f);
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown UVMode: {0}", EnumHelper<UVMode>.ToString(Mode)));
			}

			for (int i = 0; i < vh.currentVertCount; i++)
			{
				vh.PopulateUIVertex(ref vertex, i);

				vertex.uv1 = new Vector2(
					(vertex.position.x - min_x) / width / aspect_ratio.x,
					(vertex.position.y - min_y) / height / aspect_ratio.y);

				vh.SetUIVertex(vertex, i);
			}
		}

		/// <summary>
		/// Init material.
		/// </summary>
		protected virtual void InitMaterial()
		{
			SetMaterialProperties();
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected abstract void SetMaterialProperties();

		/// <summary>
		/// Update material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			SetMaterialProperties();

			if (EffectMaterial != null)
			{
				graphic.SetMaterialDirty();
			}
		}

		/// <summary>
		/// Get modified material.
		/// </summary>
		/// <param name="newBaseMaterial">Base material.</param>
		/// <returns>Modified material.</returns>
		public virtual Material GetModifiedMaterial(Material newBaseMaterial)
		{
			if ((BaseMaterial != null) && (newBaseMaterial.GetInstanceID() == BaseMaterial.GetInstanceID()))
			{
				return EffectMaterial;
			}

			if (EffectMaterial != null)
			{
#if UNITY_EDITOR
				DestroyImmediate(EffectMaterial);
#else
				Destroy(EffectMaterial);
#endif
			}

			BaseMaterial = newBaseMaterial;
			EffectMaterial = new Material(newBaseMaterial)
			{
				shader = EffectShader,
			};
			InitMaterial();

			return EffectMaterial;
		}
	}
}