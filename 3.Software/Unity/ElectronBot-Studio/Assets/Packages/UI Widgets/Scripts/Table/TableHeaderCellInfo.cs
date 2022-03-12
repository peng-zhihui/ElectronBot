namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TableHeader cell info.
	/// </summary>
	[Serializable]
	public class TableHeaderCellInfo
	{
		/// <summary>
		/// The cell RectTransform component.
		/// </summary>
		public RectTransform Rect;

		/// <summary>
		/// The cell LayoutElement component.
		/// </summary>
		public LayoutElement LayoutElement;

		/// <summary>
		/// The cell position.
		/// </summary>
		public int Position;

		/// <summary>
		/// Gets the cell width.
		/// </summary>
		/// <value>The width.</value>
		public float Width
		{
			get
			{
				return Rect.rect.width;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this gameobject active self.
		/// </summary>
		/// <value><c>true</c> if active self; otherwise, <c>false</c>.</value>
		public bool ActiveSelf
		{
			get
			{
				return Rect.gameObject.activeSelf;
			}
		}
	}
}