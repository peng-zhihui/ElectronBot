namespace UIWidgets.Examples
{
	using UIWidgets;
	using UIWidgets.Menu;
	using UnityEngine;

	/// <summary>
	/// Test context menu.
	/// </summary>
	public class TestContextMenu : MonoBehaviour
	{
		/// <summary>
		/// Info.
		/// </summary>
		[SerializeField]
		protected TextAdapter Info;

		/// <summary>
		/// Show info.
		/// </summary>
		/// <param name="info">Info.</param>
		public void ShowInfo(string info)
		{
			Info.text = info;
		}

		/// <summary>
		/// Toggle check-mark.
		/// </summary>
		/// <param name="item">Item.</param>
		public void ToggleCheckmark(MenuItem item)
		{
			item.Checked = !item.Checked;
		}
	}
}