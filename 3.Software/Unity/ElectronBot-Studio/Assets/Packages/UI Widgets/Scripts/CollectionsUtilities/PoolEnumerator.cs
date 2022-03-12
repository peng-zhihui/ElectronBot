namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Enumerates the elements of a components pool.
	/// </summary>
	/// <typeparam name="TComponent">Type of the component.</typeparam>
	[Serializable]
	public struct PoolEnumerator<TComponent> : IEnumerator<TComponent>, IDisposable, IEnumerator
	{
		private readonly TComponent template;
		private readonly List<TComponent> instances;
		private readonly List<TComponent> cache;

		private readonly int minIndex;
		private readonly int maxIndex;

		private int listIndex;

		private List<TComponent>.Enumerator enumerator;

		private TComponent current;

		/// <summary>
		/// Initializes a new instance of the <see cref="PoolEnumerator{TComponent}"/> struct.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <param name="template">Template.</param>
		/// <param name="instances">Instances.</param>
		/// <param name="cache">Cache.</param>
		internal PoolEnumerator(PoolEnumeratorMode mode, TComponent template, List<TComponent> instances, List<TComponent> cache)
		{
			this.template = template;
			this.instances = instances;
			this.cache = cache;

			switch (mode)
			{
				case PoolEnumeratorMode.Active:
					enumerator = instances.GetEnumerator();
					minIndex = 1;
					maxIndex = 3;
					break;
				case PoolEnumeratorMode.Cache:
					enumerator = cache.GetEnumerator();
					minIndex = -1;
					maxIndex = 2;
					break;
				case PoolEnumeratorMode.All:
					enumerator = cache.GetEnumerator();
					minIndex = -1;
					maxIndex = 3;
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown EnumeratorMode: {0}", EnumHelper<PoolEnumeratorMode>.ToString(mode)));
			}

			listIndex = minIndex;
			current = default(TComponent);
		}

		/// <summary>
		/// Releases all resources used by the <see cref="PoolEnumerator{TComponent}" />.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Advances the enumerator to the next element of the components pool.
		/// </summary>
		/// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
		/// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
		public bool MoveNext()
		{
			if (listIndex == minIndex)
			{
				listIndex++;
			}

			if ((listIndex == 0) && (listIndex > minIndex))
			{
				listIndex++;
				current = template;
				return true;
			}

			if ((listIndex == 1) && (listIndex > minIndex))
			{
				if (enumerator.MoveNext())
				{
					current = enumerator.Current;
					return true;
				}
				else
				{
					listIndex++;
					enumerator = instances.GetEnumerator();
				}
			}

			if ((listIndex == 2) && (listIndex < maxIndex))
			{
				if (enumerator.MoveNext())
				{
					current = enumerator.Current;
					return true;
				}
				else
				{
					listIndex++;
				}
			}

			current = default(TComponent);

			return false;
		}

		/// <summary>
		/// Gets the element at the current position of the enumerator.
		/// </summary>
		/// <returns>The element in the components pool at the current position of the enumerator.</returns>
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
		/// <returns>The element in the components pool at the current position of the enumerator.</returns>
		/// <exception cref="InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "Required.")]
		object IEnumerator.Current
		{
			get
			{
				if (listIndex == minIndex || listIndex == maxIndex)
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
			enumerator = (minIndex == -1) ? cache.GetEnumerator() : instances.GetEnumerator();
			listIndex = minIndex;
			current = default(TComponent);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the components pool />.
		/// </summary>
		/// <returns>A <see cref="PoolEnumerator{TComponent}" /> for the components pool.</returns>
		public PoolEnumerator<TComponent> GetEnumerator()
		{
			return this;
		}
	}
}