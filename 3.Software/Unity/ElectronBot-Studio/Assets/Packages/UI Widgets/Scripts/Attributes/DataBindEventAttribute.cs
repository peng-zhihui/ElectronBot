namespace UIWidgets.Attributes
{
	using System;

	/// <summary>
	/// Data bind event attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class DataBindEventAttribute : Attribute
	{
		/// <summary>
		/// The fields.
		/// </summary>
		readonly string[] fields;

		/// <summary>
		/// The fields.
		/// </summary>
		public string[] Fields
		{
			get
			{
				return fields;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.Attributes.DataBindEventAttribute"/> class.
		/// </summary>
		/// <param name="fields">Fields.</param>
		public DataBindEventAttribute(params string[] fields)
		{
			this.fields = fields;
		}
	}
}