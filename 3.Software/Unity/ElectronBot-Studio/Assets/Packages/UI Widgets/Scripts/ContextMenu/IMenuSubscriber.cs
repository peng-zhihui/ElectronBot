namespace UIWidgets.Menu
{
	/// <summary>
	/// Menu subscriber.
	/// Used to update menu when templates changed.
	/// </summary>
	public interface IMenuSubscriber
	{
		/// <summary>
		/// Reset items.
		/// </summary>
		void ResetItems();

		/// <summary>
		/// Update items.
		/// </summary>
		void UpdateItems();
	}
}