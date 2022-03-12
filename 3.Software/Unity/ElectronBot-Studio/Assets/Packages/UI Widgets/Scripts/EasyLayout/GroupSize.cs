namespace EasyLayoutNS
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Group size.
	/// </summary>
	public struct GroupSize : IEquatable<GroupSize>
	{
		/// <summary>
		/// Minimum width.
		/// </summary>
		public float MinWidth;

		/// <summary>
		/// Preferred width.
		/// </summary>
		public float PreferredWidth;

		/// <summary>
		/// Actual width.
		/// </summary>
		public float Width;

		/// <summary>
		/// Min height.
		/// </summary>
		public float MinHeight;

		/// <summary>
		/// Preferred height.
		/// </summary>
		public float PreferredHeight;

		/// <summary>
		/// Height.
		/// </summary>
		public float Height;

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupSize"/> struct.
		/// </summary>
		/// <param name="size">Size.</param>
		public GroupSize(Vector2 size)
			: this(size.x, size.y)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupSize"/> struct.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public GroupSize(float width, float height)
		{
			MinWidth = width;
			PreferredWidth = width;
			Width = width;

			MinHeight = height;
			PreferredHeight = height;
			Height = height;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GroupSize"/> struct.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public GroupSize(GroupSize width, GroupSize height)
		{
			MinWidth = width.MinWidth;
			PreferredWidth = width.PreferredWidth;
			Width = width.Width;

			MinHeight = height.MinHeight;
			PreferredHeight = height.PreferredHeight;
			Height = height.Height;
		}

		/// <summary>
		/// Get max sizes between this instance and specified element.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <returns>Maximal size.</returns>
		public GroupSize Max(LayoutElementInfo element)
		{
			MinWidth = Mathf.Max(MinWidth, element.MinWidth);
			PreferredWidth = Mathf.Max(PreferredWidth, element.PreferredWidth);
			Width = Mathf.Max(Width, element.Width);

			MinHeight = Mathf.Max(MinHeight, element.MinHeight);
			PreferredHeight = Mathf.Max(PreferredHeight, element.PreferredHeight);
			Height = Mathf.Max(Height, element.Height);

			return this;
		}

		/// <summary>
		/// Get max sizes between this instance and specified size.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <returns>Maximal size.</returns>
		public GroupSize Max(GroupSize size)
		{
			MinWidth = Mathf.Max(MinWidth, size.MinWidth);
			PreferredWidth = Mathf.Max(PreferredWidth, size.PreferredWidth);
			Width = Mathf.Max(Width, size.Width);

			MinHeight = Mathf.Max(MinHeight, size.MinHeight);
			PreferredHeight = Mathf.Max(PreferredHeight, size.PreferredHeight);
			Height = Mathf.Max(Height, size.Height);

			return this;
		}

		/// <summary>
		/// Sum two sizes.
		/// </summary>
		/// <param name="a">First size.</param>
		/// <param name="b">Second size.</param>
		/// <returns>Sum.</returns>
		public static GroupSize operator +(GroupSize a, GroupSize b)
		{
			return new GroupSize()
			{
				MinWidth = a.MinWidth + b.MinWidth,
				PreferredWidth = a.PreferredWidth + b.PreferredWidth,
				Width = a.Width + b.Width,

				MinHeight = a.MinHeight + b.MinHeight,
				PreferredHeight = a.PreferredHeight + b.PreferredHeight,
				Height = a.Height + b.Height,
			};
		}

		/// <summary>
		/// Sum size and vector.
		/// </summary>
		/// <param name="a">Size.</param>
		/// <param name="b">Vector.</param>
		/// <returns>Sum.</returns>
		public static GroupSize operator +(GroupSize a, Vector2 b)
		{
			return new GroupSize()
			{
				MinWidth = a.MinWidth + b.x,
				PreferredWidth = a.PreferredWidth + b.x,
				Width = a.Width + b.x,

				MinHeight = a.MinHeight + b.y,
				PreferredHeight = a.PreferredHeight + b.y,
				Height = a.Height + b.y,
			};
		}

		/// <summary>
		/// Sum size and element.
		/// </summary>
		/// <param name="a">Size.</param>
		/// <param name="b">Element.</param>
		/// <returns>Sum.</returns>
		public static GroupSize operator +(GroupSize a, LayoutElementInfo b)
		{
			return new GroupSize()
			{
				MinWidth = a.MinWidth + b.MinWidth,
				PreferredWidth = a.PreferredWidth + b.PreferredWidth,
				Width = a.Width + b.Width,

				MinHeight = a.MinHeight + b.MinHeight,
				PreferredHeight = a.PreferredHeight + b.PreferredHeight,
				Height = a.Height + b.Height,
			};
		}

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
		public override string ToString()
		{
			return string.Format(
				"Min: {0}x{1}; Preferred: {2}x{3}; Actual: {4}x{5}",
				MinWidth.ToString(),
				MinHeight.ToString(),
				PreferredWidth.ToString(),
				PreferredHeight.ToString(),
				Width.ToString(),
				Height.ToString());
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is GroupSize)
			{
				return Equals((GroupSize)obj);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(GroupSize other)
		{
			return (MinWidth == other.MinWidth)
				&& (PreferredWidth == other.PreferredWidth)
				&& (Width == other.Width)
				&& (MinHeight == other.MinHeight)
				&& (PreferredHeight == other.PreferredHeight)
				&& (Height == other.Height);
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return MinWidth.GetHashCode() ^ PreferredWidth.GetHashCode() ^ Width.GetHashCode() ^ MinHeight.GetHashCode() ^ PreferredHeight.GetHashCode() ^ Height.GetHashCode();
		}

		/// <summary>
		/// Compare specified instances.
		/// </summary>
		/// <param name="a">First instance.</param>
		/// <param name="b">Second instance.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(GroupSize a, GroupSize b)
		{
			return a.Equals(b);
		}

		/// <summary>
		/// Compare specified instances.
		/// </summary>
		/// <param name="a">First instance.</param>
		/// <param name="b">Second instance.</param>
		/// <returns>true if the instances not equal; otherwise, false.</returns>
		public static bool operator !=(GroupSize a, GroupSize b)
		{
			return !a.Equals(b);
		}
	}
}