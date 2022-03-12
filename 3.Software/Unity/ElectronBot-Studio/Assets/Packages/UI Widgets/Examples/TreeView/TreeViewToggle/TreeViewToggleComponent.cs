namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewToggleComponent.
	/// How to process clicks on gameobjects inside DefaultItem.
	/// </summary>
	public class TreeViewToggleComponent : TreeViewComponent
	{
		/// <summary>
		/// Process click and activate or deactivate objects depends of item value.
		/// Should be attached to Button.onClick() or other similar event.
		/// </summary>
		public void ProcessClick()
		{
			// toogle Item.Value - 1 = selected, 0 = unselected
			Item.Value = (Item.Value == 0) ? 1 : 0;

			// if node selected
			if (Item.Value == 1)
			{
				Debug.Log(string.Format("selected: {0}", Item.Name));

				// activate corresponding GameObjects
			}

			// if node deselected
			else
			{
				Debug.Log(string.Format("deselected: {0}", Item.Name));

				// deactivate corresponding GameObjects
			}
		}
	}
}