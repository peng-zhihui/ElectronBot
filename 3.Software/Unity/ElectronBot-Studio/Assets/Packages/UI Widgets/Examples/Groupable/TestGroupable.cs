namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Test groupable.
	/// </summary>
	public class TestGroupable : MonoBehaviour
	{
		/// <summary>
		/// Selection.
		/// </summary>
		[SerializeField]
		public Groupable Selection;

		/// <summary>
		/// Elements to select.
		/// </summary>
		[SerializeField]
		public List<RectTransform> Elements = new List<RectTransform>();

		/// <summary>
		/// Select elements.
		/// </summary>
		public void SelectElements()
		{
			Selection.SetSelected(Elements);
		}
	}
}