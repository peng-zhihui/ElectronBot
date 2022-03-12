namespace UIWidgets.Menu
{
	using UnityEngine.Events;

	/// <summary>
	/// Menu item over event.
	/// First argument is index of the menu item.
	/// Second argument is true if pointer event; otherwise false.
	/// </summary>
	[System.Serializable]
	public class MenuItemOverEvent : UnityEvent<int, bool>
	{
	}
}