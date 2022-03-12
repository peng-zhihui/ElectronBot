namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Ripple effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Ripple Effect")]
	public class RippleEffect : UV1Effect, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IMaterialModifier, IMeshModifier
	{
		/// <summary>
		/// IDs of ripple shader properties.
		/// </summary>
		protected struct RippleShaderIDs : IEquatable<RippleShaderIDs>
		{
			/// <summary>
			/// Start color ID.
			/// </summary>
			public readonly int StartColor;

			/// <summary>
			/// End color ID.
			/// </summary>
			public readonly int EndColor;

			/// <summary>
			/// Speed ID.
			/// </summary>
			public readonly int Speed;

			/// <summary>
			/// Max size ID.
			/// </summary>
			public readonly int MaxSize;

			/// <summary>
			/// Count ID.
			/// </summary>
			public readonly int Count;

			/// <summary>
			/// Ripple ID.
			/// </summary>
			public readonly int Ripple;

			private RippleShaderIDs(int startColor, int endColor, int speed, int maxSize, int count, int ripple)
			{
				StartColor = startColor;
				EndColor = endColor;
				Speed = speed;
				MaxSize = maxSize;
				Count = count;
				Ripple = ripple;
			}

			/// <summary>
			/// Get RippleShaderIDs instance.
			/// </summary>
			public static RippleShaderIDs Instance
			{
				get
				{
					return new RippleShaderIDs(
						Shader.PropertyToID("_RippleStartColor"),
						Shader.PropertyToID("_RippleEndColor"),
						Shader.PropertyToID("_RippleSpeed"),
						Shader.PropertyToID("_RippleMaxSize"),
						Shader.PropertyToID("_RippleCount"),
						Shader.PropertyToID("_Ripple"));
				}
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is RippleShaderIDs)
				{
					return Equals((RippleShaderIDs)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(RippleShaderIDs other)
			{
				return StartColor == other.StartColor && EndColor == other.EndColor && Speed == other.Speed && MaxSize == other.MaxSize && Count == other.Count && Ripple == other.Ripple;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return StartColor ^ EndColor ^ Speed ^ MaxSize ^ Count ^ Ripple;
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(RippleShaderIDs left, RippleShaderIDs right)
			{
				return left.Equals(right);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="left">Left instance.</param>
			/// <param name="right">Right instances.</param>
			/// <returns>true if the instances are now equal; otherwise, false.</returns>
			public static bool operator !=(RippleShaderIDs left, RippleShaderIDs right)
			{
				return !left.Equals(right);
			}
		}

		[SerializeField]
		[FormerlySerializedAs("color")]
		Color startColor = Color.white;

		/// <summary>
		/// Start color of the ripple.
		/// </summary>
		public Color StartColor
		{
			get
			{
				return startColor;
			}

			set
			{
				startColor = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Color endColor = Color.white;

		/// <summary>
		/// End color of the ripple.
		/// </summary>
		public Color EndColor
		{
			get
			{
				return endColor;
			}

			set
			{
				endColor = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		float speed = 0.5f;

		/// <summary>
		/// Ripple speed.
		/// </summary>
		public float Speed
		{
			get
			{
				return speed;
			}

			set
			{
				speed = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		[Range(0f, 1f)]
		float maxSize = 1f;

		/// <summary>
		/// Ripple size.
		/// </summary>
		public float MaxSize
		{
			get
			{
				return maxSize;
			}

			set
			{
				maxSize = value;
				UpdateMaterial();
			}
		}

		/// <summary>
		/// Ripples data.
		/// </summary>
		[NonSerialized]
		protected List<float> RipplesData;

		/// <summary>
		/// Max ripples count.
		/// </summary>
		protected static int MaxRipples = 10;

		/// <summary>
		/// Float values per ripple.
		/// </summary>
		protected static int FloatPerRipple = 3;

		/// <summary>
		/// Ripple shader ids.
		/// </summary>
		protected static RippleShaderIDs ShaderIDs = RippleShaderIDs.Instance;

		/// <summary>
		/// Remove oldest and dead ripples.
		/// </summary>
		protected void CleanRipples()
		{
			// remove oldest ripple
			if (RipplesData.Count > ((MaxRipples - 1) * FloatPerRipple))
			{
				RipplesData.RemoveRange(0, FloatPerRipple);
			}

			// remove dead ripples
			var died = UtilitiesTime.GetTime(false) - (MaxSize / Speed);
			var start = RipplesData.Count - FloatPerRipple;
			for (var i = start; i >= 0; i -= FloatPerRipple)
			{
				if (RipplesData[i + 2] < died)
				{
					RipplesData.RemoveRange(i, FloatPerRipple);
				}
			}
		}

		/// <summary>
		/// Add ripple.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void AddRipple(PointerEventData eventData)
		{
			CleanRipples();

			var size = RectTransform.rect.size;
			var aspect_ratio = size.x / size.y;

			var pivot = RectTransform.pivot;

			Vector2 current_point;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out current_point);
			current_point.x += size.x * pivot.x;
			current_point.y -= size.y * (1f - pivot.y);

			var center_x = current_point.x / size.x;
			var center_y = (1f + (current_point.y / size.y)) / aspect_ratio;

			RipplesData.Add(center_x);
			RipplesData.Add(center_y);
			RipplesData.Add(UtilitiesTime.GetTime(false));

			if (EffectMaterial != null)
			{
				EffectMaterial.SetInt(ShaderIDs.Count, RipplesData.Count / FloatPerRipple);
				EffectMaterial.SetFloatArray(ShaderIDs.Ripple, RipplesData);
				graphic.SetMaterialDirty();
			}
		}

		/// <summary>
		/// Process pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			AddRipple(eventData);
		}

		/// <summary>
		/// Process pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Process pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Init material.
		/// </summary>
		protected override void InitMaterial()
		{
			var n = MaxRipples * FloatPerRipple;
			if (RipplesData == null)
			{
				RipplesData = new List<float>(n);
			}

			for (int i = RipplesData.Count; i < n; i++)
			{
				RipplesData.Add(-2);
			}

			base.InitMaterial();
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected override void SetMaterialProperties()
		{
			if (EffectMaterial != null)
			{
				EffectMaterial.SetColor(ShaderIDs.StartColor, startColor);
				EffectMaterial.SetColor(ShaderIDs.EndColor, endColor);
				EffectMaterial.SetFloat(ShaderIDs.Speed, speed);
				EffectMaterial.SetFloat(ShaderIDs.MaxSize, maxSize);
				EffectMaterial.SetInt(ShaderIDs.Count, RipplesData.Count / FloatPerRipple);
				EffectMaterial.SetFloatArray(ShaderIDs.Ripple, RipplesData);
			}
		}
	}
}