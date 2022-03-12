namespace UIWidgets.Examples.Shops
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// TraderTreeView component.
	/// </summary>
	public class TraderTreeViewComponent : TreeViewComponentBase<JRPGOrderLine>
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
		[Obsolete("Replaced with NameAdapter.")]
		public Text Price;

		/// <summary>
		/// The available count.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with NameAdapter.")]
		public Text AvailableCount;

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
		/// The available count.
		/// </summary>
		[SerializeField]
		public TextAdapter AvailableCountAdapter;

		/// <summary>
		/// Count spinner.
		/// </summary>
		[SerializeField]
		protected Spinner Count;

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
					UtilitiesUI.GetGraphic(AvailableCountAdapter),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Gets or sets the OrderLine.
		/// </summary>
		/// <value>The OrderLine.</value>
		public JRPGOrderLine OrderLine
		{
			get;
			protected set;
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(TreeNode<JRPGOrderLine> node, int depth)
		{
			Node = node;
			base.SetData(Node, depth);

			OrderLine = Node.Item;

			UpdateView();
		}

		/// <summary>
		/// Delete node from TreeView.
		/// </summary>
		public void Delete()
		{
			Node.Parent = null;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (OrderLine.IsPlaylist)
			{
				NameAdapter.text = OrderLine.Item.Name;

				// disable unused gameobjects
				PriceAdapter.transform.parent.gameObject.SetActive(false);
				AvailableCountAdapter.gameObject.SetActive(false);
				Count.transform.parent.gameObject.SetActive(false);
			}
			else
			{
				// enable unused gameobjects
				PriceAdapter.transform.parent.gameObject.SetActive(true);
				AvailableCountAdapter.gameObject.SetActive(true);
				Count.transform.parent.gameObject.SetActive(true);

				NameAdapter.text = OrderLine.Item.Name;
				PriceAdapter.text = OrderLine.Price.ToString();
				AvailableCountAdapter.text = (OrderLine.Item.Quantity == -1) ? "∞" : OrderLine.Item.Quantity.ToString();

				Count.Min = 0;
				Count.Max = (OrderLine.Item.Quantity == -1) ? 9999 : OrderLine.Item.Quantity;
				Count.Value = OrderLine.Quantity;
			}
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void Start()
		{
			Count.onValueChangeInt.AddListener(ChangeCount);
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
					Count.Value -= 1;
					break;
				case MoveDirection.Right:
					Count.Value += 1;
					break;
				default:
					base.OnMove(eventData);
					break;
			}
		}

		void ChangeCount(int quantity)
		{
			OrderLine.Quantity = quantity;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void OnDestroy()
		{
			if (Count != null)
			{
				Count.onValueChangeInt.RemoveListener(ChangeCount);
			}
		}

		#region edit playlist name

		/// <summary>
		/// EditPlaylistDialog template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("editPlaylistDialog")]
		protected Dialog EditPlaylistDialogTemplate;

		/// <summary>
		/// Open EditPlaylistDialog.
		/// </summary>
		public void EditPlaylistDialog()
		{
			// if not playlist do nothing
			if (!OrderLine.IsPlaylist)
			{
				return;
			}

			// create dialog from template
			var dialog = EditPlaylistDialogTemplate.Clone();

			// helper component with references to input fields
			var helper = dialog.GetComponent<PlaylistDialogHelper>();

			// reset input fields to default
			helper.NameAdapter.text = OrderLine.Item.Name;

			var actions = new DialogButton[]
			{
				new DialogButton("Change", CheckPlaylistDialog),

				// on click close dialog
				new DialogButton("Cancel", DialogBase.DefaultClose),
			};

			// open dialog
			dialog.Show(
				title: "Change playlist name",
				buttons: actions,
				focusButton: "Sign in",
				modal: true,
				modalColor: new Color(0, 0, 0, 0.8f));
		}

		/// <summary>
		/// Check EditPlaylistDialog and change dialog name if valid.
		/// </summary>
		/// <param name="dialog">Dialog.</param>
		/// <param name="index">Index.</param>
		/// <returns>true if values valid; otherwise false.</returns>
		protected bool CheckPlaylistDialog(DialogBase dialog, int index)
		{
			var helper = dialog.GetComponent<PlaylistDialogHelper>();
			if (!helper.Validate())
			{
				// return false to keep dialog open
				return false;
			}

			// change name in node
			OrderLine.Item.Name = helper.NameAdapter.text;

			// change displayed name
			NameAdapter.text = helper.NameAdapter.text;

			// return true to close dialog
			return true;
		}
		#endregion

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
			Utilities.GetOrAddComponent(Price, ref PriceAdapter);
			Utilities.GetOrAddComponent(AvailableCount, ref AvailableCountAdapter);
#pragma warning restore 0612, 0618
		}
	}
}