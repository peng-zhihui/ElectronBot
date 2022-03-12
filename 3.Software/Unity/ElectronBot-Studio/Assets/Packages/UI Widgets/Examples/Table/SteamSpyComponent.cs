namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SteamSpy component.
	/// </summary>
	public class SteamSpyComponent : ListViewItem, IViewData<SteamSpyItem>
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
					UtilitiesUI.GetGraphic(ScoreRankAdapter),
					UtilitiesUI.GetGraphic(OwnersAdapter),
					UtilitiesUI.GetGraphic(PlayersAdapter),
					UtilitiesUI.GetGraphic(PlayersIn2WeekAdapter),
					UtilitiesUI.GetGraphic(TimeIn2WeekAdapter),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				graphicsBackground = new Graphic[]
				{
					(NameAdapter != null) ? NameAdapter.transform.parent.GetComponent<Graphic>() : null,
					(ScoreRankAdapter != null) ? ScoreRankAdapter.transform.parent.GetComponent<Graphic>() : null,
					(OwnersAdapter != null) ? OwnersAdapter.transform.parent.GetComponent<Graphic>() : null,
					(PlayersAdapter != null) ? PlayersAdapter.transform.parent.GetComponent<Graphic>() : null,
					(PlayersIn2WeekAdapter != null) ? PlayersIn2WeekAdapter.transform.parent.GetComponent<Graphic>() : null,
					(TimeIn2WeekAdapter != null) ? TimeIn2WeekAdapter.transform.parent.GetComponent<Graphic>() : null,
				};
				GraphicsBackgroundVersion = 1;
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NameAdapter.")]
		public Text Name;

		/// <summary>
		/// ScoreRank.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with ScoreRankAdapter.")]
		public Text ScoreRank;

		/// <summary>
		/// Owners.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with OwnersAdapter.")]
		public Text Owners;

		/// <summary>
		/// Players.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with PlayersAdapter.")]
		public Text Players;

		/// <summary>
		/// PlayersIn2Week.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with PlayersIn2WeekAdapter.")]
		public Text PlayersIn2Week;

		/// <summary>
		/// TimeIn2Week.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TimeIn2WeekAdapter.")]
		public Text TimeIn2Week;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// ScoreRank.
		/// </summary>
		[SerializeField]
		public TextAdapter ScoreRankAdapter;

		/// <summary>
		/// Owners.
		/// </summary>
		[SerializeField]
		public TextAdapter OwnersAdapter;

		/// <summary>
		/// Players.
		/// </summary>
		[SerializeField]
		public TextAdapter PlayersAdapter;

		/// <summary>
		/// PlayersIn2Week.
		/// </summary>
		[SerializeField]
		public TextAdapter PlayersIn2WeekAdapter;

		/// <summary>
		/// TimeIn2Week.
		/// </summary>
		[SerializeField]
		public TextAdapter TimeIn2WeekAdapter;

		/// <summary>
		/// TooltipText.
		/// </summary>
		[SerializeField]
		public TooltipString Tooltip;

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				return new GameObject[]
				{
					NameAdapter.transform.parent.gameObject,
					ScoreRankAdapter.transform.parent.gameObject,
					OwnersAdapter.transform.parent.gameObject,
					PlayersAdapter.transform.parent.gameObject,
					PlayersIn2WeekAdapter.transform.parent.gameObject,
					TimeIn2WeekAdapter.transform.parent.gameObject,
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SteamSpyItem item)
		{
			NameAdapter.text = item.Name;
			ScoreRankAdapter.text = (item.ScoreRank == -1) ? string.Empty : item.ScoreRank.ToString();
			OwnersAdapter.text = string.Format("{0}\n±{1}", item.Owners.ToString("N0"), item.OwnersVariance.ToString("N0"));
			PlayersAdapter.text = string.Format("{0}\n±{1}", item.Players.ToString("N0"), item.PlayersVariance.ToString("N0"));
			PlayersIn2WeekAdapter.text = string.Format("{0}\n±{1}", item.PlayersIn2Week.ToString("N0"), item.PlayersIn2WeekVariance.ToString("N0"));
			TimeIn2WeekAdapter.text = string.Format("{0}\n({1})", Minutes2String(item.AverageTimeIn2Weeks), Minutes2String(item.MedianTimeIn2Weeks));

			if (Tooltip != null)
			{
				var target = NameAdapter.transform.parent as RectTransform;
				Tooltip.Register(target.gameObject, item.Name, new TooltipSettings(TooltipPosition.TopRight, parent: target));
			}
		}

		/// <inheritdoc/>
		public override void MovedToCache()
		{
			base.MovedToCache();

			if (Tooltip != null)
			{
				Tooltip.Unregister(NameAdapter.transform.parent.gameObject);
			}
		}

		static string Minutes2String(int minutes)
		{
			return string.Format("{0}:{1}", (minutes / 60).ToString("00"), (minutes % 60).ToString("00"));
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
			Utilities.GetOrAddComponent(ScoreRank, ref ScoreRankAdapter);
			Utilities.GetOrAddComponent(Owners, ref OwnersAdapter);
			Utilities.GetOrAddComponent(Players, ref PlayersAdapter);
			Utilities.GetOrAddComponent(PlayersIn2Week, ref PlayersIn2WeekAdapter);
			Utilities.GetOrAddComponent(TimeIn2Week, ref TimeIn2WeekAdapter);
#pragma warning restore 0612, 0618
		}
	}
}