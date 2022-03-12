namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Country Flag.
	/// </summary>
	[Serializable]
	public class CountryFlag
	{
		/// <summary>
		/// County code.
		/// </summary>
		[SerializeField]
		public string Country;

		/// <summary>
		/// Flag sprite.
		/// </summary>
		[SerializeField]
		public Sprite Flag;
	}
}