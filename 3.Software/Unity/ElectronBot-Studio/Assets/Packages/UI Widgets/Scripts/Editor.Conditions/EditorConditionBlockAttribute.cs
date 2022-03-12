namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Display the field only if the value of the specified field with bool type is true.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class EditorConditionBlockAttribute : Attribute
	{
		readonly string block;

		/// <summary>
		/// Field to check.
		/// </summary>
		public string Block
		{
			get
			{
				return block;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorConditionBlockAttribute"/> class.
		/// </summary>
		/// <param name="block">Block name.</param>
		public EditorConditionBlockAttribute(string block)
		{
			this.block = block;
		}
	}
}