namespace UIWidgets.Examples.Inventory
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Inventory tooltip.
	/// </summary>
	public class InventoryTooltip : Tooltip<Item, InventoryTooltip>
	{
		[Serializable]
		public class View
		{
			/// <summary>
			/// Name view.
			/// </summary>
			[SerializeField]
			public TextAdapter NameView;

			/// <summary>
			/// Weight view.
			/// </summary>
			[SerializeField]
			public TextAdapter WeightView;

			/// <summary>
			/// Price view.
			/// </summary>
			[SerializeField]
			public TextAdapter PriceView;

			public void SetActive(bool active)
			{
				NameView.gameObject.SetActive(active);
				WeightView.gameObject.SetActive(active);
				PriceView.gameObject.SetActive(active);
			}

			public void Show(Item item)
			{
				NameView.text = item.Name;
				WeightView.text = string.Format("Weight: <b>{0:0.00}</b>", item.Weight);
				PriceView.text = string.Format("Price: <b>{0}</b>", item.Price);
				SetActive(true);
			}

			public void ShowWithDiff(Item item, Item compareItem, Color colorHigher, Color colorLower)
			{
				NameView.text = item.Name;

				WeightView.text = GetHighlightedWeightText(item, compareItem, colorHigher, colorLower);
				PriceView.text = GetHighlightedPriceText(item, compareItem, colorHigher, colorLower);
				SetActive(true);
			}

			string GetHighlightedWeightText(Item item, Item compareItem, Color colorHigher, Color colorLower)
			{
				var diff = item.Weight - compareItem.Weight;
				if (Mathf.Approximately(diff, 0f))
				{
					return string.Format("Weight: <b>{0:0.00}</b>", item.Weight);
				}

				var color = (diff > 0f) ? colorHigher : colorLower;
				return string.Format("Weight: <b>{0:0.00} <color=#{1}>({2:+0.00;-0.00})</color></b>",
					item.Weight, ColorUtility.ToHtmlStringRGBA(color), diff);
			}

			string GetHighlightedPriceText(Item item, Item compareItem, Color colorHigher, Color colorLower)
			{
				var diff = item.Price - compareItem.Price;
				if (Mathf.Approximately(diff, 0f))
				{
					return string.Format("Price: <b>{0}</b>", item.Price);
				}

				var color = (diff > 0f) ? colorHigher : colorLower;
				return string.Format("Price: <b>{0} <color=#{1}>({2:+0;-0})</color></b>",
					item.Price, ColorUtility.ToHtmlStringRGBA(color), diff);
			}
		}

		/// <summary>
		/// Selected view.
		/// </summary>
		[SerializeField]
		public View SelectedView;

		/// <summary>
		/// Highlighted view.
		/// </summary>
		[SerializeField]
		public View HighlightedView;

		/// <summary>
		/// Selected item.
		/// </summary>
		public Item SelectedItem
		{
			get;
			protected set;
		}

		/// <summary>
		/// Highlighted item.
		/// </summary>
		public Item HighlightedItem
		{
			get;
			protected set;
		}

		/// <summary>
		/// Color if highlighted value is higher.
		/// </summary>
		public Color HighlightedValueHigher = Color.green;

		/// <summary>
		/// Color if highlighted value is lower.
		/// </summary>
		public Color HighlightedValueLower = Color.red;

		/// <summary>
		/// Set selected.
		/// </summary>
		/// <param name="data">Data.</param>
		public void SetSelected(Item data)
		{
			SelectedItem = data;
			UpdateView();
		}

		/// <inheritdoc/>
		protected override void SetData(Item data)
		{
			HighlightedItem = data;
			UpdateView();
		}

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			if (SelectedItem != null)
			{
				SelectedView.Show(SelectedItem);

				if ((HighlightedItem != null) && (SelectedItem != HighlightedItem))
				{
					HighlightedView.ShowWithDiff(HighlightedItem, SelectedItem, HighlightedValueHigher, HighlightedValueLower);
				}
				else
				{
					HighlightedView.SetActive(false);
				}
			}
			else
			{
				if (HighlightedItem != null)
				{
					SelectedView.Show(HighlightedItem);
				}
				else
				{
					SelectedView.SetActive(false);
				}
					
				HighlightedView.SetActive(false);
			}
		}
	}
}