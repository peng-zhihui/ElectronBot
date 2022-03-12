namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewColor component.
	/// </summary>
	public class ListViewColorsComponent : ListViewItem, IViewData<Color>
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public Image Color;

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				GraphicsBackgroundVersion = 1;
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(Color item)
		{
			Color.color = item;
		}
	}
}