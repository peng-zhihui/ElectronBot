namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;

	/// <summary>
	/// DropSupport for ListViewCustom.
	/// </summary>
	/// <typeparam name="TListView">ListView type.</typeparam>
	/// <typeparam name="TComponent">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	public class ListViewCustomDropSupport<TListView, TComponent, TItem> : MonoBehaviour, IDropSupport<TItem>, IStylable, IDropSupport<TreeNode<TItem>>
		where TListView : ListViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
		TListView listView;

		/// <summary>
		/// Current ListView.
		/// </summary>
		/// <value>ListView.</value>
		public TListView ListView
		{
			get
			{
				if (listView == null)
				{
					listView = GetComponent<TListView>();
				}

				return listView;
			}
		}

		/// <summary>
		/// Drop position.
		/// </summary>
		[SerializeField]
		public NearestType DropPosition = NearestType.Auto;

		/// <summary>
		/// The drop indicator.
		/// </summary>
		[SerializeField]
		public ListViewDropIndicator DropIndicator;

		/// <summary>
		/// Delete dropped node.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("DeleteDroppedNode")]
		[FormerlySerializedAs("DeleteTreeNodeAfterDrop")]
		public bool DeleteNodeAfterDrop = true;

		/// <summary>
		/// Receive dropped items.
		/// </summary>
		[SerializeField]
		public bool ReceiveItems = true;

		/// <summary>
		/// Receive dropped nodes.
		/// </summary>
		[SerializeField]
		public bool ReceiveNodes = true;

		/// <summary>
		/// Receive only nodes without nested nodes.
		/// </summary>
		[SerializeField]
		[Tooltip("Allow only nodes without nested nodes.")]
		public bool ReceiveOnlyEmptyNode = true;

		/// <summary>
		/// Index of the last inserted item.
		/// </summary>
		public int LastInsertedIndex
		{
			get;
			protected set;
		}

		#region IDropSupport<TItem>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual bool CanReceiveDrop(TItem data, PointerEventData eventData)
		{
			if (!ListView.IsInteractable())
			{
				return false;
			}

			if (!ReceiveItems)
			{
				return false;
			}

			var index = ListView.GetNearestIndex(eventData, DropPosition);

			ShowDropIndicator(index);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void Drop(TItem data, PointerEventData eventData)
		{
			var index = ListView.GetNearestIndex(eventData, DropPosition);

			AddItem(data, index);

			HideDropIndicator();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void DropCanceled(TItem data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion

		#region IDropSupport<TreeNode<TItem>>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual bool CanReceiveDrop(TreeNode<TItem> data, PointerEventData eventData)
		{
			if (!ListView.IsInteractable())
			{
				return false;
			}

			if (!ReceiveNodes)
			{
				return false;
			}

			var has_subnodes = (data.Nodes != null) && (data.Nodes.Count > 0);
			if (ReceiveOnlyEmptyNode && has_subnodes)
			{
				return false;
			}

			var index = ListView.GetNearestIndex(eventData, DropPosition);

			ShowDropIndicator(index);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void Drop(TreeNode<TItem> data, PointerEventData eventData)
		{
			var index = ListView.GetNearestIndex(eventData, DropPosition);

			AddItem(data.Item, index);

			HideDropIndicator();

			if (DeleteNodeAfterDrop)
			{
				data.Parent = null;
			}
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void DropCanceled(TreeNode<TItem> data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ShowDropIndicator(int index)
		{
			if (ListView.ListType == ListViewType.ListViewEllipse)
			{
				return;
			}

			if (DropIndicator != null)
			{
				DropIndicator.Show(index, ListView);
			}
		}

		/// <summary>
		/// Hides the drop indicator.
		/// </summary>
		protected virtual void HideDropIndicator()
		{
			if (DropIndicator != null)
			{
				DropIndicator.Hide();
			}
		}

		/// <summary>
		/// Add item to the ListView.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		protected virtual void AddItem(TItem item, int index)
		{
			if (index > ListView.DataSource.Count)
			{
				index = ListView.DataSource.Count;
			}

			LastInsertedIndex = index;

			if (index == -1)
			{
				ListView.DataSource.Add(item);
			}
			else
			{
				ListView.DataSource.Insert(index, item);
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (DropIndicator != null)
			{
				DropIndicator.SetStyle(style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (DropIndicator != null)
			{
				DropIndicator.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}