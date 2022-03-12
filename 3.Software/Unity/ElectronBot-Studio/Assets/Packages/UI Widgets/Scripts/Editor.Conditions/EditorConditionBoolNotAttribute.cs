namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display the field only if the value of the specified field with bool type is false.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Obsolete("Use EditorConditionBool(field, false).")]
	public sealed class EditorConditionBoolNotAttribute : Attribute, IEditorCondition
	{
		readonly string field;

		/// <summary>
		/// Field to check.
		/// </summary>
		public string Field
		{
			get
			{
				return field;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorConditionBoolNotAttribute"/> class.
		/// </summary>
		/// <param name="field">Field to check.</param>
		public EditorConditionBoolNotAttribute(string field)
		{
			this.field = field;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Function to check field value.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>true if condition is correct; otherwise false.</returns>
		public bool IsValid(UnityEditor.SerializedProperty property)
		{
			return !property.boolValue;
		}
#endif
	}
}