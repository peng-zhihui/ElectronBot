namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display the field only if the value of the specified field with float type match condition.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Obsolete("Not supported.")]
	public sealed class EditorConditionFloatAttribute : Attribute, IEditorCondition
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

		readonly Func<float, bool> condition;

		/// <summary>
		/// Condition to display the field.
		/// </summary>
		public Func<float, bool> Condition
		{
			get
			{
				return condition;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorConditionFloatAttribute"/> class.
		/// </summary>
		/// <param name="field">Field to check.</param>
		/// /// <param name="condition">Condition to display the field.</param>
		public EditorConditionFloatAttribute(string field, Func<float, bool> condition)
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
			return Condition(property.floatValue);
		}
#endif
	}
}