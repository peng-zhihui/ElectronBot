namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Tooltip viewer.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TTooltip">Tooltip type.</typeparam>
	public class TooltipViewer<TData, TTooltip> : MonoBehaviour
		where TTooltip : Tooltip<TData, TTooltip>
	{
		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		protected TTooltip Tooltip;

		/// <summary>
		/// Data.
		/// </summary>
		[SerializeField]
		protected TData data;

		/// <summary>
		/// Data.
		/// </summary>
		public TData Data
		{
			get
			{
				return data;
			}

			set
			{
				data = value;
				Init();
			}
		}

		/// <summary>
		/// Settings.
		/// </summary>
		[SerializeField]
		protected TooltipSettings settings;

		/// <summary>
		/// Settings.
		/// </summary>
		public TooltipSettings Settings
		{
			get
			{
				return settings;
			}

			set
			{
				settings = value;
				Init();
			}
		}

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (Tooltip != null)
			{
				Tooltip.Register(gameObject, Data, Settings);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Tooltip != null)
			{
				Tooltip.Unregister(gameObject);
			}
		}
	}
}