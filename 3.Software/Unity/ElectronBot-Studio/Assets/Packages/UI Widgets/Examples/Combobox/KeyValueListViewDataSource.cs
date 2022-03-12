namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// KeyValueListView data source.
	/// </summary>
	[RequireComponent(typeof(KeyValueListView))]
	public class KeyValueListViewDataSource : MonoBehaviour
	{
		/// <summary>
		/// Add data to KeyValueListView.
		/// </summary>
		protected virtual void Start()
		{
			var items = GetComponent<KeyValueListView>().DataSource;

			items.BeginUpdate();

			items.Add(new KeyValuePair<string, string>("AT", "Austria"));
			items.Add(new KeyValuePair<string, string>("CN", "China"));
			items.Add(new KeyValuePair<string, string>("KR", "Korea"));
			items.Add(new KeyValuePair<string, string>("JP", "Japan"));
			items.Add(new KeyValuePair<string, string>("DE", "Germany"));
			items.Add(new KeyValuePair<string, string>("FI", "Finland"));

			items.EndUpdate();
		}
	}
}