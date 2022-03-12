namespace UIWidgets.Examples.Shops
{
	using System;
	using System.ComponentModel;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Item.
	/// </summary>
	[Serializable]
	public class Item : IObservable, INotifyPropertyChanged
	{
		[SerializeField]
		[FormerlySerializedAs("Name")]
		string name;

		/// <summary>
		/// The name.
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
		[FormerlySerializedAs("count")]
		int quantity;

		/// <summary>
		/// Gets or sets the quantity. -1 for infinity quantity.
		/// </summary>
		/// <value>The quantity.</value>
		public int Quantity
		{
			get
			{
				return quantity;
			}

			set
			{
				if (quantity == value)
				{
					return;
				}

				if (quantity == -1)
				{
					NotifyPropertyChanged("Quantity");
					return;
				}

				quantity = value;
				NotifyPropertyChanged("Quantity");
			}
		}

		/// <summary>
		/// Gets or sets the quantity. -1 for infinity quantity.
		/// </summary>
		/// <value>The quantity.</value>
		[Obsolete("Renamed to Quantity.")]
		public int Count
		{
			get
			{
				return Quantity;
			}

			set
			{
				Quantity = value;
			}
		}

		/// <summary>
		/// Occurs when data changed.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.Examples.Shops.Item"/> class.
		/// </summary>
		/// <param name="itemName">Name.</param>
		/// <param name="itemQuantity">Quantity.</param>
		public Item(string itemName, int itemQuantity)
		{
			name = itemName;
			quantity = itemQuantity;
		}

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
	}
}