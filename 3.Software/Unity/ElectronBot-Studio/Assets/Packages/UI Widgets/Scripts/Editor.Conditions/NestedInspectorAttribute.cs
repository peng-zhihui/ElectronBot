namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display nested inspector for the field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class NestedInspectorAttribute : Attribute
	{
	}
}