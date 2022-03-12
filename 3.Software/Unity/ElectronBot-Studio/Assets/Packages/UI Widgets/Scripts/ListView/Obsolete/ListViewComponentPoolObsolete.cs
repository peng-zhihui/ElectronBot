namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Extensions;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviour,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable
	{
		/// <summary>
		/// ListView components pool.
		/// </summary>
		/// <typeparam name="TComponent">Type of DefaultItem component.</typeparam>
		/// <typeparam name="TItem">Type of item.</typeparam>
		/// <typeparam name="TTemplateWrapper">Type of template wrapper.</typeparam>
		[Obsolete("Used by deprecated ListView.")]
		public class ListViewComponentPoolObsolete<TComponent, TItem, TTemplateWrapper>
			where TComponent : ListViewItem
			where TTemplateWrapper : ListViewItemTemplate<TComponent>, new()
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
			/// The components gameobjects container.
			/// </summary>
			public RectTransform Container;

			/// <summary>
			/// The owner.
			/// </summary>
			public ListViewBase Owner;

			/// <summary>
			/// The templates list.
			/// </summary>
			protected List<TTemplateWrapper> Templates;

			/// <summary>
			/// The components list.
			/// </summary>
			protected List<TComponent> Components;

			/// <summary>
			/// The function to add callbacks.
			/// </summary>
			public Action<TComponent> CallbackAdd;

			/// <summary>
			/// The function to remove callbacks.
			/// </summary>
			public Action<TComponent> CallbackRemove;

			/// <summary>
			/// The function to call after component instantiated.
			/// </summary>
			public Action<TComponent> ComponentCreated;

			/// <summary>
			/// The function to call before component destroyed.
			/// </summary>
			public Action<TComponent> ComponentDestroyed;

			/// <summary>
			/// The displayed indices.
			/// </summary>
			protected List<int> DisplayedIndices;

			/// <summary>
			/// Indices difference.
			/// </summary>
			protected Diff IndicesDiff = new Diff();

			/// <summary>
			/// Get template by item index.
			/// </summary>
			protected Func<int, TComponent> Index2Template;

			/// <summary>
			/// Components comparer delegate.
			/// </summary>
			protected Comparison<TComponent> ComponentsComparerDelegate;

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewComponentPoolObsolete{TComponent, TItem, TComponentTemplate}"/> class.
			/// Use parents lists to avoid problem with creating copies of the original ListView.
			/// </summary>
			/// <param name="components">Components list to use.</param>
			/// <param name="templates">Templates list to use.</param>
			/// <param name="displayedIndices">Displayed indices to use.</param>
			/// <param name="index2template">Get template by item index.</param>
			internal ListViewComponentPoolObsolete(List<TComponent> components, List<TTemplateWrapper> templates, List<int> displayedIndices, Func<int, TComponent> index2template)
			{
				Components = components;
				Templates = templates;
				DisplayedIndices = displayedIndices;
				Index2Template = index2template;
				ComponentsComparerDelegate = ComponentsComparer;
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewComponentPoolObsolete{TComponent, TItem, TComponentTemplate}" />.
			/// </summary>
			/// <returns>A <see cref="ListViewComponentEnumerator{TComponent, TTemplateWrapper}" /> for the <see cref="ListViewComponentPoolObsolete{TComponent, TItem, TComponentTemplate}" />.</returns>
			public ListViewComponentEnumerator<TComponent, TTemplateWrapper> GetEnumerator()
			{
				return new ListViewComponentEnumerator<TComponent, TTemplateWrapper>(PoolEnumeratorMode.Active, Templates);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewComponentPoolObsolete{TComponent, TItem, TComponentTemplate}" />.
			/// </summary>
			/// <param name="mode">Mode.</param>
			/// <returns>A <see cref="ListViewComponentEnumerator{TComponent, TTemplateWrapper}" /> for the <see cref="ListViewComponentPoolObsolete{TComponent, TItem, TComponentTemplate}" />.</returns>
			public ListViewComponentEnumerator<TComponent, TTemplateWrapper> GetEnumerator(PoolEnumeratorMode mode)
			{
				return new ListViewComponentEnumerator<TComponent, TTemplateWrapper>(mode, Templates);
			}

			/// <summary>
			/// Init this instance.
			/// </summary>
			public void Init()
			{
				foreach (var template in Templates)
				{
					template.UpdateId();
					template.SetCallbacks(ComponentCreated, ComponentDestroyed, CallbackAdd, CallbackRemove);
				}
			}

			/// <summary>
			/// Process locale changes.
			/// </summary>
			public void LocaleChanged()
			{
				foreach (var template in Templates)
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
				for (int i = 0; i < Components.Count; i++)
				{
					if (Components[i].Index == index)
					{
						return Components[i];
					}
				}

				return null;
			}

			/// <summary>
			/// Get template.
			/// </summary>
			/// <param name="component">Component.</param>
			/// <returns>Template.</returns>
			public TTemplateWrapper GetTemplate(TComponent component)
			{
				var id = component.GetInstanceID();

				foreach (var template in Templates)
				{
					if (template.Id == id)
					{
						return template;
					}
				}

				var added = ListViewItemTemplate<TComponent>.Create<TTemplateWrapper>(component, ComponentCreated, ComponentDestroyed, CallbackAdd, CallbackRemove, Container, Owner);
				Templates.Add(added);

				return added;
			}

			/// <summary>
			/// Get template by item index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>Template.</returns>
			public TTemplateWrapper GetTemplate(int index)
			{
				var component = Index2Template(index);

				return GetTemplate(component);
			}

			/// <summary>
			/// Set the DisplayedIndices.
			/// </summary>
			/// <param name="newIndices">New indices.</param>
			/// <param name="action">Action.</param>
			public void DisplayedIndicesSet(List<int> newIndices, Action<TComponent> action)
			{
				foreach (var template in Templates)
				{
					template.RequiredInstances = 0;
				}

				foreach (var index in newIndices)
				{
					GetTemplate(index).RequiredInstances += 1;
				}

				foreach (var template in Templates)
				{
					template.RequestInstances();
				}

				Components.Clear();
				foreach (var index in newIndices)
				{
					var instance = GetTemplate(index).RequestInstance();
					Components.Add(instance);

					instance.Index = index;
					action(instance);
				}

				SetOwnerItems();

				DisplayedIndices.Clear();
				DisplayedIndices.AddRange(newIndices);

				Components.Sort(ComponentsComparerDelegate);
				foreach (var c in Components)
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
				if (IndicesDiff.Same(DisplayedIndices, newIndices))
				{
					return;
				}

				IndicesDiff.Calculate(Components, DisplayedIndices, newIndices);

				foreach (var template in Templates)
				{
					template.RequiredInstances = 0;
				}

				foreach (var index in IndicesDiff.Displayed)
				{
					GetTemplate(index).Require(index);
				}

				foreach (var template in Templates)
				{
					template.RequestInstances();
				}

				Components.Clear();

				bool is_new;
				foreach (var index in IndicesDiff.Displayed)
				{
					var instance = GetTemplate(index).RequestInstance(index, out is_new);
					Components.Add(instance);

					if (is_new)
					{
						instance.Index = index;
						action(instance);
					}
				}

				SetOwnerItems();

				DisplayedIndices.Clear();
				DisplayedIndices.AddRange(IndicesDiff.Displayed);

				Components.Sort(ComponentsComparerDelegate);
				foreach (var c in Components)
				{
					c.transform.SetAsLastSibling();
				}
			}

			/// <summary>
			/// Set the owner items.
			/// </summary>
			protected void SetOwnerItems()
			{
				Owner.UpdateComponents<TComponent>(Components);
			}

			/// <summary>
			/// Compare components by component index.
			/// </summary>
			/// <returns>A signed integer that indicates the relative values of x and y.</returns>
			/// <param name="x">The x coordinate.</param>
			/// <param name="y">The y coordinate.</param>
			protected int ComponentsComparer(TComponent x, TComponent y)
			{
				return DisplayedIndices.IndexOf(x.Index).CompareTo(DisplayedIndices.IndexOf(y.Index));
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
				foreach (var template in Templates)
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
				foreach (var template in Templates)
				{
					template.SetStyle(styleBackground, styleText, style);
				}
			}

			/// <summary>
			/// Disable templates.
			/// </summary>
			public virtual void DisableTemplates()
			{
				foreach (var template in Templates)
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
				for (int i = Templates.Count - 1; i >= 0; i--)
				{
					var template = Templates[i];
					if (Array.IndexOf(excludeTemplates, template.Template) != -1)
					{
						continue;
					}

					template.Destroy();
					Templates.RemoveAt(i);
				}
			}

			/// <summary>
			/// Destroy cache.
			/// </summary>
			public virtual void Destroy()
			{
				foreach (var template in Templates)
				{
					template.Destroy();
				}

				Templates.Clear();
			}
		}
	}
}