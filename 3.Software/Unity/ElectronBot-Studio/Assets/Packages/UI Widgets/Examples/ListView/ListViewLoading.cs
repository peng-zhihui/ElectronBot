namespace UIWidgets.Examples
{
	using System.Collections;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// ListView loading.
	/// </summary>
	public class ListViewLoading : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// DefaultItem during loading.
		/// </summary>
		[SerializeField]
		public ListViewIconsItemComponent DefaultItemLoading;

		/// <summary>
		/// DefaultItem after loading.
		/// </summary>
		[SerializeField]
		public ListViewIconsItemComponent DefaultItemActual;

		/// <summary>
		/// Start loading.
		/// </summary>
		public void Start()
		{
			StartCoroutine(Loading());
		}

		static ObservableList<ListViewIconsItemDescription> GetNullData(int max)
		{
			var null_data = new ObservableList<ListViewIconsItemDescription>(max);
			for (int i = 0; i < max; i++)
			{
				null_data.Add(null);
			}

			return null_data;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		IEnumerator Loading()
		{
			var actual_item = (DefaultItemActual != null)
				? DefaultItemActual
				: ListView.DefaultItem;
			var actual_data = ListView.DataSource;

			ListView.Interactable = false;
			ListView.DefaultItem = DefaultItemLoading;
			ListView.DataSource = GetNullData(ListView.MaxVisibleItems);

			// imitate data loading
			yield return UtilitiesTime.Wait(5f, true);

			ListView.Interactable = true;
			ListView.DefaultItem = actual_item;
			ListView.DataSource = actual_data;
		}
	}
}