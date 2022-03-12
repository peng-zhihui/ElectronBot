namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display the field only if the value of the specified field with bool type is true.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class EditorConditionObjectNullAttribute : Attribute, IEditorCondition
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
		/// Initializes a new instance of the <see cref="EditorConditionObjectNullAttribute"/> class.
		/// </summary>
		/// <param name="field">Field to check.</param>
		public EditorConditionObjectNullAttribute(string field)
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
			return property.objectReferenceValue == null;
		}
#endif
	}
}