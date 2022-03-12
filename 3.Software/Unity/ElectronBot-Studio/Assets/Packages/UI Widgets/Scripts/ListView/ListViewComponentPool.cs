namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Extensions;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <content>
	/// Base class for custom ListViews.
	/// </content>
	public partial class ListViewCustom<TComponent, TItem> : ListViewCustomBase
		where TComponent : ListViewItem
	{
		/// <summary>
		/// ListView components pool.
		/// </summary>
		public class ListViewComponentPool
		{
			/// <summary>
			/// Indices difference.
			/// </summary>
			protected class Diff
			{
				/// <summary>
				/// Indices of the added items.
				/// </summary>
				protected List<int> Added = new List<int>();

				/// <summary>
				/// Indices of the removed items.
				/// </summary>
				protected List<int> Removed = new List<int>();

				/// <summary>
				/// Indices of the items with deactivated DisableRecycling.
				/// </summary>
				protected List<int> RestoredRecycling = new List<int>();

				/// <summary>
				/// Indices of the items which should not be recycled.
				/// </summary>
				protected List<int> DisableRecycling = new List<int>();

				/// <summary>
				/// Indices of the untouched items.
				/// </summary>
				protected List<int> Untouched = new List<int>();

				/// <summary>
				/// Indices of the displayed items.
				/// </summary>
				public List<int> Displayed = new List<int>();

				/// <summary>
				/// Calculate difference.
				/// </summary>
				/// <param name="components">Components.</param>
				/// <param name="current">Current indices.</param>
				/// <param name="required">Required indices.</param>
				public void Calculate(List<TComponent> components, List<int> current, List<int> required)
				{
					Added.Clear();
					Removed.Clear();
					RestoredRecycling.Clear();
					DisableRecycling.Clear();
					Untouched.Clear();
					Displayed.Clear();

					foreach (var component in components)
					{
						#pragma warning disable 0618
						var disable_recycling = component.DisableRecycling || component.IsDragged;
						#pragma warning restore 0618
						if (disable_recycling)
						{
							DisableRecycling.Add(component.Index);
						}
					}

					foreach (var index in required)
					{
						if (!current.Contains(index))
						{
							Added.Add(index);
						}
					}

					foreach (var index in current)
					{
						if (!required.Contains(index))
						{
							if (DisableRecycling.Contains(index))
							{
								RestoredRecycling.Add(index);
							}
							else
							{
								Removed.Add(index);
							}
						}
						else if (!DisableRecycling.Contains(index))
						{
							Untouched.Add(index);
						}
					}

					// ??? cannot remember why it's needed
					var added = Added.Count;
					for (int i = added; i < RestoredRecycling.Count; i++)
					{
						var index = Untouched.Pop();
						Added.Add(index);
						Removed.Add(index);
					}

					Displayed.AddRange(required);
					foreach (var index in DisableRecycling)
					{
						if (!Displayed.Contains(index))
						{
							Displayed.Add(index);
						}
					}
				}

				/// <summary>
				/// Check if indices are same.
				/// </summary>
				/// <param name="current">Current indices.</param>
				/// <param name="required">Required indices.</param>
				/// <returns>true if indices are same; otherwise false.</returns>
				public bool Same(List<int> current, List<int> required)
				{
					if (current.Count != required.Count)
					{
						return false;
					}

					for (int i = 0; i < current.Count; i++)
					{
						if (current[i] != required[i])
						{
							return false;
						}
					}

					return true;
				}
			}

			/// <summary>
			/// The owner.
			/// </summary>
			public readonly ListViewCustom<TComponent, TItem> Owner;

			/// <summary>
			/// The function to call after component instantiated.
			/// </summary>
			public Action<TComponent> ComponentCreated;

			/// <summary>
			/// The function to call before component destroyed.
			/// </summary>
			public Action<TComponent> ComponentDestroyed;

			/// <summary>
			/// The function to call after component activated.
			/// </summary>
			public Action<TComponent> ComponentActivated;

			/// <summary>
			/// The function to call after component moved to cache.
			/// </summary>
			public Action<TComponent> ComponentCached;

			/// <summary>
			/// Indices difference.
			/// </summary>
			protected Diff IndicesDiff = new Diff();

			/// <summary>
			/// Components comparer delegate.
			/// </summary>
			protected Comparison<TComponent> ComponentsComparerDelegate;

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewComponentPool"/> class.
			/// Use parents lists to avoid problem with creating copies of the original ListView.
			/// </summary>
			/// <param name="owner">Owner.</param>
			internal ListViewComponentPool(ListViewCustom<TComponent, TItem> owner)
			{
				Owner = owner;
				ComponentsComparerDelegate = ComponentsComparer;

				ComponentCreated = owner.ComponentCreated;
				ComponentDestroyed = owner.ComponentDestroyed;
				ComponentActivated = owner.ComponentActivated;
				ComponentCached = owner.ComponentCached;
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewComponentPool" />.
			/// </summary>
			/// <returns>A <see cref="ListViewBase.ListViewComponentEnumerator{TComponent, TTemplateWrapper}" /> for the <see cref="ListViewComponentPool" />.</returns>
			public ListViewComponentEnumerator<TComponent, Template> GetEnumerator()
			{
				return new ListViewComponentEnumerator<TComponent, Template>(PoolEnumeratorMode.Active, Owner.Templates);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewComponentPool" />.
			/// </summary>
			/// <param name="mode">Mode.</param>
			/// <returns>A <see cref="ListViewBase.ListViewComponentEnumerator{TComponent, TTemplateWrapper}" /> for the <see cref="ListViewComponentPool" />.</returns>
			public ListViewComponentEnumerator<TComponent, Template> GetEnumerator(PoolEnumeratorMode mode)
			{
				return new ListViewComponentEnumerator<TComponent, Template>(mode, Owner.Templates);
			}

			/// <summary>
			/// Init this instance.
			/// </summary>
			public void Init()
			{
				foreach (var template in Owner.Templates)
				{
					template.UpdateId();
					template.SetCallbacks(ComponentCreated, ComponentDestroyed, ComponentActivated, ComponentCached);
				}
			}

			/// <summary>
			/// Process locale changes.
			/// </summary>
			public void LocaleChanged()
			{
				foreach (var template in Owner.Templates)
				{
					template.LocaleChanged();
				}
			}

			/// <summary>
			/// Find component with the specified index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>Component with the specified index.</returns>
			public TComponent Find(int index)
			{
				for (int i = 0; i < Owner.Components.Count; i++)
				{
					if (Owner.Components[i].Index == index)
					{
						return Owner.Components[i];
					}
				}

				return null;
			}

			/// <summary>
			/// Get template.
			/// </summary>
			/// <param name="component">Component.</param>
			/// <returns>Template.</returns>
			public Template GetTemplate(TComponent component)
			{
				var id = component.GetInstanceID();

				foreach (var template in Owner.Templates)
				{
					if (template.Id == id)
					{
						return template;
					}
				}

				var added = ListViewItemTemplate<TComponent>.Create<Template>(component, ComponentCreated, ComponentDestroyed, ComponentActivated, ComponentCached, Owner.Container, Owner);
				Owner.Templates.Add(added);

				return added;
			}

			/// <summary>
			/// Get template by item index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>Template.</returns>
			public Template GetTemplate(int index)
			{
				var template = Owner.Index2Template(index);

				return GetTemplate(template);
			}

			/// <summary>
			/// Set the DisplayedIndices.
			/// </summary>
			/// <param name="newIndices">New indices.</param>
			/// <param name="action">Action.</param>
			public void DisplayedIndicesSet(List<int> newIndices, Action<TComponent> action)
			{
				foreach (var template in Owner.Templates)
				{
					template.RequiredInstances = 0;
				}

				foreach (var index in newIndices)
				{
					GetTemplate(index).RequiredInstances += 1;
				}

				foreach (var template in Owner.Templates)
				{
					template.RequestInstances();
				}

				Owner.Components.Clear();
				foreach (var index in newIndices)
				{
					var instance = GetTemplate(index).RequestInstance();
					Owner.Components.Add(instance);

					instance.Index = index;
					action(instance);
				}

				SetOwnerItems();

				Owner.ComponentsDisplayedIndices.Clear();
				Owner.ComponentsDisplayedIndices.AddRange(newIndices);

				Owner.Components.Sort(ComponentsComparerDelegate);
				foreach (var c in Owner.Components)
				{
					c.transform.SetAsLastSibling();
				}
			}

			/// <summary>
			/// Update the DisplayedIndices.
			/// </summary>
			/// <param name="newIndices">New indices.</param>
			/// <param name="action">Action.</param>
			public void DisplayedIndicesUpdate(List<int> newIndices, Action<TComponent> action)
			{
				if (IndicesDiff.Same(Owner.ComponentsDisplayedIndices, newIndices))
				{
					return;
				}

				IndicesDiff.Calculate(Owner.Components, Owner.ComponentsDisplayedIndices, newIndices);

				foreach (var template in Owner.Templates)
				{
					template.RequiredInstances = 0;
				}

				foreach (var index in IndicesDiff.Displayed)
				{
					GetTemplate(index).Require(index);
				}

				foreach (var template in Owner.Templates)
				{
					template.RequestInstances();
				}

				Owner.Components.Clear();

				bool is_new;
				foreach (var index in IndicesDiff.Displayed)
				{
					var instance = GetTemplate(index).RequestInstance(index, out is_new);
					Owner.Components.Add(instance);

					if (is_new)
					{
						instance.Index = index;
						action(instance);
					}
				}

				SetOwnerItems();

				Owner.ComponentsDisplayedIndices.Clear();
				Owner.ComponentsDisplayedIndices.AddRange(IndicesDiff.Displayed);

				Owner.Components.Sort(ComponentsComparerDelegate);
				foreach (var c in Owner.Components)
				{
					c.transform.SetAsLastSibling();
				}
			}

			/// <summary>
			/// Set the owner items.
			/// </summary>
			protected void SetOwnerItems()
			{
				Owner.UpdateComponents(Owner.Components);
			}

			/// <summary>
			/// Compare components by component index.
			/// </summary>
			/// <returns>A signed integer that indicates the relative values of x and y.</returns>
			/// <param name="x">The x coordinate.</param>
			/// <param name="y">The y coordinate.</param>
			protected int ComponentsComparer(TComponent x, TComponent y)
			{
				return Owner.ComponentsDisplayedIndices.IndexOf(x.Index).CompareTo(Owner.ComponentsDisplayedIndices.IndexOf(y.Index));
			}

			/// <summary>
			/// Apply function for each active component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEach(Action<TComponent> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.Active))
				{
					action(component);
				}
			}

			/// <summary>
			/// Apply function for each active and cached components.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachAll(Action<TComponent> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.All))
				{
					action(component);
				}
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<TComponent> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.Cache))
				{
					action(component);
				}
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<ListViewItem> action)
			{
				foreach (var component in GetEnumerator(PoolEnumeratorMode.Cache))
				{
					action(component);
				}
			}

			/// <summary>
			/// Set size of the components.
			/// </summary>
			/// <param name="size">Size.</param>
			public void SetSize(Vector2 size)
			{
				foreach (var template in Owner.Templates)
				{
					template.SetSize(size);
				}
			}

			/// <summary>
			/// Set the style.
			/// </summary>
			/// <param name="styleBackground">Style for the background.</param>
			/// <param name="styleText">Style for the text.</param>
			/// <param name="style">Full style data.</param>
			public void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
			{
				foreach (var template in Owner.Templates)
				{
					template.SetStyle(styleBackground, styleText, style);
				}
			}

			/// <summary>
			/// Disable templates.
			/// </summary>
			public virtual void DisableTemplates()
			{
				foreach (var template in Owner.Templates)
				{
					template.DisableTemplate();
				}
			}

			/// <summary>
			/// Destroy cache.
			/// </summary>
			/// <param name="excludeTemplates">Templates to exclude from destroy.</param>
			public virtual void Destroy(TComponent[] excludeTemplates)
			{
				for (int i = Owner.Templates.Count - 1; i >= 0; i--)
				{
					var template = Owner.Templates[i];
					if (Array.IndexOf(excludeTemplates, template.Template) != -1)
					{
						continue;
					}

					template.Destroy();
					Owner.Templates.RemoveAt(i);
				}
			}

			/// <summary>
			/// Destroy cache.
			/// </summary>
			public virtual void Destroy()
			{
				foreach (var template in Owner.Templates)
				{
					template.Destroy();
				}

				Owner.Templates.Clear();
			}
		}
	}
}