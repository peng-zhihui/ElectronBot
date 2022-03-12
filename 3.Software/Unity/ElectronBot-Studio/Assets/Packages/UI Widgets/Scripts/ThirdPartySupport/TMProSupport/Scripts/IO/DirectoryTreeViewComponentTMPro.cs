#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// DirectoryTreeView component TMPro.
	/// </summary>
	public class DirectoryTreeViewComponentTMPro : DirectoryTreeViewComponent
	{
		/// <summary>
		/// Text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		public TextMeshProUGUI TextTMPro;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(TextTMPro, ref TextAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif