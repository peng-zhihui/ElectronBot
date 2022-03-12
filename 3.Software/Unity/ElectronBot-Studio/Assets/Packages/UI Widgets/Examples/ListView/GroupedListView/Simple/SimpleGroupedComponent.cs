namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SimpleGroupedComponent.
	/// </summary>
	public class SimpleGroupedComponent : ListViewItem, IViewData<SimpleGroupedItem>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public SimpleGroupedItem Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(SimpleGroupedItem item)
		{
			Item = item;
			Name.text = item.Name;

			if (item.IsGroup)
			{
				Name.fontSize = 20;
				Name.fontStyle = FontStyle.Bold;
				Name.alignment = TextAnchor.MiddleCenter;
			}
			else
			{
				Name.fontSize = 14;
				Name.fontStyle = FontStyle.Normal;
				Name.alignment = TextAnchor.MiddleLeft;
			}
		}
	}
}