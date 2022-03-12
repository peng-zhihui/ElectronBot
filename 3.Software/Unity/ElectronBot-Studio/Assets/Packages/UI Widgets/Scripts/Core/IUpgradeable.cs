namespace UIWidgets
{
	/// <summary>
	/// Interface for the upgrade support.
	/// </summary>
	public interface IUpgradeable
	{
		/// <summary>
		/// Upgrade internal data to the latest version.
		/// </summary>
		void Upgrade();
	}
}