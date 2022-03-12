namespace UIWidgets
{
	using System;

	/// <summary>
	/// Updater.
	/// Replace Unity Update() with custom one without reflection.
	/// </summary>
	public static class Updater
	{
		static IUpdaterProxy proxy;

		/// <summary>
		/// Proxy to run Update().
		/// </summary>
		public static IUpdaterProxy Proxy
		{
			get
			{
				if (proxy == null)
				{
					proxy = UpdaterProxy.Instance;
				}

				return proxy;
			}

			set
			{
				proxy = value;
			}
		}

		/// <summary>
		/// Add target.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Add(IUpdatable target)
		{
			Proxy.Add(target);
		}

		/// <summary>
		/// Remove target.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Remove(IUpdatable target)
		{
			Proxy.Remove(target);
		}

		/// <summary>
		/// Add target to LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void AddLateUpdate(ILateUpdatable target)
		{
			Proxy.LateUpdateAdd(target);
		}

		/// <summary>
		/// Remove target from LateUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveLateUpdate(ILateUpdatable target)
		{
			Proxy.LateUpdateRemove(target);
		}

		/// <summary>
		/// Add target to FixedUpdate.
		/// </summary>
		/// <param name="target">target.</param>
		public static void AddFixedUpdate(IFixedUpdatable target)
		{
			Proxy.AddFixedUpdate(target);
		}

		/// <summary>
		/// Remove target from FixedUpdate.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RemoveFixedUpdate(IFixedUpdatable target)
		{
			Proxy.RemoveFixedUpdate(target);
		}

		/// <summary>
		/// Add target to run update only once.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RunOnce(IUpdatable target)
		{
			Proxy.RunOnce(target);
		}

		/// <summary>
		/// Add action to run only once.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RunOnce(Action action)
		{
			Proxy.RunOnce(action);
		}

		/// <summary>
		/// Add target to run update only once at next frame.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RunOnceNextFrame(IUpdatable target)
		{
			Proxy.RunOnceNextFrame(target);
		}

		/// <summary>
		/// Add action to run only once at next frame.
		/// </summary>
		/// <param name="action">Action.</param>
		public static void RunOnceNextFrame(Action action)
		{
			Proxy.RunOnceNextFrame(action);
		}
	}
}