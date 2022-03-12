namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the image.
	/// </summary>
	[Serializable]
	public class StyleImage : IStyleDefaultValues
	{
		/// <summary>
		/// The sprite.
		/// </summary>
		[SerializeField]
		public Sprite Sprite;

		/// <summary>
		/// The color.
		/// </summary>
		[SerializeField]
		public Color Color = Color.white;

		/// <summary>
		/// The type.
		/// </summary>
		[SerializeField]
		public Image.Type ImageType = Image.Type.Sliced;

		/// <summary>
		/// The preserve aspect.
		/// </summary>
		[SerializeField]
		public bool PreserveAspect = false;

		/// <summary>
		/// The fill center.
		/// </summary>
		[SerializeField]
		public bool FillCenter = true;

		/// <summary>
		/// The fill method.
		/// </summary>
		[SerializeField]
		public Image.FillMethod FillMethod = Image.FillMethod.Radial360;

		/// <summary>
		/// The fill origin.
		/// </summary>
		[SerializeField]
		public int FillOrigin = 0;

		/// <summary>
		/// The fill amount.
		/// </summary>
		[SerializeField]
		[Range(0f, 1f)]
		public float FillAmount = 0;

		/// <summary>
		/// The fill clockwise.
		/// </summary>
		[SerializeField]
		public bool FillClockwise = true;

		/// <summary>
		/// The material.
		/// </summary>
		[SerializeField]
		public Material Material = null;

		/// <summary>
		/// Pixels per unit multiplier.
		/// </summary>
		[SerializeField]
		public float PixelsPerUnitMultiplier = 1;

		/// <summary>
		/// Apply style to the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		public virtual void ApplyTo(GameObject go)
		{
			if (go != null)
			{
				ApplyTo(go.GetComponent<Image>());
			}
		}

		/// <summary>
		/// Apply style to the specified transform.
		/// </summary>
		/// <param name="transform">Transform.</param>
		public virtual void ApplyTo(Transform transform)
		{
			if (transform != null)
			{
				ApplyTo(transform.gameObject);
			}
		}

		/// <summary>
		/// Apply style to the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		public virtual void ApplyTo(Component component)
		{
			if (component != null)
			{
				ApplyTo(component.gameObject);
			}
		}

		/// <summary>
		/// Apply style to the specified Image.
		/// </summary>
		/// <param name="component">Image.</param>
		public virtual void ApplyTo(Image component)
		{
			if (component == null)
			{
				return;
			}

			component.sprite = Sprite;
			component.color = Color;
			component.material = Material;

			component.type = ImageType;
			component.preserveAspect = PreserveAspect;
			component.fillCenter = FillCenter;
			component.fillMethod = FillMethod;
			component.fillOrigin = FillOrigin;
			component.fillAmount = FillAmount;
			component.fillClockwise = FillClockwise;
			#if UNITY_2019_4_OR_NEWER
			component.pixelsPerUnitMultiplier = PixelsPerUnitMultiplier;
			#endif

			component.SetAllDirty();
		}

		/// <summary>
		/// Set style options from the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		public virtual void GetFrom(GameObject go)
		{
			if (go != null)
			{
				GetFrom(go.GetComponent<Image>());
			}
		}

		/// <summary>
		/// Set style options from the specified transform.
		/// </summary>
		/// <param name="transform">Transform.</param>
		public virtual void GetFrom(Transform transform)
		{
			if (transform != null)
			{
				GetFrom(transform.gameObject);
			}
		}

		/// <summary>
		/// Set style options from the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		public virtual void GetFrom(Component component)
		{
			if (component != null)
			{
				GetFrom(component.gameObject);
			}
		}

		/// <summary>
		/// Set style options from the specified Image.
		/// </summary>
		/// <param name="component">Image.</param>
		public virtual void GetFrom(Image component)
		{
			if (component == null)
			{
				return;
			}

			Style.SetValue(component.sprite, ref Sprite);
			Color = component.color;
			Style.SetValue(component.material, ref Material);

			ImageType = component.type;
			PreserveAspect = component.preserveAspect;
			FillCenter = component.fillCenter;
			FillMethod = component.fillMethod;
			FillOrigin = component.fillOrigin;
			FillAmount = component.fillAmount;
			FillClockwise = component.fillClockwise;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		/// <returns>Copy of the object.</returns>
		public StyleImage Clone()
		{
			return (StyleImage)MemberwiseClone();
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			// nothing required
		}
#endif
	}
}