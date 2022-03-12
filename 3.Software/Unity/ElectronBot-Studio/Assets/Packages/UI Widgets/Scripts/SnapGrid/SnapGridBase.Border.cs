namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Base class for the SnapGrid.
	/// </summary>
	public abstract partial class SnapGridBase : UIBehaviourConditional
	{
		/// <summary>
		/// Border.
		/// </summary>
		[Serializable]
		public struct Border : IEquatable<Border>
		{
			/// <summary>
			/// Left side.
			/// </summary>
			[SerializeField]
			public bool Left;

			/// <summary>
			/// Right side.
			/// </summary>
			[SerializeField]
			public bool Right;

			/// <summary>
			/// Top side.
			/// </summary>
			[SerializeField]
			public bool Top;

			/// <summary>
			/// Bottom side.
			/// </summary>
			[SerializeField]
			public bool Bottom;

			/// <summary>
			/// Initializes a new instance of the <see cref="Border"/> struct.
			/// </summary>
			/// <param name="left">Left border.</param>
			/// <param name="right">Right border.</param>
			/// <param name="top">Top border.</param>
			/// <param name="bottom">Bottom border.</param>
			public Border(bool left, bool right, bool top, bool bottom)
			{
				Left = left;
				Right = right;
				Top = top;
				Bottom = bottom;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is Border)
				{
					return Equals((Border)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(Border other)
			{
				return (Left == other.Left) && (Right == other.Right) && (Top == other.Top) && (Bottom == other.Bottom);
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
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(Border a, Border b)
			{
				return a.Equals(b);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances not equal; otherwise, false.</returns>
			public static bool operator !=(Border a, Border b)
			{
				return !a.Equals(b);
			}
		}
	}
}