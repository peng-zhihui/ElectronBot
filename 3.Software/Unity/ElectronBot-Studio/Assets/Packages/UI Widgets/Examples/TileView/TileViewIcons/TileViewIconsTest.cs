namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TileViewIcons.
	/// </summary>
	public class TileViewIconsTest : MonoBehaviour
	{
		/// <summary>
		/// Change layout settings.
		/// </summary>
		public void ChangeLayoutSettings()
		{
			var tiles = GetComponent<TileViewIcons>();

			tiles.Layout.Spacing = new Vector2(5, 50);
			tiles.Layout.UpdateLayout();

			var rl = tiles.ScrollRect.GetComponent<ResizeListener>();
			rl.OnResize.Invoke();
			rl.OnResizeNextFrame.Invoke();
		}
	}
}