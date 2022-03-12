#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DataSource of a ListViewHeight depending on the UIWidgets.ObservableList{System.String} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewHeight DataSource Setter")]
	public class ListViewHeightDataSourceSetter : ComponentSingleSetter<UIWidgets.ListViewHeight, UIWidgets.ObservableList<string>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewHeight target, UIWidgets.ObservableList<string> value)
		{
			target.DataSource = value;
		}
	}
}
#endif