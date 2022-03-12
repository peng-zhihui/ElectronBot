namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Extensions;
	using UnityEngine;

	/// <summary>
	/// List component pool.
	/// </summary>
	/// <typeparam name="TComponent">Component type.</typeparam>
	public class ListComponentPool<TComponent>
		where TComponent : Component
	{
		readonly List<TComponent> instances;

		readonly List<TComponent> cache;

		readonly RectTransform parent;

		readonly bool MovableToCache;

		TComponent template;

		/// <summary>
		/// Template.
		/// </summary>
		public TComponent Template
		{
			get
			{
				return template;
			}

			set
			{
				if (template != value)
				{
					template = value;

					Require(0);

					foreach (var c in cache)
					{
						UnityEngine.Object.Destroy(c);
					}

					cache.Clear();

					if (template != null)
					{
						template.gameObject.SetActive(false);
					}
				}
			}
		}

		/// <summary>
		/// Default item.
		/// </summary>
		[Obsolete("Replaced with Template.")]
		public TComponent DefaultItem
		{
			get
			{
				return template;
			}

			set
			{
				Template = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListComponentPool{TComponent}"/> class.
		/// </summary>
		/// <param name="template">Template.</param>
		/// <param name="instances">List of the active items.</param>
		/// <param name="cache">List of the cached items.</param>
		/// <param name="parent">Parent.</param>
		public ListComponentPool(TComponent template, List<TComponent> instances, List<TComponent> cache, RectTransform parent)
		{
			MovableToCache = typeof(IMovableToCache).IsAssignableFrom(typeof(TComponent));

			this.template = template;
			if (this.template != null)
			{
				this.template.gameObject.SetActive(false);
			}

			this.instances = instances;
			this.cache = cache;
			this.parent = parent;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="ListComponentPool{TComponent}" />.
		/// </summary>
		/// <returns>A <see cref="PoolEnumerator{TComponent}" /> for the <see cref="ListComponentPool{TComponent}" />.</returns>
		public PoolEnumerator<TComponent> GetEnumerator()
		{
			return new PoolEnumerator<TComponent>(PoolEnumeratorMode.Active, template, instances, cache);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="ListComponentPool{TComponent}" />.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <returns>A <see cref="PoolEnumerator{TComponent}" /> for the <see cref="ListComponentPool{TComponent}" />.</returns>
		public PoolEnumerator<TComponent> GetEnumerator(PoolEnumeratorMode mode)
		{
			return new PoolEnumerator<TComponent>(mode, template, instances, cache);
		}

		/// <summary>
		/// Get component instance by the specified index.
		/// </summary>
		/// <param name="index">Item.</param>
		/// <returns>Component instance.</returns>
		public TComponent this[int index]
		{
			get
			{
				return instances[index];
			}
		}

		/// <summary>
		/// Count of the active components.
		/// </summary>
		public int Count
		{
			get
			{
				return instances.Count;
			}
		}

		/// <summary>
		/// Set the count of the active instances.
		/// </summary>
		/// <param name="instancesCount">Required instances count.</param>
		public void Require(int instancesCount)
		{
			if (instances.Count > instancesCount)
			{
				for (var i = instances.Count - 1; i >= instancesCount; i--)
				{
					var instance = instances[i];

					if (MovableToCache)
					{
						(instance as IMovableToCache).MovedToCache();
					}

					instance.gameObject.SetActive(false);
					cache.Add(instance);

					instances.RemoveAt(i);
				}
			}

			while (instances.Count < instancesCount)
			{
				GetInstance();
			}
		}

		/// <summary>
		/// Disable components by indices.
		/// </summary>
		/// <param name="indices">Indices of the components to disable.</param>
		public void Disable(List<int> indices)
		{
			indices.Sort();

			for (int i = indices.Count - 1; i >= 0; i--)
			{
				var index = indices[i];
				var instance = instances[index];

				if (MovableToCache)
				{
					(instance as IMovableToCache).MovedToCache();
				}

				instance.gameObject.SetActive(false);
				cache.Add(instance);

				instances.RemoveAt(index);
			}
		}

		/// <summary>
		/// Get instance.
		/// </summary>
		/// <returns>Instance.</returns>
		public TComponent GetInstance()
		{
			TComponent instance;

			if (cache.Count > 0)
			{
				instance = cache.Pop();
			}
			else
			{
				instance = Compatibility.Instantiate(Template);
				Utilities.FixInstantiated(Template, instance);
				instance.transform.SetParent(parent, false);
			}

			instance.gameObject.SetActive(true);
			instances.Add(instance);

			return instance;
		}

		/// <summary>
		/// Apply function for each component.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEach(Action<TComponent> action)
		{
			foreach (var a in instances)
			{
				action(a);
			}
		}

		/// <summary>
		/// Apply function for each component and cached components.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEachAll(Action<TComponent> action)
		{
			foreach (var a in instances)
			{
				action(a);
			}

			foreach (var a in cache)
			{
				action(a);
			}
		}

		/// <summary>
		/// Apply function for each cached component.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEachCache(Action<TComponent> action)
		{
			foreach (var a in cache)
			{
				action(a);
			}
		}
	}
}