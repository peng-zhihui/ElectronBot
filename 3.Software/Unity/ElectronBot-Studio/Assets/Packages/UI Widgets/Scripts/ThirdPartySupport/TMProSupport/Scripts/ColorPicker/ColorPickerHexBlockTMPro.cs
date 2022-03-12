#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker Hex block.
	/// </summary>
	public class ColorPickerHexBlockTMPro : ColorPickerHexBlockBase
	{
		/// <summary>
		/// The input field for hex.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with InputHexAdapter.")]
		protected TMP_InputField InputHex;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(InputHex, ref InputHexAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif