namespace UIWidgets.Styles
{
	/// <summary>
	/// Style support for the big button.
	/// </summary>
	public class StyleSupportButtonBig : StyleSupportButton, IStylable
	{
		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			style.ButtonBig.ApplyTo(this);

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			style.ButtonBig.GetFrom(this);

			return true;
		}
		#endregion
	}
}