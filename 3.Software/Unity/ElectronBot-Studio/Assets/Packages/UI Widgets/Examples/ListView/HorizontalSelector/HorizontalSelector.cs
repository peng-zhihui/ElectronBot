namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Horizontal selector.
	/// </summary>
	public class HorizontalSelector : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// Paginator.
		/// </summary>
		[SerializeField]
		public ListViewPaginator Paginator;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void Start()
		{
			ListView.MultipleSelect = false;
			ListView.Select(0);

			Paginator.OnPageSelect.AddListener(ListView.Select);
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			Paginator.OnPageSelect.RemoveListener(ListView.Select);
		}
	}
}