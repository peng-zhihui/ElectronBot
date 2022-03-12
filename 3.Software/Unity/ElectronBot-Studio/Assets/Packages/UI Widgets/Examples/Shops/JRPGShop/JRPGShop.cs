namespace UIWidgets.Examples.Shops
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// JRPG shop.
	/// </summary>
	public class JRPGShop : MonoBehaviour, IUpgradeable
	{
		Trader Shop;

		/// <summary>
		/// Shop items ListView.
		/// </summary>
		[SerializeField]
		protected TraderListView ShopItems;

		/// <summary>
		/// Buy button.
		/// </summary>
		[SerializeField]
		protected Button BuyButton;

		/// <summary>
		/// Text component to display buy total.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with ShopTotalAdapter.")]
		protected Text ShopTotal;

		/// <summary>
		/// Text component to display buy total.
		/// </summary>
		[SerializeField]
		protected TextAdapter ShopTotalAdapter;

		Trader Player;

		/// <summary>
		/// Player items ListView.
		/// </summary>
		[SerializeField]
		protected TraderListView PlayerItems;

		/// <summary>
		/// Sell button.
		/// </summary>
		[SerializeField]
		protected Button SellButton;

		/// <summary>
		/// Text component to display player money.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with ShopTotalAdapter.")]
		protected Text PlayerMoney;

		/// <summary>
		/// Text component to display sell total.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with ShopTotalAdapter.")]
		protected Text PlayerTotal;

		/// <summary>
		/// Text component to display player money.
		/// </summary>
		[SerializeField]
		protected TextAdapter PlayerMoneyAdapter;

		/// <summary>
		/// Text component to display sell total.
		/// </summary>
		[SerializeField]
		protected TextAdapter PlayerTotalAdapter;

		/// <summary>
		/// Notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notify")]
		protected Notify NotifyTemplate;

		/// <summary>
		/// Start and adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			Shop = new Trader();
			Player = new Trader();

			Init();

			BuyButton.onClick.AddListener(Buy);
			SellButton.onClick.AddListener(Sell);

			Shop.OnItemsChange += UpdateShopItems;

			Player.OnItemsChange += UpdatePlayerItems;
			Player.OnMoneyChange += UpdatePlayerMoneyInfo;

			UpdateShopItems();

			UpdatePlayerItems();
			UpdatePlayerMoneyInfo();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			Shop.Money = -1;
			Shop.PriceFactor = 1;
			Shop.Inventory.Clear();
			var shop_items = new List<Item>()
			{
				new Item("Sword", 10),
				new Item("Short Sword", 5),
				new Item("Long Sword", 5),
				new Item("Knife", -1),
				new Item("Dagger", -1),
				new Item("Hammer", -1),
				new Item("Shield", -1),
				new Item("Leather Armor", 3),
				new Item("Ring", 2),
				new Item("Bow", -1),
				new Item("Crossbow", -1),

				new Item("HP Potion", -1),
				new Item("Mana Potion", -1),
				new Item("HP UP", 10),
				new Item("Mana UP", 10),
			};

			Shop.Inventory.AddRange(shop_items);

			Player.Money = 2000;
			Player.PriceFactor = 0.5f;
			Player.Inventory.Clear();
			var player_items = new List<Item>()
			{
				new Item("Stick", 1),
				new Item("Sword", 1),
				new Item("HP Potion", 5),
				new Item("Mana Potion", 5),
			};

			Player.Inventory.AddRange(player_items);
		}

		/// <summary>
		/// Updates the shop total.
		/// </summary>
		public void UpdateShopTotal()
		{
			var order = new JRPGOrder(ShopItems.DataSource);
			ShopTotalAdapter.text = order.Total().ToString();
		}

		/// <summary>
		/// Updates the player total.
		/// </summary>
		public void UpdatePlayerTotal()
		{
			var order = new JRPGOrder(PlayerItems.DataSource);
			PlayerTotalAdapter.text = order.Total().ToString();
		}

		static ObservableList<JRPGOrderLine> CreateOrderLines(Trader trader)
		{
			var result = new ObservableList<JRPGOrderLine>(trader.Inventory.Count);
			foreach (var item in trader.Inventory)
			{
				result.Add(new JRPGOrderLine(item, Prices.GetPrice(item, trader.PriceFactor)));
			}

			return result;
		}

		void UpdateShopItems()
		{
			ShopItems.DataSource = CreateOrderLines(Shop);
		}

		void UpdatePlayerItems()
		{
			PlayerItems.DataSource = CreateOrderLines(Player);
		}

		void UpdatePlayerMoneyInfo()
		{
			PlayerMoneyAdapter.text = Player.Money.ToString();
		}

		void Buy()
		{
			var order = new JRPGOrder(ShopItems.DataSource);

			if (Player.CanBuy(order))
			{
				Shop.Sell(order);
				Player.Buy(order);
			}
			else
			{
				var message = string.Format("Not enough money to buy items. Available: {0}; Required: {1}", Player.Money.ToString(), order.Total().ToString());
				NotifyTemplate.Clone().Show(message, customHideDelay: 3f, sequenceType: NotifySequence.First, clearSequence: true);
			}
		}

		void Sell()
		{
			var order = new JRPGOrder(PlayerItems.DataSource);

			if (Shop.CanBuy(order))
			{
				Shop.Buy(order);
				Player.Sell(order);
			}
			else
			{
				var message = string.Format("Not enough money in shop to sell items. Available: {0}; Required: {1}", Shop.Money.ToString(), order.Total().ToString());
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
				BuyButton.onClick.RemoveListener(Buy);
			}

			if (SellButton != null)
			{
				SellButton.onClick.RemoveListener(Sell);
			}

			if (Shop != null)
			{
				Shop.OnItemsChange -= UpdateShopItems;
			}

			if (Player != null)
			{
				Player.OnItemsChange -= UpdatePlayerItems;
				Player.OnMoneyChange -= UpdatePlayerMoneyInfo;
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(ShopTotal, ref ShopTotalAdapter);
			Utilities.GetOrAddComponent(PlayerMoney, ref PlayerMoneyAdapter);
			Utilities.GetOrAddComponent(PlayerTotal, ref PlayerTotalAdapter);
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