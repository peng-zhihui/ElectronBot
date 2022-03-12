namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Cursors.
	/// </summary>
	[Serializable]
	public class Cursors : ScriptableObject
	{
		/// <summary>
		/// Cursor.
		/// </summary>
		[Serializable]
		public struct Cursor : IEquatable<Cursor>
		{
			[SerializeField]
			Texture2D texture;

			/// <summary>
			/// Texture.
			/// </summary>
			public Texture2D Texture
			{
				get
				{
					return texture;
				}
			}

			[SerializeField]
			Vector2 hotspot;

			/// <summary>
			/// Hot spot.
			/// </summary>
			public Vector2 Hotspot
			{
				get
				{
					return hotspot;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Cursor"/> struct.
			/// </summary>
			/// <param name="texture">Texture.</param>
			/// <param name="hotspot">Hot spot.</param>
			public Cursor(Texture2D texture, Vector2 hotspot)
			{
				this.texture = texture;
				this.hotspot = hotspot;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is Cursor)
				{
					return Equals((Cursor)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(Cursor other)
			{
				return Texture == other.Texture && Hotspot == other.Hotspot;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				var a = Texture != null ? Texture.GetHashCode() : 0;
				var b = Hotspot.GetHashCode();
				return a ^ b;
			}

			/// <summary>
			/// Compare specified cursors.
			/// </summary>
			/// <param name="a">First cursor.</param>
			/// <param name="b">Second cursor.</param>
			/// <returns>true if the cursors are equal; otherwise, false.</returns>
			public static bool operator ==(Cursor a, Cursor b)
			{
				return a.Equals(b);
			}

			/// <summary>
			/// Compare specified cursors.
			/// </summary>
			/// <param name="a">First cursor.</param>
			/// <param name="b">Second cursor.</param>
			/// <returns>true if the cursors not equal; otherwise, false.</returns>
			public static bool operator !=(Cursor a, Cursor b)
			{
				return !a.Equals(b);
			}
		}

		/// <summary>
		/// Default cursor.
		/// </summary>
		[SerializeField]
		public Cursor Default;

		/// <summary>
		/// Cursor for the allowed actions.
		/// </summary>
		[SerializeField]
		[Tooltip("Cursor for the allowed actions")]
		public Cursor Allowed;

		/// <summary>
		/// Cursor for the not allowed actions.
		/// </summary>
		[SerializeField]
		[Tooltip("Cursor for the not allowed actions")]
		public Cursor Denied;

		/// <summary>
		/// North &lt;-&gt; South arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("North <-> South arrow")]
		public Cursor NorthSouthArrow;

		/// <summary>
		/// East &lt;-&gt; West arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("East <-> West arrow")]
		public Cursor EastWestArrow;

		/// <summary>
		/// NorthEast &lt;-&gt; SouthWest arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("NorthEast <-> SouthWest arrow")]
		public Cursor NorthEastSouthWestArrow;

		/// <summary>
		/// NorthWest &lt;-&gt; SouthEast arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("NorthWest <-> SouthEast arrow")]
		public Cursor NorthWestSouthEastArrow;

		/// <summary>
		/// North &lt;-&gt; West arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("North <-> West arrow")]
		public Cursor NorthWestRotateArrow;

		/// <summary>
		/// North &lt;-&gt; East arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("North <-> East arrow")]
		public Cursor NorthEastRotateArrow;

		/// <summary>
		/// South &lt;-&gt; West arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("South <-> West arrow")]
		public Cursor SouthWestRotateArrow;

		/// <summary>
		/// South &lt;-&gt; East arrow.
		/// </summary>
		[SerializeField]
		[Tooltip("South <-> East arrow")]
		public Cursor SouthEastRotateArrow;
	}
}