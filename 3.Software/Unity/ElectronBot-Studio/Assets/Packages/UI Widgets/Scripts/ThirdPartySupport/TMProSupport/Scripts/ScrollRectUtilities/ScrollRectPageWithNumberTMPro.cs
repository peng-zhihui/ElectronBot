#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// ScrollRectPage with number.
	/// </summary>
	public class ScrollRectPageWithNumberTMPro : ScrollRectPage, IUpgradeable
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NumberAdapter.")]
		public TextMeshProUGUI Number;

		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public TextAdapter NumberAdapter;

		/// <inheritdoc/>
		public override void SetPage(int page)
		{
			base.SetPage(page);
			if (NumberAdapter != null)
			{
				NumberAdapter.text = (page + 1).ToString();
			}
		}

		/// <inheritdoc/>
		public override void SetStyle(StyleText styleText, Style style)
		{
			if (NumberAdapter != null)
			{
				styleText.ApplyTo(NumberAdapter.gameObject);
			}
		}

		/// <inheritdoc/>
		public override void GetStyle(StyleText styleText, Style style)
		{
			if (NumberAdapter != null)
			{
				styleText.GetFrom(NumberAdapter.gameObject);
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Number, ref NumberAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif
	}
}
#endif