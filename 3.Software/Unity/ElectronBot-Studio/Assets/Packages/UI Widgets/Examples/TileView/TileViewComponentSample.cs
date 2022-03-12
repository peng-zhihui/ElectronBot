namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class TileViewComponentSample : ListViewItem, IViewData<TileViewItemSample>
	{
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
					UtilitiesUI.GetGraphic(CapitalAdapter),
					UtilitiesUI.GetGraphic(AreaAdapter),
					UtilitiesUI.GetGraphic(PopulationAdapter),
					UtilitiesUI.GetGraphic(DensityAdapter),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with NameAdapter.")]
		public Text Name;

		/// <summary>
		/// The capital.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with CapitalAdapter.")]
		public Text Capital;

		/// <summary>
		/// The area.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with AreaAdapter.")]
		public Text Area;

		/// <summary>
		/// The population.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with PopulationAdapter.")]
		public Text Population;

		/// <summary>
		/// The density.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with DensityAdapter.")]
		public Text Density;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// The capital.
		/// </summary>
		[SerializeField]
		public TextAdapter CapitalAdapter;

		/// <summary>
		/// The area.
		/// </summary>
		[SerializeField]
		public TextAdapter AreaAdapter;

		/// <summary>
		/// The population.
		/// </summary>
		[SerializeField]
		public TextAdapter PopulationAdapter;

		/// <summary>
		/// The density.
		/// </summary>
		[SerializeField]
		public TextAdapter DensityAdapter;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		/// <summary>
		/// TileView.
		/// </summary>
		public TileViewSample Tiles;

		/// <summary>
		/// Current item.
		/// </summary>
		public TileViewItemSample Item;

		/// <summary>
		/// Duplicate current item in TileView.DataSource.
		/// </summary>
		public void Duplicate()
		{
			Tiles.DataSource.Add(Item);
		}

		/// <summary>
		/// Remove current item from TileView.DataSource.
		/// </summary>
		public void Remove()
		{
			Tiles.DataSource.RemoveAt(Index);
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(TileViewItemSample item)
		{
			Item = item;
			if (Item == null)
			{
				if (Icon != null)
				{
					Icon.sprite = null;
				}

				if (NameAdapter != null)
				{
					NameAdapter.text = string.Empty;
				}

				if (CapitalAdapter != null)
				{
					CapitalAdapter.text = string.Empty;
				}

				if (AreaAdapter != null)
				{
					AreaAdapter.text = string.Empty;
				}

				if (PopulationAdapter != null)
				{
					PopulationAdapter.text = string.Empty;
				}

				if (DensityAdapter != null)
				{
					DensityAdapter.text = string.Empty;
				}
			}
			else
			{
				name = item.Name;

				if (Icon != null)
				{
					Icon.sprite = Item.Icon;
				}

				if (NameAdapter != null)
				{
					NameAdapter.text = Item.Name;
				}

				if (CapitalAdapter != null)
				{
					CapitalAdapter.text = string.Format("Capital: {0}", Item.Capital);
				}

				if (AreaAdapter != null)
				{
					AreaAdapter.text = string.Format("Area: {0} sq. km", Item.Area.ToString("N0"));
				}

				if (PopulationAdapter != null)
				{
					PopulationAdapter.text = string.Format("Population: {0}", Item.Population.ToString("N0"));
				}

				if (DensityAdapter != null)
				{
					var density = Item.Area == 0
						? "n/a"
						: string.Format("{0} / sq. km", Mathf.CeilToInt(((float)Item.Population) / Item.Area).ToString("N"));
					DensityAdapter.text = string.Format("Density: {0}", density);
				}
			}

			if (Icon != null)
			{
				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				// set transparent color if no icon
				Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
			Utilities.GetOrAddComponent(Capital, ref CapitalAdapter);
			Utilities.GetOrAddComponent(Area, ref AreaAdapter);
			Utilities.GetOrAddComponent(Population, ref PopulationAdapter);
			Utilities.GetOrAddComponent(Density, ref DensityAdapter);
#pragma warning restore 0612, 0618
		}
	}
}