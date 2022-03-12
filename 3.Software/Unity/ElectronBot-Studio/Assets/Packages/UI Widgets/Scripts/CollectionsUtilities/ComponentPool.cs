namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Component pool.
	/// </summary>
	/// <typeparam name="T">Type of object.</typeparam>
	public class ComponentPool<T> : MonoBehaviour
		where T : MonoBehaviour
	{
		/// <summary>
		/// The cache.
		/// </summary>
		protected static Stack<T> Cache = new Stack<T>();

		/// <summary>
		/// Clones or take from cache the object original and returns the clone.
		/// </summary>
		/// <returns>New object instance.</returns>
		public T Instance()
		{
			T instance;

			if (Cache.Count == 0)
			{
				instance = Instantiate(this) as T;
			}
			else
			{
				do
				{
					instance = (Cache.Count > 0) ? Cache.Pop() : Instantiate(this) as T;
				}
				while (instance == null);
			}

			instance.transform.SetParent(transform.parent, false);
			instance.gameObject.SetActive(true);

			return instance;
		}

		/// <summary>
		/// Clones or take from cache the object original and returns the clone.
		/// </summary>
		/// <param name="parent">Parent.</param>
		/// <returns>New object instance.</returns>
		public T Instance(Transform parent)
		{
			var instance = Instance();
			instance.transform.SetParent(parent, false);

			return instance;
		}

		/// <summary>
		/// Return current object to cache.
		/// </summary>
		public void Free()
		{
			Cache.Push(this as T);
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Return current object to cache.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public void Free(Transform parent)
		{
			Free();
			transform.SetParent(parent, false);
		}

		/// <summary>
		/// Apply action for each object in cache.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEachCache(Action<T> action)
		{
			foreach (var c in Cache)
			{
				action(c);
			}
		}
	}
}