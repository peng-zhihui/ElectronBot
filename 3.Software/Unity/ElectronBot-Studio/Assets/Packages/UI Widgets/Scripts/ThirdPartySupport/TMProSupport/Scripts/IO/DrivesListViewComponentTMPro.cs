#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// DrivesListViewComponent TMPro.
	/// Display drive.
	/// </summary>
	public class DrivesListViewComponentTMPro : DrivesListViewComponentBase
	{
		/// <summary>
		/// Text component to display drive name.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NameAdapter.")]
		protected TextMeshProUGUI Name;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif