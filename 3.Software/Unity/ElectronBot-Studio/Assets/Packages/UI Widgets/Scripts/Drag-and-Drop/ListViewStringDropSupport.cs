namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// ListViewString drop support.
	/// Receive drops from ListViewString.
	/// </summary>
	[RequireComponent(typeof(ListViewString))]
	public class ListViewStringDropSupport : ListViewCustomDropSupport<ListViewString, ListViewStringItemComponent, string>
	{
	}
}