namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display the field only if the value of the specified field with int type match condition.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class EditorConditionIntAttribute : Attribute, IEditorCondition
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

		readonly int[] values;

		/// <summary>
		/// Values to display the field.
		/// </summary>
		public int[] Values
		{
			get
			{
				return values;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorConditionIntAttribute"/> class.
		/// </summary>
		/// <param name="field">Field to check.</param>
		/// <param name="values">Values to display the field.</param>
		public EditorConditionIntAttribute(string field, params int[] values)
		{
			this.field = field;
			this.values = values;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Function to check field value.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>true if condition is correct; otherwise false.</returns>
		public bool IsValid(UnityEditor.SerializedProperty property)
		{
			return System.Array.IndexOf(values, property.intValue) >= 0;
		}
#endif
	}
}