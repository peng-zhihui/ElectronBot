namespace UIWidgets
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Tooltip.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TTooltip">Tooltip type.</typeparam>
	public abstract class Tooltip<TData, TTooltip> : TooltipBase
		where TTooltip : Tooltip<TData, TTooltip>
	{
		/// <summary>
		/// Tooltip event.
		/// </summary>
		public class TooltipEvent : UnityEvent<TTooltip, GameObject>
		{
		}

		/// <summary>
		/// Target info.
		/// </summary>
		protected class TargetInfo
		{
			/// <summary>
			/// Data.
			/// </summary>
			public TData Data;

			/// <summary>
			/// Settings.
			/// </summary>
			public TooltipSettings Settings;

			/// <summary>
			/// Helper.
			/// </summary>
			public TooltipListener Helper;

			static Stack<TargetInfo> Cache = new Stack<TargetInfo>();

			/// <summary>
			/// Initializes a new instance of the <see cref="TargetInfo"/> class.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="data">Data.</param>
			/// <param name="settings">Settings.</param>
			private TargetInfo(GameObject target, TData data, TooltipSettings settings)
			{
				Helper = Utilities.GetOrAddComponent<TooltipListener>(target);
				Data = data;
				Settings = settings;
			}

			/// <summary>
			/// Get instance.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <param name="data">Data.</param>
			/// <param name="settings">Settings.</param>
			/// <returns>Instance.</returns>
			public static TargetInfo Get(GameObject target, TData data, TooltipSettings settings)
			{
				if (Cache.Count > 0)
				{
					var info = Cache.Pop();

					info.Helper = Utilities.GetOrAddComponent<TooltipListener>(target);
					info.Data = data;
					info.Settings = settings;

					return info;
				}

				return new TargetInfo(target, data, settings);
			}

			/// <summary>
			/// Free instance.
			/// </summary>
			public void Free()
			{
				Data = default(TData);
				Helper = null;

				Cache.Push(this);
			}

			#if UNITY_2019_3_OR_NEWER
			[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
			static void Init()
			{
				Cache.Clear();
			}
			#endif
		}

		/// <summary>
		/// Targets.
		/// </summary>
		protected Dictionary<GameObject, TargetInfo> Targets = new Dictionary<GameObject, TargetInfo>();

		/// <summary>
		/// Current data.
		/// </summary>
		protected TData currentData;

		/// <summary>
		/// Current data.
		/// </summary>
		public TData CurrentData
		{
			get
			{
				return currentData;
			}

			protected set
			{
				if (currentData != null)
				{
					Unsubscribe(currentData);
				}

				currentData = value;
				SetData(value);

				if (currentData != null)
				{
					Subscribe(currentData);
				}
			}
		}

		/// <summary>
		/// Show event.
		/// </summary>
		[SerializeField]
		public TooltipEvent OnShow = new TooltipEvent();

		/// <summary>
		/// Hide event.
		/// </summary>
		[SerializeField]
		public TooltipEvent OnHide = new TooltipEvent();

		/// <summary>
		/// Data type is value type.
		/// </summary>
		protected static readonly bool IsValueType;

		/// <summary>
		/// Data type implements IObservable.
		/// </summary>
		protected static readonly bool IsDataObservable;

		/// <summary>
		/// Data type implements INotifyPropertyChanged.
		/// </summary>
		protected static readonly bool IsDataNotifyPropertyChanged;

		static Tooltip()
		{
			var type = typeof(TData);
			IsValueType = type.IsValueType;
			IsDataObservable = !IsValueType && typeof(IObservable).IsAssignableFrom(type);
			IsDataNotifyPropertyChanged = !IsValueType && typeof(INotifyPropertyChanged).IsAssignableFrom(type);
		}

		/// <summary>
		/// Subscribe to data changes.
		/// </summary>
		/// <param name="data">Data.</param>
		protected virtual void Subscribe(TData data)
		{
			if (IsDataObservable)
			{
				(data as IObservable).OnChange += DataChanged;
			}
			else if (IsDataNotifyPropertyChanged)
			{
				(data as INotifyPropertyChanged).PropertyChanged += DataChanged;
			}
		}

		/// <summary>
		/// Unsubscribe from data changes.
		/// </summary>
		/// <param name="data">Data.</param>
		protected virtual void Unsubscribe(TData data)
		{
			if (IsDataObservable)
			{
				(data as IObservable).OnChange -= DataChanged;
			}
			else if (IsDataNotifyPropertyChanged)
			{
				(data as INotifyPropertyChanged).PropertyChanged -= DataChanged;
			}
		}

		/// <summary>
		/// Process data change.
		/// </summary>
		protected virtual void DataChanged()
		{
			SetData(CurrentData);
		}

		/// <summary>
		/// Process data change.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event arguments.</param>
		protected virtual void DataChanged(object sender, PropertyChangedEventArgs e)
		{
			SetData(CurrentData);
		}

		/// <summary>
		/// Register tooltip for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="data">Tooltip data.</param>
		/// <param name="settings">Tooltip settings.</param>
		public virtual void Register(GameObject target, TData data, TooltipSettings settings)
		{
			TargetInfo info;
			if (Targets.TryGetValue(target, out info))
			{
				info.Data = data;
				info.Settings = settings;
			}
			else
			{
				info = TargetInfo.Get(target, data, settings);
				info.Helper.Register(this);
				Targets[target] = info;
			}
		}

		/// <summary>
		/// Unregister tooltip for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target was registered; otherwise false.</returns>
		public virtual bool Unregister(GameObject target)
		{
			TargetInfo info;
			if (!Targets.TryGetValue(target, out info))
			{
				return false;
			}

			info.Helper.Unregister(this);
			info.Free();

			return Targets.Remove(target);
		}

		/// <summary>
		/// Is specified target registered?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target is registered; otherwise false.</returns>
		public virtual bool Registered(GameObject target)
		{
			return Targets.ContainsKey(target);
		}

		/// <inheritdoc/>
		public override TooltipSettings GetSettings(GameObject target)
		{
			return Targets[target].Settings;
		}

		/// <inheritdoc/>
		public override bool UpdateSettings(GameObject target, TooltipSettings settings)
		{
			TargetInfo info;
			if (!Targets.TryGetValue(target, out info))
			{
				return false;
			}

			info.Settings = settings;
			return true;
		}

		/// <summary>
		/// Get data for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Data.</returns>
		public virtual TData GetData(GameObject target)
		{
			return Targets[target].Data;
		}

		/// <summary>
		/// Update data for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="data">Data.</param>
		/// <returns>true if target registered and data was updated; otherwise false.</returns>
		public virtual bool UpdateData(GameObject target, TData data)
		{
			TargetInfo info;
			if (!Targets.TryGetValue(target, out info))
			{
				return false;
			}

			info.Data = data;
			return true;
		}

		/// <inheritdoc/>
		public override void Show(GameObject target)
		{
			if (!Registered(target))
			{
				return;
			}

			if (CurrentTarget != null)
			{
				OnHide.Invoke(this as TTooltip, CurrentTarget);
			}

			CurrentTarget = target;

			var info = Targets[CurrentTarget];
			CurrentParent = info.Settings.Parent != null
				? info.Settings.Parent
				: UtilitiesUI.FindTopmostCanvas(CurrentTarget.transform);

			SetPosition(info.Settings.Position);
			CurrentData = info.Data;

			RectTransform.SetParent(CurrentParent, true);
			RectTransform.SetAsLastSibling();
			gameObject.SetActive(true);

			OnShow.Invoke(this as TTooltip, CurrentTarget);
		}

		/// <summary>
		/// Set position.
		/// </summary>
		/// <param name="position">Position.</param>
		protected virtual void SetPosition(TooltipPosition position)
		{
			RectTransform.SetParent(CurrentTarget.transform, false);

			switch (position)
			{
				case TooltipPosition.TopLeft:
					RectTransform.pivot = new Vector2(0f, 0f);
					RectTransform.anchorMin = new Vector2(0f, 1f);
					RectTransform.anchorMax = new Vector2(0f, 1f);
					break;
				case TooltipPosition.TopCenter:
					RectTransform.pivot = new Vector2(0.5f, 0f);
					RectTransform.anchorMin = new Vector2(0.5f, 1f);
					RectTransform.anchorMax = new Vector2(0.5f, 1f);
					break;
				case TooltipPosition.TopRight:
					RectTransform.pivot = new Vector2(1f, 0f);
					RectTransform.anchorMin = new Vector2(1f, 1f);
					RectTransform.anchorMax = new Vector2(1f, 1f);
					break;
				case TooltipPosition.MiddleLeft:
					RectTransform.pivot = new Vector2(1f, 0.5f);
					RectTransform.anchorMin = new Vector2(0f, 0.5f);
					RectTransform.anchorMax = new Vector2(0f, 0.5f);
					break;
				case TooltipPosition.MiddleCenter:
					RectTransform.pivot = new Vector2(0.5f, 0.5f);
					RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
					RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
					break;
				case TooltipPosition.MiddleRight:
					RectTransform.pivot = new Vector2(0f, 0.5f);
					RectTransform.anchorMin = new Vector2(1f, 0.5f);
					RectTransform.anchorMax = new Vector2(1f, 0.5f);
					break;
				case TooltipPosition.BottomLeft:
					RectTransform.pivot = new Vector2(0f, 1f);
					RectTransform.anchorMin = new Vector2(0f, 0f);
					RectTransform.anchorMax = new Vector2(0f, 0f);
					break;
				case TooltipPosition.BottomCenter:
					RectTransform.pivot = new Vector2(0.5f, 1f);
					RectTransform.anchorMin = new Vector2(0.5f, 0f);
					RectTransform.anchorMax = new Vector2(0.5f, 0f);
					break;
				case TooltipPosition.BottomRight:
					RectTransform.pivot = new Vector2(1f, 1f);
					RectTransform.anchorMin = new Vector2(1f, 0f);
					RectTransform.anchorMax = new Vector2(1f, 0f);
					break;
			}

			RectTransform.anchoredPosition = new Vector2(0f, 0f);
		}

		/// <summary>
		/// Hide tooltip.
		/// </summary>
		public override void Hide()
		{
			Hide(true);
		}

		/// <summary>
		/// Hide tooltip.
		/// </summary>
		/// <param name="isDisabled">Is instance disabled?</param>
		public virtual void Hide(bool isDisabled)
		{
			if (CurrentTarget == null)
			{
				return;
			}

			OnHide.Invoke(this as TTooltip, CurrentTarget);

			CurrentTarget = null;
			Unsubscribe(CurrentData);
			currentData = default(TData);

			if (isDisabled)
			{
				Updater.RunOnceNextFrame(ResetParent);
			}
			else
			{
				ResetParent();
			}

			gameObject.SetActive(false);
		}

		/// <summary>
		/// Reset parent.
		/// </summary>
		protected virtual void ResetParent()
		{
			if (this == null)
			{
				return;
			}

			RectTransform.SetParent(null, false);
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="data">Data.</param>
		protected abstract void SetData(TData data);

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			if (CurrentTarget != null)
			{
				Hide(false);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			foreach (var target in Targets)
			{
				var info = target.Value;
				if (info.Helper != null)
				{
					info.Helper.Unregister(this);
				}

				info.Free();
			}

			Targets.Clear();

			base.OnDestroy();
		}
	}
}