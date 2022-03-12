namespace UIWidgets
{
	/// <summary>
	/// Tooltip string.
	/// </summary>
	public class TooltipString : Tooltip<string, TooltipString>
	{
		/// <summary>
		/// Text.
		/// </summary>
		public TextAdapter Text;

		/// <summary>
		/// Item.
		/// </summary>
		public string Item
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		protected override void SetData(string data)
		{
			Item = data;
			UpdateView();
		}

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			Text.text = Item;
		}
	}
}