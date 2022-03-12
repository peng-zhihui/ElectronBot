namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ListViewIcons item description.
	/// </summary>
	[Serializable]
	public class TileViewItemSample
	{
		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Sprite Icon;

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// The capital.
		/// </summary>
		[SerializeField]
		public string Capital;

		/// <summary>
		/// The area.
		/// </summary>
		[SerializeField]
		public int Area;

		/// <summary>
		/// The population.
		/// </summary>
		[SerializeField]
		public int Population;
	}
}