namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display the field only if the value of the specified field with enum type match condition.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class EditorConditionEnumAttribute : Attribute, IEditorCondition
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

		readonly int[] condition;

		/// <summary>
		/// Values to match.
		/// </summary>
		public int[] Condition
		{
			get
			{
				return condition;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorConditionEnumAttribute"/> class.
		/// </summary>
		/// <param name="field">Field to check.</param>
		/// <param name="condition">Condition to display the field.</param>
		public EditorConditionEnumAttribute(string field, params int[] condition)
		{
			this.field = field;
			this.condition = condition;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Function to check field value.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>true if condition is correct; otherwise false.</returns>
		public bool IsValid(UnityEditor.SerializedProperty property)
		{
			return Array.IndexOf(condition, property.enumValueIndex) != -1;
		}
#endif
	}
}