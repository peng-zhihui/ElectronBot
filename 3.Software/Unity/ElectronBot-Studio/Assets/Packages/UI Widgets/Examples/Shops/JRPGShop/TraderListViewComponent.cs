namespace UIWidgets.Examples.Shops
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Trader list view component.
	/// </summary>
	public class TraderListViewComponent : ListViewItem, IViewData<JRPGOrderLine>
	{
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with NameAdapter.")]
		public Text Name;

		/// <summary>
		/// The price.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with PriceAdapter.")]
		public Text Price;

		/// <summary>
		/// The available quantity.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with AvailableCountAdapter.")]
		[FormerlySerializedAs("AvailableCount")]
		public Text AvailableQuantity;

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// The price.
		/// </summary>
		[SerializeField]
		public TextAdapter PriceAdapter;

		/// <summary>
		/// The available quantity.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("AvailableCountAdapter")]
		public TextAdapter AvailableQuantityAdapter;

		/// <summary>
		/// Quantity spinner.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("Count")]
		protected Spinner Quantity;

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
					UtilitiesUI.GetGraphic(PriceAdapter),
					UtilitiesUI.GetGraphic(AvailableQuantityAdapter),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// OrderLine.
		/// </summary>
		public JRPGOrderLine OrderLine
		{
			get;
			protected set;
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void Start()
		{
			Quantity.onValueChangeInt.AddListener(ChangeQuantity);
			base.Start();
		}

		/// <summary>
		/// Change quantity on left and right movements.
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
		public void SetData(JRPGOrderLine item)
		{
			OrderLine = item;

			NameAdapter.text = OrderLine.Item.Name;
			PriceAdapter.text = OrderLine.Price.ToString();
			AvailableQuantityAdapter.text = (OrderLine.Item.Quantity == -1) ? "∞" : OrderLine.Item.Quantity.ToString();

			Quantity.Min = 0;
			Quantity.Max = (OrderLine.Item.Quantity == -1) ? 9999 : OrderLine.Item.Quantity;
			Quantity.Value = OrderLine.Quantity;
		}

		void ChangeQuantity(int quantity)
		{
			OrderLine.Quantity = quantity;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void OnDestroy()
		{
			if (Quantity != null)
			{
				Quantity.onValueChangeInt.RemoveListener(ChangeQuantity);
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
			Utilities.GetOrAddComponent(Price, ref PriceAdapter);
			Utilities.GetOrAddComponent(AvailableQuantity, ref AvailableQuantityAdapter);
#pragma warning restore 0612, 0618
		}
	}
}