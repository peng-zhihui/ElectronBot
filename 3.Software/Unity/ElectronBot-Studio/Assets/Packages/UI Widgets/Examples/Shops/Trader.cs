namespace UIWidgets.Examples.Shops
{
	using UIWidgets;

	/// <summary>
	/// On items change.
	/// </summary>
	public delegate void OnItemsChange();

	/// <summary>
	/// On money change.
	/// </summary>
	public delegate void OnMoneyChange();

	/// <summary>
	/// Trader.
	/// </summary>
	public class Trader
	{
		int money;

		/// <summary>
		/// Gets or sets the trader money. -1 to infinity money
		/// </summary>
		/// <value>The money.</value>
		public int Money
		{
			get
			{
				return money;
			}

			set
			{
				if (money == -1)
				{
					MoneyChanged();
					return;
				}

				money = value;
				MoneyChanged();
			}
		}

		ObservableList<Item> inventory = new ObservableList<Item>();

		/// <summary>
		/// Gets or sets the inventory.
		/// </summary>
		/// <value>The inventory.</value>
		public ObservableList<Item> Inventory
		{
			get
			{
				return inventory;
			}

			set
			{
				if (inventory != null)
				{
					inventory.OnChange -= ItemsChanged;
				}

				inventory = value;

				if (inventory != null)
				{
					inventory.OnChange += ItemsChanged;
				}

				ItemsChanged();
			}
		}

		/// <summary>
		/// The price factor.
		/// </summary>
		public float PriceFactor = 1;

		/// <summary>
		/// The delete items if Item.Quantity = 0.
		/// </summary>
		public bool DeleteIfEmpty = true;

		/// <summary>
		/// Occurs when data changed.
		/// </summary>
		public event OnItemsChange OnItemsChange = () => { };

		/// <summary>
		/// Occurs when money changed.
		/// </summary>
		public event OnMoneyChange OnMoneyChange = () => { };

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.Examples.Shops.Trader"/> class.
		/// </summary>
		/// <param name="deleteIfEmpty">If set to <c>true</c> delete if empty.</param>
		public Trader(bool deleteIfEmpty = true)
		{
			DeleteIfEmpty = deleteIfEmpty;
			inventory.OnChange += ItemsChanged;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="Trader"/> class.
		/// </summary>
		~Trader()
		{
			if (inventory != null)
			{
				inventory.OnChange -= ItemsChanged;
			}
		}

		void ItemsChanged()
		{
			OnItemsChange();
		}

		void MoneyChanged()
		{
			OnMoneyChange();
		}

		/// <summary>
		/// Sell the specified order.
		/// </summary>
		/// <param name="order">Order.</param>
		public void Sell(IOrder order)
		{
			if (order.OrderLinesCount() == 0)
			{
				return;
			}

			Inventory.BeginUpdate();
			foreach (var line in order.GetOrderLines())
			{
				SellItem(line);
			}

			Inventory.EndUpdate();

			Money += order.Total();
		}

		/// <summary>
		/// Sells the item.
		/// </summary>
		/// <param name="orderLine">Order line.</param>
		void SellItem(IOrderLine orderLine)
		{
			var quantity = orderLine.Quantity;

			// decrease items quantity
			orderLine.Item.Quantity -= quantity;

			// remove item from inventory if zero quantity
			if (DeleteIfEmpty && (orderLine.Item.Quantity == 0))
			{
				Inventory.Remove(orderLine.Item);
			}
		}

		/// <summary>
		/// Buy the specified order.
		/// </summary>
		/// <param name="order">Order.</param>
		public void Buy(IOrder order)
		{
			if (order.OrderLinesCount() == 0)
			{
				return;
			}

			Inventory.BeginUpdate();
			foreach (var line in order.GetOrderLines())
			{
				BuyItem(line);
			}

			Inventory.EndUpdate();

			Money -= order.Total();
		}

		Item FindItem(string name)
		{
			foreach (var item in Inventory)
			{
				if (item.Name == name)
				{
					return item;
				}
			}

			return null;
		}

		/// <summary>
		/// Buy the item.
		/// </summary>
		/// <param name="orderLine">Order line.</param>
		void BuyItem(IOrderLine orderLine)
		{
			// find item in inventory
			var item = FindItem(orderLine.Item.Name);

			var quantity = orderLine.Quantity;

			// if not found add new item to inventory
			if (item == null)
			{
				Inventory.Add(new Item(orderLine.Item.Name, quantity));
			}

			// if found increase quantity
			else
			{
				item.Quantity += quantity;
			}
		}

		/// <summary>
		/// Determines whether this instance can buy the specified order.
		/// </summary>
		/// <returns><c>true</c> if this instance can buy the specified order; otherwise, <c>false</c>.</returns>
		/// <param name="order">Order.</param>
		public bool CanBuy(IOrder order)
		{
			return Money == -1 || Money >= order.Total();
		}
	}
}