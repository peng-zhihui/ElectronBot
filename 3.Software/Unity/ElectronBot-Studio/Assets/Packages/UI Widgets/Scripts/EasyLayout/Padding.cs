namespace EasyLayoutNS
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Padding.
	/// </summary>
	[Serializable]
	public struct Padding : IEquatable<Padding>
	{
		/// <summary>
		/// The left padding.
		/// </summary>
		[SerializeField]
		public float Left;

		/// <summary>
		/// The right padding.
		/// </summary>
		[SerializeField]
		public float Right;

		/// <summary>
		/// The top padding.
		/// </summary>
		[SerializeField]
		public float Top;

		/// <summary>
		/// The bottom padding.
		/// </summary>
		[SerializeField]
		public float Bottom;

		/// <summary>
		/// Summary horizontal padding.
		/// </summary>
		public float Horizontal
		{
			get
			{
				return Left + Right;
			}
		}

		/// <summary>
		/// Summary vertical padding.
		/// </summary>
		public float Vertical
		{
			get
			{
				return Top + Bottom;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Padding"/> struct.
		/// </summary>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="top">Top.</param>
		/// <param name="bottom">Bottom.</param>
		public Padding(float left, float right, float top, float bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "Required.")]
		public override string ToString()
		{
			return string.Format("Padding(left: {0}, right: {1}, top: {2}, bottom: {3})", Left.ToString(), Right.ToString(), Top.ToString(), Bottom.ToString());
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Padding)
			{
				return Equals((Padding)obj);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(Padding other)
		{
			return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return Left.GetHashCode() ^ Right.GetHashCode() ^ Top.GetHashCode() ^ Bottom.GetHashCode();
		}

		/// <summary>
		/// Compare specified paddings.
		/// </summary>
		/// <param name="padding1">First padding.</param>
		/// <param name="padding2">Second padding.</param>
		/// <returns>true if the paddings are equal; otherwise, false.</returns>
		public static bool operator ==(Padding padding1, Padding padding2)
		{
			return padding1.Equals(padding2);
		}

		/// <summary>
		/// Compare specified paddings.
		/// </summary>
		/// <param name="padding1">First padding.</param>
		/// <param name="padding2">Second padding.</param>
		/// <returns>true if the paddings not equal; otherwise, false.</returns>
		public static bool operator !=(Padding padding1, Padding padding2)
		{
			return !padding1.Equals(padding2);
		}
	}
}