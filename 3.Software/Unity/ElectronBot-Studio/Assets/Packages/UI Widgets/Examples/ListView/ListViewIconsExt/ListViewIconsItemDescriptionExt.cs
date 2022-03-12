namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// ListViewIconsItemDescription extended
	/// </summary>
	[Serializable]
	public class ListViewIconsItemDescriptionExt : ListViewIconsItemDescription
	{
		bool visible = true;

		/// <summary>
		/// Is visible?
		/// </summary>
		public bool Visible
		{
			get
			{
				return visible;
			}

			set
			{
				visible = value;
				Changed("Visible");
			}
		}

		bool interactable = true;

		/// <summary>
		/// Is interactable?
		/// </summary>
		public bool Interactable
		{
			get
			{
				return interactable;
			}

			set
			{
				interactable = value;
				Changed("Interactable");
			}
		}
	}
}