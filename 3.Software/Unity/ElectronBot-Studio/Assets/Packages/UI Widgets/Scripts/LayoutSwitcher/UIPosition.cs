namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// User interface position.
	/// </summary>
	[Serializable]
	public class UIPosition
	{
		/// <summary>
		/// The object.
		/// </summary>
		[SerializeField]
		public RectTransform Object;

		/// <summary>
		/// The active.
		/// </summary>
		[SerializeField]
		public bool Active;

		/// <summary>
		/// The position.
		/// </summary>
		[SerializeField]
		public Vector3 Position;

		/// <summary>
		/// The anchor max.
		/// </summary>
		[SerializeField]
		public Vector2 AnchorMax;

		/// <summary>
		/// The anchor minimum.
		/// </summary>
		[SerializeField]
		public Vector2 AnchorMin;

		/// <summary>
		/// The size delta.
		/// </summary>
		[SerializeField]
		public Vector2 SizeDelta;

		/// <summary>
		/// The pivot.
		/// </summary>
		[SerializeField]
		public Vector2 Pivot;

		/// <summary>
		/// The rotation.
		/// </summary>
		[SerializeField]
		public Vector3 Rotation;

		/// <summary>
		/// The scale.
		/// </summary>
		[SerializeField]
		public Vector3 Scale;
	}
}