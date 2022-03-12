namespace UIWidgets.Menu
{
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Menu item move event.
	/// First argument is index of the menu item.
	/// Second argument is AxisEventData.
	/// </summary>
	[System.Serializable]
	public class MenuItemMoveEvent : UnityEvent<int, AxisEventData>
	{
	}
}