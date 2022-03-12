namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base generic class for the ScrollBlock.
	/// </summary>
	/// <typeparam name="TComponent">Type of the component.</typeparam>
	public class ScrollBlockCustom<TComponent> : ScrollBlockBase
		where TComponent : ScrollBlockItem
	{
		/// <summary>
		/// Item changed delegate.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item instance.</param>
		public delegate void ItemChanged(int index, TComponent item);

		/// <summary>
		/// Do nothing.
		/// </summary>
		static void DoNothing(int index, TComponent item)
		{
		}

		[SerializeField]
		TComponent defaultItem;

		/// <summary>
		/// DefaultItem.
		/// </summary>
		public TComponent DefaultItem
		{
			get
			{
				return defaultItem;
			}

			set
			{
				if (defaultItem != value)
				{
					defaultItem = value;

					ComponentsPool.Template = defaultItem;

					UpdateLayout();
					Resize();
				}
			}
		}

		/// <summary>
		/// Used instances of the DefaultItem.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TComponent> Components = new List<TComponent>();

		/// <summary>
		/// Unused instances of the DefaultItem.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TComponent> ComponentsCache = new List<TComponent>();

		ListComponentPool<TComponent> componentsPool;

		/// <summary>
		/// Components pool.
		/// </summary>
		protected ListComponentPool<TComponent> ComponentsPool
		{
			get
			{
				if (componentsPool == null)
				{
					componentsPool = new ListComponentPool<TComponent>(DefaultItem, Components, ComponentsCache, RectTransform);
				}

				return componentsPool;
			}
		}

		/// <inheritdoc/>
		public override int Count
		{
			get
			{
				return ComponentsPool.Count;
			}
		}

		/// <inheritdoc/>
		protected override int ComponentsMedian
		{
			get
			{
				return ComponentsPool.Count / 2;
			}
		}

		/// <summary>
		/// Item changed event.
		/// </summary>
		public event ItemChanged OnItemChanged = DoNothing;

		/// <inheritdoc/>
		protected override void UpdateLayout()
		{
			var layout = GetComponent<EasyLayoutNS.EasyLayout>();
			Layout = new EasyLayoutBridge(layout, DefaultItem.transform as RectTransform, false, false)
			{
				IsHorizontal = IsHorizontal,
			};

			DefaultItemSize = Layout.GetItemSize();
			DefaultItem.gameObject.SetActive(false);

			LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform.parent as RectTransform);
		}

		/// <inheritdoc/>
		protected override void Resize()
		{
			var max = CalculateMax();

			if (max == ComponentsPool.Count)
			{
				return;
			}

			ComponentsPool.Require(max);

			var median = ComponentsMedian;
			for (int i = 0; i < ComponentsPool.Count; i++)
			{
				ComponentsPool[i].Index = i - median;
				ComponentsPool[i].Owner = this;
				ComponentsPool[i].transform.SetAsLastSibling();
			}

			UpdateView();
		}

		/// <summary>
		/// Set text of the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SetComponentText(TComponent component)
		{
			component.Text.Value = Value(component.Index);
			component.DataChanged();
			OnItemChanged(component.Index, component);
		}

		/// <summary>
		/// Set text.
		/// </summary>
		[Obsolete("Replaced with UpdateView().")]
		public void SetText()
		{
			UpdateView();
		}

		/// <inheritdoc/>
		public override void UpdateView()
		{
			Init();

			foreach (var component in ComponentsPool)
			{
				SetComponentText(component);
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (DefaultItem.Text != null)
			{
				style.ScrollBlock.Text.ApplyTo(DefaultItem.Text.GameObject);

				if (IsInited)
				{
					for (int i = 0; i < Components.Count; i++)
					{
						style.ScrollBlock.Text.ApplyTo(Components[i].Text.GameObject);
					}

					for (int i = 0; i < ComponentsCache.Count; i++)
					{
						style.ScrollBlock.Text.ApplyTo(ComponentsCache[i].Text.GameObject);
					}
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (DefaultItem.Text != null)
			{
				style.ScrollBlock.Text.GetFrom(DefaultItem.Text.GameObject);
			}

			return true;
		}
		#endregion
	}
}