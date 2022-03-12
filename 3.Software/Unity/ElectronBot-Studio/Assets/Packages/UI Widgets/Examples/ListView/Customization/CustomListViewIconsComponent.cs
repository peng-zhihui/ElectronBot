namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// CustomListViewIconsComponent
	/// </summary>
	public class CustomListViewIconsComponent : ListViewIconsItemComponent
	{
		/// <summary>
		/// This image will be change color on click.
		/// </summary>
		[SerializeField]
		public Image Image;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public override void SetData(ListViewIconsItemDescription item)
		{
			base.SetData(item);
			SetImageColor();
		}

		/// <summary>
		/// To Image gameobject EventTrigger component and ImageClick call on PointerClick event.
		/// </summary>
		public void ImageClick()
		{
			// keep current state in Value
			Item.Value = (Item.Value == 0) ? 1 : 0;

			SetImageColor();
		}

		/// <summary>
		/// Sets image color.
		/// </summary>
		public void SetImageColor()
		{
			// set image color depend of Value
			if (Item.Value == 1)
			{
				Image.color = Color.blue;
			}
			else
			{
				Image.color = Color.red;
			}
		}
	}
}