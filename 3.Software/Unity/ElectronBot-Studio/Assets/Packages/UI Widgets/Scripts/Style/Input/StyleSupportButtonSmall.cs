namespace UIWidgets.Styles
{
	/// <summary>
	/// Style support for the small button.
	/// </summary>
	public class StyleSupportButtonSmall : StyleSupportButton, IStylable
	{
		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			style.ButtonSmall.ApplyTo(this);

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			style.ButtonSmall.GetFrom(this);

			return true;
		}
		#endregion
	}
}