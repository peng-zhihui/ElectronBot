namespace UIWidgets
{
	/// <summary>
	/// Custom validation in replacement OnValidate to avoid SendMessage warnings in Unity 2019.3+.
	/// </summary>
	public interface IValidateable
	{
#if UNITY_EDITOR
		/// <summary>
		/// Validate instance.
		/// </summary>
		void Validate();
#endif
	}
}