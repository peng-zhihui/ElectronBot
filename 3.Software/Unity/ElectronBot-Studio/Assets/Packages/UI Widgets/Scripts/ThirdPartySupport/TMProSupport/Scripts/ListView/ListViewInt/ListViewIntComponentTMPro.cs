#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// ListViewInt component.
	/// </summary>
	public class ListViewIntComponentTMPro : ListViewIntComponentBase
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NumberAdapter.")]
		public TextMeshProUGUI Number;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Number, ref NumberAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif