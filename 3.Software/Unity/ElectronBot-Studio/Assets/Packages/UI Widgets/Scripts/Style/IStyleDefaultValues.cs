namespace UIWidgets.Styles
{
	/// <summary>
	/// Interface to set default values for the style.
	/// Reason: using "Resources.GetBuiltinResource" as default field value not allowed.
	/// </summary>
	public interface IStyleDefaultValues
	{
#if UNITY_EDITOR
		/// <summary>
		/// Sets the default values.
		/// </summary>
		void SetDefaultValues();
#endif
	}
}