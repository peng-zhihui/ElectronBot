namespace UIWidgets
{
	/// <summary>
	/// Interface for the updatable targets.
	/// Replace Unity Update() with custom one without reflection.
	/// </summary>
	public interface IFixedUpdatable
	{
		/// <summary>
		/// Run FixedUpdate.
		/// </summary>
		void RunFixedUpdate();
	}
}