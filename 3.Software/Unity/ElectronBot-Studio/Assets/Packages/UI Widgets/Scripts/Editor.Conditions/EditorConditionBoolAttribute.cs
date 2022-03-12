namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display the field only if the value of the specified field with bool type is true.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class EditorConditionBoolAttribute : Attribute, IEditorCondition
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

		readonly bool value;

		/// <summary>
		/// Value to match.
		/// </summary>
		public bool Value
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorConditionBoolAttribute"/> class.
		/// </summary>
		/// <param name="field">Field to check.</param>
		/// <param name="value">Value.</param>
		public EditorConditionBoolAttribute(string field, bool value = true)
		{
			this.field = field;
			this.value = value;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Function to check field value.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>true if property data match value; otherwise false.</returns>
		public bool IsValid(UnityEditor.SerializedProperty property)
		{
			return property.boolValue == Value;
		}
#endif
	}
}