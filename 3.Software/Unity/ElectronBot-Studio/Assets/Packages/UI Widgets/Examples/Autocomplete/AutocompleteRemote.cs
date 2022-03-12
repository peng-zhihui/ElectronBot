namespace UIWidgets.Examples
{
	using System.Collections;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Autocomplete for ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class AutocompleteRemote : AutocompleteCustom<ListViewIconsItemDescription, ListViewIconsItemComponent, ListViewIcons>
	{
		/// <summary>
		/// Load data from web.
		/// </summary>
		/// <returns>Yield instruction.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		protected override IEnumerator Search()
		{
			if (SearchDelay > 0)
			{
				yield return UtilitiesTime.Wait(SearchDelay, UnscaledTime);
			}

#if UNITY_2018_3_OR_NEWER
			var url = string.Format("http://example.com/?search={0}", UnityEngine.Networking.UnityWebRequest.EscapeURL(Query));
			using (var www = UnityEngine.Networking.UnityWebRequest.Get(new System.Uri(url)))
			{
				yield return www.SendWebRequest();

				if (Compatibility.IsError(www))
				{
					Debug.LogWarning(www.error);
				}
				else
				{
					DisplayListView.DataSource = Text2List(www.downloadHandler.text);
				}
			}
#else
			var url = string.Format("http://example.com/?search={0}", WWW.EscapeURL(Query));
			WWW www = new WWW(url);
			yield return www;

			DisplayListView.DataSource = Text2List(www.text);

			www.Dispose();
#endif

			if (DisplayListView.DataSource.Count > 0)
			{
				ShowOptions();
				DisplayListView.SelectedIndex = 0;
			}
			else
			{
				HideOptions();
			}
		}

		static readonly string[] LineSeparators = new string[] { "\r\n", "\r", "\n" };

		/// <summary>
		/// Convert raw data to list.
		/// </summary>
		/// <param name="text">Raw data.</param>
		/// <returns>Data list.</returns>
		protected static ObservableList<ListViewIconsItemDescription> Text2List(string text)
		{
			var result = new ObservableList<ListViewIconsItemDescription>();

			// convert text to items and add items to list
			foreach (var line in text.Split(LineSeparators, System.StringSplitOptions.None))
			{
				result.Add(new ListViewIconsItemDescription() { Name = line.TrimEnd() });
			}

			return result;
		}

		/// <summary>
		/// Determines whether the beginning of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Input; otherwise, false.</returns>
		public override bool Startswith(ListViewIconsItemDescription value)
		{
			return UtilitiesCompare.StartsWith(value.Name, Query, CaseSensitive)
				|| (value.LocalizedName != null && UtilitiesCompare.StartsWith(value.LocalizedName, Query, CaseSensitive));
		}

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(ListViewIconsItemDescription value)
		{
			return UtilitiesCompare.Contains(value.Name, Query, CaseSensitive)
				|| (value.LocalizedName != null && UtilitiesCompare.Contains(value.LocalizedName, Query, CaseSensitive));
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(ListViewIconsItemDescription value)
		{
			return value.LocalizedName ?? value.Name;
		}
	}
}