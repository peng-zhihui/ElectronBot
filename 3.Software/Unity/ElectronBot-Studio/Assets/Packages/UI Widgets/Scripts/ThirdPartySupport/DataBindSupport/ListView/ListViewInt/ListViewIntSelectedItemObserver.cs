#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an ListViewInt.
	/// </summary>
	public class ListViewIntSelectedItemObserver : ComponentDataObserver<UIWidgets.ListViewInt, int>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewInt target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectListViewInt);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewInt);
		}

		/// <inheritdoc />
		protected override int GetValue(UIWidgets.ListViewInt target)
		{
			return target.SelectedItem;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewInt target)
		{
			target.OnSelectObject.RemoveListener(OnSelectObjectListViewInt);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewInt);
		}

		void OnSelectObjectListViewInt(int arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewInt(int arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif