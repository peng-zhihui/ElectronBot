namespace UIWidgets.Examples.Inventory
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UIWidgets.l10n;

	/// <summary>
	/// Item drag.
	/// </summary>
	[RequireComponent(typeof(ItemView))]
	public class ItemDrag : DragSupport<ItemDragData>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public InventoryView Inventory;

		/// <summary>
		/// The drag info.
		/// </summary>
		[SerializeField]
		public ItemView DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);

		/// <summary>
		/// Index of the dragged item.
		/// </summary>
		protected int Index;

		/// <summary>
		/// Delete item from ListView after drop.
		/// </summary>
		[SerializeField]
		public bool DeleteAfterDrop = true;

		ItemView source;

		/// <summary>
		/// Source component.
		/// </summary>
		public ItemView Source
		{
			get
			{
				if (source == null)
				{
					source = GetComponent<ItemView>();
				}

				return source;
			}
		}

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			if (DragInfo != null)
			{
				if (DragInfo.gameObject.GetInstanceID() == gameObject.GetInstanceID())
				{
					DragInfo = null;
					Debug.LogWarning("DragInfo cannot be same gameobject as DragSupport.", this);
				}
				else
				{
					DragInfo.gameObject.SetActive(false);
				}
			}

			Localization.OnLocaleChanged += LocaleChanged;
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			Localization.OnLocaleChanged -= LocaleChanged;
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			if (DragInfo != null)
			{
				DragInfo.LocaleChanged();
			}
		}

		/// <inheritdoc/>
		public override bool CanDrag(PointerEventData eventData)
		{
			if ((Inventory != null) && !Inventory.IsInteractable())
			{
				return false;
			}

			return AllowDrag && Source.Item != null;
		}

		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Source.DisableRecycling = true;
			Data = Source.GetDragData();
			Index = Source.Index;

			ShowDragInfo();
		}

		/// <summary>
		/// Shows the drag info.
		/// </summary>
		protected virtual void ShowDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.transform.SetParent(DragPoint, false);
			DragInfo.transform.localPosition = DragInfoOffset;

			DragInfo.SetData(Data.Item);

			DragInfo.gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides the drag info.
		/// </summary>
		protected virtual void HideDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.gameObject.SetActive(false);
		}

		/// <inheritdoc/>
		public override void Dropped(bool success)
		{
			HideDragInfo();

			Source.DisableRecycling = false;

			Data = ItemDragData.Empty;
		}
	}
}