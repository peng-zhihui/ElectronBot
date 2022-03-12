namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// ListView sample. Selected item can be deselected.
	/// </summary>
	[RequireComponent(typeof(ListViewBase))]
	public class ListViewDeselect : MonoBehaviour
	{
		ListViewBase listView;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void Start()
		{
			listView = GetComponent<ListViewBase>();
			listView.MultipleSelect = true;
			listView.OnSelect.AddListener(Select);
			DeselectAllExceptLast();
		}

		/// <summary>
		/// Destroy this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			if (listView != null)
			{
				listView.OnSelect.RemoveListener(Select);
			}
		}

		void Select(int index, ListViewItem component)
		{
			DeselectAllExceptLast();
		}

		void DeselectAllExceptLast()
		{
			var indices = listView.SelectedIndices;
			for (int i = 0; i < (indices.Count - 1); i++)
			{
				listView.Deselect(indices[i]);
			}
		}
	}
}