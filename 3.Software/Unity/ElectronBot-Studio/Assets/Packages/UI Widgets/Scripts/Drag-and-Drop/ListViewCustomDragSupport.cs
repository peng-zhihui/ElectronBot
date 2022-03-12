namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// DragSupport for ListViewCustom.
	/// </summary>
	/// <typeparam name="TListView">ListView type.</typeparam>
	/// <typeparam name="TComponent">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	public abstract class ListViewCustomDragSupport<TListView, TComponent, TItem> : DragSupport<TItem>
		where TListView : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public TListView ListView;

		/// <summary>
		/// The drag info.
		/// </summary>
		[SerializeField]
		public TComponent DragInfo;

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

		TComponent source;

		/// <summary>
		/// Source component.
		/// </summary>
		public TComponent Source
		{
			get
			{
				if (source == null)
				{
					source = GetComponent<TComponent>();
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
			if (!ListView.IsInteractable())
			{
				return false;
			}

			return AllowDrag;
		}

		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Source.DisableRecycling = true;
			Data = GetData(Source);
			Index = Source.Index;

			ShowDragInfo();
		}

		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected abstract TItem GetData(TComponent component);

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected abstract void SetDragInfoData(TItem data);

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

			SetDragInfoData(Data);

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

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			HideDragInfo();

			// remove used from the current ListView.
			if (DeleteAfterDrop && success && (ListView != null))
			{
				var first_index = ListView.DataSource.IndexOf(Data);
				var last_index = ListView.DataSource.LastIndexOf(Data);
				if (Index == first_index)
				{
					ListView.DataSource.RemoveAt(Index);
				}
				else if ((Index + 1) == last_index)
				{
					ListView.DataSource.RemoveAt(Index + 1);
				}
				else
				{
					ListView.DataSource.Remove(Data);
				}
			}

			Source.DisableRecycling = false;

			base.Dropped(success);
		}
	}
}