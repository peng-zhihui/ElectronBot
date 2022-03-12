namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Styles;
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
		/// Enumerates the elements of a <see cref="ListViewCustom{TCompoment, TItem}.ListViewComponentPool" />.
		/// </summary>
		/// <typeparam name="TComponent">Type of the components.</typeparam>
		/// <typeparam name="TTemplateWrapper">Type of the template wrapper.</typeparam>
		[Serializable]
		public struct ListViewComponentEnumerator<TComponent, TTemplateWrapper> : IEnumerator<TComponent>, IDisposable, IEnumerator
			where TComponent : ListViewItem
			where TTemplateWrapper : ListViewItemTemplate<TComponent>, new()
		{
			private readonly PoolEnumeratorMode mode;

			private readonly List<TTemplateWrapper> templates;

			private readonly int maxIndex;

			private int listIndex;

			private PoolEnumerator<TComponent> enumerator;

			private TComponent current;

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewComponentEnumerator{TComponent, TTemplateWrapper}"/> struct.
			/// </summary>
			/// <param name="mode">Mode.</param>
			/// <param name="templates">Templates.</param>
			internal ListViewComponentEnumerator(PoolEnumeratorMode mode, List<TTemplateWrapper> templates)
			{
				this.mode = mode;
				this.templates = templates;

				enumerator = templates.Count > 0 ? templates[0].GetEnumerator(mode) : default(PoolEnumerator<TComponent>);
				listIndex = -1;
				maxIndex = templates.Count;
				current = default(TComponent);
			}

			/// <summary>
			/// Releases all resources used by the <see cref="ListViewComponentEnumerator{TComponent, TTemplateWrapper}" />.
			/// </summary>
			public void Dispose()
			{
			}

			/// <summary>
			/// Advances the enumerator to the next element of the <see cref="ListViewCustom{TCompoment, TItem}.ListViewComponentPool" />.
			/// </summary>
			/// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
			/// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created. </exception>
			public bool MoveNext()
			{
				if (listIndex == -1)
				{
					listIndex = 0;
				}

				if (listIndex < maxIndex)
				{
					if (enumerator.MoveNext())
					{
						current = enumerator.Current;
						return true;
					}
					else
					{
						listIndex++;
						if (listIndex == maxIndex)
						{
							current = default(TComponent);
							return false;
						}

						enumerator = templates[listIndex].GetEnumerator(mode);
					}
				}

				return false;
			}

			/// <summary>
			/// Gets the element at the current position of the enumerator.
			/// </summary>
			/// <returns>The element in the <see cref="ListViewCustom{TCompoment, TItem}.ListViewComponentPool" /> at the current position of the enumerator.</returns>
			public TComponent Current
			{
				get
				{
					return current;
				}
			}

			/// <summary>
			/// Gets the element at the current position of the enumerator.
			/// </summary>
			/// <returns>The element in the <see cref="ListViewCustom{TCompoment, TItem}.ListViewComponentPool" /> at the current position of the enumerator.</returns>
			/// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
			object IEnumerator.Current
			{
				get
				{
					if (listIndex == -1 || listIndex == maxIndex)
					{
						throw new InvalidOperationException("The enumerator is positioned before the first element of the collection or after the last element.");
					}

					return Current;
				}
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			void IEnumerator.Reset()
			{
				enumerator = templates[0].GetEnumerator(mode);
				listIndex = -2;
				current = default(TComponent);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewCustom{TCompoment, TItem}.ListViewComponentPool" />.
			/// </summary>
			/// <returns>A <see cref="ListViewComponentEnumerator{TComponent, TTemplateWrapper}" /> for the <see cref="ListViewCustom{TCompoment, TItem}.ListViewComponentPool" />.</returns>
			public ListViewComponentEnumerator<TComponent, TTemplateWrapper> GetEnumerator()
			{
				return this;
			}
		}
	}
}