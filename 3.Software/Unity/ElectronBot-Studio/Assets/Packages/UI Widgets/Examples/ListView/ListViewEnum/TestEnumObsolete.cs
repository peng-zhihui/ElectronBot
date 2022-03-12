namespace UIWidgets.Examples
{
	using System;

	/// <summary>
	/// Test enum with obsolete values.
	/// </summary>
	public enum TestEnumObsolete
	{
		/// <summary>
		/// Value1.
		/// </summary>
		Value1 = 0,

		/// <summary>
		/// Value2.
		/// </summary>
		Value2 = 1,

		/// <summary>
		/// Value3.
		/// </summary>
		[Obsolete("Deprecated value.")]
		Value3 = 2,

		/// <summary>
		/// Value4.
		/// </summary>
		Value4 = 3,
	}
}