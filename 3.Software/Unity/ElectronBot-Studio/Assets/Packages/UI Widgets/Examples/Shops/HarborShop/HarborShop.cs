namespace UIWidgets.Examples.Shops
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Harbor shop.
	/// </summary>
	public class HarborShop : MonoBehaviour, IUpgradeable
	{
		Trader Harbor;

		/// <summary>
		/// Trade ListView.
		/// </summary>
		[SerializeField]
		protected HarborListView TradeView;

		/// <summary>
		/// Text component to display total value.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TradeTotalAdapter.")]
		protected Text TradeTotal;

		/// <summary>
		/// Text component to display total value.
		/// </summary>
		[SerializeField]
		protected TextAdapter TradeTotalAdapter;

		/// <summary>
		/// Buy button.
		/// </summary>
		[SerializeField]
		protected Button BuyButton;

		Trader Player;

		/// <summary>
		/// Text component to display player money.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with PlayerMoneyAdapter.")]
		protected Text PlayerMoney;

		/// <summary>
		/// Text component to display player money.
		/// </summary>
		[SerializeField]
		protected TextAdapter PlayerMoneyAdapter;

		/// <summary>
		/// Notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notify")]
		protected Notify NotifyTemplate;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			Harbor = new Trader(false);
			Player = new Trader(false);

			Init();

			BuyButton.onClick.AddListener(Trade);

			Harbor.OnItemsChange += UpdateTraderItems;
			Player.OnItemsChange += UpdateTraderItems;

			Player.OnMoneyChange += UpdatePlayerMoneyInfo;

			UpdateTraderItems();

			UpdatePlayerMoneyInfo();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			Harbor.Money = -1;
			Harbor.PriceFactor = 1;
			Harbor.Inventory.Clear();

			var shop_items = new List<Item>()
			{
				new Item("Wood", 100),
				new Item("Wheat", 30),
				new Item("Fruits", 0),
				new Item("Sugar", 20),
				new Item("Metal", 40),
				new Item("Cotton", 0),
				new Item("Silver", 25),
				new Item("Gold", 55),
				new Item("Cocoa", 10),
				new Item("Coffee", 7),
				new Item("Tobacco", 20),
			};

			Harbor.Inventory.AddRange(shop_items);

			Player.Money = 5000;
			Player.PriceFactor = 0.5f;
			Player.Inventory.Clear();
			var player_items = new List<Item>()
			{
				new Item("Wood", 50),
				new Item("Cocoa", 100),
				new Item("Metal", 20),
				new Item("Sugar", 10),
			};

			Player.Inventory.AddRange(player_items);
		}

		/// <summary>
		/// Updates the total.
		/// </summary>
		public void UpdateTotal()
		{
			var order = new HarborOrder(TradeView.DataSource);
			TradeTotalAdapter.text = order.Total().ToString();
		}

		static ObservableList<HarborOrderLine> CreateOrderLines(Trader harbor, Trader player)
		{
			var result = new ObservableList<HarborOrderLine>(harbor.Inventory.Count);

			foreach (var item in harbor.Inventory)
			{
				var playerItem = FindItemByName(player.Inventory, item.Name);
				var order_line = new HarborOrderLine(
					item,
					Prices.GetPrice(item, harbor.PriceFactor),
					Prices.GetPrice(item, player.PriceFactor),
					item.Quantity,
					(playerItem == null) ? 0 : playerItem.Quantity);
				result.Add(order_line);
			}

			return result;
		}

		static Item FindItemByName(ObservableList<Item> items, string name)
		{
			foreach (var item in items)
			{
				if (item.Name == name)
				{
					return item;
				}
			}

			return null;
		}

		void UpdateTraderItems()
		{
			TradeView.DataSource = CreateOrderLines(Harbor, Player);
		}

		void UpdatePlayerMoneyInfo()
		{
			PlayerMoneyAdapter.text = Player.Money.ToString();
		}

		void Trade()
		{
			var order = new HarborOrder(TradeView.DataSource);

			if (Player.CanBuy(order))
			{
				Harbor.Sell(order);
				Player.Buy(order);
			}
			else
			{
				var message = string.Format("Not enough money to buy items. Available: {0}; Required: {1}", Player.Money.ToString(), order.Total().ToString());
				NotifyTemplate.Clone().Show(message, customHideDelay: 3f, sequenceType: NotifySequence.First, clearSequence: true);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (BuyButton != null)
			{
				BuyButton.onClick.RemoveListener(Trade);
			}

			if (Harbor != null)
			{
				Harbor.OnItemsChange -= UpdateTraderItems;
			}

			if (Player != null)
			{
				Player.OnItemsChange -= UpdateTraderItems;
				Player.OnMoneyChange -= UpdatePlayerMoneyInfo;
			}
		}

		/// <summary>
			/// Upgrade this instance.
			/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(TradeTotal, ref TradeTotalAdapter);
			Utilities.GetOrAddComponent(PlayerMoney, ref PlayerMoneyAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}