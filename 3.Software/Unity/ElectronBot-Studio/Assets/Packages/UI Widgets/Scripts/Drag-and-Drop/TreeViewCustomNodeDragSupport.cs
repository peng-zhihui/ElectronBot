namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;

	/// <summary>
	/// TreeViewCustomNode drag support.
	/// </summary>
	/// <typeparam name="TTreeViewComponent">Type of TreeView.DefaultItem.</typeparam>
	/// <typeparam name="TListViewComponent">Type of ListView.DefaultItem</typeparam>
	/// <typeparam name="TItem">Type of draggable item.</typeparam>
	public class TreeViewCustomNodeDragSupport<TTreeViewComponent, TListViewComponent, TItem> : DragSupport<TreeNode<TItem>>
		where TTreeViewComponent : TreeViewComponentBase<TItem>
		where TListViewComponent : ListViewItem, IViewData<TItem>
	{
		/// <summary>
		/// Component to display draggable info.
		/// </summary>
		[SerializeField]
		public TListViewComponent DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("LocalPosition")]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);

		TTreeViewComponent source;

		/// <summary>
		/// TreeViewComponent.
		/// </summary>
		protected virtual TTreeViewComponent Source
		{
			get
			{
				if (source == null)
				{
					source = GetComponent<TTreeViewComponent>();
				}

				return source;
			}
		}

		TreeViewCustom<TTreeViewComponent, TItem> treeView;

		/// <summary>
		/// TreeView instance.
		/// </summary>
		protected virtual TreeViewCustom<TTreeViewComponent, TItem> TreeView
		{
			get
			{
				if (treeView == null)
				{
					treeView = Source.Owner as TreeViewCustom<TTreeViewComponent, TItem>;
				}

				return treeView;
			}
		}

		/// <summary>
		/// Draggable node.
		/// </summary>
		protected virtual TreeNode<TItem> Node
		{
			get
			{
				return Source.Node;
			}
		}

		/*
		/// <summary>
		/// Delete item from TreeView after drop.
		/// </summary>
		[SerializeField]
		public bool DeleteAfterDrop = true;
		*/

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (DragInfo != null)
			{
				DragInfo.gameObject.SetActive(false);
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

		/// <summary>
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		public override bool CanDrag(PointerEventData eventData)
		{
			if (!Source.IsInteractable())
			{
				return false;
			}

			return AllowDrag;
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = Node;
			Source.DisableRecycling = true;

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

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			HideDragInfo();

			Source.DisableRecycling = false;

			base.Dropped(success);
		}
	}
}