namespace UIWidgets.Examples
{
	using System;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewSampleItemCountry.
	/// Sample class to display country data.
	/// </summary>
	[Serializable]
	public class TreeViewSampleItemCountry : ITreeViewSampleItem
	{
		[SerializeField]
		Sprite icon;

		/// <summary>
		/// Icon.
		/// </summary>
		public Sprite Icon
		{
			get
			{
				return icon;
			}

			set
			{
				if (icon != value)
				{
					icon = value;
					NotifyPropertyChanged("Icon");
				}
			}
		}

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

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeViewSampleItemCountry"/> class.
		/// </summary>
		/// <param name="itemName">Name.</param>
		/// <param name="itemIcon">Icon.</param>
		public TreeViewSampleItemCountry(string itemName, Sprite itemIcon = null)
		{
			name = itemName;
			icon = itemIcon;
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
		/// Display data with specified component.
		/// </summary>
		/// <param name="component">Component to display item.</param>
		public void Display(TreeViewSampleComponent component)
		{
			component.Icon.sprite = Icon;
			component.TextAdapter.text = Name;

			if (component.SetNativeSize)
			{
				component.Icon.SetNativeSize();
			}

			component.Icon.color = (component.Icon.sprite == null) ? Color.clear : Color.white;
		}
	}
}