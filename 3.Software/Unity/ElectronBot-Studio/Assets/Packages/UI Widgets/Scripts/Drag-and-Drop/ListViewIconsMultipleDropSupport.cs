namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Drop support of multiple items for the ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class ListViewIconsMultipleDropSupport : MonoBehaviour, IDropSupport<List<ListViewIconsItemDescription>>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewIcons ListView;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			ListView = GetComponent<ListViewIcons>();
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(List<ListViewIconsItemDescription> data, PointerEventData eventData)
		{
			ListView.DataSource.AddRange(data);
		}

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(List<ListViewIconsItemDescription> data, PointerEventData eventData)
		{
			return true;
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(List<ListViewIconsItemDescription> data, PointerEventData eventData)
		{
		}
	}
}