namespace UIWidgets
{
	using System;
	using System.ComponentModel;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Tree view item.
	/// </summary>
	[Serializable]
	public class TreeViewItem : IObservable, INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		Sprite icon;

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>The icon.</value>
		public Sprite Icon
		{
			get
			{
				return icon;
			}

			set
			{
				icon = value;
				NotifyPropertyChanged("Icon");
			}
		}

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		string name;

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				name = value;
				NotifyPropertyChanged("Name");
			}
		}

		[NonSerialized]
		string localizedName;

		/// <summary>
		/// The localized name.
		/// </summary>
		public string LocalizedName
		{
			get
			{
				return localizedName;
			}

			set
			{
				localizedName = value;
				NotifyPropertyChanged("LocalizedName");
			}
		}

		[SerializeField]
		[FormerlySerializedAs("_value")]
		int itemValue;

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public int Value
		{
			get
			{
				return itemValue;
			}

			set
			{
				itemValue = value;
				NotifyPropertyChanged("Value");
			}
		}

		[SerializeField]
		object tag;

		/// <summary>
		/// Gets or sets the tag.
		/// </summary>
		/// <value>The tag.</value>
		public object Tag
		{
			get
			{
				return tag;
			}

			set
			{
				tag = value;
				NotifyPropertyChanged("Tag");
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.TreeViewItem"/> class.
		/// </summary>
		/// <param name="itemName">Item name.</param>
		/// <param name="itemIcon">Item icon.</param>
		public TreeViewItem(string itemName, Sprite itemIcon = null)
		{
			name = itemName;
			icon = itemIcon;
		}

		/// <summary>
		/// Raise OnChange event.
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
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return LocalizedName ?? Name;
		}
	}
}