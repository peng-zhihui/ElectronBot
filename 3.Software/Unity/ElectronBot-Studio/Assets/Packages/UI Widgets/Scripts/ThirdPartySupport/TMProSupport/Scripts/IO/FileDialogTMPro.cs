#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UnityEngine;

	/// <summary>
	/// FileDialog TMPro.
	/// </summary>
	public class FileDialogTMPro : FileDialog
	{
		/// <summary>
		/// Filename Input.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with FilenameInputAdapter.")]
		public TMP_InputField FilenameInputTMPro;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(FilenameInputTMPro, ref FilenameInputAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif