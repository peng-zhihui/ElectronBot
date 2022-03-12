#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an ListViewHeight.
	/// </summary>
	public class ListViewHeightSelectedItemObserver : ComponentDataObserver<UIWidgets.ListViewHeight, string>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewHeight target)
		{
			target.OnSelectString.AddListener(OnSelectStringListViewHeight);
			target.OnDeselectString.AddListener(OnDeselectStringListViewHeight);
		}

		/// <inheritdoc />
		protected override string GetValue(UIWidgets.ListViewHeight target)
		{
			return target.SelectedItem;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewHeight target)
		{
			target.OnSelectString.RemoveListener(OnSelectStringListViewHeight);
			target.OnDeselectString.RemoveListener(OnDeselectStringListViewHeight);
		}

		void OnSelectStringListViewHeight(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}

		void OnDeselectStringListViewHeight(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif