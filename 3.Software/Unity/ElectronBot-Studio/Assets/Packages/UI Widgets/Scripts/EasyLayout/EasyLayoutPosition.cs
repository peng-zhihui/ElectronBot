namespace EasyLayoutNS
{
	/// <summary>
	/// Position.
	/// </summary>
	public struct EasyLayoutPosition : System.IEquatable<EasyLayoutPosition>
	{
		/// <summary>
		/// X.
		/// </summary>
		public readonly int X;

		/// <summary>
		/// Y.
		/// </summary>
		public readonly int Y;

		/// <summary>
		/// Initializes a new instance of the <see cref="EasyLayoutPosition"/> struct.
		/// </summary>
		/// <param name="x">X.</param>
		/// <param name="y">Y.</param>
		public EasyLayoutPosition(int x, int y)
		{
			X = x;
			Y = y;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return string.Format("[{0}, {1}]", X.ToString(), Y.ToString());
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is EasyLayoutPosition)
			{
				return Equals((EasyLayoutPosition)obj);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(EasyLayoutPosition other)
		{
			return X == other.X && Y == other.Y;
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return X ^ Y;
		}

		/// <summary>
		/// Compare specified instances.
		/// </summary>
		/// <param name="left">Left instance.</param>
		/// <param name="right">Right instances.</param>
		/// <returns>true if the instances are equal; otherwise, false.</returns>
		public static bool operator ==(EasyLayoutPosition left, EasyLayoutPosition right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compare specified instances.
		/// </summary>
		/// <param name="left">Left instance.</param>
		/// <param name="right">Right instances.</param>
		/// <returns>true if the instances are now equal; otherwise, false.</returns>
		public static bool operator !=(EasyLayoutPosition left, EasyLayoutPosition right)
		{
			return !left.Equals(right);
		}
	}
}