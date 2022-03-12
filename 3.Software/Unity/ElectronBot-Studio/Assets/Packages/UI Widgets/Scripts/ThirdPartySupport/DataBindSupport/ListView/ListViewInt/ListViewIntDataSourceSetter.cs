#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DataSource of a ListViewInt depending on the UIWidgets.ObservableList{System.Int32} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewInt DataSource Setter")]
	public class ListViewIntDataSourceSetter : ComponentSingleSetter<UIWidgets.ListViewInt, UIWidgets.ObservableList<int>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewInt target, UIWidgets.ObservableList<int> value)
		{
			target.DataSource = value;
		}
	}
}
#endif