namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// ListView template selector interface.
	/// </summary>
	/// <typeparam name="TComponent">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	public interface IListViewTemplateSelector<TComponent, TItem>
		where TComponent : ListViewItem
	{
		/// <summary>
		/// Get all possible templates.
		/// </summary>
		/// <returns>Templates.</returns>
		TComponent[] AllTemplates();

		/// <summary>
		/// Select template by item.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		/// <returns>Template.</returns>
		TComponent Select(int index, TItem item);
	}
}