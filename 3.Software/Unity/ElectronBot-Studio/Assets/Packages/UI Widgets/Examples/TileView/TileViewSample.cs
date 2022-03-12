namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TileViewSample.
	/// </summary>
	public class TileViewSample : TileViewCustom<TileViewComponentSample, TileViewItemSample>
	{
		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public override void Init()
		{
			ItemsEvents.Select.AddListener(ItemSelected);
			base.Init();
		}

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void OnDestroy()
		{
			ItemsEvents.Select.RemoveListener(ItemSelected);
			base.OnDestroy();
		}

		void ItemSelected(int index, ListViewItem component, BaseEventData eventData)
		{
			if (component != null)
			{
				// (component as TileViewComponentSample).DoSomething();
			}

			Debug.Log(string.Format("{0}: {1}", index.ToString(), DataSource[index].Name));
		}
	}
}