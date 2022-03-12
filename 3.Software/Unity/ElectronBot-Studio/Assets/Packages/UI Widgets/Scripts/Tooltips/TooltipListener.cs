namespace UIWidgets
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

	/// <summary>
	/// Tooltip listener.
	/// </summary>
    public class TooltipListener : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler
    {
		/// <summary>
		/// Info.
		/// </summary>
		protected class TooltipInfo
		{
			/// <summary>
			/// Tooltip.
			/// </summary>
			public TooltipBase Tooltip;

			/// <summary>
			/// Coroutine.
			/// </summary>
			public IEnumerator Coroutine;

			static Stack<TooltipInfo> Cache = new Stack<TooltipInfo>();

			/// <summary>
			/// Initializes a new instance of the <see cref="TooltipInfo"/> class.
			/// </summary>
			/// <param name="tooltip">Tooltip.</param>
			private TooltipInfo(TooltipBase tooltip)
			{
				Tooltip = tooltip;
			}

			/// <summary>
			/// Get instance.
			/// </summary>
			/// <param name="tooltip">Tooltip.</param>
			/// <returns>Instance.</returns>
			public static TooltipInfo Get(TooltipBase tooltip)
			{
				if (Cache.Count > 0)
				{
					var info = Cache.Pop();
					info.Tooltip = tooltip;

					return info;
				}

				return new TooltipInfo(tooltip);
			}

			/// <summary>
			/// Free instance.
			/// </summary>
			public void Free()
			{
				Tooltip = null;
				Coroutine = null;

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
		/// Registered tooltips.
		/// </summary>
		protected List<TooltipInfo> Tooltips = new List<TooltipInfo>();

		/// <summary>
		/// Settings.
		/// </summary>
		public TooltipSettings Settings;

		/// <summary>
		/// Process the pointer enter event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			Show();
		}

		/// <summary>
		/// Process the pointer exit event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			Hide();
		}

		/// <summary>
		/// Process the select event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnSelect(BaseEventData eventData)
		{
			Show();
		}

		/// <summary>
		/// Process the deselect event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnDeselect(BaseEventData eventData)
		{
			Hide();
		}

		/// <summary>
		/// Register tooltip.
		/// </summary>
		/// <param name="tooltip">Tooltip.</param>
		public virtual void Register(TooltipBase tooltip)
		{
			if (Registered(tooltip))
			{
				return;
			}

			Tooltips.Add(TooltipInfo.Get(tooltip));
		}

		/// <summary>
		/// Unregister tooltip.
		/// </summary>
		/// <param name="tooltip">Tooltip.</param>
		/// <returns>true if tooltip was registered; otherwise false.</returns>
		public virtual bool Unregister(TooltipBase tooltip)
		{
			var result = false;

			for (int i = Tooltips.Count - 1; i >= 0; i--)
			{
				var info = Tooltips[i];
				if (info.Tooltip == tooltip)
				{
					FreeTooltipInfo(info);
					Tooltips.RemoveAt(i);
					result = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Free info instance.
		/// </summary>
		/// <param name="tooltipInfo">Info instance.</param>
		protected virtual void FreeTooltipInfo(TooltipInfo tooltipInfo)
		{
			if (tooltipInfo.Coroutine != null)
			{
				StopCoroutine(tooltipInfo.Coroutine);
			}

			tooltipInfo.Free();
		}

		/// <summary>
		/// Is specified tooltip registered?
		/// </summary>
		/// <param name="tooltip">Tooltip.</param>
		/// <returns>true if tooltip is registered; otherwise false.</returns>
		public bool Registered(TooltipBase tooltip)
		{
			foreach (var info in Tooltips)
			{
				if (info.Tooltip == tooltip)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Show tooltip.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <returns>Coroutine.</returns>
		protected virtual IEnumerator Show(TooltipInfo info)
		{
			var settings = info.Tooltip.GetSettings(gameObject);

			yield return UtilitiesTime.Wait(settings.ShowDelay, settings.UnscaledTime);

			info.Tooltip.Show(gameObject);
		}

		/// <summary>
		/// Show tooltips.
		/// </summary>
		public void Show()
		{
			foreach (var info in Tooltips)
			{
				info.Coroutine = Show(info);
				StartCoroutine(info.Coroutine);
			}
		}

		/// <summary>
		/// Hide tooltip.
		/// </summary>
		/// <param name="info">Tooltip info.</param>
		protected virtual void Hide(TooltipInfo info)
		{
			if (info.Coroutine != null)
			{
				StopCoroutine(info.Coroutine);
				info.Coroutine = null;
			}

			info.Tooltip.Hide();
		}

		/// <summary>
		/// Hide tooltips.
		/// </summary>
		public void Hide()
		{
			foreach (var info in Tooltips)
			{
				Hide(info);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			foreach (var tooltip in Tooltips)
			{
				FreeTooltipInfo(tooltip);
			}

			Tooltips.Clear();
		}
	}
}