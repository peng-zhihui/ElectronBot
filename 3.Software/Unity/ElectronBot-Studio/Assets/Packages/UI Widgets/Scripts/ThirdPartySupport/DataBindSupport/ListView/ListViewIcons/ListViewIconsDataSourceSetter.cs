#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DataSource of a ListViewIcons depending on the UIWidgets.ObservableList{UIWidgets.ListViewIconsItemDescription} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ListViewIcons DataSource Setter")]
	public class ListViewIconsDataSourceSetter : ComponentSingleSetter<UIWidgets.ListViewIcons, UIWidgets.ObservableList<UIWidgets.ListViewIconsItemDescription>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ListViewIcons target, UIWidgets.ObservableList<UIWidgets.ListViewIconsItemDescription> value)
		{
			target.DataSource = value;
		}
	}
}
#endif