#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	using TMPro;

	/// <summary>
	/// Combobox with TMPro support.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Combobox.png
	/// </summary>
	[System.Obsolete("Replaced with ComboboxString.")]
	public class ComboboxTMPro : Combobox
	{
		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(GetComponent<TMP_InputField>(), ref InputAdapter);
#pragma warning restore 0612, 0618
		}
	}
}
#endif