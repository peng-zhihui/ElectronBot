namespace UIWidgets
{
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Scroll compensation for Tsunami effect.
	/// </summary>
	public class TsunamiEffectScrollCompensation : MonoBehaviour, ILateUpdatable
	{
		/// <summary>
		/// Scroll Rect.
		/// </summary>
		public ScrollRect ScrollRect;

		/// <summary>
		/// Layout.
		/// </summary>
		protected EasyLayout Layout;

		/// <summary>
		/// Layout margin.
		/// </summary>
		protected Padding Margin;

		/// <summary>
		/// Tsunami effect compensations.
		/// </summary>
		protected Vector2 Compensation = Vector2.zero;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Layout = GetComponent<EasyLayout>();

			if (Layout != null)
			{
				if (Layout.Symmetric)
				{
					Layout.MarginLeft = Layout.Margin.x;
					Layout.MarginRight = Layout.Margin.x;
					Layout.MarginTop = Layout.Margin.y;
					Layout.MarginBottom = Layout.Margin.y;
					Layout.Symmetric = false;
				}

				Margin = new Padding(Layout.MarginLeft, Layout.MarginRight, Layout.MarginTop, Layout.MarginBottom);
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Late update.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (Layout != null)
			{
				var compensation = ScrollRect.horizontal ? Compensation.x : Compensation.y;
				ApplyCompensatin(compensation);
			}

			Compensation = Vector2.zero;
		}

		void ApplyCompensatin(float compensation)
		{
			if (ScrollRect.horizontal)
			{
				Layout.MarginLeft = Margin.Left - (compensation * (1f - ScrollRect.horizontalNormalizedPosition));
			}
			else
			{
				Layout.MarginTop = Margin.Top - (compensation * (1f - ScrollRect.verticalNormalizedPosition));
			}

			Layout.UpdateLayout();
		}

		/// <summary>
		/// Add compensation.
		/// </summary>
		/// <param name="compensation">Compensation.</param>
		public void Add(Vector2 compensation)
		{
			Compensation += compensation;
		}
	}
}