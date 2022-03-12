namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// OnChange event for Combobox raised OnHideListView with new selected and new removed items.
	/// </summary>
	public class ComboboxChanges : MonoBehaviour
	{
		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public ComboboxIcons Combobox;

		/// <summary>
		/// Event called when Combobox.ListView closed.
		/// </summary>
		[SerializeField]
		public ComboboxChangesEvent OnChange = new ComboboxChangesEvent();

		List<int> oldSelected = new List<int>();

		List<int> newSelected = new List<int>();

		bool isShow;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void Start()
		{
			if (Combobox != null)
			{
				Combobox.OnShowListView.AddListener(ShowList);
				Combobox.OnHideListView.AddListener(HideList);
			}
		}

		void ShowList()
		{
			isShow = true;

			oldSelected.Clear();
			Combobox.ListView.GetSelectedIndices(oldSelected);
		}

		void HideList()
		{
			if (!isShow)
			{
				return;
			}

			isShow = false;

			newSelected.Clear();
			Combobox.ListView.GetSelectedIndices(newSelected);

			var added = newSelected.Except(oldSelected).ToArray();
			var removed = oldSelected.Except(newSelected).ToArray();

			OnChange.Invoke(added, removed);

			Debug.Log(string.Format("All selected indices: {0}", UtilitiesCollections.List2String(newSelected)));
			Debug.Log(string.Format("New selected indices: {0}", UtilitiesCollections.List2String(added)));
			Debug.Log(string.Format("Deselected indices: {0}", UtilitiesCollections.List2String(removed)));
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			if (Combobox != null)
			{
				Combobox.OnShowListView.RemoveListener(ShowList);
				Combobox.OnHideListView.RemoveListener(HideList);
			}
		}
	}
}