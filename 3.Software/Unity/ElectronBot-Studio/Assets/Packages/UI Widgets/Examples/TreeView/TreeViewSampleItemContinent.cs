namespace UIWidgets.Examples
{
	using System;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewSample continent item.
	/// </summary>
	[Serializable]
	public class TreeViewSampleItemContinent : ITreeViewSampleItem
	{
		[SerializeField]
		string name;

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				if (name != value)
				{
					name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		[SerializeField]
		int countries;

		/// <summary>
		/// Countries.
		/// </summary>
		public int Countries
		{
			get
			{
				return countries;
			}

			set
			{
				if (countries != value)
				{
					countries = value;
					NotifyPropertyChanged("Countries");
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeViewSampleItemContinent"/> class.
		/// </summary>
		/// <param name="itemName">Name.</param>
		/// <param name="itemCountries">Countries.</param>
		public TreeViewSampleItemContinent(string itemName, int itemCountries = 0)
		{
			name = itemName;
			countries = itemCountries;
		}

		/// <summary>
		/// OnChange event.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Notify property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
		{
			var c_handlers = OnChange;
			if (c_handlers != null)
			{
				c_handlers();
			}

			var handlers = PropertyChanged;
			if (handlers != null)
			{
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Display item data using specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		public void Display(TreeViewSampleComponent component)
		{
			component.Icon.sprite = null;
			component.Icon.color = Color.clear;
			component.TextAdapter.text = string.Format("{0} (Countries: {1})", Name, Countries.ToString());
		}
	}
}