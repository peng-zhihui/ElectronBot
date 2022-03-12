namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewSample component country.
	/// </summary>
	public class TreeViewSampleComponentCountry : TreeViewSampleComponent
	{
		/// <inheritdoc/>
		protected override void UpdateView()
		{
			var item = Item as TreeViewSampleItemCountry;

			if (item == null)
			{
				Icon.sprite = null;
				TextAdapter.text = string.Empty;
			}
			else
			{
				Icon.sprite = item.Icon;
				TextAdapter.text = item.Name;

				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
			}
		}
	}
}