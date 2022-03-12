namespace UIWidgets
{
	/// <summary>
	/// ITemplatable interface.
	/// </summary>
	public interface ITemplatable
	{
		/// <summary>
		/// Gets a value indicating whether this instance is template.
		/// </summary>
		/// <value><c>true</c> if this instance is template; otherwise, <c>false</c>.</value>
		bool IsTemplate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		string TemplateName
		{
			get;
			set;
		}
	}
}