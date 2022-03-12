#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an ListView.
	/// </summary>
	public class ListViewSelectedItemObserver : ComponentDataObserver<UIWidgets.ListView, string>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListView target)
		{
			target.OnSelectString.AddListener(OnSelectStringListView);
			target.OnDeselectString.AddListener(OnDeselectStringListView);
		}

		/// <inheritdoc />
		protected override string GetValue(UIWidgets.ListView target)
		{
			return target.SelectedItem;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListView target)
		{
			target.OnSelectString.RemoveListener(OnSelectStringListView);
			target.OnDeselectString.RemoveListener(OnDeselectStringListView);
		}

		void OnSelectStringListView(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}

		void OnDeselectStringListView(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif