namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Updater proxy.
	/// Replace Unity Update() with custom one without reflection.
	/// </summary>
	public class UpdaterProxy : MonoBehaviour, IUpdaterProxy
	{
		/// <summary>
		/// Targets to run on update.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		protected class TargetsList<T>
		{
			LinkedHashSet<T> targets = new LinkedHashSet<T>();

			List<T> temp = new List<T>();

			/// <summary>
			/// Add.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Add(T target)
			{
				targets.Add(target);
			}

			/// <summary>
			/// Remove.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Remove(T target)
			{
				targets.Remove(target);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="TargetsOnceList{T}" />.
			/// </summary>
			/// <returns>A <see cref="List{T}.Enumerator" /> for the <see cref="TargetsOnceList{T}" />.</returns>
			public List<T>.Enumerator GetEnumerator()
			{
				temp.AddRange(targets);

				return temp.GetEnumerator();
			}

			/// <summary>
			/// Cleanup.
			/// </summary>
			public void Cleanup()
			{
				temp.Clear();
			}
		}

		/// <summary>
		/// Targets to run once on nearest update.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		protected class TargetsOnceList<T>
		{
			List<T> targets = new List<T>();

			List<T> temp = new List<T>();

			/// <summary>
			/// Add.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Add(T target)
			{
				targets.Add(target);
			}

			/// <summary>
			/// Add range.
			/// </summary>
			/// <param name="list">List.</param>
			public void AddRange(List<T> list)
			{
				targets.AddRange(list);
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="TargetsOnceList{T}" />.
			/// </summary>
			/// <returns>A <see cref="List{T}.Enumerator" /> for the <see cref="TargetsOnceList{T}" />.</returns>
			public List<T>.Enumerator GetEnumerator()
			{
				temp.AddRange(targets);
				targets.Clear();

				return temp.GetEnumerator();
			}

			/// <summary>
			/// Cleanup.
			/// </summary>
			public void Cleanup()
			{
				temp.Clear();
			}
		}

		/// <summary>
		/// Targets to run on next frame.
		/// </summary>
		/// <typeparam name="T">Target type.</typeparam>
		protected class TargetsNextList<T>
		{
			/// <summary>
			/// Next frame number.
			/// </summary>
			public int Frame;

			TargetsOnceList<T> once;

			List<T> targets = new List<T>();

			/// <summary>
			/// Initializes a new instance of the <see cref="TargetsNextList{T}"/> class.
			/// </summary>
			/// <param name="once">Targets.</param>
			public TargetsNextList(TargetsOnceList<T> once)
			{
				this.once = once;
			}

			/// <summary>
			/// Add.
			/// </summary>
			/// <param name="target">Target.</param>
			public void Add(T target)
			{
				Check();

				targets.Add(target);
			}

			/// <summary>
			/// Check.
			/// </summary>
			public void Check()
			{
				var current_frame = UtilitiesTime.GetFrameCount();
				if ((Frame <= current_frame) && (targets.Count > 0))
				{
					once.AddRange(targets);
					targets.Clear();
					Frame = current_frame + 1;
				}
			}
		}

		/// <summary>
		/// Targets to run on update.
		/// </summary>
		protected TargetsList<IUpdatable> TargetsBase = new TargetsList<IUpdatable>();

		/// <summary>
		/// Targets to run on late update.
		/// </summary>
		protected TargetsList<ILateUpdatable> TargetsLate = new TargetsList<ILateUpdatable>();

		/// <summary>
		/// Targets to run on fixed update.
		/// </summary>
		protected TargetsList<IFixedUpdatable> TargetsFixed = new TargetsList<IFixedUpdatable>();

		/// <summary>
		/// Targets to run on nearest update.
		/// </summary>
		protected TargetsOnceList<IUpdatable> TargetsOnce = new TargetsOnceList<IUpdatable>();

		/// <summary>
		/// Actions to run on nearest update.
		/// </summary>
		protected TargetsOnceList<Action> ActionsOnce = new TargetsOnceList<Action>();

		/// <summary>
		/// Targets to run on next frame.
		/// </summary>
		protected TargetsNextList<IUpdatable> TargetsOnceNext;

		/// <summary>
		/// Actions to run on next frame.
		/// </summary>
		protected TargetsNextList<Action> ActionsOnceNext;

		static UpdaterProxy instance;

		/// <summary>
		/// Proxy instance.
		/// </summary>
		public static UpdaterProxy Instance
		{
			get
			{
				if (instance == null)
				{
					var go = new GameObject("New UI Widgets Updater Proxy");
					instance = go.AddComponent<UpdaterProxy>();
					instance.Start();
					DontDestroyOnLoad(instance);
				}

				return instance;
			}
		}

#if UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void Init()
		{
			instance = null;
		}
#endif

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			if (TargetsOnceNext == null)
			{
				TargetsOnceNext = new TargetsNextList<IUpdatable>(TargetsOnce);
			}

			if (ActionsOnceNext == null)
			{
				ActionsOnceNext = new TargetsNextList<Action>(ActionsOnce);
			}
		}

		/// <inheritdoc/>
		public void Add(IUpdatable target)
		{
			TargetsBase.Add(target);
		}

		/// <inheritdoc/>
		public void Remove(IUpdatable target)
		{
			TargetsBase.Remove(target);
		}

		/// <inheritdoc/>
		public void LateUpdateAdd(ILateUpdatable target)
		{
			TargetsLate.Add(target);
		}

		/// <inheritdoc/>
		public void LateUpdateRemove(ILateUpdatable target)
		{
			TargetsLate.Remove(target);
		}

		/// <inheritdoc/>
		public void AddFixedUpdate(IFixedUpdatable target)
		{
			TargetsFixed.Add(target);
		}

		/// <inheritdoc/>
		public void RemoveFixedUpdate(IFixedUpdatable target)
		{
			TargetsFixed.Remove(target);
		}

		/// <inheritdoc/>
		public void RunOnce(IUpdatable target)
		{
			TargetsOnce.Add(target);
		}

		/// <inheritdoc/>
		public void RunOnce(Action action)
		{
			ActionsOnce.Add(action);
		}

		/// <inheritdoc/>
		public void RunOnceNextFrame(IUpdatable target)
		{
			TargetsOnceNext.Add(target);
		}

		/// <inheritdoc/>
		public void RunOnceNextFrame(Action action)
		{
			ActionsOnceNext.Add(action);
		}

		/// <summary>
		/// Process update.
		/// </summary>
		protected virtual void Update()
		{
			RunOnceTargets();
			RunOnceActions();
			RunTargetsBase();
		}

		/// <summary>
		/// Process late update.
		/// </summary>
		protected virtual void LateUpdate()
		{
			RunTargetsLate();
		}

		/// <summary>
		/// Process fixed update.
		/// </summary>
		protected virtual void FixedUpdate()
		{
			RunTargetsFixed();
		}

		/// <summary>
		/// Run onceTargets update.
		/// </summary>
		protected virtual void RunOnceTargets()
		{
			TargetsOnceNext.Check();

			foreach (var target in TargetsOnce)
			{
				target.RunUpdate();
			}

			TargetsOnce.Cleanup();
		}

		/// <summary>
		/// Run onceActions.
		/// </summary>
		protected virtual void RunOnceActions()
		{
			ActionsOnceNext.Check();

			foreach (var action in ActionsOnce)
			{
				action();
			}

			ActionsOnce.Cleanup();
		}

		/// <summary>
		/// Run targets update.
		/// </summary>
		protected virtual void RunTargetsBase()
		{
			foreach (var target in TargetsBase)
			{
				target.RunUpdate();
			}

			TargetsBase.Cleanup();
		}

		/// <summary>
		/// Run targets LateUpdate.
		/// </summary>
		protected virtual void RunTargetsLate()
		{
			foreach (var target in TargetsLate)
			{
				target.RunLateUpdate();
			}

			TargetsLate.Cleanup();
		}

		/// <summary>
		/// Run targets FixedUpdate.
		/// </summary>
		protected virtual void RunTargetsFixed()
		{
			foreach (var target in TargetsFixed)
			{
				target.RunFixedUpdate();
			}

			TargetsFixed.Cleanup();
		}
	}
}