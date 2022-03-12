namespace UIWidgets.Examples.Shops
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// HarborListViewComponent.
	/// </summary>
	public class HarborListViewComponent : ListViewItem, IViewData<HarborOrderLine>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NameAdapter.")]
		public Text Name;

		/// <summary>
		/// Sell price.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with SellPriceAdapter.")]
		public Text SellPrice;

		/// <summary>
		/// Buy price.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with BuyPriceAdapter.")]
		public Text BuyPrice;

		/// <summary>
		/// Available buy count.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with AvailableBuyCountAdapter.")]
		public Text AvailableBuyCount;

		/// <summary>
		/// Available sell count.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with AvailableSellCountAdapter.")]
		public Text AvailableSellCount;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Sell price.
		/// </summary>
		[SerializeField]
		public TextAdapter SellPriceAdapter;

		/// <summary>
		/// Buy price.
		/// </summary>
		[SerializeField]
		public TextAdapter BuyPriceAdapter;

		/// <summary>
		/// Available buy count.
		/// </summary>
		[SerializeField]
		public TextAdapter AvailableBuyCountAdapter;

		/// <summary>
		/// Available sell count.
		/// </summary>
		[SerializeField]
		public TextAdapter AvailableSellCountAdapter;

		/// <summary>
		/// Quantity slider.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Count")]
		protected CenteredSlider Quantity;

		/// <summary>
		/// Current order line.
		/// </summary>
		public HarborOrderLine OrderLine;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[]
				{
					UtilitiesUI.GetGraphic(NameAdapter),
					UtilitiesUI.GetGraphic(BuyPriceAdapter),
					UtilitiesUI.GetGraphic(SellPriceAdapter),
					UtilitiesUI.GetGraphic(AvailableBuyCountAdapter),
					UtilitiesUI.GetGraphic(AvailableSellCountAdapter),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void Start()
		{
			Quantity.OnValueChanged.AddListener(ChangeCount);
			base.Start();
		}

		/// <summary>
		/// Change count on left and right movements.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public override void OnMove(AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					Quantity.Value -= 1;
					break;
				case MoveDirection.Right:
					Quantity.Value += 1;
					break;
				default:
					base.OnMove(eventData);
					break;
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="item">Order line.</param>
		public void SetData(HarborOrderLine item)
		{
			OrderLine = item;

			NameAdapter.text = OrderLine.Item.Name;

			BuyPriceAdapter.text = OrderLine.BuyPrice.ToString();
			SellPriceAdapter.text = OrderLine.SellPrice.ToString();

			AvailableBuyCountAdapter.text = OrderLine.BuyQuantity.ToString();
			AvailableSellCountAdapter.text = OrderLine.SellQuantity.ToString();

			Quantity.LimitMin = -OrderLine.SellQuantity;
			Quantity.LimitMax = OrderLine.BuyQuantity;
			Quantity.Value = OrderLine.Quantity;
		}

		void ChangeCount(int value)
		{
			OrderLine.Quantity = value;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void OnDestroy()
		{
			if (Quantity != null)
			{
				Quantity.OnValueChanged.RemoveListener(ChangeCount);
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
			Utilities.GetOrAddComponent(BuyPrice, ref BuyPriceAdapter);
			Utilities.GetOrAddComponent(SellPrice, ref SellPriceAdapter);
			Utilities.GetOrAddComponent(AvailableBuyCount, ref AvailableBuyCountAdapter);
			Utilities.GetOrAddComponent(AvailableSellCount, ref AvailableSellCountAdapter);
#pragma warning restore 0612, 0618
		}
	}
}