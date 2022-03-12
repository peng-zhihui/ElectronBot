namespace UIWidgets.Attributes
{
	/// <summary>
	/// Interface for the attributes to use conditional display of the fields.
	/// </summary>
	public interface IEditorCondition
	{
		/// <summary>
		/// Field name to check.
		/// </summary>
		string Field
		{
			get;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Function to check field value.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>true if condition is correct; otherwise false.</returns>
		bool IsValid(UnityEditor.SerializedProperty property);
#endif
	}
}