#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// PickerBool.
	/// </summary>
	public class PickerBoolTMPro : PickerBool
	{
		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with MessageAdapter.")]
		protected TextMeshProUGUI MessageTMPro;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(MessageTMPro, ref MessageAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif